namespace Gu.Reactive.Internals
{
    using System;
    using System.Collections.Concurrent;
    using System.ComponentModel;
    using System.Reflection;

    internal static class NotifyingProperty
    {
        private static readonly ConcurrentDictionary<PropertyInfo, Func<INotifyingProperty, IGetter, INotifyingProperty>> Factories = new ConcurrentDictionary<PropertyInfo, Func<INotifyingProperty, IGetter, INotifyingProperty>>();

        internal static INotifyingProperty Create(INotifyingProperty previous, PropertyInfo property)
        {
            Getter.VerifyProperty(property);
            var getter = Getter.GetOrCreate(property);
            var factory = Factories.GetOrAdd(property, CreateFactory);
            return factory(previous, getter);
        }

        private static NotifyingProperty<TSource, TValue> CastAndCreate<TSource, TValue>(INotifyingProperty previous, IGetter getter)
            where TSource : class, INotifyPropertyChanged
        {
            return new NotifyingProperty<TSource, TValue>(previous, (Getter<TSource, TValue>)getter);
        }

        private static Func<INotifyingProperty, IGetter, INotifyingProperty> CreateFactory(PropertyInfo property)
        {
            var method = typeof(NotifyingProperty).GetMethod(nameof(CastAndCreate), BindingFlags.Static | BindingFlags.NonPublic)
                                                  .MakeGenericMethod(property.ReflectedType, property.PropertyType);

            return (Func<INotifyingProperty, IGetter, INotifyingProperty>)Delegate.CreateDelegate(typeof(Func<INotifyingProperty, IGetter, INotifyingProperty>), method);
        }
    }
}