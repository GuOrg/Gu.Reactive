namespace Gu.Wpf.Reactive
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;

    public class TouchToolTip : DependencyObject
    {
        private static readonly ConditionalWeakTable<UIElement, TouchToolTip> _cache = new ConditionalWeakTable<UIElement, TouchToolTip>();

        private TouchToolTipAdorner _adorner;
        private readonly FrameworkElement _element;

        public static readonly DependencyProperty TouchToolTipStyleProperty = DependencyProperty.RegisterAttached(
            "TouchToolTipStyle",
            typeof(Style),
            typeof(TouchToolTip),
            new PropertyMetadata(default(Style), OnTouchToolTipStyleChanged));

        public TouchToolTip(FrameworkElement element)
        {
            _element = element;
            this.Initialize();
        }

        public static void SetTouchToolTipStyle(FrameworkElement element, Style value)
        {
            element.SetValue(TouchToolTipStyleProperty, value);
        }

        public static Style GetTouchToolTipStyle(FrameworkElement element)
        {
            return (Style)element.GetValue(TouchToolTipStyleProperty);
        }

        private static void OnTouchToolTipStyleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var element = (FrameworkElement)o;
            TouchToolTip behavior;
            if (!_cache.TryGetValue(element, out behavior))
            {
                behavior = new TouchToolTip(element);
                _cache.Add(element, behavior);
                if (behavior._adorner != null)
                {
                    behavior._adorner.SetValue(FrameworkElement.StyleProperty, e.NewValue);
                }
            }
        }

        private void Initialize()
        {
            if (_element.IsVisible)
            {
                _element.IsVisibleChanged -= ElementOnIsVisibleChanged;
                this.AddAdorner();
            }
            else
            {
                _element.IsVisibleChanged += ElementOnIsVisibleChanged;
            }
        }

        private void ElementOnIsVisibleChanged(object o, DependencyPropertyChangedEventArgs e)
        {
            this.AddAdorner();
        }

        private void AddAdorner()
        {
            var myAdornerLayer = AdornerLayer.GetAdornerLayer(_element);
            this._adorner = new TouchToolTipAdorner(_element);
            _adorner.SetValue(FrameworkElement.StyleProperty, _element.GetValue(TouchToolTipStyleProperty));
            myAdornerLayer.Add(this._adorner);
        }
    }
}
