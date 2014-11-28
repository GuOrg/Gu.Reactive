namespace Gu.Wpf.Reactive
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;

    internal sealed class TouchToolTipAdorner : Adorner
    {
        private PopupButton _adornerButton;

        static TouchToolTipAdorner()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TouchToolTipAdorner), new FrameworkPropertyMetadata(typeof(TouchToolTipAdorner)));
        }

        /// <summary>
        /// Be sure to call the base class constructor. 
        /// </summary>
        /// <param name="adornedElement"></param>
        /// <param name="overlayTemplate">A style for a PopupButton</param>
        public TouchToolTipAdorner(UIElement adornedElement, ToolTip toolTip, ControlTemplate overlayTemplate)
            : base(adornedElement)
        {
            Debug.Assert(adornedElement != null, "adornedElement should not be null");
            //Debug.Assert(overlayTemplate != null, "adornerTemplate should not be null");
            _adornerButton = new PopupButton
                          {
                              IsTabStop = false,
                          };
            if (overlayTemplate != null)
            {
                _adornerButton.Template = overlayTemplate;
            }
            if (toolTip != null)
            {
                _adornerButton.TouchToolTip = toolTip;

                //toolTip.DataContext = adornedElement;
                // Not sure we want ^, check bindings for DataContext and DataContext == null first 
            }
            else
            {
                _adornerButton.BorderBrush = Brushes.HotPink;
                _adornerButton.BorderThickness = new Thickness(2);
            }
            AddVisualChild(_adornerButton);
        }

        /// <summary>
        /// The clear the single child of a TemplatedAdorner
        /// </summary>
        public void ClearChild()
        {
            RemoveVisualChild(_adornerButton);
            _adornerButton = null;
        }

        protected override int VisualChildrenCount
        {
            get { return _adornerButton != null ? 1 : 0; }
        }

        /// <summary>
        ///   Derived class must implement to support Visual children. The method must return
        ///    the child at the specified index. Index must be between 0 and GetVisualChildrenCount-1.
        ///
        ///    By default a Visual does not have any children.
        ///
        ///  Remark:
        ///       During this virtual call it is not valid to modify the Visual tree.
        /// </summary>
        protected override Visual GetVisualChild(int index)
        {
            if (_adornerButton == null || index != 0)
            {
                throw new ArgumentOutOfRangeException("index", index, "nope: _child == null || index != 0");
            }

            return _adornerButton;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            Debug.Assert(_adornerButton != null, "_child should not be null");
            _adornerButton.Measure(constraint);
            if (AdornedElement != null)
            {
                AdornedElement.InvalidateMeasure();
                AdornedElement.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                return AdornedElement.RenderSize;
            }
            _adornerButton.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            return (_adornerButton).DesiredSize;
        }

        protected override Size ArrangeOverride(Size size)
        {
            Size finalSize;

            finalSize = base.ArrangeOverride(size);

            if (_adornerButton != null)
            {
                _adornerButton.Arrange(new Rect(new Point(), finalSize));
            }
            return finalSize;
        }
    }
}
