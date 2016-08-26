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

        private readonly IObservable<EventPattern<PropertyChangedAndValueEventArgs<TCollection>>> sourceObservable;

        private readonly bool signalInitial;
        private readonly PropertyPath<TItem, TValue> propertyPath;
        private readonly WeakReference wr = new WeakReference(null);
        private readonly ConcurrentDictionary<TItem, IDisposable> map = new ConcurrentDictionary<TItem, IDisposable>(IdentityComparer);
        private readonly object @lock = new object();
        private readonly SerialDisposable collectionChangedSubscription = new SerialDisposable();
        private IObserver<EventPattern<ItemPropertyChangedEventArgs<TItem, TValue>>> _observer;
        private bool disposed;
        private bool intialized;

        public CollectionItemsObservable(TCollection collection, bool signalInitial, PropertyPath<TItem, TValue> propertyPath)
        {
            this.signalInitial = signalInitial;
            this.propertyPath = propertyPath;
            this.wr.Target = collection;
        }

        public CollectionItemsObservable(
            IObservable<EventPattern<PropertyChangedAndValueEventArgs<TCollection>>> sourceObservable,
            bool signalInitial,
            PropertyPath<TItem, TValue> propertyPath)
        {
            this.sourceObservable = sourceObservable;
            this.signalInitial = signalInitial;
            this.propertyPath = propertyPath;
        }

        public IEnumerable<TItem> Collection
        {
            get
            {
                this.VerifyDisposed();
                var collection = (IEnumerable<TItem>)this.wr.Target;
                return collection ?? Enumerable.Empty<TItem>();
            }
        }

        public IEnumerator<TItem> GetEnumerator()
        {
            return this.Collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            this.VerifyDisposed();
            return this.GetEnumerator();
        }

        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            this.collectionChangedSubscription.Dispose();
            foreach (var disposable in this.map.Values.ToArray())
            {
                disposable?.Dispose();
            }

            this.map.Clear();
            this.wr.Target = null;
        }

        protected override IDisposable SubscribeCore(IObserver<EventPattern<ItemPropertyChangedEventArgs<TItem, TValue>>> observer)
        {
            this._observer = observer;
            var observableCollection = this.wr.Target as TCollection;
            if (observableCollection != null)
            {
                this.collectionChangedSubscription.Disposable = observableCollection.ObserveCollectionChanged(true)
                                                                                .Subscribe(this.Update);
            }
            else if (this.sourceObservable != null)
            {
                this.sourceObservable.Subscribe(this.UpdateCollectionSubscription);
            }
            else
            {
                throw new InvalidOperationException("Nothing to subscribe on");
            }

            this.intialized = true;
            return this;
        }

        private void UpdateCollectionSubscription(EventPattern<PropertyChangedAndValueEventArgs<TCollection>> eventPattern)
        {
            this.wr.Target = null;
            this.Reset(null);
            this.collectionChangedSubscription.Disposable = null;
            if (eventPattern.EventArgs.HasValue)
            {
                this.intialized = false;
                var collection = eventPattern.EventArgs.Value;
                this.wr.Target = collection;
                this.collectionChangedSubscription.Disposable = collection.ObserveCollectionChanged(true)
                                                           .Subscribe(this.Update);
                this.intialized = true;
            }
        }

        private void Update(EventPattern<NotifyCollectionChangedEventArgs> e)
        {
            this.VerifyDisposed();
            lock (this.@lock)
            {
                switch (e.EventArgs.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        this.AddRange(e.EventArgs.NewItems);
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        this.RemoveRange(e.EventArgs.OldItems);
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        this.AddRange(e.EventArgs.NewItems);
                        this.RemoveRange(e.EventArgs.OldItems);
                        break;
                    case NotifyCollectionChangedAction.Move:
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        this.Reset(this.Collection);
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
                foreach (var disposable in this.map.Values)
                {
                    disposable.Dispose();
                }

                this.map.Clear();
                return;
            }

            var old = this.map.Keys.Except(collection, IdentityComparer).ToArray();
            this.RemoveRange(old);
            var newItems = collection.Except(this.map.Keys, IdentityComparer).ToArray();
            this.AddRange(newItems);
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
                if (this.map.TryRemove((TItem)item, out subscription))
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

                this.map.GetOrAdd((TItem)item, this.ObserveItem);
            }
        }

        private IDisposable ObserveItem(TItem item)
        {
            var itemSubscription = item.ObservePropertyChangedWithValue(this.propertyPath, this.intialized || this.signalInitial)
                                       .Subscribe(x => this.OnItemPropertyChanged(item, x));
            return itemSubscription;
        }

        private void OnItemPropertyChanged(TItem item, EventPattern<PropertyChangedAndValueEventArgs<TValue>> x)
        {
            this._observer?.OnNext(new EventPattern<ItemPropertyChangedEventArgs<TItem, TValue>>(x.Sender, new ItemPropertyChangedEventArgs<TItem, TValue>(item, x)));
        }

        private void VerifyDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }
    }
}