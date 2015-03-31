namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Reactive;
    using System.Reactive.Subjects;

    internal sealed class ItemsObservable<T, TProperty> :
        IObservable<EventPattern<ChildPropertyChangedEventArgs<T, TProperty>>>,
        IDisposable
        where T : INotifyPropertyChanged
    {
        private readonly Expression<Func<T, TProperty>> _property;
        private readonly bool _signalInitial;
        private readonly Subject<EventPattern<ChildPropertyChangedEventArgs<T, TProperty>>> _subject = new Subject<EventPattern<ChildPropertyChangedEventArgs<T, TProperty>>>();
        private readonly WeakCollectionWrapper<T> _weakSource;
        private bool _disposed;
        private string _propertyName;
        private IValuePath<T, TProperty> _valuePath;

        public ItemsObservable(
            ObservableCollection<T> source,
            Expression<Func<T, TProperty>> property,
            bool signalInitial = true)
        {
            _property = property;
            _signalInitial = signalInitial;
            _propertyName = NameOf.Property(property);
            _valuePath = Get.ValuePath(property);
            _weakSource = new WeakCollectionWrapper<T>(source);
            source.ObserveCollectionChanged()
                  .Subscribe(OnSourceChanged);
        }

        public IDisposable Subscribe(IObserver<EventPattern<ChildPropertyChangedEventArgs<T, TProperty>>> observer)
        {
            VerifyDisposed();
            if (_signalInitial)
            {
                var collection = (ObservableCollection<T>)_weakSource.Collection;
                foreach (var item in _weakSource)
                {
                    var value = _valuePath.Value(item);
                    if (value.HasValue)
                    {
                        observer.OnNext(new EventPattern<ChildPropertyChangedEventArgs<T, TProperty>>(collection, new ChildPropertyChangedEventArgs<T, TProperty>(item, value.ValueOrDefault, _propertyName)));
                    }
                }
            }
            return _subject.Subscribe(observer);
        }

        /// <summary>
        /// Make the class sealed when using this. 
        /// Call VerifyDisposed at the start of all public methods
        /// </summary>
        public void Dispose()
        {
            _disposed = true;
            _subject.Dispose();
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

        private void OnSourceChanged(EventPattern<NotifyCollectionChangedEventArgs> eventPattern)
        {
            throw new NotImplementedException();
        }
    }
}