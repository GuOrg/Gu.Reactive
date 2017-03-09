namespace Gu.Reactive
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Gu.Reactive.Internals;

    /// <summary>
    /// Factory methods for creating <see cref="IGetter"/> from <see cref="PropertyInfo"/>
    /// </summary>
    public static class Getter
    {
        private static readonly ConcurrentDictionary<PropertyInfo, IGetter> Cache = new ConcurrentDictionary<PropertyInfo, IGetter>();

        // ReSharper disable once UnusedMember.Local for inspection in the debugger.
        internal static IReadOnlyList<CacheItem> CacheDebugView =>
            Cache.Select(x => new CacheItem(x))
                 .OrderBy(x => x.Property)
                 .ToArray();

        /// <summary>
        /// Get or create an <see cref="IGetter"/> for <paramref name="property"/>
        /// </summary>
        public static IGetter GetOrCreate(PropertyInfo property)
        {
            Ensure.NotNull(property, nameof(property));
            VerifyProperty(property);
            return Cache.GetOrAdd(property, Create);
        }

        /// <summary>
        /// Same as <see cref="PropertyInfo.GetValue(object)"/> but uses a cached delegate for performance.
        /// </summary>
        public static object GetValueViaDelegate(this PropertyInfo property, object source)
        {
            return GetOrCreate(property).GetValue(source);
        }

        /// <summary>
        /// Check that a Getter can be created for the property
        /// </summary>
        /// <exception cref="ArgumentException">If the property does not have a getter.</exception>
        public static void VerifyProperty(PropertyInfo property)
        {
            Ensure.NotNull(property, nameof(property));

            if (property.GetMethod == null)
            {
                var message = "Property cannot be write only.\r\n" +
                              $"The property {property.ReflectedType?.Namespace}.{property.ReflectedType.PrettyName()}.{property.Name} does not have a getter.";
                throw new ArgumentException(message, nameof(property));
            }
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

        internal struct CacheItem
        {
            public CacheItem(KeyValuePair<PropertyInfo, IGetter> keyValuePair)
            {
                this.KeyValuePair = keyValuePair;
            }

            public KeyValuePair<PropertyInfo, IGetter> KeyValuePair { get; }

            public string Property => $"{this.KeyValuePair.Key.DeclaringType.PrettyName()}.{this.KeyValuePair.Key.Name}";
        }
    }
}