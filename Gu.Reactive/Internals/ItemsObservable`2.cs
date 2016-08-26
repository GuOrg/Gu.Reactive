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
        private readonly IObservable<EventPattern<PropertyChangedAndValueEventArgs<TCollection>>> sourceObservable;
        private readonly bool signalInitial;
        private readonly PropertyPath<TItem, TProperty> propertyPath;
        private readonly WeakReference collectionRef = new WeakReference(null);
        private bool disposed;

        public ItemsObservable(
            TCollection source,
            Expression<Func<TItem, TProperty>> property,
            bool signalInitial = true)
        {
            this.collectionRef.Target = source;
            this.signalInitial = signalInitial;
            this.propertyPath = PropertyPath.Create(property);
        }

        public ItemsObservable(
            IObservable<EventPattern<PropertyChangedAndValueEventArgs<TCollection>>> source,
            Expression<Func<TItem, TProperty>> property)
        {
            this.sourceObservable = source;
            this.signalInitial = true;
            this.propertyPath = PropertyPath.Create(property);
        }

        protected override IDisposable SubscribeCore(IObserver<EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>> observer)
        {
            this.VerifyDisposed();

            CollectionItemsObservable<TCollection, TItem, TProperty> observable;
            if (this.collectionRef.Target != null)
            {
                observable = new CollectionItemsObservable<TCollection, TItem, TProperty>((TCollection)this.collectionRef.Target, this.signalInitial, this.propertyPath);
            }
            else if(this.sourceObservable != null)
            {
                observable = new CollectionItemsObservable<TCollection, TItem, TProperty>(this.sourceObservable, this.signalInitial, this.propertyPath);
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
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
        }

        private void VerifyDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(
                    this.GetType()
                        .FullName);
            }
        }
    }
}