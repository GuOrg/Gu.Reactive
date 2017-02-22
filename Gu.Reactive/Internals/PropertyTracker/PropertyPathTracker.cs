namespace Gu.Reactive.Internals
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    internal sealed class PropertyPathTracker : IReadOnlyList<PathPropertyTracker>, IDisposable
    {
        private readonly IReadOnlyList<PathPropertyTracker> parts;
        private bool disposed;

        internal PropertyPathTracker(INotifyPropertyChanged source, IPropertyPath path)
        {
            var items = new PathPropertyTracker[path.Count];
            for (var i = 0; i < path.Count; i++)
            {
                items[i] = new PathPropertyTracker(this, path[i]);
            }

            this.parts = items;
            items[0].Source = source;
        }

        public int Count => this.parts.Count;

        public PathPropertyTracker First => this.parts[0];

        public PathPropertyTracker Last => this.parts[this.parts.Count - 1];

        public PathPropertyTracker this[int index]
        {
            get
            {
                this.ThrowIfDisposed();
                return this.parts[index];
            }
        }

        public IEnumerator<PathPropertyTracker> GetEnumerator()
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
            foreach (var part in this.parts)
            {
                part.Dispose();
            }
        }

        public override string ToString() => $"x => x.{string.Join(".", this.parts.Select(x => x.PathProperty.PropertyInfo.Name))}";

        internal PathPropertyTracker GetNext(PathPropertyTracker pathPropertyTracker)
        {
            for (var i = 0; i < this.parts.Count - 1; i++)
            {
                var tracker = this.parts[i];
                if (ReferenceEquals(tracker, pathPropertyTracker))
                {
                    return this.parts[i + 1];
                }
            }

            return null;
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
                source = (INotifyPropertyChanged)this.parts[i - 1].PathProperty
                                                      .GetPropertyValue(source)
                                                      .ValueOrDefault();
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
    }
}