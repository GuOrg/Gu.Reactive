namespace Gu.Wpf.Reactive
{
    /// <summary>
    /// Usage:
    /// Visibility="{Binding SomeProperty, Converter={common:BooleanToVisibilityConverter WhenTrue=true, WhenFalse=false, WhenNull=false}}"
    /// No resource declaration is necessary
    /// </summary>
    public class NullableBooleanToBooleanConverter : BooleanToXConverter<bool?>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NullableBooleanToBooleanConverter"/> class
        /// </summary>
        public NullableBooleanToBooleanConverter()
            : base()
        {
            this.WhenTrue = true;
            this.WhenFalse = false;
            this.WhenNull = false;
        }
    }
}