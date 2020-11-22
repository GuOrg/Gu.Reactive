namespace Gu.Wpf.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;

    using Gu.Reactive;
    using Gu.Reactive.Internals;

    /// <summary>
    /// Typed filtered CollectionView for intellisense in xaml.
    /// </summary>
    /// <typeparam name="T">The type of the items in the collection.</typeparam>
    public class FilteredView<T> : SynchronizedEditableView<T>, IFilteredView<T>, IReadOnlyFilteredView<T>
    {
        private readonly IDisposable refreshSubscription;
        private readonly Chunk<NotifyCollectionChangedEventArgs> chunk;

        private Func<T, bool> filter;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilteredView{T}"/> class.
        /// </summary>
        /// <param name="source">The source collection.</param>
        /// <param name="filter">The predicate used when filtering.</param>
        /// <param name="bufferTime">The time to defer updates, useful if many triggers fire in short time. Then it will be only one Reset.</param>
        /// <param name="leaveOpen">True means that the <paramref name="source"/> is not disposed when this instance is disposed.</param>
        /// <param name="triggers">Triggers when to re evaluate the filter.</param>
        public FilteredView(ObservableCollection<T> source, Func<T, bool> filter, TimeSpan bufferTime, bool leaveOpen, params IObservable<object?>[]? triggers)
            : this(source, filter, bufferTime, WpfSchedulers.Dispatcher, leaveOpen, triggers)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilteredView{T}"/> class.
        /// </summary>
        /// <param name="source">The source collection.</param>
        /// <param name="filter">The predicate used when filtering.</param>
        /// <param name="bufferTime">The time to defer updates, useful if many triggers fire in short time. Then it will be only one Reset.</param>
        /// <param name="leaveOpen">True means that the <paramref name="source"/> is not disposed when this instance is disposed.</param>
        /// <param name="triggers">Triggers when to re evaluate the filter.</param>
        public FilteredView(IObservableCollection<T> source, Func<T, bool> filter, TimeSpan bufferTime, bool leaveOpen, params IObservable<object?>[]? triggers)
            : this(source, filter, bufferTime, WpfSchedulers.Dispatcher, leaveOpen, triggers)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilteredView{T}"/> class.
        /// </summary>
        /// <param name="source">The source collection.</param>
        /// <param name="filter">The predicate used when filtering.</param>
        /// <param name="bufferTime">The time to defer updates, useful if many triggers fire in short time. Then it will be only one Reset.</param>
        /// <param name="scheduler">The scheduler used when throttling. The collection changed events are raised on this scheduler.</param>
        /// <param name="leaveOpen">True means that the <paramref name="source"/> is not disposed when this instance is disposed.</param>
        /// <param name="triggers">Triggers when to re evaluate the filter.</param>
        internal FilteredView(IList<T> source, Func<T, bool> filter, TimeSpan bufferTime, IScheduler scheduler, bool leaveOpen, params IObservable<object?>[]? triggers)
            : base(source, Filtered(source, filter), leaveOpen, startEmpty: true)
        {
            this.chunk = new Chunk<NotifyCollectionChangedEventArgs>(bufferTime, scheduler ?? DefaultScheduler.Instance);
            this.filter = filter;
            this.refreshSubscription = Observable.Merge(
                                                     ((INotifyCollectionChanged)source).ObserveCollectionChangedSlim(signalInitial: false)
                                                                                       .Where(this.IsSourceChange),
                                                     this.ObservePropertyChangedSlim(x => x.Filter, signalInitial: false)
                                                         .Select(_ => CachedEventArgs.NotifyCollectionReset),
                                                     triggers.MergeOrNever()
                                                             .Select(_ => CachedEventArgs.NotifyCollectionReset))
                                                 .Slide(this.chunk)
                                                 .ObserveOn(scheduler)
                                                 .StartWith(this.chunk.Add(CachedEventArgs.NotifyCollectionReset))
                                                 .Subscribe(this.Update);
        }

        /// <summary>
        /// The predicate to filter by.
        /// </summary>
        public Func<T, bool> Filter
        {
            get
            {
                this.ThrowIfDisposed();
                return this.filter;
            }

            set
            {
                this.ThrowIfDisposed();
                if (ReferenceEquals(value, this.filter))
                {
                    return;
                }

                this.filter = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// The time to buffer changes before notifying.
        /// </summary>
        public TimeSpan BufferTime
        {
            get
            {
                this.ThrowIfDisposed();
                return this.chunk.BufferTime;
            }

            set
            {
                this.ThrowIfDisposed();
                if (value.Equals(this.chunk.BufferTime))
                {
                    return;
                }

                this.chunk.BufferTime = value;
                this.OnPropertyChanged();
            }
        }

        /// <inheritdoc/>
        public override void Refresh()
        {
            this.ThrowIfDisposed();
            using (this.chunk.ClearTransaction())
            {
                lock (this.SyncRoot())
                {
                    (this.Source as IRefreshAble)?.Refresh();
                    if (this.HasListeners)
                    {
                        this.Tracker.Reset(this.Filtered(), this.OnPropertyChanged, this.OnCollectionChanged);
                    }
                    else
                    {
                        this.Tracker.Reset(this.Filtered());
                    }
                }
            }
        }

        /// <summary>
        /// Get the filtered items from Source
        /// If source is null and empty enuerable is returned.
        /// If filter is null the raw source is returned.
        /// </summary>
        protected static IEnumerable<T> Filtered(IEnumerable<T> source, Func<T, bool> filter)
        {
            if (source is null)
            {
                return Enumerable.Empty<T>();
            }

            if (filter is null)
            {
                return source;
            }

            return source.Where(filter);
        }

        /// <summary>
        /// Get the filtered items from Source.
        /// </summary>
        protected IEnumerable<T> Filtered()
        {
            return Filtered(this.Source, this.filter);
        }

        /// <summary>
        /// Protected implementation of Dispose pattern.
        /// </summary>
        /// <param name="disposing">true: safe to free managed resources.</param>
        protected override void Dispose(bool disposing)
        {
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
            if (this.HasListeners)
            {
                this.Tracker.Reset(this.Filtered(), this.OnPropertyChanged, this.OnCollectionChanged);
            }
            else
            {
                this.Tracker.Reset(this.Filtered());
            }
        }

        private void Update(Chunk<NotifyCollectionChangedEventArgs> changes)
        {
            using (changes.ClearTransaction())
            {
                foreach (var e in changes)
                {
                    if (Gu.Reactive.Filtered.AffectsFilteredOnly(e, this.filter))
                    {
                        continue;
                    }

                    this.Refresh(changes);
                    return;
                }
            }
        }
    }
}
