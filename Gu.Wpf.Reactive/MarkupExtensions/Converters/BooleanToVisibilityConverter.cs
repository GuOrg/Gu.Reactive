#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements must be documented
namespace Gu.Wpf.Reactive
{
    using System;
    using System.Windows;

    /// <summary>
    /// Usage:
    /// Visibility="{Binding SomeProperty, Converter={common:BooleanToVisibilityConverter WhenTrue=Visible, WhenFalse=Collapsed, WhenNull=Collapsed}}"
    /// No resource declaration is necessary
    /// </summary>
    [Obsolete("To be removed.")]
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
