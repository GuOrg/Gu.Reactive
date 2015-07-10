namespace Gu.Reactive
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Reactive.Concurrency;
    using Gu.Reactive.Internals;

    public class ReadOnlyDeferredView<T> : IReadOnlyDeferredView<T>
    {
        private readonly IReadOnlyList<T> _collection;
        private readonly IScheduler _scheduler;
        private readonly IDisposable _refreshSubscription;
        private readonly CollectionSynchronizer<T> _tracker;
        private bool _disposed;

        public ReadOnlyDeferredView(ObservableCollection<T> collection, TimeSpan bufferTime, IScheduler scheduler)
            : this((IReadOnlyList<T>)collection, bufferTime, scheduler)
        {
        }

        public ReadOnlyDeferredView(ReadOnlyObservableCollection<T> collection, TimeSpan bufferTime, IScheduler scheduler)
            : this((IReadOnlyList<T>)collection, bufferTime, scheduler)
        {
        }

        public ReadOnlyDeferredView(IObservableCollection<T> collection, TimeSpan bufferTime, IScheduler scheduler)
            : this((IReadOnlyList<T>)collection, bufferTime, scheduler)
        {
        }

        public ReadOnlyDeferredView(IReadOnlyObservableCollection<T> collection, TimeSpan bufferTime, IScheduler scheduler)
            : this((IReadOnlyList<T>)collection, bufferTime, scheduler)
        {
        }

        private ReadOnlyDeferredView(IReadOnlyList<T> collection, TimeSpan bufferTime, IScheduler scheduler)
        {
            Ensure.NotNull(collection as INotifyCollectionChanged, "collection");
            _collection = collection;
            _tracker = new CollectionSynchronizer<T>(collection);
            _scheduler = scheduler;
            BufferTime = bufferTime;
            _refreshSubscription = DeferredRefresher.Create(collection,
                                                            bufferTime,
                                                            scheduler,
                                                            false)
                                                    .Subscribe(Refresh);
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        public TimeSpan BufferTime { get; private set; }

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

        #region IReadOnlyList<T>

        public int Count { get { return _tracker.Current.Count; } }

        public T this[int index]
        {
            get
            {
                VerifyDisposed();
                return _tracker.Current[index];
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            VerifyDisposed();
            return _tracker.Current.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            VerifyDisposed();
            return GetEnumerator();
        }

        #endregion IReadOnlyList<T>
    }
}
