namespace Gu.Wpf.Reactive.TypeConverters
{
    using System;
    using System.Globalization;
    using System.Linq;

    internal class NullableEnumConverter<T> : ITypeConverter<T?>
        where T : struct, IComparable, IFormattable
    {
        internal static readonly NullableEnumConverter<T> Default = new NullableEnumConverter<T>();

        private static readonly Type[] ValidTypes =
        {
            typeof (T),
        };

        public bool IsValid(object value)
        {
            if (value == null)
            {
                return true;
            }
            if (ValidTypes.Contains(value.GetType()))
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
            if (ValidTypes.Contains(value.GetType()))
            {
                return true;
            }
            var s = value as string;
            if (s != null)
            {
                T temp;
                return Enum.TryParse(s, true, out temp);
            }
            return false;
        }

        public T? ConvertTo(object value, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }
            if (ValidTypes.Contains(value.GetType()))
            {
                return (T)Convert.ChangeType(value, typeof(T));

            }
            var s = value as string;
            if (s != null)
            {
                return (T)Enum.Parse(typeof(T), s);
            }
            throw new ArgumentException("value");
        }

        object ITypeConverter.ConvertTo(object value, CultureInfo culture)
        {
            return ConvertTo(value, culture);
        }
    }

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