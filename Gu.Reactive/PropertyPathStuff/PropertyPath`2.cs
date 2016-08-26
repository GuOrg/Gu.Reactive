namespace Gu.Reactive.PropertyPathStuff
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Gu.Reactive.Internals;

    //[DebuggerTypeProxy(typeof(CollectionDebugView<>))]
    internal class PropertyPath<TSource, TValue> : IValuePath<TSource, TValue>, IPropertyPath
    {
        private readonly PropertyPath propertyPath;

        private static readonly ValueAndSender<TValue> EmptyValueAndSender = new ValueAndSender<TValue>(null, new Maybe<TValue>(false, default(TValue)));

        internal PropertyPath(PropertyPath propertyPath)
        {
            var last = propertyPath.Last();
            if (last.PropertyInfo.PropertyType != typeof(TValue))
            {
                throw new InvalidOperationException($"Valuepath type does not match. Expected: {typeof(TValue).FullName} was: {last.PropertyInfo.PropertyType.FullName}");
            }

            this.propertyPath = propertyPath;
        }

        public int Count => this.propertyPath.Count;

        public PathProperty Last => this.propertyPath.Last;

        public PathProperty this[int index] => this.propertyPath[index];

        public IMaybe<TValue> GetValue(TSource source)
        {
            var maybe = this.propertyPath.GetValue<TValue>(source);
            return maybe;
        }

        public ValueAndSender<TValue> GetValueAndSender(TSource source)
        {
            var sender = this.GetSender(source);
            if (sender == null)
            {
                return EmptyValueAndSender;
            }

            var value = this.propertyPath.Last.PropertyInfo.GetValue(sender);
            return new ValueAndSender<TValue>(sender, new Maybe<TValue>(true, (TValue)value));
        }

        public object GetSender(TSource source)
        {
            if (this.Count == 1)
            {
                return source;
            }

            var maybe = this.propertyPath[this.propertyPath.Count - 2].GetValue<object>(source);
            return maybe.HasValue
                       ? maybe.Value
                       : null;
        }

        public IEnumerator<PathProperty> GetEnumerator()
        {
            return this.propertyPath.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public override string ToString()
        {
            return string.Format("x => x.{0}", string.Join(".", this.propertyPath.Select(x => x.PropertyInfo.Name)));
        }
    }
}
