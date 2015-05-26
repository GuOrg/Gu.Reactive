namespace Gu.Wpf.Reactive.Converters.TypeConverters
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Gu.Wpf.Reactive.TypeConverters;

    internal class NullableEnumConverter<T> : ITypeConverter<T?>
        where T : struct, IComparable, IFormattable
    {
        internal static readonly NullableEnumConverter<T> Default = new NullableEnumConverter<T>();

        private static readonly Type[] ValidTypes =
        {
            typeof (T),
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
                T temp;
                return Enum.TryParse(s, true, out temp);
            }
            return false;
        }

        public T? ConvertTo(object value, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }
            if (ValidTypes.Contains(value.GetType()))
            {
                return (T)Convert.ChangeType(value, typeof(T));

            }
            var s = value as string;
            if (s != null)
            {
                return (T)Enum.Parse(typeof(T), s);
            }
            throw new ArgumentException("value");
        }

        object ITypeConverter.ConvertTo(object value, CultureInfo culture)
        {
            return ConvertTo(value, culture);
        }
    }
}
