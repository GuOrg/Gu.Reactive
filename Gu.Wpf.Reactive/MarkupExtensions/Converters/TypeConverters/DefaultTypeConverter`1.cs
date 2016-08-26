namespace Gu.Wpf.Reactive.TypeConverters
{
    using System;
    using System.Globalization;
    using System.Text.RegularExpressions;

    using Gu.Reactive;

    internal class DefaultTypeConverter<T> : ITypeConverter<T>
    {
        internal static readonly DefaultTypeConverter<T> Default = new DefaultTypeConverter<T>();

        public static explicit operator DefaultTypeConverter<T>(DefaultTypeConverter converter)
        {
            return Default;
        }

        public bool IsValid(object value)
        {
            if (value == null)
            {
                if(typeof(T).IsValueType)
                {
                    return typeof(T).IsNullable();
                }
                return true;
            }
            var name = value.GetType().Name;
            if (Regex.IsMatch(name, @"_\.di\d+\..+"))
            {
                return true;
            }
            if (typeof(T).IsValueType)
            {
                return value is T || (value.GetType() == Nullable.GetUnderlyingType(typeof(T)));
            }
            return value is T;
        }

        public bool CanConvertTo(object value, CultureInfo culture)
        {
            return this.IsValid(value);
        }

        public T ConvertTo(object value, CultureInfo culture)
        {
            return (T)value;
        }

        object ITypeConverter.ConvertTo(object value, CultureInfo culture)
        {
            return this.ConvertTo(value, culture);
        }
    }
}
