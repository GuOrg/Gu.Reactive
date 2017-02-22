namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reactive.Concurrency;

    using Gu.Reactive.Internals;
    using Gu.Reactive.Internals.Ensure;

    /// <inheritdoc/>
    [DebuggerTypeProxy(typeof(CollectionDebugView<>))]
    [DebuggerDisplay("Count = {Count}")]
    public class ReadOnlyFilteredView<T> : ReadonlySerialViewBase<T>, IReadOnlyFilteredView<T>, IUpdater
    {
        private readonly IEnumerable<T> source;
        private readonly IDisposable refreshSubscription;
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyFilteredView{T}"/> class.
        /// </summary>
        public ReadOnlyFilteredView(ObservableCollection<T> collection, Func<T, bool> filter, TimeSpan bufferTime, IScheduler scheduler, params IObservable<object>[] triggers)
            : this((IReadOnlyList<T>)collection, filter, bufferTime, scheduler, triggers)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyFilteredView{T}"/> class.
        /// </summary>
        public ReadOnlyFilteredView(ReadOnlyObservableCollection<T> collection, Func<T, bool> filter, TimeSpan bufferTime, IScheduler scheduler, params IObservable<object>[] triggers)
            : this((IReadOnlyList<T>)collection, filter, bufferTime, scheduler, triggers)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyFilteredView{T}"/> class.
        /// </summary>
        public ReadOnlyFilteredView(IObservableCollection<T> collection, Func<T, bool> filter, TimeSpan bufferTime, IScheduler scheduler, params IObservable<object>[] triggers)
            : this((IReadOnlyList<T>)collection, filter, bufferTime, scheduler, triggers)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyFilteredView{T}"/> class.
        /// </summary>
        public ReadOnlyFilteredView(IReadOnlyObservableCollection<T> collection, Func<T, bool> filter, TimeSpan bufferTime, IScheduler scheduler, params IObservable<object>[] triggers)
            : this((IReadOnlyList<T>)collection, filter, bufferTime, scheduler, triggers)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyFilteredView{T}"/> class.
        /// </summary>
        public ReadOnlyFilteredView(IEnumerable<T> source, Func<T, bool> filter, TimeSpan bufferTime, IScheduler scheduler, IEnumerable<IObservable<object>> triggers)
            : this(source, filter, scheduler, bufferTime, triggers?.ToArray())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyFilteredView{T}"/> class.
        /// </summary>
        public ReadOnlyFilteredView(IEnumerable<T> source, Func<T, bool> filter, TimeSpan bufferTime, IScheduler scheduler, IObservable<object> trigger)
            : this(source, filter, scheduler, bufferTime, new[] { trigger })
        {
            Ensure.NotNull(trigger, nameof(trigger));
        }

        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        private ReadOnlyFilteredView(IEnumerable<T> source, Func<T, bool> filter, IScheduler scheduler, TimeSpan bufferTime, IReadOnlyList<IObservable<object>> triggers)
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(filter, nameof(filter));
            this.source = source;
            this.Filter = filter;
            this.BufferTime = bufferTime;
            this.SetSourceCore(this.Filtered());
            this.refreshSubscription = FilteredRefresher.Create(this, source, bufferTime, triggers ?? Enumerable.Empty<IObservable<object>>(), scheduler, false)
                                                        .Subscribe(this.Refresh);
        }

        /// <inheritdoc/>
        public TimeSpan BufferTime { get; }

        /// <inheritdoc/>
        public Func<T, bool> Filter { get; }

        /// <inheritdoc/>
        object IUpdater.CurrentlyUpdatingSourceItem => null;

        /// <summary>
        /// Force a refresh.
        /// May be deferred if there is a buffer time.
        /// </summary>
        public new void Refresh()
        {
            (this.source as IRefreshAble)?.Refresh();
            this.SetSource(this.Filtered());
        }

        /// <inheritdoc/>
        protected override void Refresh(IReadOnlyList<NotifyCollectionChangedEventArgs> changes)
        {
            this.SetSource(this.Filtered());
        }

        /// <summary>
        /// Get the filtered items.
        /// </summary>
        protected IEnumerable<T> Filtered()
        {
            return this.source.Where(this.Filter);
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