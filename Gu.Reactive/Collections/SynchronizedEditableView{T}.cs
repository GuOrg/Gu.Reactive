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

    /// <summary>
    /// A synchronized view of a collection that supports two way bindings.
    /// </summary>
    [DebuggerTypeProxy(typeof(CollectionDebugView<>))]
    [DebuggerDisplay("Count = {this.Count}")]
    [Serializable]
    public abstract class SynchronizedEditableView<T> : IList, IList<T>, IUpdater, IRefreshAble, IDisposable, INotifyPropertyChanged, INotifyCollectionChanged
    {
        private readonly object syncRoot;

        private object isUpdatingSourceItem;
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronizedEditableView{T}"/> class.
        /// </summary>
        protected SynchronizedEditableView(IList<T> source)
            : this(source, source)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronizedEditableView{T}"/> class.
        /// </summary>
        protected SynchronizedEditableView(IList<T> source, IEnumerable<T> sourceItems)
        {
            Ensure.NotNull(source, nameof(source));
            this.Source = source;
            this.syncRoot = (this.Source as ICollection)?.SyncRoot ?? new object();
            this.Tracker = new CollectionSynchronizer<T>(sourceItems);
        }

        /// <inheritdoc/>
        public virtual event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc/>
        public virtual event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <inheritdoc/>
        public int Count => this.Tracker.Count;

        /// <inheritdoc/>
        public bool IsReadOnly => false;

        /// <inheritdoc/>
        object ICollection.SyncRoot => this.syncRoot;

        /// <inheritdoc/>
        bool ICollection.IsSynchronized => (this.Source as ICollection)?.IsSynchronized == true;

        /// <inheritdoc/>
        bool IList.IsFixedSize => false;

        /// <inheritdoc/>
        object IUpdater.CurrentlyUpdatingSourceItem => this.isUpdatingSourceItem;

        /// <summary>
        /// The source collection.
        /// </summary>
        protected IList<T> Source { get; }

        /// <summary>
        /// The <see cref="CollectionSynchronizer{T}"/> that keeps this in sync with <see cref="Source"/>
        /// </summary>
        protected CollectionSynchronizer<T> Tracker { get; }

        /// <summary>
        /// Returns true if there are any subscribers to the <see cref="PropertyChanged"/> or <see cref="CollectionChanged"/> events.
        /// </summary>
        protected bool HasListeners => this.PropertyChanged != null || this.CollectionChanged != null;

        /// <inheritdoc/>
        public T this[int index]
        {
            get
            {
                return this.Tracker[index];
            }

            set
            {
                var sourceIndex = this.Source.IndexOf(this.Tracker[index]);
                this.Source[sourceIndex] = value;
            }
        }

        /// <inheritdoc/>
        object IList.this[int index]
        {
            get
            {
                return this[index];
            }

            set
            {
                var old = this[index];
                this.isUpdatingSourceItem = old;
                this[index] = (T)value;
                this.RefreshNow(Diff.CreateReplaceEventArgs(value, old, index));
                this.isUpdatingSourceItem = null;
            }
        }

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator() => this.Tracker.GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <inheritdoc/>
        public void Add(T item) => this.Source.Add(item);

        /// <inheritdoc/>
        public void Clear() => this.Source.Clear();

        /// <inheritdoc/>
        public bool Contains(T item) => this.Tracker.Contains(item);

        /// <inheritdoc/>
        public int IndexOf(T value) => this.Tracker.IndexOf(value);

        /// <inheritdoc/>
        public void Insert(int index, T value) => this.InsertCore(index, value);

        /// <inheritdoc/>
        public bool Remove(T item)
        {
            var result = this.Source.Remove(item);
            return result;
        }

        /// <inheritdoc/>
        public void RemoveAt(int index)
        {
            this.RemoveAtCore(index);
        }

        /// <inheritdoc/>
        public void CopyTo(T[] array, int arrayIndex) => this.Tracker.CopyTo(array, arrayIndex);

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        bool IList.Contains(object value) => this.Tracker.Contains(value);

        /// <inheritdoc/>
        int IList.IndexOf(object value) => this.Tracker.IndexOf(value);

        /// <inheritdoc/>
        void IList.Insert(int index, object value)
        {
            this.isUpdatingSourceItem = value;
            this.Insert(index, (T)value);
            this.RefreshNow(Diff.CreateAddEventArgs(value, index));
            this.isUpdatingSourceItem = null;
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        void ICollection.CopyTo(Array array, int index) => ((ICollection)this.Tracker).CopyTo(array, index);

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public abstract void Refresh();

        /// <summary>
        /// Pass null as scheduler here, change came from the ui thread.
        /// </summary>
        protected abstract void RefreshNow(NotifyCollectionChangedEventArgs change);

        /// <summary>
        /// Refreshes the view. May be deferred if there is a buffer time.
        /// </summary>
        protected abstract void Refresh(IReadOnlyList<NotifyCollectionChangedEventArgs> changes);

        /// <summary>
        /// Protected implementation of Dispose pattern.
        /// </summary>
        /// <param name="disposing">true: safe to free managed resources</param>
        protected virtual void Dispose(bool disposing)
        {
        }

        /// <summary>
        /// Throws an <see cref="ObjectDisposedException"/> if the instance is disposed.
        /// </summary>
        protected void ThrowIfDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }

        /// <summary>
        /// Calls <see cref="OnPropertyChanged(PropertyChangedEventArgs)"/>
        /// </summary>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Raise PropertyChanged event to any listeners.
        /// Properties/methods modifying this <see cref="SynchronizedEditableView{T}"/> will raise
        /// a property changed event through this virtual method.
        /// </summary>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            this.PropertyChanged?.Invoke(this, e);
        }

        /// <summary>
        /// Raise CollectionChanged event to any listeners.
        /// Properties/methods modifying this <see cref="EditableListView{T}"/> will raise
        /// a collection changed event through this virtual method.
        /// </summary>
        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            this.CollectionChanged?.Invoke(this, e);
        }

        private void RemoveAtCore(int index)
        {
            this.Source.Remove(this.Tracker[index]);
        }

        private void InsertCore(int index, T value)
        {
            var i = this.Source.IndexOf(this.Tracker[index]);
            this.Source.Insert(i, value);
        }
    }
}
