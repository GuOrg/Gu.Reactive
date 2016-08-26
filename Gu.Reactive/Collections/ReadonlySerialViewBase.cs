namespace Gu.Reactive
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics;

    [DebuggerTypeProxy(typeof(CollectionDebugView<>))]
    [DebuggerDisplay("Count = {Count}")]
    public abstract class ReadonlySerialViewBase<T> : IRefreshAble, IList, IDisposable
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

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        protected IEnumerable<T> Source { get; private set; }

        /// <summary>
        /// Make the class sealed when using this.
        /// Call VerifyDisposed at the start of all public methods
        /// </summary>
        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            this.Dispose(true);
            // Dispose some stuff now
        }

        public virtual void Refresh()
        {
            this.VerifyDisposed();
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

        protected void SetSource(IEnumerable<T> source)
        {
            this.VerifyDisposed();
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

        protected void ClearSource()
        {
            this.VerifyDisposed();
            this.Source = Empty;
            this.Refresh();
        }

        protected void VerifyDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }

        protected void VerifyDisposed(Action action)
        {
            this.VerifyDisposed();
            action();
        }

        protected TResult VerifyDisposed<TResult>(Func<TResult> action)
        {
            this.VerifyDisposed();
            return action();
        }

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

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            this.CollectionChanged?.Invoke(this, e);
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            this.PropertyChanged?.Invoke(this, e);
        }

        #region IReadOnlyList<T> & IList

        public int Count => this.VerifyDisposed(() => this.tracker.Count);

        public bool IsReadOnly { get; }

        public bool IsFixedSize { get; }

        object ICollection.SyncRoot => this.tracker.SyncRoot;

        bool ICollection.IsSynchronized => this.tracker.IsSynchronized;

        object IList.this[int index]
        {
            get { return this[index]; }
            set { ThrowHelper.ThrowCollectionIsReadonly(); }
        }

        public T this[int index] => this.VerifyDisposed(() => this.tracker.Current[index]);

        public IEnumerator<T> GetEnumerator() => this.VerifyDisposed(() => this.tracker.GetEnumerator());

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