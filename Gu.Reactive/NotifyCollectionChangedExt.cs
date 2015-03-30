namespace Gu.Reactive
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reactive;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Runtime.CompilerServices;

    public static class NotifyCollectionChangedExt
    {
        public static IObservable<EventPattern<NotifyCollectionChangedEventArgs>> ObserveCollectionChanged(
            this INotifyCollectionChanged source,
            bool signalInitial = true)
        {
            IObservable<EventPattern<NotifyCollectionChangedEventArgs>> observable =
                Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                    x => source.CollectionChanged += x,
                    x => source.CollectionChanged -= x);
            if (signalInitial)
            {
                var wr = new WeakReference(source);
                return Observable.Defer(
                    () =>
                        {
                            var current = new EventPattern<NotifyCollectionChangedEventArgs>(
                                wr.Target,
                                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                            return Observable.Return(current)
                                             .Concat(observable);
                        });
            }

            return observable;
        }

        public static IObservable<EventPattern<ChildPropertyChangedEventArgs<ObservableCollection<T>, TProperty>>> ObserveItemPropertyChanges<T, TProperty>(
            this ObservableCollection<T> source,
            Expression<Func<T, TProperty>> property,
            bool signalInitial = true) where T : INotifyPropertyChanged
        {
            throw new NotImplementedException("message");
            
            return new ItemsObservable<T, TProperty>(source, property, signalInitial);
        }

        //public static IObservable<EventPattern<ChildPropertyChangedEventArgs<ObservableCollection<T>, TProperty>>> ObserveItemPropertyChanges<T, TProperty>(
        //    this IObservable<EventPattern<NotifyCollectionChangedEventArgs>> source,
        //    Expression<Func<T, TProperty>> property,
        //    bool signalInitial = true) 
        //    where T : class, INotifyPropertyChanged
        //{
        //    source.ObserveCollectionChanged(signalInitial)
        //}

        private sealed class ItemsObservable<T, TProperty> :
            IObservable<EventPattern<ChildPropertyChangedEventArgs<ObservableCollection<T>, TProperty>>>,
            IDisposable
            where T : INotifyPropertyChanged
        {
            private readonly Expression<Func<T, TProperty>> _property;
            private readonly bool _signalInitial;
            private readonly Subject<EventPattern<ChildPropertyChangedEventArgs<ObservableCollection<T>, TProperty>>> _subject = new Subject<EventPattern<ChildPropertyChangedEventArgs<ObservableCollection<T>, TProperty>>>();
            private readonly WeakCollectionWrapper<T> _weakSource;

            private bool _disposed;

            private string _propertyName;

            public ItemsObservable(
                ObservableCollection<T> source,
                Expression<Func<T, TProperty>> property,
                bool signalInitial = true)
            {
                _property = property;
                _signalInitial = signalInitial;
                _propertyName = NameOf.Property(property);
                throw new NotImplementedException("GetPropertyOrDefault");
                
                _weakSource = new WeakCollectionWrapper<T>(source);
                source.ObserveCollectionChanged()
                      .Subscribe(OnSourceChanged);
            }

            public IDisposable Subscribe(IObserver<EventPattern<ChildPropertyChangedEventArgs<ObservableCollection<T>, TProperty>>> observer)
            {
                VerifyDisposed();
                if (_signalInitial)
                {
                    foreach (var item in _weakSource)
                    {
                        
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

        private class WeakCollectionWrapper<T> : IEnumerable<T>
        {
            private WeakReference _wr; 
            public WeakCollectionWrapper(ObservableCollection<T> collection )
            {
                _wr.Target = collection;
            }

            public IEnumerator<T> GetEnumerator()
            {
                var source =(ObservableCollection<T>) _wr.Target;
                if (source == null)
                {
                    return Enumerable.Empty<T>()
                                     .GetEnumerator();
                }
                return source.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}