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
        private readonly IScheduler scheduler;
        private readonly SerialDisposable refreshSubscription = new SerialDisposable();
        private TimeSpan bufferTime;
        private bool disposed;

        public ThrottledView(ObservableCollection<T> source, TimeSpan bufferTime, IScheduler scheduler)
            : base(source)
        {
            this.scheduler = scheduler;
            this.bufferTime = bufferTime;
            this.refreshSubscription.Disposable = ThrottledRefresher.Create(this, source, bufferTime, scheduler, false)
                                                               .Subscribe(this.Refresh);
        }

        public ThrottledView(IObservableCollection<T> source, TimeSpan bufferTime, IScheduler scheduler)
            : base(source)
        {
            this.scheduler = scheduler;
            this.bufferTime = bufferTime;
            this.refreshSubscription.Disposable = ThrottledRefresher.Create(this, source, bufferTime, scheduler, false)
                                                                .Subscribe(this.Refresh);
        }

        public TimeSpan BufferTime
        {
            get
            {
                return this.bufferTime;
            }

            set
            {
                if (value == this.bufferTime)
                {
                    return;
                }

                this.bufferTime = value;
                this.refreshSubscription.Disposable = ThrottledRefresher.Create(this, this.Source, this.bufferTime, this.scheduler, true)
                                                                    .Subscribe(this.Refresh);
                this.OnPropertyChanged();
            }
        }

        public override void Refresh()
        {
            this.VerifyDisposed();
            (this.Source as IRefreshAble)?.Refresh();
            var updated = this.Source.AsReadOnly();
            this.Tracker.Reset(this, updated, this.scheduler, this.PropertyChangedEventHandler, this.NotifyCollectionChangedEventHandler);
        }

        protected override void RefreshNow(NotifyCollectionChangedEventArgs e)
        {
            var updated = this.Source.AsReadOnly();
            this.Tracker.Refresh(this, updated, new[] { e }, null, this.PropertyChangedEventHandler, this.NotifyCollectionChangedEventHandler);
        }

        protected override void Refresh(IReadOnlyList<NotifyCollectionChangedEventArgs> changes)
        {
            this.VerifyDisposed();
            var updated = this.Source as IReadOnlyList<T> ?? this.Source.ToArray();
            this.Tracker.Refresh(this, updated, changes, this.scheduler, this.PropertyChangedEventHandler, this.NotifyCollectionChangedEventHandler);
        }

        /// <summary>
        /// Protected implementation of Dispose pattern.
        /// </summary>
        /// <param name="disposing">true: safe to free managed resources</param>
        protected override void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            if (disposing)
            {
                this.refreshSubscription.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
