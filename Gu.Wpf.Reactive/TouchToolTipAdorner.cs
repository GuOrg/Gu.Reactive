namespace Gu.Wpf.Reactive
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Media;

    public class TouchToolTipAdorner : Adorner
    {
        private readonly Button _adornerButton;
        private readonly Popup _popup;

        public static readonly DependencyProperty AdornerContentProperty = DependencyProperty.Register(
            "AdornerContent",
            typeof(object),
            typeof(TouchToolTipAdorner),
            new PropertyMetadata(null, OnAdornerContentChanged));

        public static readonly DependencyProperty PopUpStyleProperty = DependencyProperty.Register(
            "PopUpStyle",
            typeof(Style),
            typeof(TouchToolTipAdorner),
            new PropertyMetadata(default(Style), OnPopUpStyleChanged));

        public static readonly DependencyProperty PopUpContentProperty = DependencyProperty.Register(
            "PopUpContent",
            typeof(object),
            typeof(TouchToolTipAdorner),
            new PropertyMetadata(default(object), OnPopUpContentChanged));

        public static readonly DependencyProperty PopUpContentTemplateProperty = DependencyProperty.Register(
            "PopUpContentTemplate",
            typeof(DataTemplate),
            typeof(TouchToolTipAdorner),
            new PropertyMetadata(default(DataTemplate), OnPopUpContentTemplateChanged));

        private readonly ContentPresenter _popUpContentPresenter;

        static TouchToolTipAdorner()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TouchToolTipAdorner), new FrameworkPropertyMetadata(typeof(TouchToolTipAdorner)));
        }

        // Be sure to call the base class constructor. 
        public TouchToolTipAdorner(FrameworkElement button)
            : base(button)
        {
            var presenter = new FrameworkElementFactory(typeof(ContentPresenter));
            _adornerButton = new Button
                          {
                              Template = new ControlTemplate(typeof(Button))
                                             {
                                                 VisualTree = presenter
                                             },
                              Content = AdornerContent
                          };
            this._popUpContentPresenter = new ContentPresenter
                                        {
                                            Content = AdornedElement,
                                            ContentTemplate = PopUpContentTemplate
                                        };
            _popup = new Popup
            {
                PlacementTarget = this,
                DataContext = button,
                Style = PopUpStyle,
                Child = this._popUpContentPresenter
            };
            AddVisualChild(this._adornerButton);
            AddLogicalChild(this._adornerButton);

            this.SetupSubscriptions();
            AdornedElement.IsEnabledChanged += this.AdornedElementChanged;
            AdornedElement.IsVisibleChanged += this.AdornedElementChanged;

        }

        private void AdornedElementChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.Visibility = AdornedElement.IsVisible && AdornedElement.IsEnabled
                                  ? Visibility.Hidden
                                  : Visibility.Visible;
        }

        public object AdornerContent
        {
            get
            {
                return (object)GetValue(AdornerContentProperty);
            }
            set
            {
                SetValue(AdornerContentProperty, value);
            }
        }

        public Style PopUpStyle
        {
            get
            {
                return (Style)GetValue(PopUpStyleProperty);
            }
            set
            {
                SetValue(PopUpStyleProperty, value);
            }
        }

        public object PopUpContent
        {
            get
            {
                return (object)GetValue(PopUpContentProperty);
            }
            set
            {
                SetValue(PopUpContentProperty, value);
            }
        }

        public DataTemplate PopUpContentTemplate
        {
            get
            {
                return (DataTemplate)GetValue(PopUpContentTemplateProperty);
            }
            set
            {
                SetValue(PopUpContentTemplateProperty, value);
            }
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return 1;
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index == 0)
            {
                return _adornerButton;
            }
            throw new ArgumentOutOfRangeException("index");
        }

        protected override Size MeasureOverride(Size constraint)
        {
            _adornerButton.Measure(constraint);
            var frameworkElement = ((FrameworkElement)AdornedElement);
            return new Size(frameworkElement.ActualWidth, frameworkElement.ActualHeight);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _adornerButton.Arrange(new Rect(new Point(0, 0), finalSize));
            return finalSize;
        }

        private static void OnAdornerContentChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var adorner = (TouchToolTipAdorner)o;
            adorner._adornerButton.SetValue(ContentControl.ContentProperty, e.NewValue);
        }

        private static void OnPopUpStyleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var adorner = (TouchToolTipAdorner)o;
            adorner._popup.SetValue(StyleProperty, e.NewValue);
        }

        private static void OnPopUpContentChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var adorner = (TouchToolTipAdorner)o;
            adorner._popUpContentPresenter.SetValue(ContentControl.ContentProperty, e.NewValue);
        }

        private static void OnPopUpContentTemplateChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var adorner = (TouchToolTipAdorner)o;
            adorner._popUpContentPresenter.SetValue(ContentControl.ContentTemplateProperty, e.NewValue);
        }

        /// <summary>
        /// Attach to the events needed for handling open/close
        /// </summary>
        private void SetupSubscriptions()
        {
            this._adornerButton.Click += (sender, args) =>
            {
                Debug.WriteLine("_adornerButton.Click");
                this._popup.IsOpen = !this._popup.IsOpen;
            };
            this.AdornedElement.LostFocus += (sender, args) =>
            {
                Debug.WriteLine("_button.LostFocus");
                if (this._popup.IsOpen)
                {
                    this._popup.IsOpen = false;
                }
            };
            this._adornerButton.LostFocus += (sender, args) =>
            {
                Debug.WriteLine("_adornerButton.LostFocus");
                if (this._popup.IsOpen && !this._popup.IsKeyboardFocusWithin)
                {
                    this._popup.IsOpen = false;
                }
            };
            this._popup.LostFocus += (sender, args) =>
            {
                Debug.WriteLine("_popup.LostFocus");
                if (this._popup.IsOpen && !(this._adornerButton.IsKeyboardFocusWithin || this._popup.IsKeyboardFocusWithin))
                {
                    this._popup.IsOpen = false;
                }
            };
        }
    }
}
