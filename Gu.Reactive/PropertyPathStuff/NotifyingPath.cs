namespace Gu.Reactive.PropertyPathStuff
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    internal sealed class NotifyingPath : IReadOnlyList<INotifyingPathItem>, IDisposable
    {
        private readonly IReadOnlyList<INotifyingPathItem> parts;
        private readonly RootItem root;
        private bool disposed;

        internal NotifyingPath(RootItem root, IPropertyPath path)
        {
            this.root = root;
            var items = new INotifyingPathItem[path.Count + 1];
            items[0] = root;
            INotifyingPathItem previous = root;
            for (int i = 0; i < path.Count; i++)
            {
                var item = new NotifyingPathItem(previous, path[i]);
                items[i + 1] = item;
                previous = item;
            }

            this.parts = items;
        }

        public int Count => this.parts.Count;

        internal INotifyPropertyChanged Source
        {
            get
            {
                this.VerifyDisposed();
                return (INotifyPropertyChanged)((RootItem)this.parts[0]).Value;
            }

            set
            {
                this.VerifyDisposed();
                this.root.Value = value;
            }
        }

        internal object LastSource => ((NotifyingPathItem)this.parts.Last()).Previous.Value;

        public INotifyingPathItem this[int index]
        {
            get
            {
                this.VerifyDisposed();
                return this.parts[index];
            }
        }

        public IEnumerator<INotifyingPathItem> GetEnumerator()
        {
            this.VerifyDisposed();
            return this.parts.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            this.VerifyDisposed();
            return this.GetEnumerator();
        }

        /// <summary>
        /// Make the class sealed when using this.
        /// Call VerifyDisposed at the start of all public methods
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

        private void VerifyDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(
                    this.GetType()
                        .FullName);
            }
        }
    }
}