namespace Gu.Wpf.Reactive
{
    using System.Windows;

    /// <summary>
    /// Usage:
    /// Visibility="{Binding SomeProperty, Converter={common:BooleanToVisibilityConverter WhenTrue=Visible, WhenFalse=Collapsed, WhenNull=Collapsed}}"
    /// No resource declaration is necessary
    /// </summary>
    public class BooleanToVisibilityConverter : BooleanToXConverter<Visibility?>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BooleanToVisibilityConverter"/> class
        /// </summary>
        public BooleanToVisibilityConverter()
            : base()
        {
            this.WhenTrue = Visibility.Visible;
            this.WhenFalse = Visibility.Collapsed;
            this.WhenNull = Visibility.Collapsed;
        }
    }
}
