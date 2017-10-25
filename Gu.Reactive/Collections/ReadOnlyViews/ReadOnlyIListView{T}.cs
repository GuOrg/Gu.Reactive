namespace Gu.Reactive
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Reactive.Disposables;

    using Gu.Reactive.Internals;

    /// <summary>
    /// For exposing a source collection as a readonly list.
    /// For example DataGrid needs <see cref="IList"/> for elements to be editable.
    /// </summary>
    public class ReadOnlyIListView<T> : IReadOnlyObservableCollection<T>, IList, IDisposable
    {
#pragma warning disable IDISP008  // Don't assign member with injected and created disposables.
        private readonly IReadOnlyList<T> source;
#pragma warning restore IDISP008  // Don't assign member with injected and created disposables.
        private readonly bool leaveOpen;
        private readonly IDisposable subscriptions;

        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyIListView{T}"/> class.
        /// </summary>
        /// <param name="source">The source collection.</param>
        /// <param name="leaveOpen">True means that the <paramref name="source"/> is not disposed when this instance is disposed.</param>
        public ReadOnlyIListView(IObservableCollection<T> source, bool leaveOpen)
        {
            this.source = new DisposingReadOnlyCollection(source);
            this.leaveOpen = leaveOpen;
            this.subscriptions = this.Subscribe(source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyIListView{T}"/> class.
        /// </summary>
        /// <param name="source">The source collection.</param>
        /// <param name="leaveOpen">True means that the <paramref name="source"/> is not disposed when this instance is disposed.</param>
        public ReadOnlyIListView(IReadOnlyObservableCollection<T> source, bool leaveOpen)
        {
            this.source = source;
            this.leaveOpen = leaveOpen;
            this.subscriptions = this.Subscribe(source);
        }

        /// <inheritdoc/>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc/>
        public int Count => this.ThrowIfDisposed(this.source.Count);

        /// <inheritdoc/>
        bool IList.IsReadOnly => this.ThrowIfDisposed(true);

        /// <inheritdoc/>
        bool IList.IsFixedSize => this.ThrowIfDisposed(true);

        /// <inheritdoc/>
        object ICollection.SyncRoot => this.ThrowIfDisposed(((ICollection)this.source).SyncRoot);

        /// <inheritdoc/>
        bool ICollection.IsSynchronized => this.ThrowIfDisposed(((ICollection)this.source).IsSynchronized);

        /// <inheritdoc/>
        public T this[int index] => this.ThrowIfDisposed(this.source[index]);

        /// <inheritdoc/>
        object IList.this[int index]
        {
            get => this[index];
            //// ReSharper disable once ValueParameterNotUsed
            set => ThrowHelper.ThrowCollectionIsReadonly();
        }

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator()
        {
            this.ThrowIfDisposed();
            return this.source.GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <inheritdoc/>
        void ICollection.CopyTo(Array array, int index) => this.source.CopyTo(array, index);

        /// <inheritdoc/>
        int IList.Add(object value) => ThrowHelper.ThrowCollectionIsReadonly<int>();

        /// <inheritdoc/>
        bool IList.Contains(object value) => this.source.Contains(value);

        /// <inheritdoc/>
        void IList.Clear() => ThrowHelper.ThrowCollectionIsReadonly();

        /// <inheritdoc/>
        int IList.IndexOf(object value) => this.source.IndexOf(value);

        /// <inheritdoc/>
        void IList.Insert(int index, object value) => ThrowHelper.ThrowCollectionIsReadonly();

        /// <inheritdoc/>
        void IList.Remove(object value) => ThrowHelper.ThrowCollectionIsReadonly();

        /// <inheritdoc/>
        void IList.RemoveAt(int index) => ThrowHelper.ThrowCollectionIsReadonly();

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Disposes of a <see cref="ReadOnlyIListView{T}"/>.
        /// </summary>
        /// <remarks>
        /// Called from Dispose() with disposing=true.
        /// Guidelines:
        /// 1. We may be called more than once: do nothing after the first call.
        /// 2. Avoid throwing exceptions if disposing is false, i.e. if we're being finalized.
        /// </remarks>
        /// <param name="disposing">True if called from Dispose(), false if called from the finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            if (disposing)
            {
                if (!this.leaveOpen)
                {
#pragma warning disable IDISP007 // Don't dispose injected.
                    (this.source as IDisposable)?.Dispose();
#pragma warning restore IDISP007 // Don't dispose injected.
                }

                this.subscriptions.Dispose();
            }
        }

        /// <summary>
        /// Raise CollectionChanged event to any listeners.
        /// Properties/methods modifying this <see cref="ReadOnlyIListView{T}"/> will raise
        /// a collection changed event through this virtual method.
        /// </summary>
        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e) => this.CollectionChanged?.Invoke(this, e);

        /// <summary>
        /// Raise PropertyChanged event to any listeners.
        /// Properties/methods modifying this <see cref="ReadOnlyIListView{T}"/> will raise
        /// a collection changed event through this virtual method.
        /// </summary>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e) => this.PropertyChanged?.Invoke(this, e);

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
        protected void ThrowIfDisposed(Action action)
        {
            this.ThrowIfDisposed();
            action();
        }

        /// <summary>
        /// Throws an <see cref="ObjectDisposedException"/> if the instance is disposed.
        /// Returns <paramref name="result"/> if not disposed.
        /// </summary>
        protected TResult ThrowIfDisposed<TResult>(TResult result)
        {
            this.ThrowIfDisposed();
            return result;
        }

        private IDisposable Subscribe<TCol>(TCol col)
            where TCol : INotifyCollectionChanged, INotifyPropertyChanged
        {
            return new CompositeDisposable(2)
                       {
                           col.ObservePropertyChangedSlim()
                              .Subscribe(this.OnPropertyChanged),
                           col.ObserveCollectionChangedSlim(signalInitial: false)
                              .Subscribe(this.OnCollectionChanged)
                       };
        }

        private sealed class DisposingReadOnlyCollection : ReadOnlyCollection<T>, IDisposable
        {
            private readonly IList<T> list;

            private bool disposed;

            public DisposingReadOnlyCollection(IList<T> list)
                : base(list)
            {
                this.list = list;
            }

            public void Dispose()
            {
                if (this.disposed)
                {
                    return;
                }

                this.disposed = true;
#pragma warning disable IDISP007 // Don't dispose injected.
                (this.list as IDisposable)?.Dispose();
#pragma warning restore IDISP007 // Don't dispose injected.
            }
        }
    }
}