namespace Gu.Wpf.Reactive
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Input;

    using Gu.Reactive;

    /// <summary>
    /// Exposes AdornedElement and sets DataContext to the CommandProxy of the adorned element
    /// </summary>
    public class ConditionToolTip : ToolTip
    {
        public static readonly DependencyProperty ConditionProperty = DependencyProperty.Register(
            "Condition",
            typeof(ICondition),
            typeof(ConditionToolTip),
            new PropertyMetadata(default(ICondition)));

        public static readonly DependencyProperty InferConditionFromCommandProperty = DependencyProperty.Register(
            "InferConditionFromCommand",
            typeof(bool),
            typeof(ConditionToolTip),
            new PropertyMetadata(true, OnInferConditionFromCommandChanged));

        private static readonly DependencyPropertyKey CommandTypePropertyKey = DependencyProperty.RegisterReadOnly(
            "CommandType",
            typeof(Type),
            typeof(ConditionToolTip),
            new PropertyMetadata(default(Type)));

        public static readonly DependencyProperty CommandTypeProperty = CommandTypePropertyKey.DependencyProperty;

        private static readonly DependencyProperty PlacementTargetProxyProperty = DependencyProperty.Register(
            "PlacementTargetProxy",
            typeof(UIElement),
            typeof(ConditionToolTip),
            new PropertyMetadata(
                default(UIElement),
                OnPlacementTargetChanged));

        private static readonly DependencyProperty CommandProxyProperty = DependencyProperty.Register(
            "CommandProxy",
            typeof(ICommand),
            typeof(ConditionToolTip),
            new PropertyMetadata(default(ICommand), OnCommandChanged));

        static ConditionToolTip()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ConditionToolTip), new FrameworkPropertyMetadata(typeof(ConditionToolTip)));
        }

        public ConditionToolTip()
        {
            UpdateInferConditionFromCommand(InferConditionFromCommand);
        }

        public bool InferConditionFromCommand
        {
            get { return (bool)GetValue(InferConditionFromCommandProperty); }
            set { SetValue(InferConditionFromCommandProperty, value); }
        }

        /// <summary>
        /// The condition if the command is a ConditionRelayCommand null otherwise
        /// </summary>
        public ICondition Condition
        {
            get { return (ICondition)GetValue(ConditionProperty); }
            set { SetValue(ConditionProperty, value); }
        }

        public Type CommandType
        {
            get { return (Type)GetValue(CommandTypeProperty); }
            protected set { SetValue(CommandTypePropertyKey, value); }
        }

        private static void OnPlacementTargetChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var commandToolTip = (ConditionToolTip)o;
            var target = commandToolTip.PlacementTarget as ButtonBase;
            if (target == null)
            {
                commandToolTip.SetValue(CommandProxyProperty, null);
            }
            else
            {
                var command = target.GetValue(ButtonBase.CommandProperty) as IConditionRelayCommand;
                commandToolTip.SetValue(CommandProxyProperty, command);
            }
        }

        private static void OnCommandChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var commandToolTip = (ConditionToolTip)o;
            var command = e.NewValue as IConditionRelayCommand;
            if (command == null)
            {
                commandToolTip.Condition = null;
                commandToolTip.CommandType = null;
                return;
            }

            commandToolTip.Condition = command.Condition;
            commandToolTip.CommandType = command.GetType();
        }

        private static void OnInferConditionFromCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var conditionToolTip = (ConditionToolTip)d;
            conditionToolTip.UpdateInferConditionFromCommand((bool)e.NewValue);
        }

        private void UpdateInferConditionFromCommand(bool infer)
        {
            if (infer)
            {
                BindingOperations.SetBinding(
                    this,
                    PlacementTargetProxyProperty,
                    this.CreateOneWayBinding(PlacementTargetProperty));
            }
            else
            {
                BindingOperations.ClearBinding(this, PlacementTargetProxyProperty);
            }
        }
    }
}
