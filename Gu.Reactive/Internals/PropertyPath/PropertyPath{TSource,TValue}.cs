namespace Gu.Reactive.Internals
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    internal abstract class PropertyPath<TSource, TValue, TPart> : IPropertyPath
        where TPart : IPathProperty
    {
        private readonly IReadOnlyList<TPart> parts;

        internal PropertyPath(IReadOnlyList<TPart> parts)
        {
            this.parts = parts;
            if (this.Last.Getter.Property.PropertyType != typeof(TValue))
            {
                throw new InvalidOperationException($"Valuepath type does not match. Expected: {typeof(TValue).FullName} was: {this.Last.Getter.Property.PropertyType.FullName}");
            }
        }

        public int Count => this.parts.Count;

        public TPart Last => this.parts[this.parts.Count - 1];

        public TPart this[int index] => this.parts[index];

        IPathProperty IReadOnlyList<IPathProperty>.this[int index] => this.parts[index];

        public IEnumerator<TPart> GetEnumerator() => this.parts.GetEnumerator();

        IEnumerator<IPathProperty> IEnumerable<IPathProperty>.GetEnumerator() => this.parts.Cast<IPathProperty>().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public override string ToString() => $"x => x.{string.Join(".", this.parts.Select(x => x.Getter.Property.Name))}";

        /// <summary>
        /// Get the source of the last item in the path.
        /// </summary>
        /// <param name="root">The root instance for the path.</param>
        internal SourceAndValue<INotifyPropertyChanged, TValue> SourceAndValue(TSource root)
        {
            if (this.Count == 1)
            {
                return Reactive.SourceAndValue.Create((INotifyPropertyChanged)root, this[0].Getter.GetMaybe(root).Cast<TValue>());
            }

            var source = (INotifyPropertyChanged)root;
            for (var i = 0; i < this.parts.Count; i++)
            {
                var value = this.parts[i].Getter.GetMaybe(source);
                if (i == this.parts.Count - 1)
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
