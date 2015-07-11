namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Reactive.Concurrency;
    using System.Reactive.Disposables;

    public class DeferredView<T> : SynchronizedEditableView<T>, IDeferredView<T>, IReadOnlyDeferredView<T>
    {
        private readonly IScheduler _scheduler;
        private readonly SerialDisposable _refreshSubscription = new SerialDisposable();
        private TimeSpan _bufferTime;
        private bool _disposed;

        public DeferredView(ObservableCollection<T> source, TimeSpan bufferTime, IScheduler scheduler)
            : base(source)
        {
            _scheduler = scheduler;
            _bufferTime = bufferTime;
            _refreshSubscription.Disposable = DeferredRefresher.Create(this, source, _bufferTime, _scheduler, false)
                                                               .Subscribe(Refresh);
        }

        public DeferredView(IObservableCollection<T> source, TimeSpan bufferTime, IScheduler scheduler)
            : base(source)
        {
            _scheduler = scheduler;
            _bufferTime = bufferTime;
            _refreshSubscription.Disposable = DeferredRefresher.Create(this, source, bufferTime, scheduler, false)
                                                               .Subscribe(Refresh);
        }

        public TimeSpan BufferTime
        {
            get
            {
                return _bufferTime;
            }
            set
            {
                if (value == _bufferTime)
                {
                    return;
                }
                _bufferTime = value;
                _refreshSubscription.Disposable = DeferredRefresher.Create(this, Source, _bufferTime, _scheduler, true)
                                                                    .Subscribe(Refresh);
                OnPropertyChanged();
            }
        }

        public override void Refresh()
        {
            VerifyDisposed();
            var updated = Source as IReadOnlyList<T> ?? Source.ToArray();
            Synchronized.Reset(this, updated, _scheduler, PropertyChangedEventHandler, NotifyCollectionChangedEventHandler);
        }

        protected override void RefreshNow(NotifyCollectionChangedEventArgs e)
        {
            var updated = Source as IReadOnlyList<T> ?? Source.ToArray();
            Synchronized.Refresh(this, updated, new[] { e }, null, PropertyChangedEventHandler, NotifyCollectionChangedEventHandler);
        }

        protected override void Refresh(IReadOnlyList<NotifyCollectionChangedEventArgs> changes)
        {
            VerifyDisposed();
            var updated = Source as IReadOnlyList<T> ?? Source.ToArray();
            Synchronized.Refresh(this, updated, changes, _scheduler, PropertyChangedEventHandler, NotifyCollectionChangedEventHandler);
        }

        /// <summary>
        /// Protected implementation of Dispose pattern. 
        /// </summary>
        /// <param name="disposing">true: safe to free managed resources</param>
        protected override void Dispose(bool disposing)
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
    }
}
