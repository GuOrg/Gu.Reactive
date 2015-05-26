namespace Gu.Wpf.Reactive.TypeConverters
{
    using System;
    using System.Globalization;
    using System.Linq;

    internal class NullableEnumConverter : ITypeConverter<object>
    {
        private readonly Type _type;

        public NullableEnumConverter(Type type)
        {
            if (type.IsEnum)
            {
                _type = type;
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var genericArgument = type.GetGenericArguments()[0];
                if (genericArgument.IsEnum)
                {
                    _type = genericArgument;
                }
                else
                {
                    throw new ArgumentException("Type must be enum or Nullable<enum>", "type");
                }
            }
            else
            {
                throw new ArgumentException("Type must be enum or Nullable<enum>", "type");
            }
        }

        public bool IsValid(object value)
        {
            if (value == null)
            {
                return true;
            }
            if (_type == value.GetType())
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
            if (_type == value.GetType())
            {
                return true;
            }
            var s = value as string;
            if (s != null)
            {
                return Enum.IsDefined(_type, s);
            }
            return false;
        }

        public object ConvertTo(object value, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }
            if (_type == value.GetType())
            {
                return Convert.ChangeType(value, _type);

            }
            var s = value as string;
            if (s != null)
            {
                return Enum.Parse(_type, s);
            }
            throw new ArgumentException("value");
        }
    }
}