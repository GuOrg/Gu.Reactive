namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
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
        public ReadOnlyThrottledView(ObservableCollection<T> source, TimeSpan bufferTime, IScheduler scheduler)
            : this(bufferTime, scheduler, source)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyThrottledView{T}"/> class.
        /// </summary>
        public ReadOnlyThrottledView(ReadOnlyObservableCollection<T> source, TimeSpan bufferTime, IScheduler scheduler)
            : this(bufferTime, scheduler, source)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyThrottledView{T}"/> class.
        /// </summary>
        public ReadOnlyThrottledView(IObservableCollection<T> source, TimeSpan bufferTime, IScheduler scheduler)
            : this(bufferTime, scheduler, source)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyThrottledView{T}"/> class.
        /// </summary>
        public ReadOnlyThrottledView(IReadOnlyObservableCollection<T> source, TimeSpan bufferTime, IScheduler scheduler)
            : this(bufferTime, scheduler, source)
        {
        }

        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        private ReadOnlyThrottledView(TimeSpan bufferTime, IScheduler scheduler, IEnumerable<T> source)
            : base(source, true, true)
        {
            this.BufferTime = bufferTime;
            this.refreshSubscription = ThrottledRefresher.Create(this, source, bufferTime, scheduler, false)
                                                         .Subscribe(this.Refresh);
        }

        /// <summary>
        /// The time to buffer changes before notifying.
        /// </summary>
        public TimeSpan BufferTime { get; }

        /// <inheritdoc/>
        object IUpdater.CurrentlyUpdatingSourceItem => null;

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
