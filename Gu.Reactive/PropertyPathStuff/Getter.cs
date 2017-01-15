namespace Gu.Reactive.PropertyPathStuff
{
    using System.Collections.Concurrent;
    using System.Reflection;

    public static class Getter
    {
        private static readonly ConcurrentDictionary<PropertyInfo, IGetter> Cache = new ConcurrentDictionary<PropertyInfo, IGetter>(PropertyInfoComparer.Default);

        public static IGetter GetOrCreate(PropertyInfo property)
        {
            return Cache.GetOrAdd(property, Create);
        }

        public static object GetValueViaDelegate(this PropertyInfo property, object source)
        {
            return GetOrCreate(property)
                .GetValue(source);
        }

        private static IGetter Create(PropertyInfo property)
        {
            var ctor = typeof(Getter<,>).MakeGenericType(property.DeclaringType, property.PropertyType)
                                                   .GetConstructor(
                                                       BindingFlags.NonPublic | BindingFlags.Instance,
                                                       null,
                                                       new[] { typeof(PropertyInfo) },
                                                       null);
            return (IGetter)ctor.Invoke(new object[] { property });
        }
    }
}