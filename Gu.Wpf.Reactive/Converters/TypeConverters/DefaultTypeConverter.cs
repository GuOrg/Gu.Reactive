namespace Gu.Wpf.Reactive
{
    using System.Globalization;

    internal class DefaultTypeConverter<T> : ITypeConverter<T>
    {
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
    }
}
