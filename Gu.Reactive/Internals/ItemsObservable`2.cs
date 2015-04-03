namespace Gu.Reactive.Internals
{
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Reactive;

    internal sealed class ItemsObservable<TItem, TProperty> :
        ObservableBase<EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>>,
        IDisposable
        where TItem : class, INotifyPropertyChanged
    {
        private readonly IObservable<EventPattern<PropertyChangedAndValueEventArgs<ObservableCollection<TItem>>>> _sourceObservable;
        private readonly bool _signalInitial;
        private readonly PropertyPath<TItem, TProperty> _propertyPath;
        private readonly WeakReference _collectionRef = new WeakReference(null);
        private bool _disposed;

        public ItemsObservable(
            ObservableCollection<TItem> source,
            Expression<Func<TItem, TProperty>> property,
            bool signalInitial = true)
        {
            _collectionRef.Target = source;
            _signalInitial = signalInitial;
            _propertyPath = PropertyPath.Create(property);
        }

        public ItemsObservable(
            IObservable<EventPattern<PropertyChangedAndValueEventArgs<ObservableCollection<TItem>>>> source,
            Expression<Func<TItem, TProperty>> property)
        {
            _sourceObservable = source;
            _signalInitial = true;
            _propertyPath = PropertyPath.Create(property);
        }

        protected override IDisposable SubscribeCore(IObserver<EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>> observer)
        {
            VerifyDisposed();

            CollectionItemsObservable<TItem, TProperty> observable;
            if (_collectionRef.Target != null)
            {
                observable = new CollectionItemsObservable<TItem, TProperty>((ObservableCollection<TItem>)_collectionRef.Target, _signalInitial, _propertyPath);
            }
            else
            {
                observable = new CollectionItemsObservable<TItem, TProperty>(_sourceObservable, _signalInitial, _propertyPath);
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