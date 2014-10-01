namespace Gu.Reactive.Demo
{
    using System;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;

    public class DisabledInfoAdorner : Adorner, IWeakEventListener
    {
        private readonly SolidColorBrush _transparentBrush = new SolidColorBrush(Colors.AliceBlue);
        private readonly ButtonBase _button;
        private readonly Border _border;

        [Obsolete("Weak event here")]
        // Be sure to call the base class constructor. 
        public DisabledInfoAdorner(ButtonBase button)
            : base(button)
        {
            //HorizontalAlignment = HorizontalAlignment.Stretch;
            //VerticalAlignment = VerticalAlignment.Stretch;
            _border = new Border
                          {
                              HorizontalAlignment = HorizontalAlignment.Stretch,
                              VerticalAlignment = VerticalAlignment.Stretch,
                              Background = this._transparentBrush
                          };
            AddVisualChild(this._border);
            AddLogicalChild(this._border);
            _button = button;
            Gu.Wpf.Reactive.CanExecuteChangedEventManager.AddListener(_button.Command, this);
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
                return _border;
            }
            throw new ArgumentOutOfRangeException("index");
        }

        protected override Size MeasureOverride(Size constraint)
        {
            _border.Measure(constraint);
            return _border.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _border.Arrange(new Rect(new Point(0, 0), finalSize));
            return finalSize;
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (!_button.IsEnabled)
            {
                MessageBox.Show("Clicked adorner");
                base.OnMouseLeftButtonDown(e);
                e.Handled = true;
                return;
            }
            e.Handled = false;
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
    }
}
