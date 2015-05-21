namespace Gu.Reactive.Demo
{
    using System;
    using System.Globalization;
    using Wpf.Reactive;

    public class StringComparisonToBoolConverter : MarkupConverter<StringComparison?, bool?>
    {
        public StringComparisonToBoolConverter()
        {
        }

        public StringComparison TrueWhen { get; set; }
        protected override bool? Convert(StringComparison? value, CultureInfo culture)
        {
            if (value == TrueWhen)
            {
                return true;
            }
            return false;
        }

        protected override StringComparison? ConvertBack(bool? value, CultureInfo culture)
        {
            if (value == true)
            {
                return TrueWhen;
            }
            return null;
        }
    }
}
