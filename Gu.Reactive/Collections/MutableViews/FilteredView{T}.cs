namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Reactive.Concurrency;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;

    /// <summary>
    /// Typed filtered CollectionView for intellisense in xaml
    /// </summary>
    [Obsolete("Candidate for removal, broken. Prefer the read only version.")]
    public class FilteredView<T> : SynchronizedEditableView<T>, IFilteredView<T>, IReadOnlyFilteredView<T>
    {
        private readonly ObservableCollection<IObservable<object>> triggers;
        private readonly SerialDisposable refreshSubscription = new SerialDisposable();
        private readonly IDisposable triggerSubscription;

        private Func<T, bool> filter;
        private bool disposed;
        private TimeSpan bufferTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilteredView{T}"/> class.
        /// </summary>
        /// <param name="source">The source collection.</param>
        /// <param name="filter">The predicate used when filtering.</param>
        /// <param name="bufferTime">The time to defer updates, useful if many triggers fire in short time. Then it will be only one Reset</param>
        /// <param name="scheduler">The scheduler used when throttling. The collection changed events are raised on this scheduler</param>
        /// <param name="triggers">Triggers when to re evaluate the filter</param>
        public FilteredView(
            IList<T> source,
            Func<T, bool> filter,
            TimeSpan bufferTime,
            IScheduler scheduler,
            params IObservable<object>[] triggers)
            : base(source, Filtered(source, filter))
        {
            if (filter == null)
            {
                filter = x => true;
            }

            this.bufferTime = bufferTime;
            this.filter = filter;
            if (triggers == null || triggers.Length == 0)
            {
                this.triggers = new ObservableCollection<IObservable<object>>();
            }
            else
            {
                this.triggers = new ObservableCollection<IObservable<object>>(triggers);
            }

            this.refreshSubscription.Disposable = FilteredRefresher.Create(this, source, bufferTime, triggers, scheduler, false)
                                                                   .ObserveOn(scheduler ?? Scheduler.Immediate)
                                                                   .Subscribe(this.Refresh);
            var observables = new IObservable<object>[]
                                            {
                                                this.Triggers.ObserveCollectionChangedSlim(false),
                                                this.ObservePropertyChangedSlim(nameof(this.Filter), false),
                                                this.ObservePropertyChangedSlim(nameof(this.BufferTime), false)
                                            };
            this.triggerSubscription = observables.Merge()
                                                  .ThrottleOrDefault(bufferTime, scheduler)
                                                  .Subscribe(_ => this.refreshSubscription.Disposable = FilteredRefresher.Create(this, source, bufferTime, triggers, scheduler, true)
                                                                                                                         .ObserveOn(scheduler ?? Scheduler.Immediate)
                                                                                                                         .Subscribe(this.Refresh));
        }

        /// <summary>
        /// The triggers for updating the filter.
        /// </summary>
        public ObservableCollection<IObservable<object>> Triggers
        {
            get
            {
                this.ThrowIfDisposed();
                return this.triggers;
            }
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
                if (Equals(value, this.filter))
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
        protected override void RefreshNow(NotifyCollectionChangedEventArgs e)
        {
            var currentFilter = this.Filter;
            if (currentFilter != null)
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        {
                            if (e.TryGetSingleNewItem(out T item) &&
                                !currentFilter(item))
                            {
                                // added item that is not visible
                                return;
                            }

                            break;
                        }

                    case NotifyCollectionChangedAction.Remove:
                        {
                            if (e.TryGetSingleOldItem(out T item) &&
                                !currentFilter(item))
                            {
                                // removed item that is not visible
                                return;
                            }

                            break;
                        }

                    case NotifyCollectionChangedAction.Replace:
                        {
                            if (e.TryGetSingleNewItem(out T newItem) &&
                                !currentFilter(newItem) &&
                                e.TryGetSingleOldItem(out T oldItem) &&
                                !currentFilter(oldItem))
                            {
                                return; // replaced item that is not visible
                            }

                            break;
                        }

                    case NotifyCollectionChangedAction.Move:
                        {
                            if (e.TryGetSingleNewItem(out T item) &&
                                !currentFilter(item))
                            {
                                // added item that is not visible
                                return;
                            }

                            break;
                        }

                    case NotifyCollectionChangedAction.Reset:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (this.HasListeners)
            {
                this.Tracker.Refresh(this.Filtered(), CachedEventArgs.SingleNotifyCollectionReset, this.OnPropertyChanged, this.OnCollectionChanged);
            }
            else
            {
                this.Tracker.Refresh(this.Filtered(), CachedEventArgs.SingleNotifyCollectionReset);
            }
        }

        /// <inheritdoc/>
        protected override void Refresh(IReadOnlyList<NotifyCollectionChangedEventArgs> changes)
        {
            if (this.HasListeners)
            {
                this.Tracker.Refresh(this.Filtered(), null, this.OnPropertyChanged, this.OnCollectionChanged);
            }
            else
            {
                this.Tracker.Refresh(this.Filtered());
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
                this.triggerSubscription?.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
