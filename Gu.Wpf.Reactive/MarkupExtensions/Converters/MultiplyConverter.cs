namespace Gu.Wpf.Reactive
{
    using System;
    using System.Globalization;

    public class MultiplyConverter : MarkupConverter<double?, double?>
    {
        public MultiplyConverter()
            : base()
        {
            this.Factor = 1;
        }

        public double Factor { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        protected override double? Convert(double? value, CultureInfo culture)
        {
            return value * this.Factor;
        }

        protected override double? ConvertBack(double? value, CultureInfo culture)
        {
            return value / this.Factor;
        }
    }
}