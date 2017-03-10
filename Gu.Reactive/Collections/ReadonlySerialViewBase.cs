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
    [DebuggerDisplay("Count = {Count}")]
    public abstract class ReadonlySerialViewBase<T> : IRefreshAble, IList, IReadOnlyList<T>, IDisposable, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private static readonly IReadOnlyList<T> Empty = new T[0];
        private readonly CollectionSynchronizer<T> tracker;

        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadonlySerialViewBase{T}"/> class.
        /// </summary>
        protected ReadonlySerialViewBase()
            : this(null, true, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadonlySerialViewBase{T}"/> class.
        /// </summary>
        protected ReadonlySerialViewBase(IEnumerable<T> source)
            : this(source, true, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadonlySerialViewBase{T}"/> class.
        /// </summary>
        protected ReadonlySerialViewBase(bool isReadOnly, bool isFixedSize)
            : this(null, isReadOnly, isFixedSize)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadonlySerialViewBase{T}"/> class.
        /// </summary>
        protected ReadonlySerialViewBase(IEnumerable<T> source, bool isReadOnly, bool isFixedSize)
        {
            this.IsReadOnly = isReadOnly;
            this.IsFixedSize = isFixedSize;
            this.Source = source ?? Empty;
            this.tracker = new CollectionSynchronizer<T>(source ?? Empty);
        }

        /// <inheritdoc/>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc/>
        public bool IsReadOnly { get; }

        /// <inheritdoc/>
        public bool IsFixedSize { get; }

        /// <inheritdoc/>
        public int Count => this.ThrowIfDisposed(() => this.tracker.Count);

        /// <inheritdoc/>
        object ICollection.SyncRoot => this.tracker.SyncRoot;

        /// <inheritdoc/>
        bool ICollection.IsSynchronized => this.tracker.IsSynchronized;

        /// <summary>
        /// Returns true if there are any subscribers to the <see cref="PropertyChanged"/> or <see cref="CollectionChanged"/> events.
        /// </summary>
        protected bool HasListeners => this.PropertyChanged != null || this.CollectionChanged != null;

        /// <summary>
        /// The source collection.
        /// </summary>
        protected IEnumerable<T> Source { get; private set; }

        /// <inheritdoc/>
        public T this[int index] => this.ThrowIfDisposed(() => this.tracker.Current[index]);

        /// <inheritdoc/>
        object IList.this[int index]
        {
            get
            {
                return this[index];
            }

            // ReSharper disable once ValueParameterNotUsed
            set
            {
                ThrowHelper.ThrowCollectionIsReadonly();
            }
        }

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator() => this.ThrowIfDisposed(() => this.tracker.GetEnumerator());

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <inheritdoc/>
        int IList.Add(object value) => ThrowHelper.ThrowCollectionIsReadonly<int>();

        /// <inheritdoc/>
        bool IList.Contains(object value) => this.tracker.Contains(value);

        /// <inheritdoc/>
        void IList.Clear() => ThrowHelper.ThrowCollectionIsReadonly();

        /// <inheritdoc/>
        int IList.IndexOf(object value) => this.tracker.IndexOf(value);

        /// <inheritdoc/>
        void IList.Insert(int index, object value) => ThrowHelper.ThrowCollectionIsReadonly();

        /// <inheritdoc/>
        void IList.Remove(object value) => ThrowHelper.ThrowCollectionIsReadonly();

        /// <inheritdoc/>
        void IList.RemoveAt(int index) => ThrowHelper.ThrowCollectionIsReadonly();

        /// <inheritdoc/>
        void ICollection.CopyTo(Array array, int index) => this.tracker.CopyTo(array, index);

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
                    if (this.HasListeners)
                    {
                        this.tracker.Reset(this.Source, this.OnPropertyChanged, this.OnCollectionChanged);
                    }
                    else
                    {
                        this.tracker.Reset(this.Source);
                    }
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
        protected virtual void SetSource(IEnumerable<T> source)
        {
            this.SetSourceCore(source);
        }

        /// <summary>
        /// Update the source collection and notify about changes.
        /// </summary>
        protected void SetSourceCore(IEnumerable<T> source)
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
                this.Source = Empty;
            }
        }

        /// <summary>
        /// Set <see cref="Source"/> to empty and notify about changes.
        /// </summary>
        protected virtual void ClearSource()
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
                    if (this.HasListeners)
                    {
                        this.tracker.Refresh(this.Source, changes, this.OnPropertyChanged, this.OnCollectionChanged);
                    }
                    else
                    {
                        this.tracker.Refresh(this.Source, changes);
                    }
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
    }
}