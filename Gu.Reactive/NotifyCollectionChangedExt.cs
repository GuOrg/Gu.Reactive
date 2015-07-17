namespace Gu.Reactive
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Reactive;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;

    using Gu.Reactive.Internals;

    public static partial class NotifyCollectionChangedExt
    {
        public static IObservable<NotifyCollectionChangedEventArgs> ObserveCollectionChangedSlim(
            this INotifyCollectionChanged source, bool signalInitial)
        {
            var observable = Observable.Create<NotifyCollectionChangedEventArgs>(o =>
            {
                if (signalInitial)
                {
                    o.OnNext(Diff.NotifyCollectionResetEventArgs);
                }
                NotifyCollectionChangedEventHandler fsHandler = (_, e) =>
                {
                    o.OnNext(e); 
                };
                source.CollectionChanged += fsHandler;
                return Disposable.Create(() => source.CollectionChanged -= fsHandler);
            });
            return observable;
        } 

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
        /// <typeparam name="TProperty"></typeparam>
        /// <typeparam name="TCollection"></typeparam>
        /// <typeparam name="TItem"></typeparam>
        /// <param name="source"></param>
        /// <param name="property"></param>
        /// <param name="signalInitial"></param>
        /// <returns></returns>
        public static IObservable<EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>> ObserveItemPropertyChanged<TCollection, TItem, TProperty>(
            this TCollection source,
            Expression<Func<TItem, TProperty>> property,
            bool signalInitial = true)
            where TCollection : class, IEnumerable<TItem>, INotifyCollectionChanged
            where TItem : class, INotifyPropertyChanged
        {
            return new ItemsObservable<TCollection, TItem, TProperty>(source, property, signalInitial);
        }

        public static IObservable<EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>> ItemPropertyChanged<TCollection, TItem, TProperty>(
             this IObservable<EventPattern<PropertyChangedAndValueEventArgs<TCollection>>> source,
             Expression<Func<TItem, TProperty>> property)
            where TCollection : class, IEnumerable<TItem>, INotifyCollectionChanged
            where TItem : class, INotifyPropertyChanged
        {
            var observable = new ItemsObservable<TCollection, TItem, TProperty>(source, property);
            return observable;
        }
    }
}