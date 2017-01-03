#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements must be documented
namespace Gu.Wpf.Reactive
{
    using System;

    /// <summary>
    /// Usage:
    /// Visibility="{Binding SomeProperty, Converter={common:BooleanToVisibilityConverter WhenTrue=true, WhenFalse=false, WhenNull=false}}"
    /// No resource declaration is necessary
    /// </summary>
    [Obsolete("To be removed.")]
    public class NullableBooleanToBooleanConverter : BooleanToXConverter<bool?>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NullableBooleanToBooleanConverter"/> class
        /// </summary>
        public NullableBooleanToBooleanConverter()
        {
            this.WhenTrue = true;
            this.WhenFalse = false;
            this.WhenNull = false;
        }
    }
}