namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;

    /// <inheritdoc cref="IReadOnlyFilteredView{T}" />
#pragma warning disable CA1010 // Collections should implement generic interface
    public partial class ReadOnlyFilteredView<T> : ReadonlyViewBase<T, T>, IReadOnlyFilteredView<T>
#pragma warning restore CA1010 // Collections should implement generic interface
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
        public ReadOnlyFilteredView(IEnumerable<T> source, Func<T, bool> filter, TimeSpan bufferTime, IScheduler? scheduler, bool leaveOpen, IEnumerable<IObservable<object?>>? triggers)
            : base(source, x => x.Where(filter), leaveOpen, startEmpty: true)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            this.Filter = filter ?? throw new ArgumentNullException(nameof(filter));
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

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyFilteredView{T}"/> class.
        /// </summary>
        /// <param name="source">The source collection.</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="observableFactory">Factory for creating observables for the items.</param>
        /// <param name="bufferTime">The time to buffer source changes before updating the view.</param>
        /// <param name="scheduler">The scheduler to perform the filtering and notification on.</param>
        /// <param name="leaveOpen">True means that the <paramref name="source"/> is not disposed when this instance is disposed.</param>
        public ReadOnlyFilteredView(IEnumerable<T> source, Func<T, bool> filter, Func<T, IObservable<object?>> observableFactory, TimeSpan bufferTime, IScheduler? scheduler, bool leaveOpen)
            : base(source, x => x.Where(filter), leaveOpen, startEmpty: true)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (observableFactory is null)
            {
                throw new ArgumentNullException(nameof(observableFactory));
            }

            this.Filter = filter ?? throw new ArgumentNullException(nameof(filter));
            this.BufferTime = bufferTime;
            this.chunk = new Chunk<NotifyCollectionChangedEventArgs>(bufferTime, scheduler ?? DefaultScheduler.Instance);
            this.refreshSubscription = Observable.Merge(
                                                     source.ObserveCollectionChangedSlimOrDefault(signalInitial: false),
                                                     ItemsTracker())
                                                 .Slide(this.chunk)
                                                 .ObserveOn(scheduler ?? ImmediateScheduler.Instance)
                                                 .StartWith(this.chunk.Add(CachedEventArgs.NotifyCollectionReset))
                                                 .Subscribe(this.Update);

            IObservable<NotifyCollectionChangedEventArgs> ItemsTracker()
            {
                if (source is INotifyCollectionChanged)
                {
                    return Observable.Create<NotifyCollectionChangedEventArgs>(x =>
                    {
                        return new MappingView<T, IDisposable>(
                            source,
                            Mapper.Create<T, IDisposable>(
                                item => observableFactory(item)
                                        .Select(_ => CachedEventArgs.NotifyCollectionReset)
                                        .Subscribe(x),
                                disposable => disposable.Dispose()),
                            TimeSpan.Zero,
                            null,
                            leaveOpen: true);
                    });
                }

                return source.Select(observableFactory)
                             .Merge()
                             .Select(_ => CachedEventArgs.NotifyCollectionReset);
            }
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
            if (changes is null || changes.Count == 0)
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
