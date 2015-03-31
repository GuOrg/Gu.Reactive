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
        private readonly PathItem _last;
        private ValuePath(IReadOnlyList<IPathItem> parts)
        {
            _parts = parts;
            _last = (PathItem) parts[parts.Count - 1];
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

        public object LastSource
        {
            get
            {
                var beforeLast = _last.Previous; // This is the source
                return beforeLast.Value;
            }
        }

        public bool HasValue
        {
            get
            {
                return LastSource != null;
            }
        }

        public object ValueOrDefault
        {
            get { return _last.Value; }
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

        public static ValuePath Create<TValue>(Expression<Func<TValue>> propertyExpression)
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
            var constants = ConstantVisitor.GetConstants(propertyExpression);
            if (!constants.Any())
            {
                throw new ArgumentException("Expression contains no constants", "propertyExpression");
            }
            //valuePath.Source = source;
            return new ValuePath(parts) { Source = constants.Last().Value };
        }

        IEnumerator<IPathItem> IEnumerable<IPathItem>.GetEnumerator()
        {
            return _parts.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _parts.GetEnumerator();
        }

        public IValuePath<TSource, TValue> As<TSource, TValue>()
        {
            return new ValuePath<TSource, TValue>(this);
        }
    }
}