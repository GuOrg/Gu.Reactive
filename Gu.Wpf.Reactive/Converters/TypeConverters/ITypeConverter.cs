namespace Gu.Wpf.Reactive.TypeConverters
{
    using System.Globalization;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITypeConverter<out T> : ITypeConverter
    {
        T ConvertTo(object value, CultureInfo culture);
    }

    public interface ITypeConverter
    {
        bool IsValid(object value);

        bool CanConvertTo(object value, CultureInfo culture);

        object ConvertTo(object value, CultureInfo culture);
    }
}