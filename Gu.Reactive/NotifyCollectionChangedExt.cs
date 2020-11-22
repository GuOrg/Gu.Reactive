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
        /// Observes collection changed events for <paramref name="source"/>.
        /// </summary>
        /// <param name="source">The source collection.</param>
        /// <param name="signalInitial">When true a reset is signaled on subscribe.</param>
        public static IObservable<NotifyCollectionChangedEventArgs> ObserveCollectionChangedSlim(this INotifyCollectionChanged source, bool signalInitial)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (signalInitial)
            {
                return Observable.Return(CachedEventArgs.NotifyCollectionReset)
                                 .Concat(source.ObserveCollectionChangedSlim(signalInitial: false));
            }

            return Observable.Create<NotifyCollectionChangedEventArgs>(o =>
            {
                source.CollectionChanged += Handler;
                return Disposable.Create(() => source.CollectionChanged -= Handler);

                void Handler(object _, NotifyCollectionChangedEventArgs e)
                {
                    o.OnNext(e);
                }
            });
        }

        /// <summary>
        /// Observes collection changed events for <paramref name="source"/>.
        /// </summary>
        /// <typeparam name="TCollection">The source collection type.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="signalInitial">When true a reset is signaled on subscribe.</param>
        public static IObservable<EventPattern<NotifyCollectionChangedEventArgs>> ObserveCollectionChanged<TCollection>(this TCollection source, bool signalInitial = true)
            where TCollection : IEnumerable, INotifyCollectionChanged
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (signalInitial)
            {
                return Observable.Return(
                                     new EventPattern<NotifyCollectionChangedEventArgs>(
                                         source,
                                         CachedEventArgs.NotifyCollectionReset))
                                 .Concat(source.ObserveCollectionChanged(signalInitial: false));
            }

            return Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                x => source.CollectionChanged += x,
                x => source.CollectionChanged -= x);
        }

        /// <summary>
        /// Observes property changes for items of the collection.
        /// </summary>
        /// <typeparam name="TCollection">The source collection type.</typeparam>
        /// <typeparam name="TItem">The type of the items in the collection.</typeparam>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="property">The expression specifying the property path.</param>
        /// <param name="signalInitial">When true a reset is signaled on subscribe.</param>
        public static IObservable<EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>> ObserveItemPropertyChanged<TCollection, TItem, TProperty>(
                TCollection source,
                Expression<Func<TItem, TProperty>> property,
                bool signalInitial = true)
            where TCollection : class, IEnumerable<TItem>, INotifyCollectionChanged
            where TItem : class?, INotifyPropertyChanged?
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (property is null)
            {
                throw new ArgumentNullException(nameof(property));
            }

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
        /// Observes property changes for items of the collection.
        /// </summary>
        /// <typeparam name="TCollection">The source collection type.</typeparam>
        /// <typeparam name="TItem">The type of the items in the collection.</typeparam>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="property">The expression specifying the property path.</param>
        /// <param name="signalInitial">When true a reset is signaled on subscribe.</param>
        public static IObservable<PropertyChangedEventArgs> ObserveItemPropertyChangedSlim<TCollection, TItem, TProperty>(
                TCollection source,
                Expression<Func<TItem, TProperty>> property,
                bool signalInitial = true)
            where TCollection : class, IEnumerable<TItem>, INotifyCollectionChanged
            where TItem : class?, INotifyPropertyChanged?
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (property is null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            return Observable.Create<PropertyChangedEventArgs>(
                o => source.ObserveItemPropertyChangedCore(
                    o,
                    property,
                    signalInitial,
                    (item, sender, args, sourceAndValue) => args));
        }

        /// <summary>
        /// Observes property changes for items of the collection.
        /// </summary>
        /// <typeparam name="TCollection">The source collection type.</typeparam>
        /// <typeparam name="TItem">The type of the items in the collection.</typeparam>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="property">The expression specifying the property path.</param>
        public static IObservable<EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>> ItemPropertyChanged<TCollection, TItem, TProperty>(
                this IObservable<EventPattern<PropertyChangedAndValueEventArgs<TCollection>>> source,
                Expression<Func<TItem, TProperty>> property)
            where TCollection : class, IEnumerable<TItem>, INotifyCollectionChanged
            where TItem : class?, INotifyPropertyChanged?
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (property is null)
            {
                throw new ArgumentNullException(nameof(property));
            }

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
        /// Observes property changes for items of the collection.
        /// </summary>
        /// <typeparam name="TCollection">The source collection type.</typeparam>
        /// <typeparam name="TItem">The type of the items in the collection.</typeparam>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="property">The expression specifying the property path.</param>
        public static IObservable<EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>> ItemPropertyChanged<TCollection, TItem, TProperty>(
                this IObservable<TCollection> source,
                Expression<Func<TItem, TProperty>> property)
            where TCollection : class, IEnumerable<TItem>, INotifyCollectionChanged
            where TItem : class?, INotifyPropertyChanged?
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (property is null)
            {
                throw new ArgumentNullException(nameof(property));
            }

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
        /// Observes property changes for items of the collection.
        /// </summary>
        /// <typeparam name="TCollection">The source collection type.</typeparam>
        /// <typeparam name="TItem">The type of the items in the collection.</typeparam>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="property">The expression specifying the property path.</param>
        public static IObservable<PropertyChangedEventArgs> ItemPropertyChangedSlim<TCollection, TItem, TProperty>(
                this IObservable<TCollection> source,
                Expression<Func<TItem, TProperty>> property)
            where TCollection : class, IEnumerable<TItem>, INotifyCollectionChanged
            where TItem : class?, INotifyPropertyChanged?
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (property is null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            return Observable.Create<PropertyChangedEventArgs>(
                o => source.ItemPropertyChangedCore(
                    o,
                    property,
                    (item, sender, args, sourceAndValue) => args));
        }

        /// <summary>
        /// Observes collection changed events for <paramref name="source"/>.
        /// </summary>
        internal static IObservable<NotifyCollectionChangedEventArgs> ObserveCollectionChangedSlimOrDefault(
            this IEnumerable source,
            bool signalInitial)
        {
            if (source is INotifyCollectionChanged notifyingSource)
            {
                return notifyingSource.ObserveCollectionChangedSlim(signalInitial);
            }

            return signalInitial
                ? Observable.Return(CachedEventArgs.NotifyCollectionReset)
                : Observable.Never<NotifyCollectionChangedEventArgs>();
        }

        private static IDisposable ItemPropertyChangedCore<TCollection, TItem, TProperty, T>(
            this IObservable<EventPattern<PropertyChangedAndValueEventArgs<TCollection>>> source,
            IObserver<T> observer,
            Expression<Func<TItem, TProperty>> property,
            Func<TItem, object?, PropertyChangedEventArgs, SourceAndValue<INotifyPropertyChanged?, TProperty>, T> create)
            where TCollection : class, IEnumerable<TItem>, INotifyCollectionChanged
            where TItem : class?, INotifyPropertyChanged?
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
            Func<TItem, object?, PropertyChangedEventArgs, SourceAndValue<INotifyPropertyChanged?, TProperty>, T> create)
            where TCollection : class, IEnumerable<TItem>, INotifyCollectionChanged
            where TItem : class?, INotifyPropertyChanged?
        {
            var tracker = ItemsTracker.Create((TCollection?)null, NotifyingPath.GetOrCreate(property));
            tracker.TrackedItemChanged += Handler;
            var subscription = source.Subscribe(x => tracker.UpdateSource(x));
            return new CompositeDisposable(3)
            {
                Disposable.Create(() => tracker.TrackedItemChanged -= Handler),
                tracker,
                subscription,
            };

            void Handler(TItem item, object? sender, PropertyChangedEventArgs args, SourceAndValue<INotifyPropertyChanged?, TProperty> sourceAndValue)
            {
                observer.OnNext(create(item, sender, args, sourceAndValue));
            }
        }

        private static IDisposable ObserveItemPropertyChangedCore<TCollection, TItem, TProperty, T>(
            this TCollection source,
            IObserver<T> o,
            Expression<Func<TItem, TProperty>> property,
            bool signalInitial,
            Func<TItem, object?, PropertyChangedEventArgs, SourceAndValue<INotifyPropertyChanged?, TProperty>, T> create)
            where TCollection : class, IEnumerable<TItem>, INotifyCollectionChanged
            where TItem : class?, INotifyPropertyChanged?
        {
            var tracker = ItemsTracker.Create(
                signalInitial
                    ? null
                    : source,
                NotifyingPath.GetOrCreate(property));

            tracker.TrackedItemChanged += Handler;
            if (signalInitial)
            {
                tracker.UpdateSource(source);
            }

            return new CompositeDisposable(2)
            {
                Disposable.Create(() => tracker.TrackedItemChanged -= Handler),
                tracker,
            };

            void Handler(TItem item, object? sender, PropertyChangedEventArgs args, SourceAndValue<INotifyPropertyChanged?, TProperty> sourceAndValue)
            {
                o.OnNext(
                    create(
                        item,
                        sender,
                        args,
                        sourceAndValue));
            }
        }
    }
}
