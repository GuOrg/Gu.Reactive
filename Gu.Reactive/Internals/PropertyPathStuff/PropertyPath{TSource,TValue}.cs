namespace Gu.Reactive.Internals
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    ////[DebuggerTypeProxy(typeof(CollectionDebugView<>))]
    internal class PropertyPath<TSource, TValue> : IPropertyPath
    {
        private readonly PropertyPath path;

        internal PropertyPath(PropertyPath path)
        {
            var last = path.Last();
            if (last.PropertyInfo.PropertyType != typeof(TValue))
            {
                throw new InvalidOperationException($"Valuepath type does not match. Expected: {typeof(TValue).FullName} was: {last.PropertyInfo.PropertyType.FullName}");
            }

            this.path = path;
        }

        public int Count => this.path.Count;

        public PathProperty Last => this.path.Last;

        public PathProperty this[int index] => this.path[index];

        public IEnumerator<PathProperty> GetEnumerator() => this.path.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public override string ToString() => $"x => x.{string.Join(".", this.path.Select(x => x.PropertyInfo.Name))}";

        /// <summary>
        /// Get the source of the last item in the path.
        /// </summary>
        /// <param name="root">The root instance for the path.</param>
        internal SourceAndValue<TValue> SourceAndValue(TSource root)
        {
            if (this.Count == 1)
            {
                return new SourceAndValue<TValue>(root, this[0].GetPropertyValue(root).Cast<TValue>());
            }

            object source = root;
            for (var i = 0; i < this.path.Count; i++)
            {
                var value = this.path[i].GetPropertyValue(source);
                if (i == this.path.Count - 1)
                {
                    return value.HasValue
                               ? new SourceAndValue<TValue>(source, value.Cast<TValue>())
                               : new SourceAndValue<TValue>(source, Maybe<TValue>.None);
                }

                if (value.GetValueOrDefault() == null)
                {
                    return new SourceAndValue<TValue>(source, Maybe<TValue>.None);
                }

                source = value.Value;
            }

            return new SourceAndValue<TValue>(source, Maybe<TValue>.None);
        }

        internal Maybe<TValue> GetValueFromRoot(object rootSource) => this.path.GetValueFromRoot<TValue>(rootSource);
    }
}
