namespace Gu.Reactive
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reactive.Concurrency;

    using Gu.Reactive.Internals;

    [SuppressMessage("ReSharper", "PossibleMultipleEnumeration", Justification = "We need the reference")]
    public class MappingView<TSource, TResult> : IReadOnlyObservableCollection<TResult>, IUpdater, IDisposable
    {
        private readonly IEnumerable<TSource> _source;
        private readonly IScheduler _scheduler;
        private readonly List<TResult> _mapped;
        private readonly IDisposable _changeSubscription;
        private readonly IMappingFactory<TSource, TResult> _factory;

        private bool _disposed;

        public MappingView(ObservableCollection<TSource> source, Func<TSource, TResult> selector, IScheduler scheduler)
            : this(source, scheduler, selector)
        {
        }

        public MappingView(IObservableCollection<TSource> source, Func<TSource, TResult> selector, IScheduler scheduler)
            : this(source, scheduler, selector)
        {
        }

        public MappingView(ReadOnlyObservableCollection<TSource> source, Func<TSource, TResult> selector, IScheduler scheduler)
            : this(source, scheduler, selector)
        {
        }

        public MappingView(IReadOnlyObservableCollection<TSource> source, Func<TSource, TResult> selector, IScheduler scheduler)
            : this(source, scheduler, selector)
        {
        }

        private MappingView(IEnumerable<TSource> source, IScheduler scheduler, Func<TSource, TResult> selector)
            : this(source, scheduler)
        {
            Ensure.NotNull(selector, "selector");
            _factory = MappingFactory.Create(selector);
            _mapped = source.Select(GetOrCreateValue)
                            .ToList();

        }

        public MappingView(ObservableCollection<TSource> source, Func<TSource, int, TResult> indexSelector, IScheduler scheduler)
            : this(source, scheduler, indexSelector, null)
        {
        }

        public MappingView(IObservableCollection<TSource> source, Func<TSource, int, TResult> indexSelector, IScheduler scheduler)
            : this(source, scheduler, indexSelector, null)
        {
        }

        public MappingView(ReadOnlyObservableCollection<TSource> source, Func<TSource, int, TResult> indexSelector, IScheduler scheduler)
            : this(source, scheduler, indexSelector, null)
        {
        }

        public MappingView(IReadOnlyObservableCollection<TSource> source, Func<TSource, int, TResult> indexSelector, IScheduler scheduler)
            : this(source, scheduler, indexSelector, null)
        {
        }

        public MappingView(ObservableCollection<TSource> source, Func<TSource, int, TResult> indexSelector, Func<TResult, int, TResult> indexUpdater, IScheduler scheduler)
            : this(source, scheduler, indexSelector, indexUpdater)
        {
        }

        public MappingView(IObservableCollection<TSource> source, Func<TSource, int, TResult> indexSelector, Func<TResult, int, TResult> indexUpdater, IScheduler scheduler)
            : this(source, scheduler, indexSelector, indexUpdater)
        {
        }

        public MappingView(ReadOnlyObservableCollection<TSource> source, Func<TSource, int, TResult> indexSelector, Func<TResult, int, TResult> indexUpdater, IScheduler scheduler)
            : this(source, scheduler, indexSelector, indexUpdater)
        {
        }

        public MappingView(IReadOnlyObservableCollection<TSource> source, Func<TSource, int, TResult> indexSelector, Func<TResult, int, TResult> indexUpdater, IScheduler scheduler)
            : this(source, scheduler, indexSelector, indexUpdater)
        {
        }

        private MappingView(IEnumerable<TSource> source, IScheduler scheduler, Func<TSource, int, TResult> indexSelector, Func<TResult, int, TResult> indexUpdater)
            : this(source, scheduler)
        {
            Ensure.NotNull(indexSelector, "indexSelector");
            _factory = MappingFactory.Create(indexSelector, indexUpdater);
            _mapped = source.Select(GetOrCreateValue)
                            .ToList();

        }

        private MappingView(IEnumerable<TSource> source, IScheduler scheduler)
        {
            Ensure.NotNull(source, "source");
            Ensure.NotNull(source as INotifyCollectionChanged, "source");
            _source = source;
            _scheduler = scheduler;
            _changeSubscription = ThrottledRefresher.Create(this, source, TimeSpan.Zero, scheduler, false)
                                                    .Subscribe(OnSourceCollectionChanged);
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        public int Count
        {
            get { return _mapped.Count; }
        }

        public TResult this[int index]
        {
            get { return _mapped[index]; }
        }

        object IUpdater.IsUpdatingSourceItem
        {
            get { return null; }
        }

        public void Refresh()
        {
            VerifyDisposed();
            var mapped = _source.Select(GetOrCreateValue)
                                .ToArray();
            var change = Diff.CollectionChange(_mapped, mapped);
            _mapped.Clear();
            _mapped.AddRange(mapped);
            Notifier.Notify(this, change, _scheduler, PropertyChanged, CollectionChanged);
        }

        public IEnumerator<TResult> GetEnumerator()
        {
            VerifyDisposed();
            return _mapped.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;
            _changeSubscription.Dispose();
            Dispose(true);
        }

        protected void VerifyDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
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
                            Notifier.Notify(this, changes, _scheduler, PropertyChanged, CollectionChanged);
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
                            Notifier.Notify(this, changes, _scheduler, PropertyChanged, CollectionChanged);
                            break;
                        }
                    case NotifyCollectionChangedAction.Replace:
                        {
                            var index = singleChange.NewStartingIndex;
                            var value = GetOrCreateValue(_source.ElementAt(index), index);
                            var oldValue = _mapped[singleChange.OldStartingIndex];
                            _mapped[index] = value;
                            var change = Diff.CreateReplaceEventArgs(value, oldValue, index);
                            Notifier.Notify(this, change, _scheduler, PropertyChanged, CollectionChanged);
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
                            Notifier.Notify(this, change, _scheduler, PropertyChanged, CollectionChanged);
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

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _changeSubscription.Dispose();
                _factory.Dispose();
            }
        }
    }
}
