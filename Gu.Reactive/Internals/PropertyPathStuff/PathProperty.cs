namespace Gu.Reactive.Internals
{
    using System;
    using System.Collections.Concurrent;
    using System.Reflection;

    internal static class PathProperty
    {
        private static readonly ConcurrentDictionary<PropertyInfo, Func<IPathProperty, IGetter, IPathProperty>> Factories = new ConcurrentDictionary<PropertyInfo, Func<IPathProperty, IGetter, IPathProperty>>();

        internal static IPathProperty Create(IPathProperty previous, PropertyInfo propertyInfo)
        {
            Getter.VerifyProperty(propertyInfo);
            var getter = Getter.GetOrCreate(propertyInfo);
            var factory = Factories.GetOrAdd(propertyInfo, CreateFactory);
            return factory(previous, getter);
        }

        internal static PathProperty<TSource, TValue> Create<TSource, TValue>(IPathProperty previous, Getter<TSource, TValue> getter)
        {
            return new PathProperty<TSource, TValue>(previous, getter);
        }

        private static PathProperty<TSource, TValue> CastAndCreate<TSource, TValue>(IPathProperty previous, IGetter getter)
        {
            return new PathProperty<TSource, TValue>(previous, (Getter<TSource, TValue>)getter);
        }

        private static Func<IPathProperty, IGetter, IPathProperty> CreateFactory(PropertyInfo arg)
        {
            var method = typeof(PathProperty).GetMethod(nameof(CastAndCreate), BindingFlags.Static | BindingFlags.NonPublic)
                                             .MakeGenericMethod(arg.ReflectedType, arg.PropertyType);

            return (Func<IPathProperty, IGetter, IPathProperty>)Delegate.CreateDelegate(typeof(Func<IPathProperty, IGetter, IPathProperty>), method);
        }
    }
}