namespace Gu.Wpf.Reactive.TypeConverters
{
    using System.Globalization;

    public interface ITypeConverter
    {
        bool IsValid(object value);

        bool CanConvertTo(object value, CultureInfo culture);

        object ConvertTo(object value, CultureInfo culture);
    }
}