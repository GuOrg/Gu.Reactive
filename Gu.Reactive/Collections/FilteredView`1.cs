namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.Linq;
    using System.Reactive.Concurrency;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;

    /// <summary>
    /// Typed CollectionView for intellisense in xaml
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DebuggerTypeProxy(typeof(CollectionDebugView<>))]
    [DebuggerDisplay("Count = {Count}")]
    public class FilteredView<T> : SynchronizedEditableView<T>, IFilteredView<T>, IReadOnlyFilteredView<T>
    {
        private readonly ObservableCollection<IObservable<object>> triggers;
        private readonly IScheduler scheduler;
        private readonly SerialDisposable refreshSubscription = new SerialDisposable();
        private Func<T, bool> filter;
        private bool disposed;
        private TimeSpan bufferTime;

        /// <summary>
        ///
        /// </summary>
        /// <param name="source">The collection to wrap</param>
        /// <param name="filter"></param>
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

            this.scheduler = scheduler;
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
                                                this.Triggers.ObserveCollectionChanged(false),
                                                this.ObservePropertyChangedSlim(nameof(this.Filter), false),
                                                this.ObservePropertyChangedSlim(nameof(this.BufferTime), false)
                                            };
            observables.Merge()
                       .ThrottleOrDefault(bufferTime, scheduler)
                       .Subscribe(_ => this.refreshSubscription.Disposable = FilteredRefresher.Create(this, source, bufferTime, triggers, scheduler, true)
                                                                                          .ObserveOn(scheduler ?? Scheduler.Immediate)
                                                                                          .Subscribe(this.Refresh));
        }

        public Func<T, bool> Filter
        {
            get
            {
                this.VerifyDisposed();
                return this.filter;
            }

            set
            {
                this.VerifyDisposed();
                if (Equals(value, this.filter))
                {
                    return;
                }

                this.filter = value;
                this.OnPropertyChanged();
            }
        }

        public ObservableCollection<IObservable<object>> Triggers
        {
            get
            {
                this.VerifyDisposed();
                return this.triggers;
            }
        }

        public TimeSpan BufferTime
        {
            get
            {
                this.VerifyDisposed();
                return this.bufferTime;
            }

            set
            {
                this.VerifyDisposed();
                if (value.Equals(this.bufferTime))
                {
                    return;
                }

                this.bufferTime = value;
                this.OnPropertyChanged();
            }
        }

        public override void Refresh()
        {
            this.VerifyDisposed();
            lock (this.Source.SyncRootOrDefault(this.Tracker.SyncRoot))
            {
                lock (this.Tracker.SyncRoot)
                {
                    (this.Source as IRefreshAble)?.Refresh();
                    var updated = this.Filtered().ToArray();
                    this.Tracker.Reset(this, updated, this.scheduler, this.PropertyChangedEventHandler, this.NotifyCollectionChangedEventHandler);
                }
            }
        }

        protected override void RefreshNow(NotifyCollectionChangedEventArgs e)
        {
            var filter = this.Filter;
            if (filter != null)
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        {
                            if (!e.IsSingleNewItem())
                            {
                                break;
                            }

                            var newItem = e.NewItem<T>();
                            if (!filter(newItem))
                            {
                                return; // added item that is not visible
                            }

                            break;
                        }

                    case NotifyCollectionChangedAction.Remove:
                        {
                            if (!e.IsSingleOldItem())
                            {
                                break;
                            }

                            var item = e.OldItem<T>();
                            if (!filter(item))
                            {
                                return; // removed item that is not visible
                            }

                            break;
                        }

                    case NotifyCollectionChangedAction.Replace:
                        {
                            if (!(e.IsSingleNewItem() && e.IsSingleOldItem()))
                            {
                                break;
                            }

                            var newItem = e.NewItem<T>();
                            var oldItem = e.OldItem<T>();
                            if (!(filter(newItem) || filter(oldItem)))
                            {
                                return; // replaced item that is not visible
                            }

                            break;
                        }

                    case NotifyCollectionChangedAction.Move:
                        {
                            if (!e.IsSingleNewItem())
                            {
                                break;
                            }

                            var newItem = e.NewItem<T>();
                            if (!filter(newItem))
                            {
                                return; // moved item that is not visible
                            }

                            break;
                        }

                    case NotifyCollectionChangedAction.Reset:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            lock (this.Source.SyncRootOrDefault(this.Tracker.SyncRoot))
            {
                lock (this.Tracker.SyncRoot)
                {
                    this.Tracker.Refresh(this, this.Filtered().ToArray(), CollectionSynchronizer<T>.ResetArgs, this.scheduler, this.PropertyChangedEventHandler, this.NotifyCollectionChangedEventHandler);
                }
            }
        }

        protected override void Refresh(IReadOnlyList<NotifyCollectionChangedEventArgs> changes)
        {
            lock (this.Source.SyncRootOrDefault(this.Tracker.SyncRoot))
            {
                lock (this.Tracker.SyncRoot)
                {
                    var updated = this.Filtered().ToArray();
                    this.Tracker.Refresh(
                        this,
                        updated,
                        null,
                        this.scheduler,
                        this.PropertyChangedEventHandler,
                        this.NotifyCollectionChangedEventHandler);
                }
            }
        }

        protected IEnumerable<T> Filtered()
        {
            if (this.Filter == null)
            {
                return this.Source;
            }

            return this.Source.Where(this.filter);
        }

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
