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
        private readonly Expression<Func<TItem, TProperty>> _property;
        private readonly bool _signalInitial;
        private readonly string _propertyName;
        private readonly PropertyPath<TItem, TProperty> _propertyPath;
        private readonly WeakReference _collectionRef = new WeakReference(null);
        private bool _disposed;

        public ItemsObservable(
            ObservableCollection<TItem> source,
            Expression<Func<TItem, TProperty>> property,
            bool signalInitial = true)
        {
            _collectionRef.Target = source;
            _property = property;
            _signalInitial = signalInitial;
            _propertyName = NameOf.Property(property);
            _propertyPath = Internals.PropertyPath.Create(property);
        }

        public ItemsObservable(
            IObservable<EventPattern<PropertyChangedAndValueEventArgs<ObservableCollection<TItem>>>> source,
            Expression<Func<TItem, TProperty>> property)
        {
            _property = property;
            _signalInitial = true;
            _propertyName = NameOf.Property(property);
            _propertyPath = Internals.PropertyPath.Create(property);
            throw new NotImplementedException();
        }

        public ItemsObservable(
            IObservable<EventPattern<NotifyCollectionChangedEventArgs>> collectionChanged,
            Expression<Func<TItem, TProperty>> property,
            bool signalInitial)
        {
            _property = property;
            _signalInitial = signalInitial;
            _propertyName = NameOf.Property(property);
            _propertyPath = Internals.PropertyPath.Create(property);
            throw new NotImplementedException();
        }

        protected override IDisposable SubscribeCore(IObserver<EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>> observer)
        {
            VerifyDisposed();
            var observable = new CollectionItemsObservable<TItem, TProperty>((ObservableCollection<TItem>)_collectionRef.Target, _signalInitial, _propertyPath, _property);
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