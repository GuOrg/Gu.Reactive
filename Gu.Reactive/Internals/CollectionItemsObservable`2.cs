namespace Gu.Reactive.Internals
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Disposables;

    using Gu.Reactive.PropertyPathStuff;

    internal sealed class CollectionItemsObservable<TCollection, TItem, TValue> :
        ObservableBase<EventPattern<ItemPropertyChangedEventArgs<TItem, TValue>>>,
        IEnumerable<TItem>, IDisposable
        where TCollection : class, IEnumerable<TItem>, INotifyCollectionChanged
        where TItem : class, INotifyPropertyChanged
    {
        private static readonly ObjectIdentityComparer<TItem> IdentityComparer = new ObjectIdentityComparer<TItem>();

        private readonly IObservable<EventPattern<PropertyChangedAndValueEventArgs<TCollection>>> _sourceObservable;

        private readonly bool _signalInitial;
        private readonly PropertyPath<TItem, TValue> _propertyPath;
        private readonly WeakReference _wr = new WeakReference(null);
        private readonly ConcurrentDictionary<TItem, IDisposable> _map = new ConcurrentDictionary<TItem, IDisposable>(IdentityComparer);
        private readonly object _lock = new object();
        private readonly SerialDisposable _collectionChangedSubscription = new SerialDisposable();
        private IObserver<EventPattern<ItemPropertyChangedEventArgs<TItem, TValue>>> _observer;
        private bool _disposed;
        private bool _intialized;

        public CollectionItemsObservable(TCollection collection, bool signalInitial, PropertyPath<TItem, TValue> propertyPath)
        {
            _signalInitial = signalInitial;
            _propertyPath = propertyPath;
            _wr.Target = collection;
        }

        public CollectionItemsObservable(
            IObservable<EventPattern<PropertyChangedAndValueEventArgs<TCollection>>> sourceObservable,
            bool signalInitial,
            PropertyPath<TItem, TValue> propertyPath)
        {
            _sourceObservable = sourceObservable;
            _signalInitial = signalInitial;
            _propertyPath = propertyPath;
        }

        public IEnumerable<TItem> Collection
        {
            get
            {
                VerifyDisposed();
                var collection = (IEnumerable<TItem>)_wr.Target;
                return collection ?? Enumerable.Empty<TItem>();
            }
        }

        public IEnumerator<TItem> GetEnumerator()
        {
            return Collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            VerifyDisposed();
            return GetEnumerator();
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            _collectionChangedSubscription.Dispose();
            foreach (var disposable in _map.Values.ToArray())
            {
                disposable?.Dispose();
            }

            _map.Clear();
            _wr.Target = null;
        }

        protected override IDisposable SubscribeCore(IObserver<EventPattern<ItemPropertyChangedEventArgs<TItem, TValue>>> observer)
        {
            _observer = observer;
            var observableCollection = _wr.Target as TCollection;
            if (observableCollection != null)
            {
                _collectionChangedSubscription.Disposable = observableCollection.ObserveCollectionChanged(true)
                                                                                .Subscribe(Update);
            }
            else if (_sourceObservable != null)
            {
                _sourceObservable.Subscribe(UpdateCollectionSubscription);
            }
            else
            {
                throw new InvalidOperationException("Nothing to subscribe on");
            }

            _intialized = true;
            return this;
        }

        private void UpdateCollectionSubscription(EventPattern<PropertyChangedAndValueEventArgs<TCollection>> eventPattern)
        {
            _wr.Target = null;
            Reset(null);
            _collectionChangedSubscription.Disposable = null;
            if (eventPattern.EventArgs.HasValue)
            {
                _intialized = false;
                var collection = eventPattern.EventArgs.Value;
                _wr.Target = collection;
                _collectionChangedSubscription.Disposable = collection.ObserveCollectionChanged(true)
                                                           .Subscribe(Update);
                _intialized = true;
            }
        }

        private void Update(EventPattern<NotifyCollectionChangedEventArgs> e)
        {
            VerifyDisposed();
            lock (_lock)
            {
                switch (e.EventArgs.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        AddRange(e.EventArgs.NewItems);
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        RemoveRange(e.EventArgs.OldItems);
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        AddRange(e.EventArgs.NewItems);
                        RemoveRange(e.EventArgs.OldItems);
                        break;
                    case NotifyCollectionChangedAction.Move:
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        Reset(Collection);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void Reset(IEnumerable<TItem> collection)
        {
            if (collection == null)
            {
                foreach (var disposable in _map.Values)
                {
                    disposable.Dispose();
                }

                _map.Clear();
                return;
            }

            var old = _map.Keys.Except(collection, IdentityComparer).ToArray();
            RemoveRange(old);
            var newItems = collection.Except(_map.Keys, IdentityComparer).ToArray();
            AddRange(newItems);
        }

        private void RemoveRange(IList items)
        {
            if (items == null)
            {
                return;
            }

            foreach (var item in items)
            {
                if (item == null)
                {
                    continue;
                }

                IDisposable subscription;
                if (_map.TryRemove((TItem)item, out subscription))
                {
                    subscription.Dispose();
                }
            }
        }

        private void AddRange(IList items)
        {
            if (items == null)
            {
                return;
            }

            foreach (var item in items)
            {
                if (item == null)
                {
                    continue;
                }

                _map.GetOrAdd((TItem)item, ObserveItem);
            }
        }

        private IDisposable ObserveItem(TItem item)
        {
            var itemSubscription = item.ObservePropertyChangedWithValue(_propertyPath, _intialized || _signalInitial)
                                       .Subscribe(x => OnItemPropertyChanged(item, x));
            return itemSubscription;
        }

        private void OnItemPropertyChanged(TItem item, EventPattern<PropertyChangedAndValueEventArgs<TValue>> x)
        {
            _observer?.OnNext(new EventPattern<ItemPropertyChangedEventArgs<TItem, TValue>>(x.Sender, new ItemPropertyChangedEventArgs<TItem, TValue>(item, x)));
        }

        private void VerifyDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }
    }
}