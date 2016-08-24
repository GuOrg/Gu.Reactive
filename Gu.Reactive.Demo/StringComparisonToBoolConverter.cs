namespace Gu.Reactive.Demo
{
    using System;
    using System.Globalization;
    using Wpf.Reactive;

    public class StringComparisonToBoolConverter : MarkupConverter<StringComparison?, bool?>
    {
        public static readonly string GroupName = "StringComparison";

        public StringComparisonToBoolConverter()
        {
        }

        public StringComparison TrueWhen { get; set; }

        protected override bool? Convert(StringComparison? value, CultureInfo culture)
        {
            return value == TrueWhen;
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
