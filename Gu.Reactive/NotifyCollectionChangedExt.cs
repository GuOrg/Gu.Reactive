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
        public static IObservable<NotifyCollectionChangedEventArgs> ObserveCollectionChangedSlim(
            this INotifyCollectionChanged source,
            bool signalInitial)
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
                TCollection source,
                Expression<Func<TItem, TProperty>> property,
                bool signalInitial = true)
            where TCollection : class, IEnumerable<TItem>, INotifyCollectionChanged
            where TItem : class, INotifyPropertyChanged
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(property, nameof(property));

            return Observable.Create<EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>>(
                o => source.ObserveItemPropertyChangedCore(
                    o,
                    property,
                    signalInitial,
                    (item, sender, args, sourceAndValue) =>
                        new EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>(
                            sender,
                            new ItemPropertyChangedEventArgs<TItem, TProperty>(
                                item,
                                sourceAndValue,
                                args.PropertyName))));
        }

        /// <summary>
        /// Observes propertychanges for items of the collection.
        /// </summary>
        public static IObservable<PropertyChangedEventArgs> ObserveItemPropertyChangedSlim<TCollection, TItem, TProperty>(
                TCollection source,
                Expression<Func<TItem, TProperty>> property,
                bool signalInitial = true)
            where TCollection : class, IEnumerable<TItem>, INotifyCollectionChanged
            where TItem : class, INotifyPropertyChanged
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(property, nameof(property));

            return Observable.Create<PropertyChangedEventArgs>(
                o => source.ObserveItemPropertyChangedCore(
                    o,
                    property,
                    signalInitial,
                    (item, sender, args, sourceAndValue) => args));
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
                o => source.ItemPropertyChangedCore(
                    o,
                    property,
                    (item, sender, args, sourceAndValue) =>
                        new EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>(
                            sender,
                            new ItemPropertyChangedEventArgs<TItem, TProperty>(
                                item,
                                sourceAndValue,
                                args.PropertyName))));
        }

        /// <summary>
        /// Observes propertychanges for items of the collection.
        /// </summary>
        public static IObservable<EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>> ItemPropertyChanged<TCollection, TItem, TProperty>(
                this IObservable<TCollection> source,
                Expression<Func<TItem, TProperty>> property)
            where TCollection : class, IEnumerable<TItem>, INotifyCollectionChanged
            where TItem : class, INotifyPropertyChanged
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(property, nameof(property));

            return Observable.Create<EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>>(
                o => source.ItemPropertyChangedCore(
                    o,
                    property,
                    (item, sender, args, sourceAndValue) =>
                        new EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>(
                            sender,
                            new ItemPropertyChangedEventArgs<TItem, TProperty>(
                                item,
                                sourceAndValue,
                                args.PropertyName))));
        }

        /// <summary>
        /// Observes propertychanges for items of the collection.
        /// </summary>
        public static IObservable<PropertyChangedEventArgs> ItemPropertyChangedSlim<TCollection, TItem, TProperty>(
                this IObservable<TCollection> source,
                Expression<Func<TItem, TProperty>> property)
            where TCollection : class, IEnumerable<TItem>, INotifyCollectionChanged
            where TItem : class, INotifyPropertyChanged
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(property, nameof(property));

            return Observable.Create<PropertyChangedEventArgs>(
                o => source.ItemPropertyChangedCore(
                    o,
                    property,
                    (item, sender, args, sourceAndValue) => args));
        }

        /// <summary>
        /// Observes collectionchanged events for <paramref name="source"/>.
        /// </summary>
        internal static IObservable<NotifyCollectionChangedEventArgs> ObserveCollectionChangedSlimOrDefault(
            this IEnumerable source,
            bool signalInitial)
        {
            var notifyingSource = source as INotifyCollectionChanged;
            if (notifyingSource == null)
            {
                return signalInitial
                           ? Observable.Return(CachedEventArgs.NotifyCollectionReset)
                           : Observable.Never<NotifyCollectionChangedEventArgs>();
            }

            return notifyingSource.ObserveCollectionChangedSlim(signalInitial);
        }

        private static IDisposable ItemPropertyChangedCore<TCollection, TItem, TProperty, T>(
            this IObservable<EventPattern<PropertyChangedAndValueEventArgs<TCollection>>> source,
            IObserver<T> observer,
            Expression<Func<TItem, TProperty>> property,
            Func<TItem, object, PropertyChangedEventArgs, SourceAndValue<INotifyPropertyChanged, TProperty>, T> create)
            where TCollection : class, IEnumerable<TItem>, INotifyCollectionChanged
            where TItem : class, INotifyPropertyChanged
        {
            return source.Select(x => x.EventArgs.Value)
                     .ItemPropertyChangedCore(
                         observer,
                         property,
                         create);
        }

        private static IDisposable ItemPropertyChangedCore<TCollection, TItem, TProperty, T>(
            this IObservable<TCollection> source,
            IObserver<T> observer,
            Expression<Func<TItem, TProperty>> property,
            Func<TItem, object, PropertyChangedEventArgs, SourceAndValue<INotifyPropertyChanged, TProperty>, T> create)
            where TCollection : class, IEnumerable<TItem>, INotifyCollectionChanged
            where TItem : class, INotifyPropertyChanged
        {
            var tracker = ItemsTracker.Create((TCollection)null, NotifyingPath.GetOrCreate(property));
            TrackedItemPropertyChangedEventHandler<TItem, TProperty> handler =
                (item, sender, args, sourceAndValue) => observer.OnNext(create(item, sender, args, sourceAndValue));
            tracker.TrackedItemChanged += handler;
            var subscription = source.Subscribe(x => tracker.UpdateSource(x));
            return new CompositeDisposable(3)
            {
                Disposable.Create(() => tracker.TrackedItemChanged -= handler),
                tracker,
                subscription,
            };
        }

        private static IDisposable ObserveItemPropertyChangedCore<TCollection, TItem, TProperty, T>(
            this TCollection source,
            IObserver<T> o,
            Expression<Func<TItem, TProperty>> property,
            bool signalInitial,
            Func<TItem, object, PropertyChangedEventArgs, SourceAndValue<INotifyPropertyChanged, TProperty>, T> create)
            where TCollection : class, IEnumerable<TItem>, INotifyCollectionChanged
            where TItem : class, INotifyPropertyChanged
        {
            var tracker = ItemsTracker.Create(
                signalInitial
                    ? null
                    : source,
                NotifyingPath.GetOrCreate(property));

            TrackedItemPropertyChangedEventHandler<TItem, TProperty> handler =
                (item, sender, args, sourceAndValue) => o.OnNext(
                    create(
                        item,
                        sender,
                        args,
                        sourceAndValue));

            tracker.TrackedItemChanged += handler;
            if (signalInitial)
            {
                tracker.UpdateSource(source);
            }

            return new CompositeDisposable(2)
            {
                Disposable.Create(() => tracker.TrackedItemChanged -= handler),
                tracker
            };
        }
    }
}