namespace Gu.Wpf.Reactive.TypeConverters
{
    using System;
    using System.Globalization;
    using System.Linq;

    internal class NullableDoubleConverter : ITypeConverter<double?>
    {
        internal static readonly NullableDoubleConverter Default = new NullableDoubleConverter();

        private static readonly Type[] ValidTypes =
            {
                typeof(double),
                typeof(float),
                typeof(Int16),
                typeof(Int32),
                typeof(Int64),
                typeof(UInt16),
                typeof(UInt32),
                typeof(UInt64),
            };

        public bool IsValid(object value)
        {
            if (value == null)
            {
                return true;
            }
            if (ValidTypes.Contains(value.GetType()))
            {
                return true;
            }
            return false;
        }

        public bool CanConvertTo(object value, CultureInfo culture)
        {
            if (value == null)
            {
                return true;
            }
            if (ValidTypes.Contains(value.GetType()))
            {
                return true;
            }
            var s = value as string;
            if (s != null)
            {
                double d;
                return double.TryParse(s, NumberStyles.Float, culture, out d);
            }
            return false;
        }

        public double? ConvertTo(object value, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }
            if (ValidTypes.Contains(value.GetType()))
            {
                return (double)Convert.ChangeType(value, typeof(double));

            }
            var s = value as string;
            if (s != null)
            {
                return double.Parse(s, NumberStyles.Float, culture);
            }
            throw new ArgumentException("value");
        }

        object ITypeConverter.ConvertTo(object value, CultureInfo culture)
        {
            return ConvertTo(value, culture);
        }
    }
}