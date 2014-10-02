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

        private TouchToolTipAdorner _adorner;

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
            var disabledInfoBehavior = (DisabledInfoBehavior)behaviors.FirstOrDefault(b => b is DisabledInfoBehavior);
            if (disabledInfoBehavior == null)
            {
                disabledInfoBehavior = new DisabledInfoBehavior();
                behaviors.Add(disabledInfoBehavior);
            }
            if (disabledInfoBehavior._adorner != null)
            {
                disabledInfoBehavior._adorner.Visibility = ((bool)e.NewValue) ? Visibility.Visible : Visibility.Hidden;
            }
        }

        private void AddAdorner()
        {
            var myAdornerLayer = AdornerLayer.GetAdornerLayer(AssociatedObject);
            this._adorner = new TouchToolTipAdorner(this.AssociatedObject);
            _adorner.Visibility = (bool)AssociatedObject.GetValue(ShowInfoWhenDisabledProperty) ? Visibility.Visible : Visibility.Hidden;
            myAdornerLayer.Add(this._adorner);
        }
    }
}
