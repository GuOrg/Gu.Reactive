namespace Gu.Wpf.Reactive
{
    using System.Globalization;

    public class CastingConverter<T> : ITypeConverter<T>
    {
        private readonly ITypeConverter<object> _inner;

        public CastingConverter(ITypeConverter<object> inner)
        {
            _inner = inner;
        }

        public bool IsValid(object value)
        {
            return _inner.IsValid(value);
        }

        public bool CanConvertTo(object value, CultureInfo culture)
        {
            return _inner.CanConvertTo(value, culture);
        }

        public T ConvertTo(object value, CultureInfo culture)
        {
            return (T)_inner.ConvertTo(value, culture);
        }

        object ITypeConverter.ConvertTo(object value, CultureInfo culture)
        {
            return ConvertTo(value, culture);
        }
    }
}
