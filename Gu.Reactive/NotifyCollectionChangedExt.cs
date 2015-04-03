namespace Gu.Reactive
{
    using System;
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.Linq.Expressions;
    using System.Reactive;
    using System.Reactive.Linq;
    using Internals;

    public static class NotifyCollectionChangedExt
    {
        /// <summary>
        /// Observes collectionchanged events for source.
        /// </summary>
        /// <typeparam name="TCollection"></typeparam>
        /// <param name="source"></param>
        /// <param name="signalInitial"></param>
        /// <returns></returns>
        public static IObservable<EventPattern<NotifyCollectionChangedEventArgs>> ObserveCollectionChanged<TCollection>(
            this TCollection source,
            bool signalInitial = true)
            where TCollection : IEnumerable, INotifyCollectionChanged
        {
            //Contract.Requires<ArgumentNullException>(source != null);
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

        /// <summary>
        /// Observes propertychanges for items of the collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="source"></param>
        /// <param name="property"></param>
        /// <param name="signalInitial"></param>
        /// <returns></returns>
        public static IObservable<EventPattern<ItemPropertyChangedEventArgs<T, TProperty>>> ObserveItemPropertyChanged<T, TProperty>(
            this ObservableCollection<T> source,
            Expression<Func<T, TProperty>> property,
            bool signalInitial = true) where T : class, INotifyPropertyChanged
        {
            return new ItemsObservable<T, TProperty>(source, property, signalInitial);
        }

        public static IObservable<EventPattern<ItemPropertyChangedEventArgs<T, TProperty>>> ItemPropertyChanged<T, TProperty>(
             this IObservable<EventPattern<PropertyChangedAndValueEventArgs<ObservableCollection<T>>>> source,
             Expression<Func<T, TProperty>> property) where T : class, INotifyPropertyChanged
        {
            var observable = new ItemsObservable<T, TProperty>(source, property);
            return observable;
        }
    }
}