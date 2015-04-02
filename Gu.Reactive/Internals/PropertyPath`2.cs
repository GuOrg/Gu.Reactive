namespace Gu.Reactive.Internals
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    internal class PropertyPath<TSource, TValue> : IValuePath<TSource, TValue>, IPropertyPath
    {
        private readonly PropertyPath _propertyPath;

        internal PropertyPath(PropertyPath propertyPath)
        {
            var last = propertyPath.Last();
            if (last.PropertyInfo.PropertyType != typeof(TValue))
            {
                throw new InvalidOperationException(
                    string.Format(
                        "Valuepath type does not match. Expected: {0} was: {1}",
                        typeof(TValue).FullName,
                        last.PropertyInfo.PropertyType.FullName));
            }
            _propertyPath = propertyPath;
        }

        public int Count
        {
            get { return _propertyPath.Count; }
        }
        
        public PathProperty Last
        {
            get { return _propertyPath.Last; }
        }
        
        public PathProperty this[int index]
        {
            get { return _propertyPath[index]; }
        }

        public IMaybe<TValue> Value(TSource source)
        {
            var maybe = _propertyPath.GetValue(source);
            return maybe.As<TValue>();
        }

        public IEnumerator<PathProperty> GetEnumerator()
        {
            return _propertyPath.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
