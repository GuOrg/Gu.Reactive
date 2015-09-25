namespace Gu.Reactive
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    using Gu.Reactive.Annotations;
    using Gu.Reactive.Internals;

    [DebuggerTypeProxy(typeof(CollectionDebugView<>))]
    [DebuggerDisplay("Count = {Count}")] 
    public abstract class SynchronizedEditableView<T> : IList, IUpdater, IRefreshAble, IDisposable
    {
        protected readonly IList<T> Source;
        protected readonly CollectionSynchronizer<T> Synchronized;
        private bool _disposed;
        private object _isUpdatingSourceItem;

        protected SynchronizedEditableView(IList<T> source)
            : this(source, source)
        {
            Ensure.NotNull(source, nameof(source));
            Source = source;
            Synchronized = new CollectionSynchronizer<T>(source);
        }

        protected SynchronizedEditableView(IList<T> source, IEnumerable<T> sourceItems )
        {
            Ensure.NotNull(source, nameof(source));
            Source = source;
            Synchronized = new CollectionSynchronizer<T>(sourceItems);
        }

        public virtual event PropertyChangedEventHandler PropertyChanged;

        public virtual event NotifyCollectionChangedEventHandler CollectionChanged;

        object IUpdater.IsUpdatingSourceItem => _isUpdatingSourceItem;

        protected PropertyChangedEventHandler PropertyChangedEventHandler => PropertyChanged;

        protected NotifyCollectionChangedEventHandler NotifyCollectionChangedEventHandler => CollectionChanged;

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
            PropertyChanged?.Invoke(this, e);
        }

        #region IList<TItem>

        public int Count => Synchronized.Current.Count;

        public bool IsReadOnly => false;

        public T this[int index]
        {
            get { return Synchronized.Current[index]; }
            set
            {
                var sourceIndex = Source.IndexOf(Synchronized.Current[index]);
                Source[sourceIndex] = value;
            }
        }

        public IEnumerator<T> GetEnumerator() => Synchronized.Current.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(T item) => Source.Add(item);

        public void Clear() => Source.Clear();

        public bool Contains(T item) => Synchronized.Contains(item);

        public int IndexOf(T value) => Synchronized.IndexOf(value);

        public void Insert(int index, T value) => InsertCore(index, value);

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

        public void CopyTo(T[] array, int arrayIndex) => Synchronized.CopyTo(array, arrayIndex);

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

        bool IList.IsFixedSize => false;

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

        bool IList.Contains(object value) => Synchronized.Contains(value);

        int IList.IndexOf(object value) => Synchronized.IndexOf(value);

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

        void ICollection.CopyTo(Array array, int index) => Synchronized.CopyTo(array, index);

        object ICollection.SyncRoot => ((ICollection)Source).SyncRoot;

        bool ICollection.IsSynchronized => ((ICollection)Source).IsSynchronized;

        #endregion ICollection
    }
}
