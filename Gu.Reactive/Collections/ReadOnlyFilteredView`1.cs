namespace Gu.Reactive
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Reactive.Concurrency;

    using Gu.Reactive.Internals;

    public class ReadOnlyFilteredView<T> : IReadOnlyFilteredView<T>, IUpdater
    {
        private readonly IEnumerable<T> _source;
        private readonly IScheduler _scheduler;
        private readonly IDisposable _refreshSubscription;
        private readonly CollectionSynchronizer<T> _tracker;
        private bool _disposed;

        public ReadOnlyFilteredView(ObservableCollection<T> collection, Func<T, bool> filter, TimeSpan bufferTime, IScheduler scheduler, params IObservable<object>[] triggers)
            : this((IReadOnlyList<T>)collection, filter, bufferTime, scheduler, triggers)
        {
        }

        public ReadOnlyFilteredView(ReadOnlyObservableCollection<T> collection, Func<T, bool> filter, TimeSpan bufferTime, IScheduler scheduler, params IObservable<object>[] triggers)
            : this((IReadOnlyList<T>)collection, filter, bufferTime, scheduler, triggers)
        {
        }

        public ReadOnlyFilteredView(IObservableCollection<T> collection, Func<T, bool> filter, TimeSpan bufferTime, IScheduler scheduler, params IObservable<object>[] triggers)
            : this((IReadOnlyList<T>)collection, filter, bufferTime, scheduler, triggers)
        {
        }

        public ReadOnlyFilteredView(IReadOnlyObservableCollection<T> collection, Func<T, bool> filter, TimeSpan bufferTime, IScheduler scheduler, params IObservable<object>[] triggers)
            : this((IReadOnlyList<T>)collection, filter, bufferTime, scheduler, triggers)
        {
        }

        public ReadOnlyFilteredView(IEnumerable<T> source, Func<T, bool> filter, TimeSpan bufferTime, IScheduler scheduler, IEnumerable<IObservable<object>> triggers)
            : this(source, filter, scheduler, bufferTime, triggers.ToArray())
        {
            Ensure.NotNull(triggers, "triggers");
        }

        public ReadOnlyFilteredView(IEnumerable<T> source, Func<T, bool> filter, TimeSpan bufferTime, IScheduler scheduler, IObservable<object> trigger)
            : this(source, filter, scheduler, bufferTime, new[] { trigger })
        {
            Ensure.NotNull(trigger, "trigger");
        }

        private ReadOnlyFilteredView(IEnumerable<T> source, Func<T, bool> filter, IScheduler scheduler, TimeSpan bufferTime, IReadOnlyList<IObservable<object>> triggers)
        {
            Ensure.NotNull(source, "collection");
            Ensure.NotNull(filter, "filter");
            _source = source;
            _tracker = new CollectionSynchronizer<T>(source);
            _scheduler = scheduler;
            Filter = filter;
            BufferTime = bufferTime;
            _refreshSubscription = FilteredRefresher.Create(this, source, bufferTime, triggers, scheduler, false)
                                                    .Subscribe(Refresh);
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        public TimeSpan BufferTime { get; private set; }

        public Func<T, bool> Filter { get; private set; }

        object IUpdater.IsUpdatingSourceItem
        {
            get { return null; }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Refresh()
        {
            VerifyDisposed();
            _tracker.Reset(this, Filtered().ToArray(), _scheduler, PropertyChanged, CollectionChanged);
        }

        protected void Refresh(IReadOnlyList<NotifyCollectionChangedEventArgs> changes)
        {
            _tracker.Refresh(this, Filtered().ToArray(), changes, _scheduler, PropertyChanged, CollectionChanged);
        }

        protected IEnumerable<T> Filtered()
        {
            return _source.Where(Filter);
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