namespace Gu.Wpf.Reactive
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;

    using Gu.Wpf.ToolTips;

    /// <summary>
    /// Exposes AdornedElement and sets DataContext to the Command of the adorned element
    /// </summary>
    public class CommandToolTip : ToolTip, ITouchToolTip
    {
        static CommandToolTip()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CommandToolTip), new FrameworkPropertyMetadata(typeof(CommandToolTip)));
        }

        public void OnToolTipChanged(UIElement adornedElement)
        {
            throw new System.NotImplementedException();
            //var frameworkElement = e.NewValue as FrameworkElement;
            //if (frameworkElement != null)
            //{
            //    var binding = new Binding(ButtonBase.CommandProperty.Name)
            //    {
            //        Mode = BindingMode.OneWay,
            //        Source = frameworkElement
            //    };
            //    BindingOperations.SetBinding(o, DataContextProperty, binding);
            //}
        }
    }
}