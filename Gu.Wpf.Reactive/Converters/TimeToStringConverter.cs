namespace Gu.Wpf.Reactive
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using Gu.Reactive;

    [ValueConversion(typeof(ConditionHistoryPoint), typeof(string))]
    public sealed class TimeToStringConverter : IValueConverter
    {
        /// <summary> Gets the default instance </summary>
        public static readonly TimeToStringConverter LongTime = new TimeToStringConverter("T");

        private readonly string format;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeToStringConverter"/> class.
        /// </summary>
        /// <param name="format">The string format for the timestamp.</param>
        public TimeToStringConverter(string format)
        {
            this.format = format;
        }

        /// <inheritdoc/>
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value switch
            {
                ConditionHistoryPoint p => p.TimeStamp.ToLocalTime().ToString(this.format, DateTimeFormatInfo.CurrentInfo),
                _ => throw new ArgumentException($"Expected {nameof(ConditionHistoryPoint)}", nameof(value)),
            };
        }

        /// <inheritdoc/>
        object IValueConverter.ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotSupportedException($"{nameof(TimeToStringConverter)} can only be used in OneWay bindings");
        }
    }
}
