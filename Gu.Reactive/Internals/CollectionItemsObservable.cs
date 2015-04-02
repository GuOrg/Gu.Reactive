namespace Gu.Reactive.Internals
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reactive;

    internal sealed class CollectionItemsObservable<TItem, TValue> :
        ObservableBase<EventPattern<ItemPropertyChangedEventArgs<TItem, TValue>>>,
        IEnumerable<TItem>, IDisposable
        where TItem : class, INotifyPropertyChanged
    {
        private readonly bool _signalInitial;
        private readonly PropertyPath<TItem, TValue> _propertyPath;
        private readonly Expression<Func<TItem, TValue>> _property;

        private static readonly ObjectIdentityComparer<TItem> IdentityComparer = new ObjectIdentityComparer<TItem>();
        private readonly WeakReference _wr = new WeakReference(null);
        private readonly ConcurrentDictionary<TItem, IDisposable> _map = new ConcurrentDictionary<TItem, IDisposable>(IdentityComparer);
        private readonly object _lock = new object();
        private IObserver<EventPattern<ItemPropertyChangedEventArgs<TItem, TValue>>> _observer;
        private bool _disposed;
        private bool _intialized;
        private IDisposable _sourceSubscription;

        public CollectionItemsObservable(ObservableCollection<TItem> collection, bool signalInitial, PropertyPath<TItem, TValue> propertyPath, Expression<Func<TItem, TValue>> property)
        {
            throw new NotImplementedException("Use propertypath for subscriptions");
            _signalInitial = signalInitial;
            _propertyPath = propertyPath;
            _property = property;
            _wr.Target = collection;
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
            if (_sourceSubscription != null)
            {
                _sourceSubscription.Dispose();
            }
            foreach (var disposable in _map.Values.ToArray())
            {
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
            _map.Clear();
            _wr.Target = null;
        }

        protected override IDisposable SubscribeCore(IObserver<EventPattern<ItemPropertyChangedEventArgs<TItem, TValue>>> observer)
        {
            _observer = observer;
            var observableCollection = _wr.Target as ObservableCollection<TItem>;
            if (observableCollection != null)
            {
                _sourceSubscription = observableCollection.ObserveCollectionChanged(true)
                                                          .Subscribe(Update);
            }
            _intialized = true;
            return this;
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
            var itemSubscription = item.ObservePropertyChangedWithValue(_property, _intialized || _signalInitial)
                                         .Subscribe(x => OnItemPropertyChanged(item, x));
            return itemSubscription;
        }

        private void OnItemPropertyChanged(TItem item, EventPattern<PropertyChangedAndValueEventArgs<TValue>> x)
        {
            if (_observer != null)
            {
                _observer.OnNext(new EventPattern<ItemPropertyChangedEventArgs<TItem, TValue>>(_wr.Target, new ItemPropertyChangedEventArgs<TItem, TValue>(item, x.EventArgs)));
            }
        }

        private void VerifyDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(
                    GetType()
                        .FullName);
            }
        }
    }
}