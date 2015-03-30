namespace Gu.Reactive
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    public class ValuePath : IReadOnlyList<PathItem>
    {
        /// <summary>
        /// The _path.
        /// </summary>
        private readonly IReadOnlyList<PathItem> _parts;

        public ValuePath(IReadOnlyList<PropertyInfo> properties)
        {
            var parts = new PathItem[properties.Count];
            PathItem previous = null;
            for (int i = 0; i < properties.Count; i++)
            {
                var propertyInfo = properties[i];
                if ((i != properties.Count - 1) && propertyInfo.PropertyType.IsValueType)
                {
                    throw new NotImplementedException("Not sure how to handle copy by value");
                }
                parts[i] = new PathItem(previous, propertyInfo);
            }
            _parts = parts;
        }

        public int Count
        {
            get { return this._parts.Count; }
        }

        PathItem IReadOnlyList<PathItem>.this[int index]
        {
            get { return _parts[index]; }
        }

        internal static ValuePath Create<TSource, TValue>(Expression<Func<TSource, TValue>> propertyExpression)
        {
            var path = PropertyPathVisitor.GetPath(propertyExpression);
            return new ValuePath(path.Cast<PropertyInfo>().ToArray());
        }

        IEnumerator<PathItem> IEnumerable<PathItem>.GetEnumerator()
        {
            return this._parts.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this._parts.GetEnumerator();
        }
    }
}