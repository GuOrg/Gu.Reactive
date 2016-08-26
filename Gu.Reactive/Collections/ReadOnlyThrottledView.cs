namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Reactive.Concurrency;

    [DebuggerTypeProxy(typeof(CollectionDebugView<>))]
    [DebuggerDisplay("Count = {Count}")]
    public class ReadOnlyThrottledView<T> : ReadonlySerialViewBase<T>, IReadOnlyThrottledView<T>, IUpdater
    {
        private readonly IDisposable refreshSubscription;
        private bool disposed;

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
            : base(collection, true, true)
        {
            this.BufferTime = bufferTime;
            this.refreshSubscription = ThrottledRefresher.Create(this, collection, bufferTime, scheduler, false)
                                                     .Subscribe(this.Refresh);
        }

        public TimeSpan BufferTime { get; }

        object IUpdater.IsUpdatingSourceItem => null;

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
