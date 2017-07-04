namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;

    using Gu.Reactive.Internals;

    /// <inheritdoc/>
    public partial class ReadOnlyFilteredView<T> : ReadonlyViewBase<T, T>, IReadOnlyFilteredView<T>
    {
        private readonly IDisposable refreshSubscription;
        private readonly Chunk<NotifyCollectionChangedEventArgs> chunk;

        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyFilteredView{T}"/> class.
        /// </summary>
        /// <param name="source">The source collection.</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="bufferTime">The time to buffer source changes before updating the view.</param>
        /// <param name="scheduler">The scheduler to perform the filtering and notification on.</param>
        /// <param name="leaveOpen">True means that the <paramref name="source"/> is not disposed when this instance is disposed.</param>
        /// <param name="triggers">Additional triggers for when to filter.</param>
        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        public ReadOnlyFilteredView(IEnumerable<T> source, Func<T, bool> filter, TimeSpan bufferTime, IScheduler scheduler, bool leaveOpen, IEnumerable<IObservable<object>> triggers)
            : base(source, x => x.Where(filter), leaveOpen, startEmpty: true)
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(filter, nameof(filter));
            this.Filter = filter;
            this.BufferTime = bufferTime;
            this.chunk = new Chunk<NotifyCollectionChangedEventArgs>(bufferTime, scheduler ?? DefaultScheduler.Instance);
            this.refreshSubscription = Observable.Merge(
                                                     source.ObserveCollectionChangedSlimOrDefault(signalInitial: false),
                                                     triggers.MergeOrNever()
                                                             .Select(x => CachedEventArgs.NotifyCollectionReset))
                                                 .Slide(this.chunk)
                                                 .ObserveOn(scheduler ?? ImmediateScheduler.Instance)
                                                 .StartWith(this.chunk.Add(CachedEventArgs.NotifyCollectionReset))
                                                 .Subscribe(this.Update);
        }

        /// <inheritdoc/>
        public TimeSpan BufferTime { get; }

        /// <inheritdoc/>
        public Func<T, bool> Filter { get; }

        /// <inheritdoc/>
        public override void Refresh()
        {
            using (this.chunk.ClearTransaction())
            {
                base.Refresh();
            }
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        protected sealed override void Refresh(IReadOnlyList<NotifyCollectionChangedEventArgs> changes)
        {
            base.Refresh(CachedEventArgs.SingleNotifyCollectionReset);
        }

        private void Update(Chunk<NotifyCollectionChangedEventArgs> changes)
        {
            if (changes == null || changes.Count == 0)
            {
                return;
            }

            using (changes.ClearTransaction())
            {
                foreach (var change in changes)
                {
                    if (Filtered.AffectsFilteredOnly(change, this.Filter))
                    {
                        continue;
                    }

                    this.Refresh(CachedEventArgs.SingleNotifyCollectionReset);
                    return;
                }
            }
        }
    }
}