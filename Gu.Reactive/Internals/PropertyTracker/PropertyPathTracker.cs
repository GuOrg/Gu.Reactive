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

        internal PropertyPathTracker(TSource source, PropertyPath<TSource, TValue> path)
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

        public event TrackedPropertyChangedEventHandler TrackedPropertyChanged;

        public int Count => this.parts.Count;

        public IPathPropertyTracker First => this.parts[0];

        public IPathPropertyTracker Last => this.parts[this.parts.Count - 1];

        public TSource Source => (TSource)this.parts[0].Source;

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

        internal SourceAndValue<INotifyPropertyChanged, object> SourceAndValue()
        {
            for (var i = this.parts.Count - 1; i >= 0; i--)
            {
                var part = this.parts[i];
                var source = part.Source;
                if (source != null)
                {
                    return i == this.parts.Count - 1
                               ? Reactive.SourceAndValue.Create(source, part.Property.Getter.GetMaybe(source))
                               : Reactive.SourceAndValue.Create(source, Maybe<object>.None);
                }
            }

            return Reactive.SourceAndValue.Create((INotifyPropertyChanged)null, Maybe<object>.None);
        }

        /// <summary>
        /// Refreshes the path recursively from source.
        /// This is for extra security in case changes are notified on different threads.
        /// </summary>
        internal void Refresh()
        {
            var source = this.parts[0].Source;
            for (var i = 1; i < this.parts.Count; i++)
            {
                source = (INotifyPropertyChanged)this.parts[i - 1].Property
                                                     .Getter
                                                     .GetMaybe(source)
                                                     .GetValueOrDefault();
                var part = this.parts[i];
                if (!ReferenceEquals(part.Source, source))
                {
                    part.Source = source;
                }
            }
        }

        private void ThrowIfDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }

        private void OnTrackedPropertyChanged(IPathPropertyTracker tracker, object sender, PropertyChangedEventArgs e, SourceAndValue<INotifyPropertyChanged, object> sourceandvalue)
        {
            this.TrackedPropertyChanged?.Invoke(tracker, sender, e, sourceandvalue);
        }
    }
}