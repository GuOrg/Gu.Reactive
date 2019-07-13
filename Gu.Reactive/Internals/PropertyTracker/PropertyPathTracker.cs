namespace Gu.Reactive.Internals
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    internal sealed class PropertyPathTracker<TSource, TValue> : IReadOnlyList<IPropertyTracker>, IPropertyPathTracker
        where TSource : class, INotifyPropertyChanged
    {
        private readonly IReadOnlyList<IPropertyTracker> parts;
        private bool disposed;

        internal PropertyPathTracker(TSource source, IReadOnlyList<INotifyingGetter> path)
        {
            var items = new IPropertyTracker[path.Count];
            for (var i = 0; i < path.Count; i++)
            {
                items[i] = path[i].CreateTracker(this);
            }

            this.parts = items;
            items[0].Source = source;
            this.Refresh();
            this.Last.TrackedPropertyChanged += this.OnTrackedPropertyChanged;
        }

        internal event TrackedPropertyChangedEventHandler<TValue> TrackedPropertyChanged;

        public int Count => this.parts.Count;

        internal TSource Source => (TSource)this.parts[0].Source;

        private IPropertyTracker<TValue> Last => (IPropertyTracker<TValue>)this.parts[this.parts.Count - 1];

        public IPropertyTracker this[int index]
        {
            get
            {
                this.ThrowIfDisposed();
                return this.parts[index];
            }
        }

        IPropertyTracker IPropertyPathTracker.GetNext(IPropertyTracker tracker)
        {
            for (var i = 0; i < this.parts.Count - 1; i++)
            {
                var candidate = this.parts[i];
                if (ReferenceEquals(candidate, tracker))
                {
                    return this.parts[i + 1];
                }
            }

            return null;
        }

        public IEnumerator<IPropertyTracker> GetEnumerator()
        {
            this.ThrowIfDisposed();
            return this.parts.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            this.ThrowIfDisposed();
            return this.GetEnumerator();
        }

        /// <summary>
        /// Make the class sealed when using this.
        /// Call ThrowIfDisposed at the start of all public methods.
        /// </summary>
        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            this.Last.TrackedPropertyChanged -= this.OnTrackedPropertyChanged;
            foreach (var part in this.parts)
            {
                part.Dispose();
            }
        }

        public override string ToString() => $"x => x.{string.Join(".", this.parts.Select(x => x.Getter.Property.Name))}";

        /// <summary>
        /// Gets the value recursively from the root.
        /// This is for extra security in case changes are notified on different threads.
        /// </summary>
        internal SourceAndValue<INotifyPropertyChanged, TValue> SourceAndValue()
        {
            var valueSource = (INotifyPropertyChanged)this.Source;
            var value = Maybe.Some(valueSource);
            for (var i = 0; i < this.parts.Count; i++)
            {
                var part = this.parts[i];
                var newSource = value.GetValueOrDefault();

                part.Source = newSource;
                if (newSource != null)
                {
                    valueSource = newSource;
                }

                if (i == this.parts.Count - 1)
                {
                    return Reactive.SourceAndValue.Create(valueSource, this.Last.GetMaybe());
                }

                value = newSource == null
                            ? Maybe<INotifyPropertyChanged>.None
                            : Maybe<INotifyPropertyChanged>.Some((INotifyPropertyChanged)part.Getter.GetValue(newSource));
            }

            return Reactive.SourceAndValue.Create(valueSource, Maybe<TValue>.None);
        }

        private void Refresh()
        {
            // Called for side effect of refreshing the path
            _ = this.SourceAndValue();
        }

        private void ThrowIfDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(typeof(PropertyPathTracker<TSource, TValue>).FullName);
            }
        }

        private void OnTrackedPropertyChanged(IPropertyTracker tracker, object sender, PropertyChangedEventArgs e, SourceAndValue<INotifyPropertyChanged, TValue> sourceandvalue)
        {
            this.TrackedPropertyChanged?.Invoke(tracker, sender, e, sourceandvalue);
        }
    }
}
