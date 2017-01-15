namespace Gu.Reactive.PropertyPathStuff
{
    using System.Collections.Concurrent;
    using System.Reflection;

    using Gu.Reactive.Internals;

    /// <summary>
    /// Factory methods for creating <see cref="IGetter"/> from <see cref="PropertyInfo"/>
    /// </summary>
    public static class Getter
    {
        private static readonly ConcurrentDictionary<PropertyInfo, IGetter> Cache = new ConcurrentDictionary<PropertyInfo, IGetter>(PropertyInfoComparer.Default);

        public static IGetter GetOrCreate(PropertyInfo property)
        {
            return Cache.GetOrAdd(property, Create);
        }

        public static object GetValueViaDelegate(this PropertyInfo property, object source)
        {
            return GetOrCreate(property).GetValue(source);
        }

        private static IGetter Create(PropertyInfo property)
        {
            Ensure.NotNull(property.ReflectedType, nameof(property));
            var typeDef = property.ReflectedType.IsValueType
                ? typeof(StructGetter<,>)
                : typeof(ClassGetter<,>);

            var ctor = typeDef.MakeGenericType(property.ReflectedType, property.PropertyType)
                                                   .GetConstructor(
                                                       BindingFlags.NonPublic | BindingFlags.Instance,
                                                       null,
                                                       new[] { typeof(PropertyInfo) },
                                                       null);
            return (IGetter)ctor.Invoke(new object[] { property });
        }
    }
}