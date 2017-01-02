namespace Gu.Wpf.Reactive
{
    using System.Globalization;

    public interface ITypeConverter<out T> : ITypeConverter
    {
        new T ConvertTo(object value, CultureInfo culture);
    }
}
