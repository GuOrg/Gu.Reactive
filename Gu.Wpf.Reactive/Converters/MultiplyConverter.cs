namespace Gu.Wpf.Reactive
{
    using System;
    using System.ComponentModel;
    using System.Globalization;

    public class MultiplyConverter : MarkupConverter<double?, double?>
    {
        //private object _factor;
        public MultiplyConverter()
            : base(NullableDoubleConverter.Default, NullableDoubleConverter.Default)
        {
        }

        public double Factor { get; set; }
        //{
        //    get { return _factor; }
        //    set
        //    {
        //        if (!NullableDoubleConverter.Default.CanConvertTo(value, CultureInfo.CurrentUICulture))
        //        {
        //            throw new ArgumentException("Not a valid factor, expected int or double", "value");
        //        }
        //        _factor = NullableDoubleConverter.Default.ConvertTo(value, CultureInfo.CurrentUICulture);
        //    }
        //}

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return base.ProvideValue(serviceProvider);
        }

        protected override double? Convert(double? value, CultureInfo culture)
        {
            return value.HasValue
                ? value.Value * Factor
                : default(double?);
        }

        protected override double? ConvertBack(double? value, CultureInfo culture)
        {
            return value.HasValue
                ? value.Value / Factor
                : default(double?);
        }
    }
}