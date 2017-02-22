namespace Gu.Reactive.Internals
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    internal sealed class PropertyPathTracker : IReadOnlyList<IPathPropertyTracker>, IDisposable
    {
        private readonly IReadOnlyList<IPathPropertyTracker> parts;
        private bool disposed;

        internal PropertyPathTracker(RootPropertyTracker root, IPropertyPath path)
        {
            var items = new IPathPropertyTracker[path.Count + 1];
            items[0] = root;
            IPathPropertyTracker previous = root;
            for (var i = 0; i < path.Count; i++)
            {
                var item = new PathPropertyTracker(previous, path[i]);
                items[i + 1] = item;
                previous = item;
            }

            this.parts = items;
        }

        public int Count => this.parts.Count;

        public IPathPropertyTracker this[int index]
        {
            get
            {
                this.ThrowIfDisposed();
                return this.parts[index];
            }
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
            foreach (var part in this.parts)
            {
                part.Dispose();
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