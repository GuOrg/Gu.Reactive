namespace Gu.Reactive.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Reactive;

    using Gu.Reactive.PropertyPathStuff;

    internal sealed class ItemsObservable<TCollection, TItem, TProperty> :
        ObservableBase<EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>>,
        IDisposable
        where TCollection : class, IEnumerable<TItem>, INotifyCollectionChanged
        where TItem : class, INotifyPropertyChanged
    {
        private readonly IObservable<EventPattern<PropertyChangedAndValueEventArgs<TCollection>>> _sourceObservable;
        private readonly bool _signalInitial;
        private readonly PropertyPath<TItem, TProperty> _propertyPath;
        private readonly WeakReference _collectionRef = new WeakReference(null);
        private bool _disposed;

        public ItemsObservable(
            TCollection source,
            Expression<Func<TItem, TProperty>> property,
            bool signalInitial = true)
        {
            _collectionRef.Target = source;
            _signalInitial = signalInitial;
            _propertyPath = PropertyPath.Create(property);
        }

        public ItemsObservable(
            IObservable<EventPattern<PropertyChangedAndValueEventArgs<TCollection>>> source,
            Expression<Func<TItem, TProperty>> property)
        {
            _sourceObservable = source;
            _signalInitial = true;
            _propertyPath = PropertyPath.Create(property);
        }

        protected override IDisposable SubscribeCore(IObserver<EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>> observer)
        {
            VerifyDisposed();

            CollectionItemsObservable<TCollection, TItem, TProperty> observable;
            if (_collectionRef.Target != null)
            {
                observable = new CollectionItemsObservable<TCollection, TItem, TProperty>((TCollection)_collectionRef.Target, _signalInitial, _propertyPath);
            }
            else if(_sourceObservable != null)
            {
                observable = new CollectionItemsObservable<TCollection, TItem, TProperty>(_sourceObservable, _signalInitial, _propertyPath);
            }
            else
            {
                throw new InvalidOperationException();
            }
            return observable.Subscribe(observer);
        }

        /// <summary>
        /// Make the class sealed when using this. 
        /// Call VerifyDisposed at the start of all public methods
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;
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