namespace Gu.Reactive.Internals
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    internal class ValuePath : IReadOnlyList<IPathItem>
    {
        private readonly IReadOnlyList<IPathItem> _parts;

        private ValuePath(IReadOnlyList<IPathItem> parts)
        {
            _parts = parts;
        }

        public object Source
        {
            get
            {
                return ((RootItem)_parts[0]).Value;
            }
            set
            {
                ((RootItem)_parts[0]).Value = value;
            }
        }

        public int Count
        {
            get { return _parts.Count; }
        }

        public IPathItem this[int index]
        {
            get { return _parts[index]; }
        }

        public static ValuePath Create<TSource, TValue>(Expression<Func<TSource, TValue>> propertyExpression)
        {
            var path = PropertyPathVisitor.GetPath(propertyExpression);
            var propertyInfos = path.Cast<PropertyInfo>().ToArray();
            var parts = new IPathItem[propertyInfos.Length + 1];
            IPathItem previous = new RootItem();
            parts[0] = previous;
            for (int i = 0; i < propertyInfos.Length; i++)
            {
                var propertyInfo = propertyInfos[i];
                var item = new PathItem(previous, propertyInfo);
                parts[i + 1] = item;
                previous = item;
            }
            return new ValuePath(parts);
        }

        IEnumerator<IPathItem> IEnumerable<IPathItem>.GetEnumerator()
        {
            return _parts.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _parts.GetEnumerator();
        }
    }
}