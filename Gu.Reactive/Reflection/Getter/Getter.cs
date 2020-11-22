namespace Gu.Reactive
{
    using System;
    using System.Collections.Concurrent;
    using System.ComponentModel;
    using System.Reflection;

    using Gu.Reactive.Internals;

    /// <summary>
    /// Factory methods for creating <see cref="IGetter"/> from <see cref="PropertyInfo"/>.
    /// </summary>
    public static class Getter
    {
        private static readonly ConcurrentDictionary<PropertyInfo, IGetter> Cache = new ConcurrentDictionary<PropertyInfo, IGetter>();

        /// <summary>
        /// Get or create an <see cref="IGetter"/> for <paramref name="property"/>.
        /// </summary>
        /// <param name="property">The <see cref="PropertyInfo"/>.</param>
        public static IGetter GetOrCreate(PropertyInfo property)
        {
            if (property is null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            VerifyProperty(property);
            return Cache.GetOrAdd(property, Create);

            static IGetter Create(PropertyInfo property)
            {
                var ctor = GetGetterType(property)
                           .MakeGenericType(property.ReflectedType!, property.PropertyType)
                           .GetConstructor(
                               bindingAttr: BindingFlags.NonPublic | BindingFlags.Instance,
                               binder: null,
                               types: new[] { typeof(PropertyInfo) },
                               modifiers: null);
                return (IGetter)ctor!.Invoke(new object[] { property });

                static Type GetGetterType(PropertyInfo property)
                {
                    if (property.ReflectedType?.IsValueType == true)
                    {
                        return typeof(StructGetter<,>);
                    }

                    return typeof(INotifyPropertyChanged).IsAssignableFrom(property.ReflectedType)
                        ? typeof(NotifyingGetter<,>)
                        : typeof(ClassGetter<,>);
                }
            }
        }

        /// <summary>
        /// Same as <see cref="PropertyInfo.GetValue(object)"/> but uses a cached delegate for performance.
        /// </summary>
        /// <param name="property">The <see cref="PropertyInfo"/>.</param>
        public static object? GetValueViaDelegate(this PropertyInfo property, object source)
        {
            return GetOrCreate(property).GetValue(source);
        }

        /// <summary>
        /// Check that a Getter can be created for the property.
        /// </summary>
        /// <param name="property">The <see cref="PropertyInfo"/>.</param>
        /// <exception cref="ArgumentException">If the property does not have a getter.</exception>
        public static void VerifyProperty(PropertyInfo property)
        {
            if (property is null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            if (property.ReflectedType is null)
            {
                var message = $"Property.ReflectedType is null {property}.";
                throw new ArgumentException(message, nameof(property));
            }

            if (property.GetMethod is null)
            {
                var message = "Property cannot be write only.\r\n" +
                                   $"The property {property.ReflectedType.Namespace}.{property.ReflectedType.PrettyName()}.{property.Name} does not have a getter.";
                throw new ArgumentException(message, nameof(property));
            }
        }
    }
}
