namespace Gu.Wpf.Reactive
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Media;

    public class ConditionInfoAdorner : Adorner, IWeakEventListener
    {
        private readonly ButtonBase _button;
        private readonly Button _adornerButton;
        private readonly Popup _popup;

        public static readonly DependencyProperty AdornerContentProperty = DependencyProperty.Register(
            "AdornerContent",
            typeof(object),
            typeof(ConditionInfoAdorner),
            new PropertyMetadata(null, PropertyChangedCallback));

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

        static ConditionInfoAdorner()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ConditionInfoAdorner), new FrameworkPropertyMetadata(typeof(ConditionInfoAdorner)));
        }

        // Be sure to call the base class constructor. 
        public ConditionInfoAdorner(ButtonBase button)
            : base(button)
        {
            var presenter = new FrameworkElementFactory(typeof(ContentPresenter));
            _adornerButton = new Button
                          {
                              Background = Brushes.Transparent,
                              HorizontalContentAlignment = HorizontalAlignment.Right,
                              VerticalContentAlignment = VerticalAlignment.Top,
                              FocusVisualStyle = null,
                              SnapsToDevicePixels = true,
                              Template = new ControlTemplate(typeof(Button))
                                             {
                                                 VisualTree = presenter
                                             },
                              Content = AdornerContent
                          };

            AddVisualChild(this._adornerButton);
            AddLogicalChild(this._adornerButton);
            _button = button;
            _popup = new Popup
            {
                PlacementTarget = _button,
                Placement = PlacementMode.Bottom,
                DataContext = _button.DataContext,
                Child = new Border
                {
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(2),
                    Background = Brushes.White,
                    Focusable = false,
                    Child = new ConditionControl
                    {
                        Focusable = false,
                        Condition = ((ConditionRelayCommand)_button.Command).Condition,
                    }
                },
            };
            _adornerButton.Click += (sender, args) =>
                {
                    Debug.WriteLine("_adornerButton.Click");
                    _popup.IsOpen = !_popup.IsOpen;
                };
            _button.LostFocus += (sender, args) =>
            {
                Debug.WriteLine("_button.LostFocus");
                if (_popup.IsOpen)
                {
                    _popup.IsOpen = false;
                }
            };
            _adornerButton.LostFocus += (sender, args) =>
            {
                Debug.WriteLine("_adornerButton.LostFocus");
                if (_popup.IsOpen && !_popup.IsKeyboardFocusWithin)
                {
                    _popup.IsOpen = false;
                }
            };
            _popup.LostFocus += (sender, args) =>
            {
                Debug.WriteLine("_popup.LostFocus");
                if (_popup.IsOpen && !(_adornerButton.IsKeyboardFocusWithin || _popup.IsKeyboardFocusWithin))
                {
                    _popup.IsOpen = false;
                }
            };
            Gu.Wpf.Reactive.CanExecuteChangedEventManager.AddListener(_button.Command, this);
        }

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            if (managerType != typeof(Gu.Wpf.Reactive.CanExecuteChangedEventManager))
            {
                return false;
            }
            var command = _button.Command;
            if (command != null)
            {
                this.Visibility = command.CanExecute(_button.CommandParameter)
                    ? Visibility.Hidden
                    : Visibility.Visible;
            }
            else
            {
                this.Visibility = Visibility.Hidden;
            }
            return true;
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
            return new Size(_button.ActualWidth, _button.ActualHeight);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _adornerButton.Arrange(new Rect(new Point(0, 0), finalSize));
            return finalSize;
        }

        private static void PropertyChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var adorner = (ConditionInfoAdorner)o;
            adorner._adornerButton.SetValue(ContentControl.ContentProperty, e.NewValue);
        }
    }
}
