#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements must be documented
namespace Gu.Wpf.Reactive
{
    using System;
    using System.Globalization;

    [Obsolete("To be removed.")]
    internal class DefaultTypeConverter : ITypeConverter<object>
    {
        private readonly Type type;

        public DefaultTypeConverter(Type type)
        {
            this.type = type;
        }

        public bool IsValid(object value)
        {
            return this.type.IsInstanceOfType(value);
        }

        public bool CanConvertTo(object value, CultureInfo culture)
        {
            return this.IsValid(value);
        }

        public object ConvertTo(object value, CultureInfo culture)
        {
            return value;
        }
    }
}
