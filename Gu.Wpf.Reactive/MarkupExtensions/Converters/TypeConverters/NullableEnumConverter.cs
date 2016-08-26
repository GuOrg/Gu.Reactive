namespace Gu.Wpf.Reactive.TypeConverters
{
    using System;
    using System.Globalization;

    internal class NullableEnumConverter : ITypeConverter<object>
    {
        private readonly Type type;

        public NullableEnumConverter(Type type)
        {
            if (type.IsEnum)
            {
                this.type = type;
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var genericArgument = type.GetGenericArguments()[0];
                if (genericArgument.IsEnum)
                {
                    this.type = genericArgument;
                }
                else
                {
                    throw new ArgumentException("Type must be enum or Nullable<enum>", nameof(type));
                }
            }
            else
            {
                throw new ArgumentException("Type must be enum or Nullable<enum>", nameof(type));
            }
        }

        public bool IsValid(object value)
        {
            if (value == null)
            {
                return true;
            }
            if (this.type == value.GetType())
            {
                return true;
            }
            return false;
        }

        public bool CanConvertTo(object value, CultureInfo culture)
        {
            if (value == null)
            {
                return true;
            }
            if (this.type == value.GetType())
            {
                return true;
            }
            var s = value as string;
            if (s != null)
            {
                return Enum.IsDefined(this.type, s);
            }
            return false;
        }

        public object ConvertTo(object value, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }
            if (this.type == value.GetType())
            {
                return Convert.ChangeType(value, this.type);

            }
            var s = value as string;
            if (s != null)
            {
                return Enum.Parse(this.type, s);
            }
            throw new ArgumentException("value");
        }
    }
}