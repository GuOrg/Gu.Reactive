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

        public ValuePath(IEnumerable<PropertyInfo> properties)
        {
            _parts = properties.Select(x => new PathItem(x)).ToArray();
            _parts.Last().IsLast = true;
            for (var i = 0; i < (_parts.Count - 1); i++)
            {
                var propertyInfo = this._parts[i].PropertyInfo;
                if (propertyInfo.PropertyType.IsValueType)
                {
                    throw new NotImplementedException("Not sure how to handle copy by value");
                }
            }
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
            return new ValuePath(path.Cast<PropertyInfo>());
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