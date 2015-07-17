namespace Gu.Reactive.PropertyPathStuff
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Internals;

    internal class PropertyPath<TSource, TValue> : IValuePath<TSource, TValue>, IPropertyPath
    {
        private readonly PropertyPath _propertyPath;

        private static readonly ValueAndSender<TValue> EmptyValueAndSender = new ValueAndSender<TValue>(null, new Maybe<TValue>(false, default(TValue)));

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

        public IMaybe<TValue> GetValue(TSource source)
        {
            var maybe = _propertyPath.GetValue<TValue>(source);
            return maybe;
        }

        public ValueAndSender<TValue> GetValueAndSender(TSource source)
        {
            var sender = GetSender(source);
            if (sender == null)
            {
                return EmptyValueAndSender;
            }
            var value = _propertyPath.Last.PropertyInfo.GetValue(sender);
            return new ValueAndSender<TValue>(sender, new Maybe<TValue>(true, (TValue)value));
        }

        public object GetSender(TSource source)
        {
            if (Count == 1)
            {
                return source;
            }
            var maybe = _propertyPath[_propertyPath.Count - 2].GetValue<object>(source);
            return maybe.HasValue
                       ? maybe.Value
                       : null;
        }

        public IEnumerator<PathProperty> GetEnumerator()
        {
            return _propertyPath.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return string.Format("x => x.{0}", string.Join(".", _propertyPath.Select(x => x.PropertyInfo.Name)));
        }
    }
}
