namespace Gu.Wpf.Reactive
{
    using System;
    using System.Globalization;

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
