using System.Windows;
using System.Windows.Controls;

namespace Gu.Wpf.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Windows.Input;
    using Gu.Reactive;

    /// <summary>
    /// A control for displaying conditions
    /// </summary>
    public class ConditionControl : Control
    {
        public static readonly DependencyProperty ConditionProperty = DependencyProperty.Register(
            "Condition",
            typeof(ICondition),
            typeof(ConditionControl),
            new PropertyMetadata(default(ICondition), OnConditionChanged));

        public static readonly DependencyProperty PrerequisitesProperty = DependencyProperty.Register(
            "Prerequisites",
            typeof(IEnumerable<ICondition>),
            typeof(ConditionControl),
            new PropertyMetadata(default(IEnumerable<ICondition>)));

        public static readonly DependencyProperty RootConditionProperty = DependencyProperty.Register(
            "RootCondition",
            typeof(IEnumerable<ICondition>),
            typeof(ConditionControl),
            new PropertyMetadata(default(IEnumerable<ICondition>)));

        private static readonly DependencyPropertyKey FlatListPropertyKey = DependencyProperty.RegisterReadOnly(
            "FlatList",
            typeof(IEnumerable<ICondition>),
            typeof(ConditionControl),
            new PropertyMetadata(default(IEnumerable<ICondition>)));

        private static readonly DependencyPropertyKey NotSatisfiedOnlyPropertyKey = DependencyProperty.RegisterReadOnly(
            "NotSatisfiedOnly",
            typeof(ICollectionView<ICondition>),
            typeof(ConditionControl),
            new PropertyMetadata(default(ICollectionView<ICondition>)));

        public static readonly DependencyProperty NotSatisfiedOnlyProperty = NotSatisfiedOnlyPropertyKey.DependencyProperty;

        public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(
            "ItemTemplate",
            typeof(DataTemplate),
            typeof(ConditionControl),
            new PropertyMetadata(default(DataTemplate)));

        public static readonly DependencyProperty FlatListProperty = FlatListPropertyKey.DependencyProperty;

        static ConditionControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ConditionControl), new FrameworkPropertyMetadata(typeof(ConditionControl)));
        }

        public ICondition Condition
        {
            get
            {
                return (ICondition)GetValue(ConditionProperty);
            }
            set
            {
                SetValue(ConditionProperty, value);
            }
        }

        /// <summary>
        /// The conditions that must be met for the root condition to be satisfied
        /// </summary>
        public IEnumerable<ICondition> Prerequisites
        {
            get
            {
                return (IEnumerable<ICondition>)GetValue(PrerequisitesProperty);
            }
            protected set
            {
                SetValue(PrerequisitesProperty, value);
            }
        }

        /// <summary>
        /// The root condition :)
        /// </summary>
        public IEnumerable<ICondition> RootCondition
        {
            get
            {
                return (IEnumerable<ICondition>)GetValue(RootConditionProperty);
            }
            protected set
            {
                SetValue(RootConditionProperty, value);
            }
        }

        /// <summary>
        /// A flat list of all conditions
        /// </summary>
        public IEnumerable<ICondition> FlatList
        {
            get
            {
                return (IEnumerable<ICondition>)GetValue(FlatListProperty);
            }
            protected set
            {
                SetValue(FlatListPropertyKey, value);
            }
        }

        /// <summary>
        /// A filtered view with all conditions where .IsSatisfied != true 
        /// and not due top a prerequisite.
        /// </summary>
        public ICollectionView<ICondition> NotSatisfiedOnly
        {
            get
            {
                return (ICollectionView<ICondition>)GetValue(NotSatisfiedOnlyProperty);
            }
            protected set
            {
                SetValue(NotSatisfiedOnlyPropertyKey, value);
            }
        }

        public DataTemplate ItemTemplate
        {
            get
            {
                return (DataTemplate)GetValue(ItemTemplateProperty);
            }
            set
            {
                SetValue(ItemTemplateProperty, value);
            }
        }

        private static void OnConditionChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var conditionControl = (ConditionControl)o;
            var condition = e.NewValue as ICondition;
            if (condition == null)
            {
                conditionControl.Prerequisites = null;
                conditionControl.RootCondition = Enumerable.Empty<ICondition>();
                conditionControl.FlatList = Enumerable.Empty<ICondition>();
                conditionControl.NotSatisfiedOnly = new CollectionView<ICondition>(Enumerable.Empty<ICondition>());
                return;
            }
            conditionControl.Prerequisites = condition.Prerequisites;
            conditionControl.RootCondition = Enumerable.Repeat<ICondition>(condition, 1);
            var flatList = Flatten(condition);
            conditionControl.FlatList = flatList;
            var updateTrigger = flatList.Select(x => x.ToObservable(y => y.IsSatisfied)).Merge();
            conditionControl.NotSatisfiedOnly = new CollectionView<ICondition>(flatList, updateTrigger)
            {
                Filter = IsNotSatisfied
            };
        }

        private static List<ICondition> Flatten(ICondition condition, List<ICondition> list = null)
        {
            if (list == null)
            {
                list = new List<ICondition>();
            }
            if (list.Contains(condition))
            {
                return list; // Break recursion
            }
            list.Add(condition);
            foreach (var pre in condition.Prerequisites)
            {
                Flatten(pre, list);
            }
            return list;
        }

        private static bool IsNotSatisfied(ICondition condition)
        {
            if (condition.Prerequisites.Any(x => x.IsSatisfied != true))
            {
                return false;
            }
            return condition.IsSatisfied != true;
        }
    }
}