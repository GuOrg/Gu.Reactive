namespace Gu.Reactive
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using Gu.Reactive.Internals;
    using JetBrains.Annotations;

    [DebuggerTypeProxy(typeof(CollectionDebugView<>))]
    [DebuggerDisplay("Count = {Count}")]
    public abstract class SynchronizedEditableView<T> : IList, IUpdater, IRefreshAble, IDisposable
    {
        protected readonly IList<T> Source;
        protected readonly CollectionSynchronizer<T> Tracker;
        private bool disposed;
        private object isUpdatingSourceItem;

        protected SynchronizedEditableView(IList<T> source)
            : this(source, source)
        {
            Ensure.NotNull(source, nameof(source));
            this.Source = source;
            this.Tracker = new CollectionSynchronizer<T>(source);
        }

        protected SynchronizedEditableView(IList<T> source, IEnumerable<T> sourceItems)
        {
            Ensure.NotNull(source, nameof(source));
            this.Source = source;
            this.Tracker = new CollectionSynchronizer<T>(sourceItems);
        }

        public virtual event PropertyChangedEventHandler PropertyChanged;

        public virtual event NotifyCollectionChangedEventHandler CollectionChanged;

        object IUpdater.IsUpdatingSourceItem => this.isUpdatingSourceItem;

        protected PropertyChangedEventHandler PropertyChangedEventHandler => this.PropertyChanged;

        protected NotifyCollectionChangedEventHandler NotifyCollectionChangedEventHandler => this.CollectionChanged;

        /// <summary>
        /// Dispose(true); //I am calling you from Dispose, it's safe
        /// GC.SuppressFinalize(this); //Hey, GC: don't bother calling finalize later
        /// </summary>
        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            this.Dispose(true);
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
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            this.PropertyChanged?.Invoke(this, e);
        }

        #region IList<TItem>

        public int Count => this.Tracker.Current.Count;

        public bool IsReadOnly => false;

        public T this[int index]
        {
            get { return this.Tracker.Current[index]; }

            set
            {
                var sourceIndex = this.Source.IndexOf(this.Tracker.Current[index]);
                this.Source[sourceIndex] = value;
            }
        }

        public IEnumerator<T> GetEnumerator() => this.Tracker.Current.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public void Add(T item) => this.Source.Add(item);

        public void Clear() => this.Source.Clear();

        public bool Contains(T item) => this.Tracker.Contains(item);

        public int IndexOf(T value) => this.Tracker.IndexOf(value);

        public void Insert(int index, T value) => this.InsertCore(index, value);

        private void InsertCore(int index, T value)
        {
            var i = this.Source.IndexOf(this.Tracker.Current[index]);
            this.Source.Insert(i, value);
        }

        public bool Remove(T item)
        {
            var result = this.Source.Remove(item);
            return result;
        }

        public void RemoveAt(int index)
        {
            this.RemoveAtCore(index);
        }

        private void RemoveAtCore(int index)
        {
            this.Source.Remove(this.Tracker.Current[index]);
        }

        public void CopyTo(T[] array, int arrayIndex) => this.Tracker.CopyTo(array, arrayIndex);

        #endregion  IList<TItem>

        #region IList

        object IList.this[int index]
        {
            get { return this[index]; }

            set
            {
                var old = this[index];
                this.isUpdatingSourceItem = old;
                this[index] = (T)value;
                this.RefreshNow(Diff.CreateReplaceEventArgs(value, old, index));
                this.isUpdatingSourceItem = null;
            }
        }

        bool IList.IsFixedSize => false;

        int IList.Add(object value)
        {
            // IList.Add happens when a new row is added in DataGrid, we need to notify here to avoid out of sync exception.
            this.isUpdatingSourceItem = value;
            ((IList)this.Source).Add(value); // Adding to inner
            this.RefreshNow(Diff.CreateAddEventArgs(value, this.Count));
            this.isUpdatingSourceItem = null;
            var index = this.Tracker.LastIndexOf((T)value);
            return index;
        }

        bool IList.Contains(object value) => this.Tracker.Contains(value);

        int IList.IndexOf(object value) => this.Tracker.IndexOf(value);

        void IList.Insert(int index, object value)
        {
            this.isUpdatingSourceItem = value;
            this.Insert(index, (T)value);
            this.RefreshNow(Diff.CreateAddEventArgs(value, index));
            this.isUpdatingSourceItem = null;
        }

        void IList.Remove(object value)
        {
            var index = this.IndexOf((T)value);
            if (index < 0)
            {
                return;
            }

            this.isUpdatingSourceItem = value;
            this.RemoveAtCore(index);
            this.RefreshNow(Diff.CreateRemoveEventArgs(value, index));
            this.isUpdatingSourceItem = null;
        }

        #endregion IList

        #region ICollection

        void ICollection.CopyTo(Array array, int index) => this.Tracker.CopyTo(array, index);

        object ICollection.SyncRoot => (this.Source as ICollection)?.SyncRoot;

        bool ICollection.IsSynchronized => (this.Source as ICollection)?.IsSynchronized == true;

        #endregion ICollection
    }
}
