namespace Gu.Wpf.Reactive
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;

    /// <summary>
    /// Exposes AdornedElement and sets DataContext to the Command of the adorned element
    /// </summary>
    public class CommandToolTip : ToolTip
    {
        /// <summary>
        /// Exposong the adorned element for convenience
        /// </summary>
        public static readonly DependencyProperty AdornedElementProperty = TouchToolTip.AdornedElementProperty.AddOwner(
            typeof(CommandToolTip),
            new PropertyMetadata(
                default(FrameworkElement),
                OnAdornedElementChanged));

        static CommandToolTip()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CommandToolTip), new FrameworkPropertyMetadata(typeof(CommandToolTip)));
        }

        public FrameworkElement AdornedElement
        {
            get
            {
                return (FrameworkElement)GetValue(AdornedElementProperty);
            }
            set
            {
                SetValue(AdornedElementProperty, value);
            }
        }

        private static void OnAdornedElementChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var frameworkElement = e.NewValue as FrameworkElement;
            if (frameworkElement != null)
            {
                var binding = new Binding(ButtonBase.CommandProperty.Name)
                {
                    Mode = BindingMode.OneWay,
                    Source = frameworkElement
                };
                BindingOperations.SetBinding(o, DataContextProperty, binding);
            }
        }
    }
}