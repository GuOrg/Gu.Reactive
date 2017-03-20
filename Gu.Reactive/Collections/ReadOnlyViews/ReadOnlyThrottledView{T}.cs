namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Diagnostics.CodeAnalysis;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;

    /// <summary>
    /// A readonly view of a collection that buffers changes before notifying.
    /// </summary>
    public class ReadOnlyThrottledView<T> : ReadonlySerialViewBase<T, T>, IReadOnlyThrottledView<T>
    {
        private readonly IDisposable refreshSubscription;
        private readonly Chunk<NotifyCollectionChangedEventArgs> chunk;

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
            : base(source, s => s, true)
        {
            this.BufferTime = bufferTime;
            this.chunk = new Chunk<NotifyCollectionChangedEventArgs>(bufferTime, scheduler ?? DefaultScheduler.Instance);
            this.refreshSubscription = ((INotifyCollectionChanged)source).ObserveCollectionChangedSlim(false)
                                                                         .AsSlidingChunk(chunk)
                                                                         .ObserveOn(scheduler ?? ImmediateScheduler.Instance)
                                                                         .StartWith(Chunk.One(CachedEventArgs.NotifyCollectionReset))
                                                                         .Subscribe(this.Refresh);
        }

        /// <summary>
        /// The time to buffer changes before notifying.
        /// </summary>
        public TimeSpan BufferTime { get; }

        /// <inheritdoc/>
        public override void Refresh()
        {
            using (this.chunk.ClearTransaction())
            {
                base.Refresh();
            }
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
                this.chunk.ClearItems();
            }

            base.Dispose(disposing);
        }

        private void Refresh(Chunk<NotifyCollectionChangedEventArgs> changes)
        {
            using (changes.ClearTransaction())
            {
                base.Refresh(changes);
            }
        }
    }
}
