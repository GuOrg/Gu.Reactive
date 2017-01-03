#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements must be documented
namespace Gu.Wpf.Reactive
{
    using System;
    using System.Windows.Media;

    /// <summary>
    /// Usage:
    /// Visibility="{Binding SomeProperty, Converter={common:BooleanToVisibilityConverter WhenTrue=Green, WhenFalse=Red, WhenNull=Gray}}"
    /// No resource declaration is necessary
    /// </summary>
    [Obsolete("To be removed.")]
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