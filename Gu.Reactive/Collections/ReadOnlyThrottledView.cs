namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Reactive.Concurrency;

    /// <summary>
    /// A readonly view of a collection that buffers changes before notifying.
    /// </summary>
    [DebuggerTypeProxy(typeof(CollectionDebugView<>))]
    [DebuggerDisplay("Count = {Count}")]
    public class ReadOnlyThrottledView<T> : ReadonlySerialViewBase<T>, IReadOnlyThrottledView<T>, IUpdater
    {
        private readonly IDisposable refreshSubscription;
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyThrottledView{T}"/> class.
        /// </summary>
        public ReadOnlyThrottledView(ObservableCollection<T> collection, TimeSpan bufferTime, IScheduler scheduler)
            : this(bufferTime, scheduler, collection.AsReadOnly())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyThrottledView{T}"/> class.
        /// </summary>
        public ReadOnlyThrottledView(ReadOnlyObservableCollection<T> collection, TimeSpan bufferTime, IScheduler scheduler)
            : this(bufferTime, scheduler, collection.AsReadOnly())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyThrottledView{T}"/> class.
        /// </summary>
        public ReadOnlyThrottledView(IObservableCollection<T> collection, TimeSpan bufferTime, IScheduler scheduler)
            : this(bufferTime, scheduler, collection.AsReadOnly())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyThrottledView{T}"/> class.
        /// </summary>
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

        /// <summary>
        /// The time to buffer changes before notifying.
        /// </summary>
        public TimeSpan BufferTime { get; }

        /// <inheritdoc/>
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
