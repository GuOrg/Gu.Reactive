namespace Gu.Reactive
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Reactive;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;

    using Gu.Reactive.Internals;

    /// <summary>
    /// Extension methods for subscribing to collection changes.
    /// </summary>
    public static partial class NotifyCollectionChangedExt
    {
        /// <summary>
        /// Observes collectionchanged events for <paramref name="source"/>.
        /// </summary>
        public static IObservable<NotifyCollectionChangedEventArgs> ObserveCollectionChangedSlim(this INotifyCollectionChanged source, bool signalInitial)
        {
            if (signalInitial)
            {
                return Observable.Return(CachedEventArgs.NotifyCollectionReset)
                                 .Concat(source.ObserveCollectionChangedSlim(false));
            }

            return Observable.Create<NotifyCollectionChangedEventArgs>(o =>
            {
                NotifyCollectionChangedEventHandler handler = (_, e) => o.OnNext(e);
                source.CollectionChanged += handler;
                return Disposable.Create(() => source.CollectionChanged -= handler);
            });
        }

        /// <summary>
        /// Observes collectionchanged events for <paramref name="source"/>.
        /// </summary>
        public static IObservable<EventPattern<NotifyCollectionChangedEventArgs>> ObserveCollectionChanged<TCollection>(
            this TCollection source,
            bool signalInitial = true)
            where TCollection : IEnumerable, INotifyCollectionChanged
        {
            if (signalInitial)
            {
                return Observable.Return(
                                     new EventPattern<NotifyCollectionChangedEventArgs>(
                                         source,
                                         CachedEventArgs.NotifyCollectionReset))
                                 .Concat(source.ObserveCollectionChanged(false));
            }

            return Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                x => source.CollectionChanged += x,
                x => source.CollectionChanged -= x);
        }

        /// <summary>
        /// Observes propertychanges for items of the collection.
        /// </summary>
        public static IObservable<EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>> ObserveItemPropertyChanged<TCollection, TItem, TProperty>(
            this TCollection source,
            Expression<Func<TItem, TProperty>> property,
            bool signalInitial = true)
            where TCollection : class, IEnumerable<TItem>, INotifyCollectionChanged
            where TItem : class, INotifyPropertyChanged
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(property, nameof(property));

            return Observable.Create<EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>>(
                    o =>
                        {
                            var tracker = new ItemsTracker<TCollection, TItem, TProperty>(
                               signalInitial ? null : source,
                                NotifyingPath.GetOrCreate(property));
                            TrackedPropertyChangedEventHandler handler = (propertyTracker, sender, args, sourceAndValue) =>
                            {
                                o.OnNext(
                                    new EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>(
                                        sender,
                                        new ItemPropertyChangedEventArgs<TItem, TProperty>(
                                            (TItem)propertyTracker.PathTracker.First.Source,
                                            sourceAndValue,
                                            args.PropertyName)));
                            };
                            tracker.TrackedItemChanged += handler;
                            if (signalInitial)
                            {
                                tracker.UpdateSource(source);
                            }

                            return new CompositeDisposable(2)
                                       {
                                           tracker,
                                           Disposable.Create(() => tracker.TrackedItemChanged -= handler)
                                       };
                        });
        }

        /// <summary>
        /// Observes propertychanges for items of the collection.
        /// </summary>
        public static IObservable<EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>> ItemPropertyChanged<TCollection, TItem, TProperty>(
             this IObservable<EventPattern<PropertyChangedAndValueEventArgs<TCollection>>> source,
             Expression<Func<TItem, TProperty>> property)
            where TCollection : class, IEnumerable<TItem>, INotifyCollectionChanged
            where TItem : class, INotifyPropertyChanged
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(property, nameof(property));

            return Observable.Create<EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>>(
                o =>
                    {
                        var tracker = new ItemsTracker<TCollection, TItem, TProperty>(null, NotifyingPath.GetOrCreate(property));
                        TrackedPropertyChangedEventHandler handler = (propertyTracker, sender, args, sourceAndValue) =>
                        {
                            o.OnNext(
                                new EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>(
                                    sender,
                                    new ItemPropertyChangedEventArgs<TItem, TProperty>(
                                        (TItem)propertyTracker.PathTracker.First.Source,
                                        sourceAndValue,
                                        args.PropertyName)));
                        };
                        tracker.TrackedItemChanged += handler;
                        var subscription = source.Subscribe(x => tracker.UpdateSource(x.EventArgs.Value));
                        return new CompositeDisposable(3)
                                   {
                                       tracker,
                                       subscription,
                                       Disposable.Create(() => tracker.TrackedItemChanged -= handler)
                                   };
                    });
        }
    }
}