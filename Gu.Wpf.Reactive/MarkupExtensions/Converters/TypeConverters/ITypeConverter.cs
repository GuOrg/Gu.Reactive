#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements must be documented
namespace Gu.Wpf.Reactive
{
    using System;
    using System.Globalization;

    [Obsolete("To be removed.")]
    public interface ITypeConverter
    {
        bool IsValid(object value);

        bool CanConvertTo(object value, CultureInfo culture);

        object ConvertTo(object value, CultureInfo culture);
    }
}