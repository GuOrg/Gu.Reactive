namespace Gu.Wpf.Reactive
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Interactivity;
    using System.Windows.Media;

    public class DisabledInfoBehavior : Behavior<Button>
    {
        public static readonly DependencyProperty ShowInfoWhenDisabledProperty = DependencyProperty.RegisterAttached(
            "ShowInfoWhenDisabled",
            typeof(bool),
            typeof(DisabledInfoBehavior),
            new PropertyMetadata(false, OnShowInfoWhenDisabledChanged));

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

        private ConditionInfoAdorner _adorner;

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

        public static void SetShowInfoWhenDisabled(Button element, bool value)
        {
            element.SetValue(ShowInfoWhenDisabledProperty, value);
        }

        public static bool GetShowInfoWhenDisabled(Button element)
        {
            return (bool)element.GetValue(ShowInfoWhenDisabledProperty);
        }

        protected override void OnAttached()
        {
            if ((bool)AssociatedObject.GetValue(ShowInfoWhenDisabledProperty))
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
        }

        private static void OnShowInfoWhenDisabledChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var button = (Button)o;
            var behaviors = Interaction.GetBehaviors(button);
            if (!behaviors.Any(b => b is DisabledInfoBehavior))
            {
                 behaviors.Add(new DisabledInfoBehavior());
            }
        }

        private void AddAdorner()
        {
            var myAdornerLayer = AdornerLayer.GetAdornerLayer(AssociatedObject);
            //var adorner = new ConditionInfoAdorner(AssociatedObject) { AdornerContent = this.Adorner };
            this._adorner = new ConditionInfoAdorner(this.AssociatedObject);
            if (Adorner != null)
            {
                this._adorner.AdornerContent = Adorner;
            }
            myAdornerLayer.Add(this._adorner);
        }
    }
}
