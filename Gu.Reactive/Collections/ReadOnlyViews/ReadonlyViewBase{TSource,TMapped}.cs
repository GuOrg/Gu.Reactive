namespace Gu.Reactive
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using Gu.Reactive.Internals;

    /// <summary>
    /// A base class for swapping out an <see cref="IEnumerable{T}"/> source and get notifications.
    /// </summary>
    /// <typeparam name="TSource">The source type.</typeparam>
    /// <typeparam name="TMapped">The mapped type.</typeparam>
    [Serializable]
#pragma warning disable CA1010 // Collections should implement generic interface
    public abstract class ReadonlyViewBase<TSource, TMapped> : IRefreshAble, IList, IReadOnlyView<TMapped>
#pragma warning restore CA1010 // Collections should implement generic interface
    {
        private readonly Func<IEnumerable<TSource>, IEnumerable<TMapped>> mapper;
        private readonly bool leaveOpen;
        private IEnumerable<TSource> source;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadonlyViewBase{TSource,TMapped}"/> class.
        /// </summary>
        /// <param name="source">The source collection.</param>
        /// <param name="mapper">The mapping function.</param>
        /// <param name="leaveOpen">True means that the <paramref name="source"/> is not disposed when this instance is disposed.</param>
        /// <param name="startEmpty">True means that the tracker is empty after the constructor.</param>
        protected ReadonlyViewBase(IEnumerable<TSource>? source, Func<IEnumerable<TSource>, IEnumerable<TMapped>> mapper, bool leaveOpen, bool startEmpty = false)
        {
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.leaveOpen = leaveOpen;
            this.source = source ?? Array.Empty<TSource>();
            this.Tracker = new CollectionSynchronizer<TMapped>(startEmpty ? mapper(Array.Empty<TSource>()) : mapper(this.source));
        }

        /// <inheritdoc/>
        [field: NonSerialized]
        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        /// <inheritdoc/>
        [field: NonSerialized]
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <inheritdoc/>
        public bool IsReadOnly => true;

        /// <inheritdoc/>
        public bool IsFixedSize => true;

        /// <inheritdoc cref="IReadOnlyView{TMapped}" />
        public int Count => this.ThrowIfDisposed(() => this.Tracker.Count);

#pragma warning disable CA1033 // Interface methods should be callable by child types
                              /// <inheritdoc/>
        object ICollection.SyncRoot => ((ICollection)this.Tracker).SyncRoot;

        /// <inheritdoc/>
        bool ICollection.IsSynchronized => false;
#pragma warning restore CA1033 // Interface methods should be callable by child types

        /// <summary>
        /// Gets the collection synchronizer.
        /// </summary>
        protected CollectionSynchronizer<TMapped> Tracker { get; }

        /// <summary>
        /// Gets a value indicating whether there are any subscribers to the <see cref="PropertyChanged"/> or <see cref="CollectionChanged"/> events.
        /// </summary>
        protected bool HasListeners => this.PropertyChanged != null || this.CollectionChanged != null;

        /// <summary>
        /// Gets a value indicating whether this instance is disposed.
        /// </summary>
        protected bool IsDisposed { get; private set; }

        /// <summary>
        /// Gets the source collection.
        /// </summary>
        protected IEnumerable<TSource> Source
        {
            get => this.source;
            private set
            {
                if (ReferenceEquals(value, this.source))
                {
                    return;
                }

                lock (this.source.SyncRootOrDefault(this.SyncRoot()))
                {
                    if (ReferenceEquals(value, this.source))
                    {
                        return;
                    }

                    this.source = value;
                    this.OnPropertyChanged();
                }
            }
        }

        /// <inheritdoc/>
        public TMapped this[int index] => this.ThrowIfDisposed(() => this.Tracker[index]);

        /// <inheritdoc/>
        object? IList.this[int index]
        {
            get => this[index];
#pragma warning disable CA1033 // Interface methods should be callable by child types
            set => ThrowHelper.ThrowCollectionIsReadonly();
#pragma warning restore CA1033 // Interface methods should be callable by child types
        }

        /// <inheritdoc/>
        public IEnumerator<TMapped> GetEnumerator() => this.ThrowIfDisposed(() => this.Tracker.GetEnumerator());

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <summary>
        /// Lock the collection and get a copy.
        /// </summary>
        /// <returns>A copy of this instance.</returns>
        public IReadOnlyList<TMapped> Snapshot()
        {
            return this.Tracker.Snapshot();
        }

#pragma warning disable CA1033 // Interface methods should be callable by child types
        /// <inheritdoc/>
        int IList.Add(object? value) => ThrowHelper.ThrowCollectionIsReadonly<int>();

        /// <inheritdoc/>
        bool IList.Contains(object? value) => this.Tracker.Contains(value);

        /// <inheritdoc/>
        void IList.Clear() => ThrowHelper.ThrowCollectionIsReadonly();

        /// <inheritdoc/>
        int IList.IndexOf(object? value) => this.Tracker.IndexOf(value);

        /// <inheritdoc/>
        void IList.Insert(int index, object? value) => ThrowHelper.ThrowCollectionIsReadonly();

        /// <inheritdoc/>
        void IList.Remove(object? value) => ThrowHelper.ThrowCollectionIsReadonly();

        /// <inheritdoc/>
        void IList.RemoveAt(int index) => ThrowHelper.ThrowCollectionIsReadonly();

        /// <inheritdoc/>
        void ICollection.CopyTo(Array array, int index) => ((ICollection)this.Tracker).CopyTo(array, index);
#pragma warning restore CA1033 // Interface methods should be callable by child types

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Refresh this collection from <see cref="Source"/> and notify changes.
        /// </summary>
        public virtual void Refresh()
        {
            this.ThrowIfDisposed();
            lock (this.SyncRoot())
            {
                (this.Source as IRefreshAble)?.Refresh();
                if (this.HasListeners)
                {
                    this.Tracker.Reset(this.mapper(this.source ?? Array.Empty<TSource>()), this.OnPropertyChanged, this.OnCollectionChanged);
                }
                else
                {
                    this.Tracker.Reset(this.mapper(this.Source ?? Array.Empty<TSource>()));
                }
            }
        }

        /// <summary>
        /// Update the source collection and notify about changes.
        /// </summary>
        /// <param name="newSource">The <see cref="IEnumerable{TSource}"/>.</param>
        protected virtual void SetSource(IEnumerable<TSource>? newSource)
        {
            this.SetSourceCore(newSource);
        }

        /// <summary>
        /// Update the source collection and notify about changes.
        /// </summary>
        /// <param name="newSource">The <see cref="IEnumerable{TSource}"/>.</param>
        protected void SetSourceCore(IEnumerable<TSource>? newSource)
        {
            this.ThrowIfDisposed();
            this.Source = newSource ?? Array.Empty<TSource>();
        }

        /// <summary>
        /// Protected implementation of Dispose pattern.
        /// </summary>
        /// <param name="disposing">true: safe to free managed resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.IsDisposed)
            {
                return;
            }

            this.IsDisposed = true;
            if (disposing)
            {
                lock (this.source.SyncRootOrDefault(this.SyncRoot()))
                {
                    if (!this.leaveOpen)
                    {
#pragma warning disable IDISP007 // Don't dispose injected.
                        (this.source as IDisposable)?.Dispose();
#pragma warning restore IDISP007 // Don't dispose injected.
                    }

                    this.source = Array.Empty<TSource>();
                }

                this.Tracker.Clear();
            }
        }

        /// <summary>
        /// Set <see cref="Source"/> to empty and notify about changes.
        /// </summary>
        protected virtual void ClearSource()
        {
            this.ThrowIfDisposed();
            this.Source = Array.Empty<TSource>();
        }

        /// <summary>
        /// Throws an <see cref="ObjectDisposedException"/> if the instance is disposed.
        /// </summary>
        protected void ThrowIfDisposed()
        {
            if (this.IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }

        /// <summary>
        /// Throws an <see cref="ObjectDisposedException"/> if the instance is disposed.
        /// Returns <paramref name="result"/> if not disposed.
        /// </summary>
        /// <typeparam name="TResult">The type of the value.</typeparam>
        /// <param name="result">The action to invoke.</param>
        /// <returns>The value produced by <paramref name="result"/>.</returns>
        protected TResult ThrowIfDisposed<TResult>(Func<TResult> result)
        {
            if (result is null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            this.ThrowIfDisposed();
            return result();
        }

        /// <summary>
        /// Synchronize with source and notify about changes.
        /// </summary>
        /// <param name="changes">The <see cref="IReadOnlyList{NotifyCollectionChangedEventArgs}"/>.</param>
        protected virtual void Refresh(IReadOnlyList<NotifyCollectionChangedEventArgs> changes)
        {
            lock (this.source.SyncRootOrDefault(this.SyncRoot()))
            {
                if (this.HasListeners)
                {
                    this.Tracker.Refresh(this.mapper(this.source ?? Array.Empty<TSource>()), changes, this.OnPropertyChanged, this.OnCollectionChanged);
                }
                else
                {
                    this.Tracker.Refresh(this.mapper(this.source ?? Array.Empty<TSource>()), changes);
                }
            }
        }

        /// <summary>
        /// Raise CollectionChanged event to any listeners.
        /// Properties/methods modifying this <see cref="ReadonlyViewBase{TSource,TMapped}"/> will raise
        /// a collection changed event through this virtual method.
        /// </summary>
        /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/>.</param>
        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            this.CollectionChanged?.Invoke(this, e);
        }

        /// <summary>
        /// Raise PropertyChanged event to any listeners.
        /// Properties/methods modifying this <see cref="ReadonlyViewBase{TSource,TMapped}"/> will raise
        /// a property changed event through this virtual method.
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Raise PropertyChanged event to any listeners.
        /// Properties/methods modifying this <see cref="ReadonlyViewBase{TSource,TMapped}"/> will raise
        /// a property changed event through this virtual method.
        /// </summary>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/>.</param>
        [Obsolete("Use OnPropertyChanged([CallerMemberName] string? propertyName = null)")]
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            this.PropertyChanged?.Invoke(this, e);
        }

        /// <summary>
        /// Notify about the changes [PropertyChanged("Count"), PropertyChanged("Item[]", CollectionChanged].
        /// Count is not signaled for move and replace.
        ///  If the list has more than one change a reset notification is performed.
        /// </summary>
        /// <param name="changes">The <see cref="IReadOnlyList{NotifyCollectionChangedEventArgs}"/>.</param>
        protected virtual void Notify(IReadOnlyList<NotifyCollectionChangedEventArgs> changes)
        {
            if (changes is null ||
                changes.Count == 0)
            {
                return;
            }

            if (changes.Count == 1)
            {
                var change = changes[0];
                this.Notify(change);
            }
            else
            {
                this.Notify(CachedEventArgs.NotifyCollectionReset);
            }
        }

        /// <summary>
        /// Notify about the changes [PropertyChanged("Count"), PropertyChanged("Item[]", CollectionChanged].
        /// Count is not signaled for move and replace.
        /// </summary>
        /// <param name="change">The <see cref="NotifyCollectionChangedEventArgs"/>.</param>
        protected virtual void Notify(NotifyCollectionChangedEventArgs change)
        {
            if (change is null)
            {
                return;
            }

            switch (change.Action)
            {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Remove:
                    this.OnPropertyChanged(CachedEventArgs.CountPropertyChanged);
                    this.OnPropertyChanged(CachedEventArgs.IndexerPropertyChanged);
                    this.OnCollectionChanged(change);
                    break;
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Move:
                    this.OnPropertyChanged(CachedEventArgs.IndexerPropertyChanged);
                    this.OnCollectionChanged(change);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    this.OnPropertyChanged(CachedEventArgs.CountPropertyChanged);
                    this.OnPropertyChanged(CachedEventArgs.IndexerPropertyChanged);
                    this.OnCollectionChanged(change);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(change));
            }
        }
    }
}
