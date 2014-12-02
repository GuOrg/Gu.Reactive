﻿namespace Gu.Wpf.Reactive
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Input;
    using Gu.Reactive;
    using ToolTips;

    /// <summary>
    /// Exposes AdornedElement and sets DataContext to the CommandProxy of the adorned element
    /// </summary>
    public class CommandToolTip : TouchToolTip
    {
        private static readonly DependencyProperty CommandProxyProperty = DependencyProperty.Register(
            "CommandProxyProxy",
            typeof(ICommand),
            typeof(CommandToolTip),
            new PropertyMetadata(default(ICommand), OnCommandChanged));

        private ICommand CommandProxy
        {
            get { return (ICommand)GetValue(CommandProxyProperty); }
            set { SetValue(CommandProxyProperty, value); }
        }

        public static readonly DependencyProperty ToolTipTextProperty = DependencyProperty.Register(
            "ToolTipText",
            typeof(string),
            typeof(CommandToolTip),
            new PropertyMetadata(default(string)));

        public static readonly DependencyProperty ConditionProperty = DependencyProperty.Register(
            "Condition",
            typeof(ICondition),
            typeof(CommandToolTip),
            new PropertyMetadata(default(ICondition)));

        public static readonly DependencyProperty CommandTypeProperty = DependencyProperty.Register(
            "CommandType",
            typeof(Type),
            typeof(CommandToolTip),
            new PropertyMetadata(default(Type)));

        static CommandToolTip()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CommandToolTip), new FrameworkPropertyMetadata(typeof(CommandToolTip)));
        }

        public string ToolTipText
        {
            get { return (string)GetValue(ToolTipTextProperty); }
            set { SetValue(ToolTipTextProperty, value); }
        }

        public ICondition Condition
        {
            get { return (ICondition)GetValue(ConditionProperty); }
            set { SetValue(ConditionProperty, value); }
        }

        public Type CommandType
        {
            get { return (Type)GetValue(CommandTypeProperty); }
            set { SetValue(CommandTypeProperty, value); }
        }

        public override void OnToolTipChanged(UIElement adornedElement)
        {
            base.OnToolTipChanged(adornedElement);
            var commandBinding = new Binding(ButtonBase.CommandProperty.Name)
            {
                Source = adornedElement,
                Mode = BindingMode.OneWay
            };
            BindingOperations.SetBinding(this, CommandProxyProperty, commandBinding);
        }

        private static void OnCommandChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var commandToolTip = (CommandToolTip)o;
            var command = e.NewValue as ICommand;
            if (command == null)
            {
                BindingOperations.ClearBinding(commandToolTip, ToolTipTextProperty);
                commandToolTip.ClearValue(ToolTipTextProperty);;
                commandToolTip.Condition = null;
                commandToolTip.CommandType = null;
                return;
            }
            var toolTipCommand = command as IToolTipCommand;
            if (toolTipCommand != null)
            {
                var prop = PathExpressionVisitor.GetPath<IToolTipCommand, string>(x => x.ToolTipText)
                                                .Last().Member.Name;
                var binding = new Binding(prop)
                                  {
                                      Source = toolTipCommand, 
                                      Mode = BindingMode.OneWay
                                  };

                BindingOperations.SetBinding(commandToolTip, ToolTipTextProperty, binding);
            }

            var conditionCommand = command as IConditionRelayCommand;
            if (conditionCommand != null)
            {
                commandToolTip.Condition = conditionCommand.Condition;
            }
            commandToolTip.CommandType = command.GetType();
        }
    }
}