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
    using System.Reactive.Disposables;
    using System.Reactive.Linq;

    using Gu.Reactive.Internals;

    [SuppressMessage("ReSharper", "PossibleMultipleEnumeration", Justification = "We need the reference")]
    [DebuggerTypeProxy(typeof(CollectionDebugView<>))]
    [DebuggerDisplay("Count = {Count}")]
    public class MappingView<TSource, TResult> : ReadonlySerialViewBase<TResult>, IReadOnlyObservableCollection<TResult>, IUpdater, IRefreshAble
    {
        private readonly IEnumerable<TSource> _source;
        private readonly CompositeDisposable _updateSubscription = new CompositeDisposable();
        private readonly IMappingFactory<TSource, TResult> _factory;
        private readonly List<TResult> _mapped =new List<TResult>();

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

        private MappingView(IEnumerable<TSource> source, IScheduler scheduler, Func<TSource, TResult> selector, params IObservable<object>[] triggers)
            : this(source, scheduler, MappingFactory.Create(selector), triggers)
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

        private MappingView(IEnumerable<TSource> source, IScheduler scheduler, Func<TSource, int, TResult> indexSelector, Func<TResult, int, TResult> indexUpdater, params IObservable<object>[] triggers)
            : this(source, scheduler, MappingFactory.Create(indexSelector, indexUpdater), triggers)
        {
        }

        private MappingView(IEnumerable<TSource> source, IScheduler scheduler, IMappingFactory<TSource,TResult> factory , params IObservable<object>[] triggers)
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(source as INotifyCollectionChanged, "source");
            Ensure.NotNull(factory, nameof(factory));
            
            _source = source;
            _factory = factory;
            _mapped.AddRange(source.Select(GetOrCreateValue));
            _updateSubscription.Add(ThrottledRefresher.Create(this, source, TimeSpan.Zero, scheduler, false)
                                                      .Subscribe(OnSourceCollectionChanged));
            if (triggers != null && triggers.Any(t => t != null))
            {
                var triggerSubscription = triggers.Merge()
                                                  .ObserveOn(scheduler ?? Scheduler.Default)
                                                  .Subscribe(_ => Refresh());
                _updateSubscription.Add(triggerSubscription);
            }
            SetSource(_mapped);
        }

        object IUpdater.IsUpdatingSourceItem => null;

        public override void Refresh()
        {
            lock (Source.SyncRootOrDefault(_mapped.SyncRoot()))
            {
                lock (_mapped.SyncRoot())
                {
                    (Source as IRefreshAble)?.Refresh();
                    _mapped.Clear();
                    _mapped.AddRange(_source.Select(GetOrCreateValue));
                    base.Refresh();
                }
            }
        }

        protected virtual TResult GetOrCreateValue(TSource key, int index)
        {
            return _factory.GetOrCreateValue(key, index);
        }

        protected virtual NotifyCollectionChangedEventArgs UpdateIndex(int index)
        {
            if (!_factory.CanUpdateIndex)
            {
                return null;
            }
            var key = _source.ElementAt(index);
            var updated = _factory.UpdateIndex(key, index);
            var old = _mapped[index];
            if (ReferenceEquals(updated, old))
            {
                return null;
            }
            _mapped[index] = updated;
            return Diff.CreateReplaceEventArgs(updated, old, index);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns>True if NotifyCollectionChanged.Reset was raised</returns>
        protected virtual List<NotifyCollectionChangedEventArgs> UpdateIndicesFrom(int index)
        {
            if (!_factory.CanUpdateIndex)
            {
                return new List<NotifyCollectionChangedEventArgs>();
            }

            var count = _source.Count();
            var changes = new List<NotifyCollectionChangedEventArgs>();
            for (int i = index; i < count; i++)
            {
                var change = UpdateIndex(i);
                if (change != null)
                {
                    changes.Add(change);
                }
            }
            return changes;
        }

        protected virtual void OnSourceCollectionChanged(IReadOnlyList<NotifyCollectionChangedEventArgs> changeCollection)
        {
            if (changeCollection == null || changeCollection.Count == 0)
            {
                return;
            }
            if (changeCollection.Count > 1)
            {
                Refresh();
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
                            var value = GetOrCreateValue(_source.ElementAt(index), index);
                            _mapped.Insert(index, value);
                            var changes = UpdateIndicesFrom(index + 1);
                            var change = Diff.CreateAddEventArgs(value, index);
                            changes.Add(change);
                            base.Refresh(changes);
                            break;
                        }
                    case NotifyCollectionChangedAction.Remove:
                        {
                            var index = singleChange.OldStartingIndex;
                            var value = _mapped[index];
                            _mapped.RemoveAt(index);
                            var changes = UpdateIndicesFrom(index);
                            var change = Diff.CreateRemoveEventArgs(value, index);
                            changes.Add(change);
                            base.Refresh(changes);
                            break;
                        }
                    case NotifyCollectionChangedAction.Replace:
                        {
                            var index = singleChange.NewStartingIndex;
                            var value = GetOrCreateValue(_source.ElementAt(index), index);
                            var oldValue = _mapped[singleChange.OldStartingIndex];
                            _mapped[index] = value;
                            var change = Diff.CreateReplaceEventArgs(value, oldValue, index);
                            base.Refresh(new[] { change });
                            break;
                        }

                    case NotifyCollectionChangedAction.Move:
                        {
                            var value = _mapped[singleChange.OldStartingIndex];
                            _mapped.RemoveAt(singleChange.OldStartingIndex);
                            _mapped.Insert(singleChange.NewStartingIndex, value);
                            UpdateIndex(singleChange.OldStartingIndex);
                            UpdateIndex(singleChange.NewStartingIndex);
                            var change = Diff.CreateMoveEventArgs(_mapped[singleChange.NewStartingIndex], singleChange.NewStartingIndex, singleChange.OldStartingIndex);
                            base.Refresh(new[] { change});
                            break;
                        }

                    case NotifyCollectionChangedAction.Reset:
                        Refresh();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception)
            {
                Refresh(); // Resetting
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _updateSubscription.Dispose();
                _factory.Dispose();
            }
        }
    }
}
