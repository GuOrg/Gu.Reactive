namespace Gu.Reactive.PropertyPathStuff
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    internal sealed class NotifyingPath : IReadOnlyList<INotifyingPathItem>, IDisposable
    {
        private readonly IReadOnlyList<INotifyingPathItem> parts;
        private bool disposed;

        internal NotifyingPath(RootItem root, IPropertyPath path)
        {
            var items = new INotifyingPathItem[path.Count + 1];
            items[0] = root;
            INotifyingPathItem previous = root;
            for (var i = 0; i < path.Count; i++)
            {
                var item = new NotifyingPathItem(previous, path[i]);
                items[i + 1] = item;
                previous = item;
            }

            this.parts = items;
        }

        public int Count => this.parts.Count;

        public INotifyingPathItem this[int index]
        {
            get
            {
                this.ThrowIfDisposed();
                return this.parts[index];
            }
        }

        public IEnumerator<INotifyingPathItem> GetEnumerator()
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