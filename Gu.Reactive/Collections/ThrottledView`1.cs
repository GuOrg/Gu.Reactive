namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.Linq;
    using System.Reactive.Concurrency;
    using System.Reactive.Disposables;

    [DebuggerTypeProxy(typeof(CollectionDebugView<>))]
    [DebuggerDisplay("Count = {Count}")] 
    public class ThrottledView<T> : SynchronizedEditableView<T>, IThrottledView<T>, IReadOnlyThrottledView<T>
    {
        private readonly IScheduler _scheduler;
        private readonly SerialDisposable _refreshSubscription = new SerialDisposable();
        private TimeSpan _bufferTime;
        private bool _disposed;

        public ThrottledView(ObservableCollection<T> source, TimeSpan bufferTime, IScheduler scheduler)
            : base(source)
        {
            _scheduler = scheduler;
            _bufferTime = bufferTime;
            _refreshSubscription.Disposable = ThrottledRefresher.Create(this, source, _bufferTime, _scheduler, false)
                                                               .Subscribe(Refresh);
        }

        public ThrottledView(IObservableCollection<T> source, TimeSpan bufferTime, IScheduler scheduler)
            : base(source)
        {
            _scheduler = scheduler;
            _bufferTime = bufferTime;
            _refreshSubscription.Disposable = ThrottledRefresher.Create(this, source, bufferTime, scheduler, false)
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
                _refreshSubscription.Disposable = ThrottledRefresher.Create(this, Source, _bufferTime, _scheduler, true)
                                                                    .Subscribe(Refresh);
                OnPropertyChanged();
            }
        }

        public override void Refresh()
        {
            VerifyDisposed();
            (Source as IRefreshAble)?.Refresh();
            var updated = Source.AsReadOnly();
            Tracker.Reset(this, updated, _scheduler, PropertyChangedEventHandler, NotifyCollectionChangedEventHandler);
        }

        protected override void RefreshNow(NotifyCollectionChangedEventArgs e)
        {
            var updated = Source.AsReadOnly();
            Tracker.Refresh(this, updated, new[] { e }, null, PropertyChangedEventHandler, NotifyCollectionChangedEventHandler);
        }

        protected override void Refresh(IReadOnlyList<NotifyCollectionChangedEventArgs> changes)
        {
            VerifyDisposed();
            var updated = Source as IReadOnlyList<T> ?? Source.ToArray();
            Tracker.Refresh(this, updated, changes, _scheduler, PropertyChangedEventHandler, NotifyCollectionChangedEventHandler);
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

            base.Dispose(disposing);
        }
    }
}
