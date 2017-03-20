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

    /// <summary>
    /// Typed filtered CollectionView for intellisense in xaml
    /// </summary>
    public class FilteredView<T> : SynchronizedEditableView<T>, IFilteredView<T>, IReadOnlyFilteredView<T>
    {
        private readonly IDisposable refreshSubscription;

        private Func<T, bool> filter;
        private bool disposed;
        private TimeSpan bufferTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilteredView{T}"/> class.
        /// </summary>
        /// <param name="source">The source collection.</param>
        /// <param name="filter">The predicate used when filtering.</param>
        /// <param name="bufferTime">The time to defer updates, useful if many triggers fire in short time. Then it will be only one Reset</param>
        /// <param name="triggers">Triggers when to re evaluate the filter</param>
        public FilteredView(ObservableCollection<T> source, Func<T, bool> filter, TimeSpan bufferTime, params IObservable<object>[] triggers)
            : this(source, filter, bufferTime, WpfSchedulers.Dispatcher, triggers)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilteredView{T}"/> class.
        /// </summary>
        /// <param name="source">The source collection.</param>
        /// <param name="filter">The predicate used when filtering.</param>
        /// <param name="bufferTime">The time to defer updates, useful if many triggers fire in short time. Then it will be only one Reset</param>
        /// <param name="triggers">Triggers when to re evaluate the filter</param>
        public FilteredView(IObservableCollection<T> source, Func<T, bool> filter, TimeSpan bufferTime, params IObservable<object>[] triggers)
            : this(source, filter, bufferTime, WpfSchedulers.Dispatcher, triggers)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilteredView{T}"/> class.
        /// </summary>
        /// <param name="source">The source collection.</param>
        /// <param name="filter">The predicate used when filtering.</param>
        /// <param name="bufferTime">The time to defer updates, useful if many triggers fire in short time. Then it will be only one Reset</param>
        /// <param name="scheduler">The scheduler used when throttling. The collection changed events are raised on this scheduler</param>
        /// <param name="triggers">Triggers when to re evaluate the filter</param>
        internal FilteredView(IList<T> source, Func<T, bool> filter, TimeSpan bufferTime, IScheduler scheduler, params IObservable<object>[] triggers)
            : base(source, Filtered(source, filter), true)
        {
            this.bufferTime = bufferTime;
            this.filter = filter;

            this.refreshSubscription = Observable.Merge(
                                                     ((INotifyCollectionChanged)source).ObserveCollectionChangedSlim(false)
                                                                                       .Where(this.IsSourceChange),
                                                     this.ObservePropertyChangedSlim(x => x.Filter, false)
                                                         .Select(_ => CachedEventArgs.NotifyCollectionReset),
                                                     triggers.MergeOrNever()
                                                             .Select(_ => CachedEventArgs.NotifyCollectionReset))
                                                 .Publish(
                                                     shared =>
                                                         this.ObserveValue(x => x.BufferTime, true)
                                                             .Select(bt => shared.Chunks(bt.Value, scheduler))
                                                             .Switch())
                                                 .ObserveOn(scheduler)
                                                 .StartWith(CachedEventArgs.SingleNotifyCollectionReset)
                                                 .Subscribe(this.Refresh);
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
                return this.bufferTime;
            }

            set
            {
                this.ThrowIfDisposed();
                if (value.Equals(this.bufferTime))
                {
                    return;
                }

                this.bufferTime = value;
                this.OnPropertyChanged();
            }
        }

        /// <inheritdoc/>
        public override void Refresh()
        {
            this.ThrowIfDisposed();
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

        /// <inheritdoc/>
        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Get the filtered items from Source
        /// If source is null and empty enuerable is returned.
        /// If filter is null the raw source is returned.
        /// </summary>
        protected static IEnumerable<T> Filtered(IEnumerable<T> source, Func<T, bool> filter)
        {
            if (source == null)
            {
                return Enumerable.Empty<T>();
            }

            if (filter == null)
            {
                return source;
            }

            return source.Where(filter);
        }

        /// <inheritdoc/>
        protected override void Refresh(IReadOnlyList<NotifyCollectionChangedEventArgs> changes)
        {
            foreach (var e in changes)
            {
                if (!Gu.Reactive.Filtered.AffectsFilteredOnly(e, this.filter))
                {
                    if (this.HasListeners)
                    {
                        this.Tracker.Reset(this.Filtered(), this.OnPropertyChanged, this.OnCollectionChanged);
                    }
                    else
                    {
                        this.Tracker.Reset(this.Filtered());
                    }

                    return;
                }
            }
        }

        /// <summary>
        /// Get the filtered items from Source
        /// </summary>
        protected IEnumerable<T> Filtered()
        {
            return Filtered(this.Source, this.filter);
        }

        /// <summary>
        /// Protected implementation of Dispose pattern.
        /// </summary>
        /// <param name="disposing">true: safe to free managed resources</param>
        protected virtual void Dispose(bool disposing)
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
        }

        /// <summary>
        /// Throws an <see cref="ObjectDisposedException"/> if the instance is disposed.
        /// </summary>
        protected void ThrowIfDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }
    }
}
