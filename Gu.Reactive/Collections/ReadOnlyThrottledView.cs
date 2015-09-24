namespace Gu.Reactive
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Reactive.Concurrency;

    using Gu.Reactive.Internals;

    [DebuggerTypeProxy(typeof(CollectionDebugView<>))]
    [DebuggerDisplay("Count = {Count}")]
    public class ReadOnlyThrottledView<T> : IReadOnlyThrottledView<T>, IUpdater, IList, IRefreshAble
    {
        private readonly IReadOnlyList<T> _collection;
        private readonly IScheduler _scheduler;
        private readonly IDisposable _refreshSubscription;
        private readonly CollectionSynchronizer<T> _tracker;
        private bool _disposed;

        public ReadOnlyThrottledView(ObservableCollection<T> collection, TimeSpan bufferTime, IScheduler scheduler)
            : this(bufferTime, scheduler, collection.AsReadOnly())
        {
        }

        public ReadOnlyThrottledView(ReadOnlyObservableCollection<T> collection, TimeSpan bufferTime, IScheduler scheduler)
            : this(bufferTime, scheduler, collection.AsReadOnly())
        {
        }

        public ReadOnlyThrottledView(IObservableCollection<T> collection, TimeSpan bufferTime, IScheduler scheduler)
            : this(bufferTime, scheduler, collection.AsReadOnly())
        {
        }

        public ReadOnlyThrottledView(IReadOnlyObservableCollection<T> collection, TimeSpan bufferTime, IScheduler scheduler)
            : this(bufferTime, scheduler, collection)
        {
        }

        private ReadOnlyThrottledView(TimeSpan bufferTime, IScheduler scheduler, IReadOnlyList<T> collection)
        {
            Ensure.NotNull(collection as INotifyCollectionChanged, "collection");
            _collection = collection;
            _tracker = new CollectionSynchronizer<T>(collection);
            _scheduler = scheduler;
            BufferTime = bufferTime;
            _refreshSubscription = ThrottledRefresher.Create(this, collection, bufferTime, scheduler, false)
                                                     .Subscribe(Refresh);
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        public TimeSpan BufferTime { get; }

        object IUpdater.IsUpdatingSourceItem => null;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Refresh()
        {
            _tracker.Reset(this, _collection, _scheduler, PropertyChanged, CollectionChanged);
        }

        protected void Refresh(IReadOnlyList<NotifyCollectionChangedEventArgs> changes)
        {
            _tracker.Refresh(this, _collection, changes, _scheduler, PropertyChanged, CollectionChanged);
        }

        /// <summary>
        /// Protected implementation of Dispose pattern. 
        /// </summary>
        /// <param name="disposing">true: safe to free managed resources</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;
            if (disposing)
            {
                _refreshSubscription.Dispose();
            }
        }

        protected void VerifyDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }

        protected void VerifyDisposed(Action action)
        {
            VerifyDisposed();
            action();
        }

        protected TResult VerifyDisposed<TResult>(TResult result)
        {
            VerifyDisposed();
            return result;
        }

        #region IReadOnlyList<T> & IList

        public int Count => VerifyDisposed(_tracker.Current.Count);

        bool IList.IsReadOnly => true;

        bool IList.IsFixedSize => true;

        object ICollection.SyncRoot => VerifyDisposed(_tracker.SyncRoot);

        bool ICollection.IsSynchronized => VerifyDisposed(_tracker.IsSynchronized);

        object IList.this[int index]
        {
            get { return this[index]; }
            set { ThrowHelper.ThrowCollectionIsReadonly(); }
        }

        public T this[int index] => VerifyDisposed(_tracker.Current[index]);

        public IEnumerator<T> GetEnumerator() => VerifyDisposed(_tracker.Current.GetEnumerator());

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        int IList.Add(object value) => ThrowHelper.ThrowCollectionIsReadonly<int>();

        bool IList.Contains(object value) => VerifyDisposed(_tracker.Contains(value));

        void IList.Clear() => ThrowHelper.ThrowCollectionIsReadonly();

        int IList.IndexOf(object value) => VerifyDisposed(_tracker.IndexOf(value));

        void IList.Insert(int index, object value) => ThrowHelper.ThrowCollectionIsReadonly();

        void IList.Remove(object value) => ThrowHelper.ThrowCollectionIsReadonly();

        void IList.RemoveAt(int index) => ThrowHelper.ThrowCollectionIsReadonly();

        void ICollection.CopyTo(Array array, int index) => VerifyDisposed(() => _tracker.CopyTo(array, index));

        #endregion IReadOnlyList<T>
    }
}
