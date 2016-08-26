namespace Gu.Reactive.PropertyPathStuff
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Internals;

    internal class PropertyPath : IPropertyPath
    {
        private readonly IReadOnlyList<PathProperty> parts;

        private PropertyPath(IReadOnlyList<PathProperty> parts)
        {
            this.parts = parts;
            this.Last = parts[parts.Count - 1];
        }

        public PathProperty Last { get; }

        public int Count => this.parts.Count;

        public PathProperty this[int index] => this.parts[index];

        public static PropertyPath<TSource, TValue> Create<TSource, TValue>(Expression<Func<TSource, TValue>> propertyExpression)
        {
            var path = PropertyPathVisitor.GetPath(propertyExpression);
            var propertyInfos = path.Cast<PropertyInfo>().ToArray();
            var parts = new PathProperty[propertyInfos.Length];
            PathProperty previous = null;
            for (int i = 0; i < propertyInfos.Length; i++)
            {
                var propertyInfo = propertyInfos[i];
                var item = new PathProperty(previous, propertyInfo);
                parts[i] = item;
                previous = item;
            }

            return new PropertyPath<TSource, TValue>(new PropertyPath(parts));
        }

        public static PropertyPath<TValue> Create<TValue>(Expression<Func<TValue>> propertyExpression)
        {
            var path = PropertyPathVisitor.GetPath(propertyExpression);
            var propertyInfos = path.Cast<PropertyInfo>().ToArray();
            var parts = new PathProperty[propertyInfos.Length];
            PathProperty previous = null;
            for (int i = 0; i < propertyInfos.Length; i++)
            {
                var propertyInfo = propertyInfos[i];
                var item = new PathProperty(previous, propertyInfo);
                parts[i] = item;
                previous = item;
            }

            var constants = ConstantVisitor.GetConstants(propertyExpression);
            if (!constants.Any())
            {
                throw new ArgumentException("Expression contains no constants", nameof(propertyExpression));
            }

            //valuePath.Source = source;
            var source = constants.Last().Value;
            var propertyPath = new PropertyPath(parts);
            return new PropertyPath<TValue>(propertyPath, source);
        }

        public Maybe<T> GetValue<T>(object source)
        {
            return this.Last.GetValue<T>(source);
        }

        public IEnumerator<PathProperty> GetEnumerator()
        {
            return this.parts.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.parts.GetEnumerator();
        }

        public override string ToString()
        {
            var first = this.parts.First().PropertyInfo;
            var path = string.Join(".", this.parts.Skip(1).Select(x => x.PropertyInfo.Name));
            return $"{first.DeclaringType?.FullName}.{first.Name}{(!string.IsNullOrEmpty(path) ? "." : string.Empty)}{path}";
        }
    }
}