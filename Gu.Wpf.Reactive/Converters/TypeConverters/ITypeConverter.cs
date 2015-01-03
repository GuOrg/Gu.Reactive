namespace Gu.Wpf.Reactive
{
    using System.Globalization;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITypeConverter<out T>
    {
        bool IsValid(object value);

        bool CanConvertTo(object value, CultureInfo culture);

        T ConvertTo(object value, CultureInfo culture);
    }
}