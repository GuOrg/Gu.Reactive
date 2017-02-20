namespace Gu.Reactive
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

        /// <summary>
        /// Get or create an <see cref="IGetter"/> for <paramref name="property"/>
        /// </summary>
        public static IGetter GetOrCreate(PropertyInfo property)
        {
            return Cache.GetOrAdd(property, Create);
        }

        /// <summary>
        /// Same as <see cref="PropertyInfo.GetValue(object)"/> but uses a cached delegate for performance.
        /// </summary>
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
                                  bindingAttr: BindingFlags.NonPublic | BindingFlags.Instance,
                                  binder: null,
                                  types: new[] { typeof(PropertyInfo) },
                                  modifiers: null);
            return (IGetter)ctor.Invoke(new object[] { property });
        }
    }
}