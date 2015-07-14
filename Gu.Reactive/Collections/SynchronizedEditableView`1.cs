namespace Gu.Reactive
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using Gu.Reactive.Annotations;
    using Gu.Reactive.Internals;

    public abstract class SynchronizedEditableView<T> : IList, IUpdater, IDisposable
    {
        protected readonly IList<T> Source;
        protected readonly CollectionSynchronizer<T> Synchronized;
        private bool _disposed;
        private object _isUpdatingSourceItem;

        protected SynchronizedEditableView(IList<T> source)
        {
            Ensure.NotNull(source, "source");
            Source = source;
            Synchronized = new CollectionSynchronizer<T>(source);
        }

        public virtual event PropertyChangedEventHandler PropertyChanged;

        public virtual event NotifyCollectionChangedEventHandler CollectionChanged;

        object IUpdater.IsUpdatingSourceItem
        {
            get { return _isUpdatingSourceItem; }
        }

        protected PropertyChangedEventHandler PropertyChangedEventHandler
        {
            get { return PropertyChanged; }
        }

        protected NotifyCollectionChangedEventHandler NotifyCollectionChangedEventHandler
        {
            get { return CollectionChanged; }
        }

        /// <summary>
        /// Dispose(true); //I am calling you from Dispose, it's safe
        /// GC.SuppressFinalize(this); //Hey, GC: don't bother calling finalize later
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public abstract void Refresh();

        /// <summary>
        /// Pass null as scheduler here, change came from the ui thread.
        /// </summary>
        protected abstract void RefreshNow(NotifyCollectionChangedEventArgs change);

        protected abstract void Refresh(IReadOnlyList<NotifyCollectionChangedEventArgs> changes);

        protected virtual void Dispose(bool disposing)
        {
        }

        protected void VerifyDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #region IList<TItem>

        public int Count
        {
            get { return Synchronized.Current.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public T this[int index]
        {
            get { return Synchronized.Current[index]; }
            set
            {
                var sourceIndex = Source.IndexOf(Synchronized.Current[index]);
                Source[sourceIndex] = value;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Synchronized.Current.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            Source.Add(item);
        }

        public void Clear()
        {
            Source.Clear();
        }

        public bool Contains(T item)
        {
            return Synchronized.Contains(item);
        }

        public int IndexOf(T value)
        {
            return Synchronized.IndexOf(value);
        }

        public void Insert(int index, T value)
        {
            InsertCore(index, value);
        }

        private void InsertCore(int index, T value)
        {
            var i = Source.IndexOf(Synchronized.Current[index]);
            Source.Insert(i, value);
        }

        public bool Remove(T item)
        {
            var result = Source.Remove(item);
            return result;
        }

        public void RemoveAt(int index)
        {
            RemoveAtCore(index);
        }

        private void RemoveAtCore(int index)
        {
            Source.Remove(Synchronized.Current[index]);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Synchronized.CopyTo(array, arrayIndex);
        }

        #endregion  IList<TItem>

        #region IList

        object IList.this[int index]
        {
            get { return this[index]; }
            set
            {
                var old = this[index];
                _isUpdatingSourceItem = old;
                this[index] = (T)value;
                RefreshNow(Diff.CreateReplaceEventArgs(value, old, index));
                _isUpdatingSourceItem = null;
            }
        }

        bool IList.IsFixedSize
        {
            get { return false; }
        }

        int IList.Add(object value)
        {
            // IList.Add happens when a new row is added in DataGrid, we need to notify here to avoid out of sync exception.
            _isUpdatingSourceItem = value;
            ((IList)Source).Add(value); // Adding to inner
            RefreshNow(Diff.CreateAddEventArgs(value, Count));
            _isUpdatingSourceItem = null;
            var index = Synchronized.LastIndexOf((T)value);
            return index;
        }

        bool IList.Contains(object value)
        {
            return Synchronized.Contains(value);
        }

        int IList.IndexOf(object value)
        {
            return Synchronized.IndexOf(value);
        }

        void IList.Insert(int index, object value)
        {
            _isUpdatingSourceItem = value;
            Insert(index, (T)value);
            RefreshNow(Diff.CreateAddEventArgs(value, index));
            _isUpdatingSourceItem = null;
        }

        void IList.Remove(object value)
        {
            var index = IndexOf((T)value);
            if (index < 0)
            {
                return;
            }
            _isUpdatingSourceItem = value;
            RemoveAtCore(index);
            RefreshNow(Diff.CreateRemoveEventArgs(value, index));
            _isUpdatingSourceItem = null;
        }

        #endregion IList

        #region ICollection

        void ICollection.CopyTo(Array array, int index)
        {
            Synchronized.CopyTo(array, index);
        }

        object ICollection.SyncRoot
        {
            get { return ((ICollection)Source).SyncRoot; }
        }

        bool ICollection.IsSynchronized
        {
            get { return ((ICollection)Source).IsSynchronized; }
        }

        #endregion ICollection
    }
}
