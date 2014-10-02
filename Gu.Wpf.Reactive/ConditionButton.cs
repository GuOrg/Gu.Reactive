using System.Windows;

namespace Gu.Wpf.Reactive
{
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    /// <summary>
    /// A button to be used with ConditionRelayCommand
    /// </summary>
    public class ConditionButton : Button
    {
        public new static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            "Command",
            typeof (ConditionRelayCommand),
            typeof (ConditionButton), 
            new PropertyMetadata(default(ConditionRelayCommand)));

        public static readonly DependencyProperty AdornerContentProperty = DependencyProperty.Register(
            "AdornerContent", 
            typeof (UIElement),
            typeof (ConditionButton), 
            new PropertyMetadata(default(UIElement)));

        public static readonly DependencyProperty AdornerTemplateProperty = DependencyProperty.Register(
            "AdornerTemplate",
            typeof (DataTemplate), 
            typeof (ConditionButton), 
            new PropertyMetadata(default(DataTemplate)));

        public static readonly DependencyProperty PopupContentProperty = DependencyProperty.Register(
            "PopupContent", 
            typeof (UIElement), 
            typeof (ConditionButton), 
            new PropertyMetadata(default(UIElement)));

        public static readonly DependencyProperty PopupTemplateProperty = DependencyProperty.Register(
            "PopupTemplate",
            typeof (DataTemplate),
            typeof (ConditionButton), 
            new PropertyMetadata(default(DataTemplate)));

        static ConditionButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ConditionButton), new FrameworkPropertyMetadata(typeof(ConditionButton)));
        }

        /// <summary>
        /// Hacking it with new to change type
        /// </summary>
        public new ConditionRelayCommand Command
        {
            get { return (ConditionRelayCommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public UIElement AdornerContent
        {
            get { return (UIElement)GetValue(AdornerContentProperty); }
            set { SetValue(AdornerContentProperty, value); }
        }

        public DataTemplate AdornerTemplate
        {
            get { return (DataTemplate)GetValue(AdornerTemplateProperty); }
            set { SetValue(AdornerTemplateProperty, value); }
        }

        public UIElement PopupContent
        {
            get { return (UIElement)GetValue(PopupContentProperty); }
            set { SetValue(PopupContentProperty, value); }
        }

        public DataTemplate PopupTemplate
        {
            get { return (DataTemplate)GetValue(PopupTemplateProperty); }
            set { SetValue(PopupTemplateProperty, value); }
        }
    }
}
