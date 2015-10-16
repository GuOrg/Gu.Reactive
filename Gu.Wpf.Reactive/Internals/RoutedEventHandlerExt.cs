namespace Gu.Wpf.Reactive
{
    using System;
    using System.Windows;

    internal static class RoutedEventHandlerExt
    {
        internal static void UpdateHandler(this UIElement element, RoutedEvent routedEvent, Delegate handler)
        {
            element.RemoveHandler(routedEvent, handler);
            element.AddHandler(routedEvent, handler);
        }
    }
}