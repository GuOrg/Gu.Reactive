namespace Gu.Reactive.Internals
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq.Expressions;

    internal static class PropertyPath
    {
        private static readonly ConcurrentDictionary<LambdaExpression, IPropertyPath> Cache = new ConcurrentDictionary<LambdaExpression, IPropertyPath>(PropertyPathComparer.Default);

        public static PropertyPath<TSource, TValue> GetOrCreate<TSource, TValue>(Expression<Func<TSource, TValue>> propertyPath)
        {
            return (PropertyPath<TSource, TValue>)Cache.GetOrAdd(propertyPath, Create<TSource, TValue>);
        }

        private static PropertyPath<TSource, TValue> Create<TSource, TValue>(LambdaExpression propertyPath)
        {
            var propertyInfos = PropertyPathParser.GetPath(propertyPath);
            var parts = new IPathProperty[propertyInfos.Count];
            IPathProperty previous = null;
            for (int i = 0; i < propertyInfos.Count; i++)
            {
                var propertyInfo = propertyInfos[i];
                var item = PathProperty.Create(previous, propertyInfo);
                parts[i] = item;
                previous = item;
            }

            return new PropertyPath<TSource, TValue>(parts);
        }
    }
}