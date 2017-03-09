namespace Gu.Reactive.Internals
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    internal sealed class PropertyPath : IPropertyPath
    {
        private static readonly ConcurrentDictionary<LambdaExpression, PropertyPath> CachedPaths = new ConcurrentDictionary<LambdaExpression, PropertyPath>(PropertyPathComparer.Default);
        private static readonly ConcurrentDictionary<LambdaExpression, IPropertyPath> Cache = new ConcurrentDictionary<LambdaExpression, IPropertyPath>(PropertyPathComparer.Default);
        private readonly IReadOnlyList<PathProperty> parts;

        private PropertyPath(IReadOnlyList<PathProperty> parts)
        {
            this.parts = parts;
            this.Last = parts[parts.Count - 1];
        }

        public PathProperty Last { get; }

        public int Count => this.parts.Count;

        public PathProperty this[int index] => this.parts[index];

        public static PropertyPath<TSource, TValue> GetOrCreate<TSource, TValue>(Expression<Func<TSource, TValue>> propertyPath)
        {
            return (PropertyPath<TSource, TValue>)Cache.GetOrAdd(propertyPath, Create<TSource, TValue>);
        }

        public Maybe<T> GetValueFromRoot<T>(object rootSource) => this.Last.GetValueFromRoot<T>(rootSource);

        public IEnumerator<PathProperty> GetEnumerator() => this.parts.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.parts.GetEnumerator();

        public override string ToString()
        {
            var first = this.parts.First().PropertyInfo;
            var path = string.Join(".", this.parts.Skip(1).Select(x => x.PropertyInfo.Name));
            return $"{first.DeclaringType?.FullName}.{first.Name}{(!string.IsNullOrEmpty(path) ? "." : string.Empty)}{path}";
        }

        private static PropertyPath<TSource, TValue> Create<TSource, TValue>(LambdaExpression propertyPath)
        {
            var path = CachedPaths.GetOrAdd(propertyPath, CreatePath);
            return new PropertyPath<TSource, TValue>(path);
        }

        private static PropertyPath CreatePath(LambdaExpression propertyPath)
        {
            var propertyInfos = PropertyPathParser.GetPath(propertyPath);
            var parts = new PathProperty[propertyInfos.Count];
            PathProperty previous = null;
            for (int i = 0; i < propertyInfos.Count; i++)
            {
                var propertyInfo = propertyInfos[i];
                var item = new PathProperty(previous, propertyInfo);
                parts[i] = item;
                previous = item;
            }

            return new PropertyPath(parts);
        }
    }
}