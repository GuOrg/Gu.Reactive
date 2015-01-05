namespace Gu.Wpf.Reactive
{
    using System;
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

    internal class DefaultTypeConverter : ITypeConverter<object>
    {
        private readonly Type _type;

        public DefaultTypeConverter(Type type)
        {
            _type = type;
        }

        public bool IsValid(object value)
        {
            return _type.IsInstanceOfType(value);
        }

        public bool CanConvertTo(object value, CultureInfo culture)
        {
            return IsValid(value);
        }

        public object ConvertTo(object value, CultureInfo culture)
        {
            return value;
        }
    }
}
