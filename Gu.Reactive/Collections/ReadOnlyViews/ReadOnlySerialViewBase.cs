namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;

#pragma warning disable CA1010 // Collections should implement generic interface
    /// <summary>
    /// A view where the source can be updated that notifies about changes.
    /// </summary>
    public abstract class ReadOnlySerialViewBase<T> : ReadonlyViewBase<T, T>, IReadOnlyObservableCollection<T>
#pragma warning restore CA1010 // Collections should implement generic interface
    {
        private readonly IDisposable refreshSubscription;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlySerialViewBase{T}"/> class.
        /// </summary>
        /// <param name="source">The source collection.</param>
        /// <param name="bufferTime">The time to buffer changes from the source collection.</param>
        /// <param name="scheduler">The scheduler to observe changes on.</param>
        /// <param name="leaveOpen">True means that <paramref name="source"/> is not disposed when this instance is disposed.</param>
        protected ReadOnlySerialViewBase(IEnumerable<T> source, TimeSpan bufferTime, IScheduler scheduler, bool leaveOpen)
            : base(source, s => s, leaveOpen, startEmpty: true)
        {
            this.Chunk = new Chunk<NotifyCollectionChangedEventArgs>(bufferTime, scheduler ?? DefaultScheduler.Instance);
            this.refreshSubscription = this.ObserveValue(x => x.Source)
                                           .Select(x => NotifyCollectionChangedExt.ObserveCollectionChangedSlimOrDefault(x.GetValueOrDefault(), signalInitial: true)
                                                         .Slide(this.Chunk))
                                           .Switch()
                                           .ObserveOn(scheduler ?? ImmediateScheduler.Instance)
                                           .Subscribe(this.Update);
        }

        /// <summary>
        /// The current chunk of changes in the source.
        /// </summary>
        protected Chunk<NotifyCollectionChangedEventArgs> Chunk { get; }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.refreshSubscription.Dispose();
                this.Chunk.ClearItems();
            }

            base.Dispose(disposing);
        }

        private void Update(Chunk<NotifyCollectionChangedEventArgs> changes)
        {
            using (changes.ClearTransaction())
            {
                if (changes.Count > 0)
                {
                    this.Refresh(changes);
                }
            }
        }
    }
}
