namespace Gu.Reactive.Internals
{
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;

    internal abstract class PropertyPath<TSource, TValue> : IPropertyPath
    {
        private readonly IReadOnlyList<IGetter> parts;

        internal PropertyPath(IReadOnlyList<IGetter> parts)
        {
            Debug.Assert(parts[0].Property.ReflectedType == typeof(TSource), "parts[0].Property.ReflectedType == typeof(TSource)");
            Debug.Assert(parts[parts.Count - 1].Property.PropertyType == typeof(TValue), "parts[0].Property.DeclaringType == typeof(TSource)");
            this.parts = parts;
        }

        public int Count => this.parts.Count;

        internal IGetter<TValue> Last => (IGetter<TValue>)this.parts[this.parts.Count - 1];

        public IGetter this[int index] => this.parts[index];

        public IEnumerator<IGetter> GetEnumerator() => this.parts.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public override string ToString() => $"x => x.{string.Join(".", this.parts.Select(x => x.Property.Name))}";

        /// <summary>
        /// Get the source of the last item in the path.
        /// </summary>
        /// <param name="root">The root instance for the path.</param>
        internal SourceAndValue<INotifyPropertyChanged, TValue> SourceAndValue(TSource root)
        {
            if (this.Count == 1)
            {
                return Reactive.SourceAndValue.Create((INotifyPropertyChanged)root, this.Last.GetMaybe(root));
            }

            var source = (INotifyPropertyChanged)root;
            for (var i = 0; i < this.parts.Count; i++)
            {
                var value = this.parts[i].GetMaybe(source);
                if (i == this.parts.Count - 1)
                {
                    return value.HasValue
                               ? Reactive.SourceAndValue.Create(source, this.Last.GetMaybe(source))
                               : Reactive.SourceAndValue.Create(source, Maybe<TValue>.None);
                }

                if (value.GetValueOrDefault() is null)
                {
                    return Reactive.SourceAndValue.Create(source, Maybe<TValue>.None);
                }

                source = (INotifyPropertyChanged)value.Value;
            }

            return Reactive.SourceAndValue.Create(source, Maybe<TValue>.None);
        }
    }
}
