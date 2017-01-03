#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements must be documented
namespace Gu.Wpf.Reactive
{
    using System;
    using System.Globalization;

    /// <inheritdoc/>
    [Obsolete("To be removed.")]
    public class MultiplyConverter : MarkupConverter<double?, double?>
    {
        public MultiplyConverter()
            : base()
        {
            this.Factor = 1;
        }

        public double Factor { get; set; }

        /// <inheritdoc/>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        /// <inheritdoc/>
        protected override double? Convert(double? value, CultureInfo culture)
        {
            return value * this.Factor;
        }

        /// <inheritdoc/>
        protected override double? ConvertBack(double? value, CultureInfo culture)
        {
            return value / this.Factor;
        }
    }
}