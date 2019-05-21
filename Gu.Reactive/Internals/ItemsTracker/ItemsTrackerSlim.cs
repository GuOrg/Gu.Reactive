namespace Gu.Reactive.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;

    internal abstract class ItemsTrackerSlim : IDisposable
    {
        public event Action<PropertyChangedEventArgs> ItemPropertyChanged;

        protected bool Disposed { get; private set; }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        internal static ItemsTrackerSlim Create<TCollection, TItem, TProperty>(
            TCollection source,
            NotifyingPath<TItem, TProperty> path)
            where TCollection : class, IEnumerable<TItem>, INotifyCollectionChanged
            where TItem : class, INotifyPropertyChanged
        {
            if (path.Count == 1)
            {
                return new SimpleItemsTrackerSlim<TCollection, TItem, TProperty>(
                    source,
                    (Getter<TItem, TProperty>)path[0]);
            }

            return new NestedItemsTrackerSlim<TCollection, TItem, TProperty>(source, path);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.Disposed)
            {
                return;
            }

            this.Disposed = true;
        }

        protected virtual void ThrowIfDisposed()
        {
            if (this.Disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }

        protected virtual void OnItemPropertyChanged(PropertyChangedEventArgs e)
        {
            this.ItemPropertyChanged?.Invoke(e);
        }
    }
}
