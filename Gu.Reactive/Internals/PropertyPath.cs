namespace Gu.Reactive.Internals
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    internal class PropertyPath : IPropertyPath
    {
        private readonly IReadOnlyList<PathProperty> _parts;

        private PropertyPath(IReadOnlyList<PathProperty> parts)
        {
            _parts = parts;
            Last = parts[parts.Count - 1];
        }

        public PathProperty Last { get; private set; }

        public int Count
        {
            get { return _parts.Count; }
        }

        public PathProperty this[int index]
        {
            get { return _parts[index]; }
        }

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
                throw new ArgumentException("Expression contains no constants", "propertyExpression");
            }
            //valuePath.Source = source;
            var source = constants.Last().Value;
            var propertyPath = new PropertyPath(parts);
            return new PropertyPath<TValue>(propertyPath, source);
        }

        public Maybe<object> GetValue(object source)
        {
            return Last.GetValue(source);
        }

        public IEnumerator<PathProperty> GetEnumerator()
        {
            return _parts.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _parts.GetEnumerator();
        }
    }
}