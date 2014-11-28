namespace Gu.Wpf.Reactive
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    /// <summary>
    /// Exposes AdornedElement and sets DataContext to 
    /// </summary>
    public class TouchToolTip : ToolTip
    {
        public static readonly DependencyProperty AdornedElementProperty = DependencyProperty.Register(
            "AdornedElement",
            typeof(FrameworkElement),
            typeof(TouchToolTip),
            new PropertyMetadata(default(FrameworkElement), OnAdornedElementChanged));

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
                var binding = new Binding(DataContextProperty.Name)
                                  {
                                      Mode = BindingMode.OneWay, 
                                      Source = frameworkElement
                                  };
                BindingOperations.SetBinding(o, DataContextProperty, binding);
            }
        }
    }
}
