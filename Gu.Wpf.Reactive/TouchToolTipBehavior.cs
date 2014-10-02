namespace Gu.Wpf.Reactive
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Interactivity;

    public class TouchToolTipBehavior : DependencyObject //Behavior<Button>
    {
        private static ConditionalWeakTable<UIElement, TouchToolTipBehavior> _cache = new ConditionalWeakTable<UIElement, TouchToolTipBehavior>();
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ShowInfoWhenDisabledProperty = DependencyProperty.RegisterAttached(
            "ShowInfoWhenDisabled",
            typeof(bool),
            typeof(TouchToolTipBehavior),
            new PropertyMetadata(false, OnShowInfoWhenDisabledChanged));

        private TouchToolTipAdorner _adorner;
        private readonly FrameworkElement _element;

        public TouchToolTipBehavior()
        {
            throw new NotImplementedException("Get parent somehow");
        }

        public TouchToolTipBehavior(FrameworkElement element)
        {
            _element = element;
            this.Initialize();
        }

        public static void SetShowInfoWhenDisabled(FrameworkElement element, bool value)
        {
            element.SetValue(ShowInfoWhenDisabledProperty, value);
        }

        public static bool GetShowInfoWhenDisabled(FrameworkElement element)
        {
            return (bool)element.GetValue(ShowInfoWhenDisabledProperty);
        }

        private static void OnShowInfoWhenDisabledChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var element = (FrameworkElement)o;
            TouchToolTipBehavior behavior;
            if (!_cache.TryGetValue(element, out behavior))
            {
                behavior = new TouchToolTipBehavior(element);
                _cache.Add(element, behavior);
            }

            if (behavior._adorner != null)
            {
                behavior._adorner.Visibility = ((bool)e.NewValue) ? Visibility.Visible : Visibility.Hidden;
            }
        }

        private void Initialize()
        {
            if ((bool)_element.GetValue(ShowInfoWhenDisabledProperty))
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
        }

        private void ElementOnIsVisibleChanged(object o, DependencyPropertyChangedEventArgs e)
        {
            this.AddAdorner();
        }

        private void AddAdorner()
        {
            var myAdornerLayer = AdornerLayer.GetAdornerLayer(_element);
            this._adorner = new TouchToolTipAdorner(_element);
            _adorner.Visibility = (bool)_element.GetValue(ShowInfoWhenDisabledProperty) ? Visibility.Visible : Visibility.Hidden;
            myAdornerLayer.Add(this._adorner);
        }
    }
}
