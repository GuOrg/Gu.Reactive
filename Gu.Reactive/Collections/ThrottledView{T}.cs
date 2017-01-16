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

    /// <summary>
    /// A view of a collection that buffers changes before notifying.
    /// </summary>
    [DebuggerTypeProxy(typeof(CollectionDebugView<>))]
    [DebuggerDisplay("Count = {Count}")]
    public class ThrottledView<T> : SynchronizedEditableView<T>, IThrottledView<T>, IReadOnlyThrottledView<T>
    {
        private readonly IScheduler scheduler;
        private readonly SerialDisposable refreshSubscription = new SerialDisposable();
        private TimeSpan bufferTime;
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThrottledView{T}"/> class.
        /// </summary>
        public ThrottledView(ObservableCollection<T> source, TimeSpan bufferTime, IScheduler scheduler)
            : base(source)
        {
            this.scheduler = scheduler;
            this.bufferTime = bufferTime;
            this.refreshSubscription.Disposable = ThrottledRefresher.Create(this, source, bufferTime, scheduler, false)
                                                               .Subscribe(this.Refresh);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThrottledView{T}"/> class.
        /// </summary>
        public ThrottledView(IObservableCollection<T> source, TimeSpan bufferTime, IScheduler scheduler)
            : base(source)
        {
            this.scheduler = scheduler;
            this.bufferTime = bufferTime;
            this.refreshSubscription.Disposable = ThrottledRefresher.Create(this, source, bufferTime, scheduler, false)
                                                                .Subscribe(this.Refresh);
        }

        /// <summary>
        /// The time to buffer changes before notifying.
        /// </summary>
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

        /// <inheritdoc/>
        public override void Refresh()
        {
            this.ThrowIfDisposed();
            (this.Source as IRefreshAble)?.Refresh();
            var updated = this.Source.AsReadOnly();
            this.Tracker.Reset(this, updated, this.scheduler, this.PropertyChangedEventHandler, this.NotifyCollectionChangedEventHandler);
        }

        /// <inheritdoc/>
        protected override void RefreshNow(NotifyCollectionChangedEventArgs e)
        {
            var updated = this.Source.AsReadOnly();
            this.Tracker.Refresh(this, updated, new[] { e }, null, this.PropertyChangedEventHandler, this.NotifyCollectionChangedEventHandler);
        }

        /// <inheritdoc/>
        protected override void Refresh(IReadOnlyList<NotifyCollectionChangedEventArgs> changes)
        {
            this.ThrowIfDisposed();
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
