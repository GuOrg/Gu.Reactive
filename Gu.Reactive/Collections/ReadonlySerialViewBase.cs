namespace Gu.Reactive
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// A base class for swapping out an <see cref="IEnumerable{T}"/> source and get notifications.
    /// </summary>
    [DebuggerTypeProxy(typeof(CollectionDebugView<>))]
    //// ReSharper disable once UseNameofExpression
    [DebuggerDisplay("Count = {Count}")]
    public abstract class ReadonlySerialViewBase<T> : IRefreshAble, IList, IDisposable, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private static readonly IReadOnlyList<T> Empty = new T[0];
        private readonly CollectionSynchronizer<T> tracker;

        private bool disposed;

        protected ReadonlySerialViewBase()
            : this(null, true, true)
        {
        }

        protected ReadonlySerialViewBase(IEnumerable<T> source)
            : this(source, true, true)
        {
        }

        protected ReadonlySerialViewBase(bool isreadonly, bool isFixedSize)
            : this(null, isreadonly, isFixedSize)
        {
        }

        protected ReadonlySerialViewBase(IEnumerable<T> source, bool isreadonly, bool isFixedSize)
        {
            this.IsReadOnly = isreadonly;
            this.IsFixedSize = isFixedSize;
            this.Source = source ?? Empty;
            this.tracker = new CollectionSynchronizer<T>(source ?? Empty);
        }

        /// <inheritdoc/>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The source collection.
        /// </summary>
        protected IEnumerable<T> Source { get; private set; }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Refresh this collection from <see cref="Source"/> and notify changes.
        /// </summary>
        public virtual void Refresh()
        {
            this.ThrowIfDisposed();
            lock (this.Source.SyncRootOrDefault(this.tracker.SyncRoot))
            {
                lock (this.tracker.SyncRoot)
                {
                    (this.Source as IRefreshAble)?.Refresh();
                    var source = this.Source.AsReadOnly();
                    this.tracker.Reset(this, source, null, this.PropertyChanged, this.CollectionChanged);
                }
            }
        }

        /// <summary>
        /// Raise PropertyChanged event to any listeners.
        /// Properties/methods modifying this <see cref="ReadonlySerialViewBase{T}"/> will raise
        /// a property changed event through this virtual method.
        /// </summary>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Update the source collection and notify about changes.
        /// </summary>
        protected void SetSource(IEnumerable<T> source)
        {
            this.ThrowIfDisposed();
            this.Source = source ?? Empty;
            this.Refresh();
        }

        /// <summary>
        /// Protected implementation of Dispose pattern.
        /// </summary>
        /// <param name="disposing">true: safe to free managed resources</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            if (disposing)
            {
                // Free any other managed objects here.
            }

            // Free any unmanaged objects here.
        }

        /// <summary>
        /// Set <see cref="Source"/> to empty and notify about changes.
        /// </summary>
        protected void ClearSource()
        {
            this.ThrowIfDisposed();
            this.Source = Empty;
            this.Refresh();
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
        /// Throws an <see cref="ObjectDisposedException"/> if the instance is disposed.
        /// Invokes <paramref name="action"/> if not disposed.
        /// </summary>
        /// <param name="action">The action to invoke.</param>
        protected void ThrowIfDisposed(Action action)
        {
            this.ThrowIfDisposed();
            action();
        }

        /// <summary>
        /// Throws an <see cref="ObjectDisposedException"/> if the instance is disposed.
        /// Returns <paramref name="result"/> if not disposed.
        /// </summary>
        /// <param name="result">The action to invoke.</param>
        protected TResult ThrowIfDisposed<TResult>(Func<TResult> result)
        {
            this.ThrowIfDisposed();
            return result();
        }

        /// <summary>
        /// Syncronize with source and notify about changes.
        /// </summary>
        protected virtual void Refresh(IReadOnlyList<NotifyCollectionChangedEventArgs> changes)
        {
            lock (this.Source.SyncRootOrDefault(this.tracker.SyncRoot))
            {
                lock (this.tracker.SyncRoot)
                {
                    var source = this.Source.AsReadOnly();
                    this.tracker.Refresh(this, source, changes, null, this.PropertyChanged, this.CollectionChanged);
                }
            }
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

        /// <summary>
        /// Raise PropertyChanged event to any listeners.
        /// Properties/methods modifying this <see cref="EditableListView{T}"/> will raise
        /// a property changed event through this virtual method.
        /// </summary>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            this.PropertyChanged?.Invoke(this, e);
        }

        #region IReadOnlyList<T> & IList

        public int Count => this.ThrowIfDisposed(() => this.tracker.Count);

        public bool IsReadOnly { get; }

        public bool IsFixedSize { get; }

        object ICollection.SyncRoot => this.tracker.SyncRoot;

        bool ICollection.IsSynchronized => this.tracker.IsSynchronized;

        object IList.this[int index]
        {
            get { return this[index]; }
            set { ThrowHelper.ThrowCollectionIsReadonly(); }
        }

        public T this[int index] => this.ThrowIfDisposed(() => this.tracker.Current[index]);

        public IEnumerator<T> GetEnumerator() => this.ThrowIfDisposed(() => this.tracker.GetEnumerator());

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        int IList.Add(object value) => ThrowHelper.ThrowCollectionIsReadonly<int>();

        bool IList.Contains(object value) => this.tracker.Contains(value);

        void IList.Clear() => ThrowHelper.ThrowCollectionIsReadonly();

        int IList.IndexOf(object value) => this.tracker.IndexOf(value);

        void IList.Insert(int index, object value) => ThrowHelper.ThrowCollectionIsReadonly();

        void IList.Remove(object value) => ThrowHelper.ThrowCollectionIsReadonly();

        void IList.RemoveAt(int index) => ThrowHelper.ThrowCollectionIsReadonly();

        void ICollection.CopyTo(Array array, int index) => this.tracker.CopyTo(array, index);

        #endregion IReadOnlyList<T>
    }
}