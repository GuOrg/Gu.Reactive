namespace Gu.Wpf.Reactive
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;

    public class PopupButton : Button
    {
        public static readonly DependencyProperty TouchToolTipProperty = DependencyProperty.Register(
            "TouchToolTip", 
            typeof(ToolTip), 
            typeof(PopupButton), 
            new PropertyMetadata(
                default(ToolTip),
                OnTouchToolTipChanged));

        static PopupButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PopupButton), new FrameworkPropertyMetadata(typeof(PopupButton)));
        }

        public PopupButton()
        {
            AddHandler(PreviewMouseLeftButtonDownEvent, new RoutedEventHandler(OnClick), true);
            LostFocus += OnLostFocus;
        }

        public ToolTip TouchToolTip
        {
            get
            {
                return (ToolTip)GetValue(TouchToolTipProperty);
            }
            set
            {
                SetValue(TouchToolTipProperty, value);
            }
        }

        private static void OnTouchToolTipChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var oldTip = e.OldValue as ToolTip;
            if (oldTip != null)
            {
                oldTip.PlacementTarget = null;
            }
            var newTip = e.NewValue as ToolTip;
            var uiElement = o as UIElement;
            if (newTip != null && uiElement != null)
            {
                newTip.PlacementTarget = uiElement;
            }
        }

        private void OnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            Debug.WriteLine("OnClick");
            var toolTip = TouchToolTip;
            if (toolTip == null)
            {
                Debug.WriteLine("toolTip == null");
                return;
            }
            toolTip.IsOpen = !toolTip.IsOpen;
            Debug.WriteLine(toolTip.IsOpen);
        }

        private void OnLostFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            Debug.WriteLine("OnLostFocus");
            var toolTip = TouchToolTip;
            if (toolTip == null)
            {
                Debug.WriteLine("toolTip == null");
                return;
            }
            if (toolTip.IsOpen && !(this.IsKeyboardFocusWithin || toolTip.IsKeyboardFocusWithin))
            {
                toolTip.IsOpen = false;
                Debug.WriteLine(toolTip.IsOpen);
            }
        }
    }
}
