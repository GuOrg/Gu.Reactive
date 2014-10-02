namespace Gu.Wpf.Reactive
{
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Documents;
    /// <summary>
    /// Helper class with attached properties for the TouchToolTipAdorner
    /// Enables using it in syles
    /// </summary>
    public class TouchToolTip : DependencyObject
    {
        private static readonly ConditionalWeakTable<UIElement, TouchToolTip> Cache = new ConditionalWeakTable<UIElement, TouchToolTip>();
        private TouchToolTipAdorner _adorner;
        private readonly FrameworkElement _element;

        /// <summary>
        /// Set the style to enable the TouchTooltipAdorner, sample:
        /// <Button Width="100">
        ///     <reactiveUi:TouchToolTip.Style>
        ///        <Style TargetType="{x:Type reactiveUi:TouchToolTipAdorner}">
        ///           ...
        /// or:
        /// <ResourceDictionary> 
        ///   <ResourceDictionary.MergedDictionaries>
        ///     <ResourceDictionary Source="/Gu.Wpf.Reactive;component/Themes/TouchToolTip.xaml" />
        ///   </ResourceDictionary.MergedDictionaries>
        ///  <Style TargetType="{x:Type Button}">
        ///    <Setter Property="reactiveUi:TouchToolTip.Style" Value="{StaticResource CondtionToolTipStyle}" />
        ///    ...
        ///  </Style>
        /// </ResourceDictionary>
        /// </summary>
        public static readonly DependencyProperty StyleProperty = DependencyProperty.RegisterAttached(
            "Style",
            typeof(Style),
            typeof(TouchToolTip),
            new PropertyMetadata(default(Style), OnStyleChanged));

        public TouchToolTip(FrameworkElement element)
        {
            _element = element;
            this.Initialize();
        }

        public static void SetStyle(FrameworkElement element, Style value)
        {
            element.SetValue(StyleProperty, value);
        }

        public static Style GetStyle(FrameworkElement element)
        {
            return (Style)element.GetValue(StyleProperty);
        }

        private static void OnStyleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var element = (FrameworkElement)o;
            TouchToolTip behavior;
            if (!Cache.TryGetValue(element, out behavior))
            {
                behavior = new TouchToolTip(element);
                Cache.Add(element, behavior);
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
            _adorner.SetValue(FrameworkElement.StyleProperty, _element.GetValue(StyleProperty));
            myAdornerLayer.Add(this._adorner);
        }
    }
}
