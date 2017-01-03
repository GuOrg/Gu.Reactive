namespace Gu.Reactive
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Reactive.Disposables;

    public class EditableListView<T> : IObservableCollection<T>, IList, IDisposable
    {
        private readonly IObservableCollection<T> source;
        private readonly IDisposable subscriptions;

        private bool disposed;

        public EditableListView(IObservableCollection<T> source)
        {
            this.source = source;
            this.subscriptions = this.Subscribe(source);
        }

        /// <inheritdoc/>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc/>
        public int Count => this.ThwrowIfDisposed(this.source.Count);

        /// <inheritdoc/>
        public bool IsReadOnly => this.ThwrowIfDisposed(false);

        /// <inheritdoc/>
        bool IList.IsFixedSize => this.ThwrowIfDisposed(false);

        /// <inheritdoc/>
        public T this[int index]
        {
            get { return this.source[index]; }
            set { this.source[index] = value; }
        }

        object IList.this[int index]
        {
            get { return this[index]; }
            set { throw new NotImplementedException("we must notify immediately here"); }
        }

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator() => this.ThwrowIfDisposed(this.source.GetEnumerator());

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <inheritdoc/>
        public void Add(T item) => this.ThwrowIfDisposed(() => this.source.Add(item));

        /// <inheritdoc/>
        int IList.Add(object value)
        {
            var count = this.source.Count;
            this.source.Add((T)value);
            (this.source as IRefreshAble)?.Refresh();
            return count;
        }

        /// <inheritdoc/>
        public bool Remove(T item) => this.source.Remove(item);

        /// <inheritdoc/>
        void IList.Remove(object value)
        {
            this.Remove((T)value);
            (this.source as IRefreshAble)?.Refresh();
        }

        /// <inheritdoc/>
        public void RemoveAt(int index) => this.source.RemoveAt(index);

        /// <inheritdoc/>
        public void Clear()
        {
            this.source.Clear();
        }

        /// <inheritdoc/>
        public void Insert(int index, T item) => this.source.Insert(index, item);

        /// <inheritdoc/>
        void IList.Insert(int index, object value)
        {
            this.Insert(index, (T)value);
            (this.source as IRefreshAble)?.Refresh();
        }

        /// <inheritdoc/>
        object ICollection.SyncRoot => this.ThwrowIfDisposed(((ICollection)this.source).SyncRoot);

        /// <inheritdoc/>
        bool ICollection.IsSynchronized => this.ThwrowIfDisposed(((ICollection)this.source).IsSynchronized);

        /// <inheritdoc/>
        public void CopyTo(T[] array, int arrayIndex) => this.source.CopyTo(array, arrayIndex);

        /// <inheritdoc/>
        void ICollection.CopyTo(Array array, int index) => ListExt.CopyTo(this, array, index);

        /// <inheritdoc/>
        public bool Contains(T item) => this.source.Contains(item);

        /// <inheritdoc/>
        bool IList.Contains(object value) => this.Contains((T)value);

        /// <inheritdoc/>
        public int IndexOf(T item) => this.source.IndexOf(item);

        /// <inheritdoc/>
        int IList.IndexOf(object value) => this.IndexOf((T)value);

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Disposes of a <see cref="EditableListView{T}"/>.
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
                this.subscriptions.Dispose();
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
        /// a collection changed event through this virtual method.
        /// </summary>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e) => this.PropertyChanged?.Invoke(this, e);

        /// <summary>
        /// Throws an <see cref="ObjectDisposedException"/> if the instance is disposed.
        /// </summary>
        protected void ThwrowIfDisposed()
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
        protected void ThwrowIfDisposed(Action action)
        {
            this.ThwrowIfDisposed();
            action();
        }

        /// <summary>
        /// Throws an <see cref="ObjectDisposedException"/> if the instance is disposed.
        /// Returns <paramref name="result"/> if not disposed.
        /// </summary>
        /// <param name="result">The action to invoke.</param>
        protected TResult ThwrowIfDisposed<TResult>(TResult result)
        {
            this.ThwrowIfDisposed();
            return result;
        }

        private IDisposable Subscribe<TCol>(TCol col)
            where TCol : INotifyCollectionChanged, INotifyPropertyChanged
        {
            return new CompositeDisposable(2)
            {
                col.ObservePropertyChangedSlim()
                   .Subscribe(this.OnPropertyChanged),
                col.ObserveCollectionChangedSlim(false)
                    .Subscribe(this.OnCollectionChanged)
            };
        }
    }
}