namespace Gu.Wpf.Reactive
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Interactivity;
    using System.Windows.Media;

    public class DisabledInfoBehavior : Behavior<Button>
    {
        public static readonly DependencyProperty AdornerProperty = DependencyProperty.Register(
            "Adorner",
            typeof(FrameworkElement),
            typeof(DisabledInfoBehavior),
            new FrameworkPropertyMetadata(default(FrameworkElement), FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

        public static readonly DependencyProperty PopUpProperty = DependencyProperty.Register(
            "PopUp",
            typeof(FrameworkElement),
            typeof(DisabledInfoBehavior),
            new PropertyMetadata(default(FrameworkElement)));

        public FrameworkElement Adorner
        {
            get
            {
                return (FrameworkElement)GetValue(AdornerProperty);
            }
            set
            {
                SetValue(AdornerProperty, value);
            }
        }

        public FrameworkElement PopUp
        {
            get
            {
                return (FrameworkElement)GetValue(PopUpProperty);
            }
            set
            {
                SetValue(PopUpProperty, value);
            }
        }

        protected override void OnAttached()
        {
            if (AssociatedObject.IsLoaded)
            {
                this.AddAdorner();
            }
            else
            {
                AssociatedObject.Loaded += (sender, args) => this.AddAdorner();
            }
        }

        private void AddAdorner()
        {
            var myAdornerLayer = AdornerLayer.GetAdornerLayer(AssociatedObject);
            //var adorner = new ConditionInfoAdorner(AssociatedObject) { AdornerContent = this.Adorner };
            var adorner = new ConditionInfoAdorner(AssociatedObject);
            if (Adorner != null)
            {
                adorner.AdornerContent = Adorner;
            }
            myAdornerLayer.Add(adorner);
        }
    }
}
