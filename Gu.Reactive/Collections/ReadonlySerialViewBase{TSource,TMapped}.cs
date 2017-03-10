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
    /// A base class for swapping out an <see cref="IEnumerable{T}"/> source and get notifications.
    /// </summary>
    [Serializable]
    public abstract class ReadonlySerialViewBase<TSourceItem, TITem> : IRefreshAble, IList, IReadOnlyList<TITem>, IDisposable, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private static readonly IReadOnlyList<TSourceItem> EmptySource = new TSourceItem[0];

        private readonly Func<IEnumerable<TSourceItem>, IEnumerable<TITem>> mapper;
        private readonly CollectionSynchronizer<TITem> tracker;

        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadonlySerialViewBase{TSourceItem, TITem}"/> class.
        /// </summary>
        protected ReadonlySerialViewBase(IEnumerable<TSourceItem> source, Func<IEnumerable<TSourceItem>, IEnumerable<TITem>> mapper)
        {
            Ensure.NotNull(mapper, nameof(mapper));
            this.mapper = mapper;
            this.Source = source ?? EmptySource;
            this.tracker = new CollectionSynchronizer<TITem>(mapper(source));
        }

        /// <inheritdoc/>
        [field: NonSerialized]
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <inheritdoc/>
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc/>
        public bool IsReadOnly => true;

        /// <inheritdoc/>
        public bool IsFixedSize => true;

        /// <inheritdoc/>
        public int Count => this.ThrowIfDisposed(() => this.tracker.Count);

        /// <inheritdoc/>
        object ICollection.SyncRoot => this.tracker;

        /// <inheritdoc/>
        bool ICollection.IsSynchronized => false;

        /// <summary>
        /// Returns true if there are any subscribers to the <see cref="PropertyChanged"/> or <see cref="CollectionChanged"/> events.
        /// </summary>
        protected bool HasListeners => this.PropertyChanged != null || this.CollectionChanged != null;

        /// <summary>
        /// The collection synchronizer.
        /// </summary>
        protected CollectionSynchronizer<TITem> Tracker => this.tracker;

        /// <summary>
        /// The source collection.
        /// </summary>
        protected IEnumerable<TSourceItem> Source { get; private set; }

        /// <inheritdoc/>
        public TITem this[int index] => this.ThrowIfDisposed(() => this.tracker[index]);

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
        public IEnumerator<TITem> GetEnumerator() => this.ThrowIfDisposed(() => this.tracker.GetEnumerator());

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
        void ICollection.CopyTo(Array array, int index) => ((ICollection)this.tracker).CopyTo(array, index);

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
            lock (this.Source.SyncRootOrDefault(this.tracker))
            {
                (this.Source as IRefreshAble)?.Refresh();
                if (this.HasListeners)
                {
                    this.tracker.Reset(this.mapper(this.Source ?? EmptySource), this.OnPropertyChanged, this.OnCollectionChanged);
                }
                else
                {
                    this.tracker.Reset(this.mapper(this.Source ?? EmptySource));
                }
            }
        }

        /// <summary>
        /// Update the source collection and notify about changes.
        /// </summary>
        protected virtual void SetSource(IEnumerable<TSourceItem> source)
        {
            this.SetSourceCore(source);
        }

        /// <summary>
        /// Update the source collection and notify about changes.
        /// </summary>
        protected void SetSourceCore(IEnumerable<TSourceItem> source)
        {
            this.ThrowIfDisposed();
            this.Source = source ?? EmptySource;
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
                this.Source = EmptySource;
            }
        }

        /// <summary>
        /// Set <see cref="Source"/> to empty and notify about changes.
        /// </summary>
        protected virtual void ClearSource()
        {
            this.ThrowIfDisposed();
            this.Source = EmptySource;
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
            lock (this.Source.SyncRootOrDefault(this.tracker))
            {
                if (this.HasListeners)
                {
                    this.tracker.Refresh(this.mapper(this.Source ?? EmptySource), changes, this.OnPropertyChanged,
                                         this.OnCollectionChanged);
                }
                else
                {
                    this.tracker.Refresh(this.mapper(this.Source ?? EmptySource), changes);
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
        /// Properties/methods modifying this <see cref="ReadonlySerialViewBase{T}"/> will raise
        /// a property changed event through this virtual method.
        /// </summary>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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