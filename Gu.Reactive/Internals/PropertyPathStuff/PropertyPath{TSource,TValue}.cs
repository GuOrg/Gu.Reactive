﻿namespace Gu.Reactive.Internals
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    ////[DebuggerTypeProxy(typeof(CollectionDebugView<>))]
    internal class PropertyPath<TSource, TValue> : IValuePath<TSource, TValue>, IPropertyPath
    {
        private readonly PropertyPath propertyPath;

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

        IMaybe<TValue> IValuePath<TSource, TValue>.GetValue(TSource source) => this.GetValueFromRoot(source);

        /// <summary>
        /// Get the source of the last item in the path.
        /// </summary>
        /// <param name="source">The root instance for the path.</param>
        public object GetSender(TSource source)
        {
            if (this.Count == 1)
            {
                return source;
            }

            var maybe = this.propertyPath[this.propertyPath.Count - 2].GetValueFromRoot<object>(source);
            return maybe.HasValue
                       ? maybe.Value
                       : null;
        }

        public IEnumerator<PathProperty> GetEnumerator() => this.propertyPath.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public override string ToString() => $"x => x.{string.Join(".", this.propertyPath.Select(x => x.PropertyInfo.Name))}";

        internal Maybe<TValue> GetValueFromRoot(object rootSource) => this.propertyPath.GetValueFromRoot<TValue>(rootSource);
    }
}