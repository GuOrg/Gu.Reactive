namespace Gu.Reactive.Internals
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    ////[DebuggerTypeProxy(typeof(CollectionDebugView<>))]
    internal class PropertyPath<TSource, TValue> : IPropertyPath
    {
        private readonly PropertyPath path;

        internal PropertyPath(PropertyPath path)
        {
            this.path = path;
            if (this.Last.PropertyInfo.PropertyType != typeof(TValue))
            {
                throw new InvalidOperationException($"Valuepath type does not match. Expected: {typeof(TValue).FullName} was: {this.Last.PropertyInfo.PropertyType.FullName}");
            }
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
        internal SourceAndValue<INotifyPropertyChanged, TValue> SourceAndValue(TSource root)
        {
            if (this.Count == 1)
            {
                return Reactive.SourceAndValue.Create((INotifyPropertyChanged)root, this[0].GetPropertyValue(root).Cast<TValue>());
            }

            var source = (INotifyPropertyChanged)root;
            for (var i = 0; i < this.path.Count; i++)
            {
                var value = this.path[i].GetPropertyValue(source);
                if (i == this.path.Count - 1)
                {
                    return value.HasValue
                               ? Reactive.SourceAndValue.Create(source, value.Cast<TValue>())
                               : Reactive.SourceAndValue.Create(source, Maybe<TValue>.None);
                }

                if (value.GetValueOrDefault() == null)
                {
                    return Reactive.SourceAndValue.Create(source, Maybe<TValue>.None);
                }

                source = (INotifyPropertyChanged)value.Value;
            }

            return Reactive.SourceAndValue.Create(source, Maybe<TValue>.None);
        }
    }
}
