namespace Gu.Wpf.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Data;
    using Gu.Reactive;

    /// <summary>
    /// Formats condition history as a string for display in tooltip.
    /// </summary>
    [ValueConversion(typeof(ConditionHistoryPoint), typeof(string))]
    public sealed class ConditionHistoryToStringConverter : IValueConverter
    {
        /// <summary> Gets the default instance.</summary>
        public static readonly ConditionHistoryToStringConverter Default = new();

        /// <inheritdoc/>
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value switch
            {
                ICondition condition => this.Convert(condition.History, targetType, parameter, culture),
                IEnumerable<ConditionHistoryPoint> xs
                   when SameDay(xs)
                   => string.Join(Environment.NewLine, xs.OrderByDescending(x => x.TimeStamp).Select(x => $"{x.TimeStamp.ToLocalTime().ToString("T", DateTimeFormatInfo.CurrentInfo)} {State(x)}")),
                IEnumerable<ConditionHistoryPoint> xs
                    => string.Join(Environment.NewLine, xs.OrderByDescending(x => x.TimeStamp).Select(x => $"{x.TimeStamp.ToLocalTime().ToString("yyyy-MM-dd HH: mm:ss", DateTimeFormatInfo.CurrentInfo)} {State(x)}")),
                _ => throw new ArgumentException($"Expected {nameof(IEnumerable<ConditionHistoryPoint>)} or {nameof(ICondition)}", nameof(value)),
            };

            bool SameDay(IEnumerable<ConditionHistoryPoint> xs)
            {
                var min = DateTime.MaxValue;
                var max = DateTime.MinValue;
                foreach (var p in xs)
                {
                    if (p.TimeStamp < min)
                    {
                        min = p.TimeStamp;
                    }

                    if (p.TimeStamp > max)
                    {
                        max = p.TimeStamp;
                    }
                }

                return max - min < TimeSpan.FromHours(24);
            }

            string State(ConditionHistoryPoint p) => p.State switch
            {
                true => "true",
                false => "false",
                null => "null",
            };
        }

        /// <inheritdoc/>
        object IValueConverter.ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotSupportedException($"{nameof(ConditionHistoryToStringConverter)} can only be used in OneWay bindings");
        }
    }
}
