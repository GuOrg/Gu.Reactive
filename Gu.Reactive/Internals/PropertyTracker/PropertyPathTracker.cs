namespace Gu.Reactive.Internals
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    internal sealed class PropertyPathTracker<TSource, TValue> : IReadOnlyList<IPathPropertyTracker>, IPropertyPathTracker
        where TSource : class, INotifyPropertyChanged
    {
        private readonly IReadOnlyList<IPathPropertyTracker> parts;
        private bool disposed;

        internal PropertyPathTracker(TSource source, NotifyingPath<TSource, TValue> path)
        {
            var items = new IPathPropertyTracker[path.Count];
            for (var i = 0; i < path.Count; i++)
            {
                items[i] = path[i].CreateTracker(this);
            }

            this.parts = items;
            items[0].Source = source;
            this.Refresh();
            this.Last.TrackedPropertyChanged += this.OnTrackedPropertyChanged;
        }

        public event TrackedPropertyChangedEventHandler<TValue> TrackedPropertyChanged;

        public int Count => this.parts.Count;

        public TSource Source => (TSource)this.parts[0].Source;

        private IPathPropertyTracker<TValue> Last => (IPathPropertyTracker<TValue>)this.parts[this.parts.Count - 1];

        public IPathPropertyTracker this[int index]
        {
            get
            {
                this.ThrowIfDisposed();
                return this.parts[index];
            }
        }

        IPathPropertyTracker IPropertyPathTracker.GetNext(IPathPropertyTracker tracker)
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

        IPathPropertyTracker IPropertyPathTracker.GetPrevious(IPathPropertyTracker tracker)
        {
            for (var i = 1; i < this.parts.Count; i++)
            {
                var candidate = this.parts[i];
                if (ReferenceEquals(candidate, tracker))
                {
                    return this.parts[i - 1];
                }
            }

            return null;
        }

        public IEnumerator<IPathPropertyTracker> GetEnumerator()
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
        /// Call ThrowIfDisposed at the start of all public methods
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

        public override string ToString() => $"x => x.{string.Join(".", this.parts.Select(x => x.Property.Getter.Property.Name))}";

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
                    return Reactive.SourceAndValue.Create(valueSource, this.Last.GetValue());
                }

                value = newSource == null
                            ? Maybe<INotifyPropertyChanged>.None
                            : Maybe<INotifyPropertyChanged>.Some((INotifyPropertyChanged)part.Property.Getter.GetValue(newSource));
            }

            return Reactive.SourceAndValue.Create(valueSource, Maybe<TValue>.None);
        }

        internal void Refresh()
        {
            // Called for side effect of refreshing the path
            this.SourceAndValue().IgnoreReturnValue();
        }

        private void ThrowIfDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }

        private void OnTrackedPropertyChanged(IPathPropertyTracker tracker, object sender, PropertyChangedEventArgs e, SourceAndValue<INotifyPropertyChanged, TValue> sourceandvalue)
        {
            this.TrackedPropertyChanged?.Invoke(tracker, sender, e, sourceandvalue);
        }
    }
}