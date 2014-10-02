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
        private static readonly DependencyPropertyKey AdornedElementPropertyKey = DependencyProperty.RegisterReadOnly(
            "AdornedElement",
            typeof(FrameworkElement),
            typeof(TouchToolTipAdorner),
            new PropertyMetadata((FrameworkElement)null));

        public static readonly DependencyProperty AdornedElementProperty = AdornedElementPropertyKey.DependencyProperty;

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
     
        private readonly Button _adornerButton;
        private readonly Popup _popup;
        private readonly ContentPresenter _popUpContentPresenter;

        //static TouchToolTipAdorner()
        //{
        //    DefaultStyleKeyProperty.OverrideMetadata(typeof(TouchToolTipAdorner), new FrameworkPropertyMetadata(typeof(TouchToolTipAdorner)));
        //}

        // Be sure to call the base class constructor. 
        public TouchToolTipAdorner(FrameworkElement element)
            : base(element)
        {
            AdornedElement = element;
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
                                            Content = PopUpContent,
                                            ContentTemplate = PopUpContentTemplate
                                        };
            _popup = new Popup
            {
                PlacementTarget = this,
                DataContext = element,
                Style = PopUpStyle,
                Child = this._popUpContentPresenter
            };
            AddVisualChild(this._adornerButton);
            AddLogicalChild(this._adornerButton);

            this.SetupSubscriptions();
            base.AdornedElement.IsEnabledChanged += this.AdornedElementStateChanged;
            base.AdornedElement.IsVisibleChanged += this.AdornedElementStateChanged;

        }

        public new FrameworkElement AdornedElement
        {
            get { return (FrameworkElement)GetValue(AdornedElementProperty); }
            private set { SetValue(AdornedElementPropertyKey, value); }
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
            var frameworkElement = ((FrameworkElement)base.AdornedElement);
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
            base.AdornedElement.LostFocus += (sender, args) =>
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

        private void AdornedElementStateChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.Visibility = base.AdornedElement.IsVisible && base.AdornedElement.IsEnabled
                                  ? Visibility.Hidden
                                  : Visibility.Visible;
        }
    }
}
