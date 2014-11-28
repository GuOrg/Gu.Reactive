using System.Windows;
using System.Windows.Controls;

namespace Gu.Wpf.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Input;
    using Gu.Reactive;

    /// <summary>
    /// Convenience contorl for displaying conditions
    /// </summary>
    public class ConditionControl : Control
    {
        /// <summary>
        /// The command to show info for
        /// </summary>
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            "Command",
            typeof(ICommand),
            typeof(ConditionControl),
            new PropertyMetadata(default(ICommand), OnCommandChanged));

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
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public ICondition Condition
        {
            get { return (ICondition)GetValue(ConditionProperty); }
            set { SetValue(ConditionProperty, value); }
        }

        public IEnumerable<ICondition> Prerequisites
        {
            get { return (IEnumerable<ICondition>)GetValue(PrerequisitesProperty); }
            protected set { SetValue(PrerequisitesProperty, value); }
        }

        public IEnumerable<ICondition> RootCondition
        {
            get { return (IEnumerable<ICondition>)GetValue(RootConditionProperty); }
            protected set { SetValue(RootConditionProperty, value); }
        }

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }


        private static void OnCommandChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var command = e.NewValue as ConditionRelayCommand;
            if (command != null)
            {
                ((ConditionControl) o).Condition = command.Condition;
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
