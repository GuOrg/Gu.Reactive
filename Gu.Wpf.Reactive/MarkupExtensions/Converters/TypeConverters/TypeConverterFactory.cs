namespace Gu.Wpf.Reactive.TypeConverters
{
    using System;
    using System.Collections.Concurrent;

    internal static class TypeConverterFactory
    {
        private static readonly ConcurrentDictionary<Type, ITypeConverter> TypeConverterMap = new ConcurrentDictionary<Type, ITypeConverter>();

        internal static ITypeConverter<T> Create<T>()
        {
            var type = typeof(T);
            if (type == typeof(bool) || type == typeof(bool?))
            {
                return (ITypeConverter<T>)TypeConverterMap.GetOrAdd(type, t => NullableBoolConverter.Default);
            }

            if (type == typeof(double) || type == typeof(double?))
            {
                return (ITypeConverter<T>)TypeConverterMap.GetOrAdd(type, t => NullableDoubleConverter.Default);
            }

            if (IsEnumOrNullableEnum(type))
            {
                return (ITypeConverter<T>)TypeConverterMap.GetOrAdd(type, t => new CastingConverter<T>(new NullableEnumConverter(typeof(T))));
            }
            return (ITypeConverter<T>)TypeConverterMap.GetOrAdd(type, t => DefaultTypeConverter<T>.Default);
        }

        /// <summary>
        /// http://msdn.microsoft.com/en-us/library/ms366789.aspx
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static bool IsEnumOrNullableEnum(Type type)
        {
            if (type.IsEnum)
            {
                return true;
            }
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var genericArgument = type.GetGenericArguments()[0];
                return genericArgument.IsEnum;
            }
            return false;
        }

        internal static bool IsEnumOrNullableEnum<T>()
        {
            return IsEnumOrNullableEnum(typeof(T));
        }
    }
}