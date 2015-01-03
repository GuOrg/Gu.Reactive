namespace Gu.Wpf.Reactive
{
    public class MultiplyConverter : MarkupConverter<double?, double?>
    {
        public double Factor { get; set; }

        protected override double? Convert(double? value)
        {
            return value.HasValue ? value.Value * Factor : default(double?);
        }

        protected override double? ConvertBack(double? value)
        {
            return value.HasValue ? value.Value / Factor : default(double?);
        }
    }
}