namespace Gu.Wpf.Reactive
{
    using System.Windows.Media;

    /// <summary>
    /// Usage:
    /// Visibility="{Binding SomeProperty, Converter={common:BooleanToVisibilityConverter WhenTrue=Green, WhenFalse=Red, WhenNull=Gray}}"
    /// No resource declaration is necessary
    /// </summary>
    public class BooleanToBrushConverter : BooleanToXConverter<Brush>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BooleanToBrushConverter"/> class
        /// </summary>
        public BooleanToBrushConverter()
            : base()
        {
        }
    }
}