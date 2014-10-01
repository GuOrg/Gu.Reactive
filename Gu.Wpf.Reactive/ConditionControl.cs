using System.Windows;
using System.Windows.Controls;

namespace Gu.Wpf.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Gu.Reactive;

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

        public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(
            "ItemTemplate",
            typeof(DataTemplate),
            typeof(ConditionControl), 
            new PropertyMetadata(default(DataTemplate)));

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
                return;
            }
            conditionControl.Prerequisites = condition.Prerequisites;
            conditionControl.RootCondition = Enumerable.Repeat<ICondition>(condition, 1);
        }
    }
}
