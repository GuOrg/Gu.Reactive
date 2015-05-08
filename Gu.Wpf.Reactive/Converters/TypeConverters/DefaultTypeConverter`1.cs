namespace Gu.Wpf.Reactive.Converters.TypeConverters
{
    using System.Globalization;

    internal class DefaultTypeConverter<T> : ITypeConverter<T>
    {
        internal static readonly DefaultTypeConverter<T> Default = new DefaultTypeConverter<T>();

        public static explicit operator DefaultTypeConverter<T>(DefaultTypeConverter converter)
        {
            return Default;
        }

        public bool IsValid(object value)
        {
            if (!typeof(T).IsValueType && value == null)
            {
                return true;
            }
            return value is T;
        }

        public bool CanConvertTo(object value, CultureInfo culture)
        {
            return IsValid(value);
        }

        public T ConvertTo(object value, CultureInfo culture)
        {
            return (T)value;
        }

        object ITypeConverter.ConvertTo(object value, CultureInfo culture)
        {
            return ConvertTo(value, culture);
        }
    }

}
