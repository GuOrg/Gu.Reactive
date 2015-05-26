namespace Gu.Wpf.Reactive.TypeConverters
{
    using System.Globalization;

    public interface ITypeConverter<out T> : ITypeConverter
    {
        T ConvertTo(object value, CultureInfo culture);
    }
}
