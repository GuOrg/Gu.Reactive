namespace Gu.Wpf.Reactive
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Xaml;

    using Gu.Reactive;
    using Gu.Wpf.ToolTips;

    /// <summary>
    /// Exposes AdornedElement and sets DataContext to the CommandProxy of the adorned element
    /// </summary>
    public class CommandToolTip : TouchToolTip
    {
        private static readonly DependencyProperty CommandProxyProperty = DependencyProperty.Register(
            "CommandProxy",
            typeof(ICommand),
            typeof(CommandToolTip),
            new PropertyMetadata(default(ICommand), OnCommandChanged));

        private static readonly DependencyProperty UseAsMouseOverToolTipProperty =
            DependencyProperty.Register(
                "UseAsMouseOverToolTip",
                typeof(bool),
                typeof(CommandToolTip),
                new PropertyMetadata(false, OnUseAsMouseOverToolTipChanged));

        public static readonly DependencyProperty ConditionProperty = DependencyProperty.Register(
            "Condition",
            typeof(ICondition),
            typeof(CommandToolTip),
            new PropertyMetadata(default(ICondition), OnConditionChanged));

        private static readonly DependencyPropertyKey CommandTypePropertyKey = DependencyProperty.RegisterReadOnly(
            "CommandType",
            typeof(Type),
            typeof(CommandToolTip),
            new PropertyMetadata(default(Type)));

        public static readonly DependencyProperty CommandTypeProperty = CommandTypePropertyKey.DependencyProperty;

        private Binding _adornedElementUseTouchToolTipAsMouseOverTooltipBinding;

        private static readonly PropertyPath UseTouchTooltipAsMouseOverPropertyPath = new PropertyPath(TouchToolTipService.UseTouchToolTipAsMouseOverToolTipProperty);

        static CommandToolTip()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CommandToolTip), new FrameworkPropertyMetadata(typeof(CommandToolTip)));
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

        public override void OnToolTipChanged(UIElement adornedElement)
        {
            base.OnToolTipChanged(adornedElement);
            var commandBinding = new Binding(ButtonBase.CommandProperty.Name)
            {
                Source = adornedElement,
                Mode = BindingMode.OneWay
            };
            BindingOperations.SetBinding(this, CommandProxyProperty, commandBinding);
            _adornedElementUseTouchToolTipAsMouseOverTooltipBinding = new Binding
            {
                Path = UseTouchTooltipAsMouseOverPropertyPath,
                Source = adornedElement,
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
        }

        private static void OnCommandChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var commandToolTip = (CommandToolTip)o;
            var command = e.NewValue as IConditionRelayCommand;
            if (command == null)
            {
                commandToolTip.Condition = null;
                commandToolTip.CommandType = null;
                BindingOperations.ClearBinding(o, UseAsMouseOverToolTipProperty);
                return;
            }

            commandToolTip.Condition = command.Condition;
            commandToolTip.CommandType = command.GetType();
        }

        private static void OnConditionChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var commandToolTip = (CommandToolTip)o;
            var multiBinding = new MultiBinding
            {
                Converter = UseTouchToolTipAsMouseOverToolConverter.Default,
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            if (commandToolTip._adornedElementUseTouchToolTipAsMouseOverTooltipBinding != null)
            {
                multiBinding.Bindings.Add(commandToolTip._adornedElementUseTouchToolTipAsMouseOverTooltipBinding);
            }

            var binding = new Binding
            {
                Path = UseTouchTooltipAsMouseOverPropertyPath,
                Source = commandToolTip,
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            multiBinding.Bindings.Add(binding);
            BindingOperations.SetBinding(commandToolTip, UseAsMouseOverToolTipProperty, multiBinding);
        }

        private static void OnUseAsMouseOverToolTipChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((e.NewValue as bool?) == true)
            {
                var commandToolTip = (CommandToolTip)d;
                var source = (DependencyObject)commandToolTip._adornedElementUseTouchToolTipAsMouseOverTooltipBinding.Source;
                ToolTipService.SetShowOnDisabled(source, true);
                ToolTipService.SetToolTip(source, commandToolTip);
            }
        }

        private class UseTouchToolTipAsMouseOverToolConverter : IMultiValueConverter
        {
            public static UseTouchToolTipAsMouseOverToolConverter Default = new UseTouchToolTipAsMouseOverToolConverter();

            private UseTouchToolTipAsMouseOverToolConverter()
            {
            }

            public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            {
                var any = values.OfType<bool>().Any(x => x == true);
                return any;
            }

            public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
    }
}
