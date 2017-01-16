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

    using Gu.Reactive.Internals;

    [DebuggerTypeProxy(typeof(CollectionDebugView<>))]
    [DebuggerDisplay("Count = {Count}")]
    public class MappingView<TSource, TResult> : ReadonlySerialViewBase<TResult>, IReadOnlyObservableCollection<TResult>, IUpdater, IRefreshAble
    {
        private readonly IEnumerable<TSource> source;
        private readonly CompositeDisposable updateSubscription = new CompositeDisposable();
        private readonly IMappingFactory<TSource, TResult> factory;
        private readonly List<TResult> mapped = new List<TResult>();

        public MappingView(ObservableCollection<TSource> source, Func<TSource, TResult> selector, IScheduler scheduler, params IObservable<object>[] triggers)
            : this(source, scheduler, selector, triggers)
        {
        }

        public MappingView(IObservableCollection<TSource> source, Func<TSource, TResult> selector, IScheduler scheduler, params IObservable<object>[] triggers)
            : this(source, scheduler, selector, triggers)
        {
        }

        public MappingView(ReadOnlyObservableCollection<TSource> source, Func<TSource, TResult> selector, IScheduler scheduler, params IObservable<object>[] triggers)
            : this(source, scheduler, selector, triggers)
        {
        }

        public MappingView(IReadOnlyObservableCollection<TSource> source, Func<TSource, TResult> selector, IScheduler scheduler, params IObservable<object>[] triggers)
            : this(source, scheduler, selector, triggers)
        {
        }

        public MappingView(ObservableCollection<TSource> source, Func<TSource, int, TResult> indexSelector, IScheduler scheduler, params IObservable<object>[] triggers)
            : this(source, scheduler, indexSelector, null, triggers)
        {
        }

        public MappingView(IObservableCollection<TSource> source, Func<TSource, int, TResult> indexSelector, IScheduler scheduler, params IObservable<object>[] triggers)
            : this(source, scheduler, indexSelector, null, triggers)
        {
        }

        public MappingView(ReadOnlyObservableCollection<TSource> source, Func<TSource, int, TResult> indexSelector, IScheduler scheduler, params IObservable<object>[] triggers)
            : this(source, scheduler, indexSelector, null, triggers)
        {
        }

        public MappingView(IReadOnlyObservableCollection<TSource> source, Func<TSource, int, TResult> indexSelector, IScheduler scheduler, params IObservable<object>[] triggers)
            : this(source, scheduler, indexSelector, null, triggers)
        {
        }

        public MappingView(ObservableCollection<TSource> source, Func<TSource, int, TResult> indexSelector, Func<TResult, int, TResult> indexUpdater, IScheduler scheduler, params IObservable<object>[] triggers)
            : this(source, scheduler, indexSelector, indexUpdater, triggers)
        {
        }

        public MappingView(IObservableCollection<TSource> source, Func<TSource, int, TResult> indexSelector, Func<TResult, int, TResult> indexUpdater, IScheduler scheduler, params IObservable<object>[] triggers)
            : this(source, scheduler, indexSelector, indexUpdater, triggers)
        {
        }

        public MappingView(ReadOnlyObservableCollection<TSource> source, Func<TSource, int, TResult> indexSelector, Func<TResult, int, TResult> indexUpdater, IScheduler scheduler, params IObservable<object>[] triggers)
            : this(source, scheduler, indexSelector, indexUpdater, triggers)
        {
        }

        public MappingView(IReadOnlyObservableCollection<TSource> source, Func<TSource, int, TResult> indexSelector, Func<TResult, int, TResult> indexUpdater, IScheduler scheduler, params IObservable<object>[] triggers)
            : this(source, scheduler, indexSelector, indexUpdater, triggers)
        {
        }

        private MappingView(IEnumerable<TSource> source, IScheduler scheduler, Func<TSource, TResult> selector, params IObservable<object>[] triggers)
           : this(source, scheduler, MappingFactory.Create(selector), triggers)
        {
        }

        private MappingView(IEnumerable<TSource> source, IScheduler scheduler, Func<TSource, int, TResult> indexSelector, Func<TResult, int, TResult> indexUpdater, params IObservable<object>[] triggers)
            : this(source, scheduler, MappingFactory.Create(indexSelector, indexUpdater), triggers)
        {
        }

        private MappingView(IEnumerable<TSource> source, IScheduler scheduler, IMappingFactory<TSource, TResult> factory, params IObservable<object>[] triggers)
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(source as INotifyCollectionChanged, "source");
            Ensure.NotNull(factory, nameof(factory));

            this.source = source;
            this.factory = factory;
            this.mapped.AddRange(source.Select(this.GetOrCreateValue));
            this.updateSubscription.Add(ThrottledRefresher.Create(this, source, TimeSpan.Zero, scheduler, false)
                                                          .ObserveOn(scheduler ?? Scheduler.Immediate)
                                                          .Subscribe(this.OnSourceCollectionChanged));
            if (triggers != null && triggers.Any(t => t != null))
            {
                var triggerSubscription = triggers.Merge()
                                                  .ObserveOn(scheduler ?? Scheduler.Immediate)
                                                  .Subscribe(_ => this.Refresh());
                this.updateSubscription.Add(triggerSubscription);
            }

            this.SetSource(this.mapped);
        }

        /// <inheritdoc/>
        object IUpdater.IsUpdatingSourceItem => null;

        /// <inheritdoc/>
        public override void Refresh()
        {
            lock (this.Source.SyncRootOrDefault(this.mapped.SyncRoot()))
            {
                lock (this.mapped.SyncRoot())
                {
                    (this.Source as IRefreshAble)?.Refresh();
                    this.mapped.Clear();
                    this.mapped.AddRange(this.source.Select(this.GetOrCreateValue));
                    base.Refresh();
                }
            }
        }

        /// <summary>
        /// Delegates creation to mapping factory.
        /// </summary>
        protected virtual TResult GetOrCreateValue(TSource key, int index) => this.factory.GetOrCreateValue(key, index);

        /// <summary>
        /// Delegates updating of item at index to mapping factory.
        /// </summary>
        /// <param name="index">The index to update the item for.</param>
        /// <returns>The collection changed args the update causes. Can be null.</returns>
        protected virtual NotifyCollectionChangedEventArgs UpdateIndex(int index)
        {
            if (!this.factory.CanUpdateIndex)
            {
                throw new InvalidOperationException("Cannot update.");
            }

            var key = this.source.ElementAt(index);
            var updated = this.factory.UpdateIndex(key, index);
            var old = this.mapped[index];
            if (ReferenceEquals(updated, old))
            {
                return null;
            }

            this.mapped[index] = updated;
            return Diff.CreateReplaceEventArgs(updated, old, index);
        }

        /// <summary>
        /// Delegates updating of items at and above index to mapping factory.
        /// </summary>
        /// <param name="index">The index to update the item for.</param>
        /// <returns>The collection changed args the update causes.</returns>
        protected virtual List<NotifyCollectionChangedEventArgs> UpdateIndicesFrom(int index)
        {
            if (!this.factory.CanUpdateIndex)
            {
                throw new InvalidOperationException("Cannot update.");
            }

            var count = this.source.Count();
            var changes = new List<NotifyCollectionChangedEventArgs>();
            for (var i = index; i < count; i++)
            {
                var change = this.UpdateIndex(i);
                if (change != null)
                {
                    changes.Add(change);
                }
            }

            return changes;
        }

        /// <summary>
        /// Called when the source collection changed.
        /// </summary>
        /// <param name="changeCollection">The changes accumulated during the buffer time.</param>
        protected virtual void OnSourceCollectionChanged(IReadOnlyList<NotifyCollectionChangedEventArgs> changeCollection)
        {
            if (changeCollection == null || changeCollection.Count == 0)
            {
                return;
            }

            if (changeCollection.Count > 1)
            {
                this.Refresh();
                return;
            }

            var singleChange = changeCollection[0];
            try
            {
                switch (singleChange.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        {
                            var index = singleChange.NewStartingIndex;
                            var value = this.GetOrCreateValue(this.source.ElementAt(index), index);
                            this.mapped.Insert(index, value);
                            var changes = this.UpdateIndicesFrom(index + 1);
                            var change = Diff.CreateAddEventArgs(value, index);
                            changes.Add(change);
                            this.Refresh(changes);
                            break;
                        }

                    case NotifyCollectionChangedAction.Remove:
                        {
                            var index = singleChange.OldStartingIndex;
                            var value = this.mapped[index];
                            this.mapped.RemoveAt(index);
                            var changes = this.UpdateIndicesFrom(index);
                            var change = Diff.CreateRemoveEventArgs(value, index);
                            changes.Add(change);
                            this.Refresh(changes);
                            break;
                        }

                    case NotifyCollectionChangedAction.Replace:
                        {
                            var index = singleChange.NewStartingIndex;
                            var value = this.GetOrCreateValue(this.source.ElementAt(index), index);
                            var oldValue = this.mapped[singleChange.OldStartingIndex];
                            this.mapped[index] = value;
                            var change = Diff.CreateReplaceEventArgs(value, oldValue, index);
                            this.Refresh(new[] { change });
                            break;
                        }

                    case NotifyCollectionChangedAction.Move:
                        {
                            var value = this.mapped[singleChange.OldStartingIndex];
                            this.mapped.RemoveAt(singleChange.OldStartingIndex);
                            this.mapped.Insert(singleChange.NewStartingIndex, value);
                            this.UpdateIndex(singleChange.OldStartingIndex);
                            this.UpdateIndex(singleChange.NewStartingIndex);
                            var change = Diff.CreateMoveEventArgs(this.mapped[singleChange.NewStartingIndex], singleChange.NewStartingIndex, singleChange.OldStartingIndex);
                            this.Refresh(new[] { change });
                            break;
                        }

                    case NotifyCollectionChangedAction.Reset:
                        this.Refresh();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception)
            {
                this.Refresh(); // Resetting
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.updateSubscription.Dispose();
                this.factory.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
