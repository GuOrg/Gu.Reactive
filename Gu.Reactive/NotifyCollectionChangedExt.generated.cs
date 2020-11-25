#nullable enable
#pragma warning disable RS0026 // Do not add multiple public overloads with optional parameters
#pragma warning disable RS0027 // Public API with optional parameter(s) should have the most parameters amongst its public overloads
namespace Gu.Reactive
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq.Expressions;

    using System.Reactive;
	using System.Reactive.Disposables;
    using System.Reactive.Linq;

    using Gu.Reactive.Internals;

    /// <summary>
    /// Factory methods for creating observables from notifying collections.
    /// </summary>
    public static partial class NotifyCollectionChangedExt
    {
        /// <summary>
        /// Observes property changes for items in the collection.
        /// </summary>
        /// <typeparam name="TItem">The type of the items in the collection.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="source">The source item to track changes for.</param>
        /// <param name="property">Sample: item => item.SomeProp.SomeNestedProp</param>
        /// <param name="signalInitial">When true a reset is signaled on subscribe.</param>
        public static IObservable<EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>> ObserveItemPropertyChanged<TItem, TProperty>(
            this ObservableCollection<TItem> source,
            Expression<Func<TItem, TProperty>> property,
            bool signalInitial = true)
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

            return Observable.Create<EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>>(o =>
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

                void Handler(TItem? item, object? sender, PropertyChangedEventArgs args, SourceAndValue<INotifyPropertyChanged?, TProperty> sourceAndValue)
                {
                    o.OnNext(
                        new EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>(
                            sender,
                            new ItemPropertyChangedEventArgs<TItem, TProperty>(
                                item,
                                sourceAndValue,
                                args.PropertyName)));
                }
            });
        }

        /// <summary>
        /// Observes property changes for items in the collection.
        /// </summary>
        /// <typeparam name="TItem">The type of the items in the collection.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="source">The source item to track changes for.</param>
        /// <param name="property">Sample: item => item.SomeProp.SomeNestedProp</param>
        /// <param name="signalInitial">When true a reset is signaled on subscribe.</param>
        public static IObservable<PropertyChangedEventArgs> ObserveItemPropertyChangedSlim<TItem, TProperty>(
            this ObservableCollection<TItem> source,
            Expression<Func<TItem, TProperty>> property,
            bool signalInitial = true)
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

            var observable = Observable.Create<PropertyChangedEventArgs>(o =>
            {
                var tracker = ItemsTrackerSlim.Create(source, NotifyingPath.GetOrCreate(property));
                tracker.ItemPropertyChanged += o.OnNext;
                return new CompositeDisposable(2)
                {
                    tracker,
                    Disposable.Create(() => tracker.ItemPropertyChanged -= o.OnNext)
                };
            });

            if (signalInitial)
            {
                return observable.StartWith(CachedEventArgs.StringEmpty);
            }

            return observable;
        }

        /// <summary>
        /// Used for chained subscriptions sample:
        /// source.ItemPropertyChanged(x => x.Name).
        /// </summary>
        /// <typeparam name="TItem">The type of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="source">The source instance.</param>
        /// <param name="property">An expression with the property path.</param>
        /// <returns>An observable that notifies when the property changes.</returns>
        public static IObservable<EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>> ItemPropertyChanged<TItem, TProperty>(
             this IObservable<ObservableCollection<TItem>> source,
             Expression<Func<TItem, TProperty>> property)
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

            return ItemPropertyChanged<ObservableCollection<TItem>, TItem, TProperty>(source, property);
        }

        /// <summary>
        /// Used for chained subscriptions sample:
        /// source.ItemPropertyChangedSlim(x => x.Name).
        /// </summary>
        /// <typeparam name="TItem">The type of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="source">The source instance.</param>
        /// <param name="property">An expression with the property path.</param>
        /// <returns>An observable that notifies when the property changes.</returns>
        public static IObservable<PropertyChangedEventArgs> ItemPropertyChangedSlim<TItem, TProperty>(
             this IObservable<ObservableCollection<TItem>> source,
             Expression<Func<TItem, TProperty>> property)
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

            return ItemPropertyChangedSlim<ObservableCollection<TItem>, TItem, TProperty>(source, property);
        }


        /// <summary>
        /// Used for chained subscriptions sample:
        /// source.ObserveValue(x => x.Collection, true)
        ///       .ItemPropertyChanged(x => x.Name).
        /// </summary>
        /// <typeparam name="TItem">The type of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="source">The source instance.</param>
        /// <param name="property">An expression with the property path.</param>
        /// <returns>An observable that notifies when the property changes.</returns>
        public static IObservable<EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>> ItemPropertyChanged<TItem, TProperty>(
             this IObservable<Maybe<ObservableCollection<TItem>>> source,
             Expression<Func<TItem, TProperty>> property)
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

            return ItemPropertyChanged<ObservableCollection<TItem>, TItem, TProperty>(source, property);
        }

        /// <summary>
        /// Used for chained subscriptions sample:
        /// source.ObserveValue(x => x.Collection, true)
        ///       .ItemPropertyChangedSlim(x => x.Name).
        /// </summary>
        /// <typeparam name="TItem">The type of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="source">The source instance.</param>
        /// <param name="property">An expression with the property path.</param>
        /// <returns>An observable that notifies when the property changes.</returns>
        public static IObservable<PropertyChangedEventArgs> ItemPropertyChangedSlim<TItem, TProperty>(
             this IObservable<Maybe<ObservableCollection<TItem>>> source,
             Expression<Func<TItem, TProperty>> property)
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

            return ItemPropertyChangedSlim<ObservableCollection<TItem>, TItem, TProperty>(source, property);
        }

        /// <summary>
        /// Observes property changes for items in the collection.
        /// </summary>
        /// <typeparam name="TItem">The type of the items in the collection.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="source">The source item to track changes for.</param>
        /// <param name="property">Sample: item => item.SomeProp.SomeNestedProp</param>
        /// <param name="signalInitial">When true a reset is signaled on subscribe.</param>
        public static IObservable<EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>> ObserveItemPropertyChanged<TItem, TProperty>(
            this ReadOnlyObservableCollection<TItem> source,
            Expression<Func<TItem, TProperty>> property,
            bool signalInitial = true)
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

            return Observable.Create<EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>>(o =>
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

                void Handler(TItem? item, object? sender, PropertyChangedEventArgs args, SourceAndValue<INotifyPropertyChanged?, TProperty> sourceAndValue)
                {
                    o.OnNext(
                        new EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>(
                            sender,
                            new ItemPropertyChangedEventArgs<TItem, TProperty>(
                                item,
                                sourceAndValue,
                                args.PropertyName)));
                }
            });
        }

        /// <summary>
        /// Observes property changes for items in the collection.
        /// </summary>
        /// <typeparam name="TItem">The type of the items in the collection.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="source">The source item to track changes for.</param>
        /// <param name="property">Sample: item => item.SomeProp.SomeNestedProp</param>
        /// <param name="signalInitial">When true a reset is signaled on subscribe.</param>
        public static IObservable<PropertyChangedEventArgs> ObserveItemPropertyChangedSlim<TItem, TProperty>(
            this ReadOnlyObservableCollection<TItem> source,
            Expression<Func<TItem, TProperty>> property,
            bool signalInitial = true)
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

            var observable = Observable.Create<PropertyChangedEventArgs>(o =>
            {
                var tracker = ItemsTrackerSlim.Create(source, NotifyingPath.GetOrCreate(property));
                tracker.ItemPropertyChanged += o.OnNext;
                return new CompositeDisposable(2)
                {
                    tracker,
                    Disposable.Create(() => tracker.ItemPropertyChanged -= o.OnNext)
                };
            });

            if (signalInitial)
            {
                return observable.StartWith(CachedEventArgs.StringEmpty);
            }

            return observable;
        }

        /// <summary>
        /// Used for chained subscriptions sample:
        /// source.ItemPropertyChanged(x => x.Name).
        /// </summary>
        /// <typeparam name="TItem">The type of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="source">The source instance.</param>
        /// <param name="property">An expression with the property path.</param>
        /// <returns>An observable that notifies when the property changes.</returns>
        public static IObservable<EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>> ItemPropertyChanged<TItem, TProperty>(
             this IObservable<ReadOnlyObservableCollection<TItem>> source,
             Expression<Func<TItem, TProperty>> property)
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

            return ItemPropertyChanged<ReadOnlyObservableCollection<TItem>, TItem, TProperty>(source, property);
        }

        /// <summary>
        /// Used for chained subscriptions sample:
        /// source.ItemPropertyChangedSlim(x => x.Name).
        /// </summary>
        /// <typeparam name="TItem">The type of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="source">The source instance.</param>
        /// <param name="property">An expression with the property path.</param>
        /// <returns>An observable that notifies when the property changes.</returns>
        public static IObservable<PropertyChangedEventArgs> ItemPropertyChangedSlim<TItem, TProperty>(
             this IObservable<ReadOnlyObservableCollection<TItem>> source,
             Expression<Func<TItem, TProperty>> property)
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

            return ItemPropertyChangedSlim<ReadOnlyObservableCollection<TItem>, TItem, TProperty>(source, property);
        }


        /// <summary>
        /// Used for chained subscriptions sample:
        /// source.ObserveValue(x => x.Collection, true)
        ///       .ItemPropertyChanged(x => x.Name).
        /// </summary>
        /// <typeparam name="TItem">The type of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="source">The source instance.</param>
        /// <param name="property">An expression with the property path.</param>
        /// <returns>An observable that notifies when the property changes.</returns>
        public static IObservable<EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>> ItemPropertyChanged<TItem, TProperty>(
             this IObservable<Maybe<ReadOnlyObservableCollection<TItem>>> source,
             Expression<Func<TItem, TProperty>> property)
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

            return ItemPropertyChanged<ReadOnlyObservableCollection<TItem>, TItem, TProperty>(source, property);
        }

        /// <summary>
        /// Used for chained subscriptions sample:
        /// source.ObserveValue(x => x.Collection, true)
        ///       .ItemPropertyChangedSlim(x => x.Name).
        /// </summary>
        /// <typeparam name="TItem">The type of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="source">The source instance.</param>
        /// <param name="property">An expression with the property path.</param>
        /// <returns>An observable that notifies when the property changes.</returns>
        public static IObservable<PropertyChangedEventArgs> ItemPropertyChangedSlim<TItem, TProperty>(
             this IObservable<Maybe<ReadOnlyObservableCollection<TItem>>> source,
             Expression<Func<TItem, TProperty>> property)
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

            return ItemPropertyChangedSlim<ReadOnlyObservableCollection<TItem>, TItem, TProperty>(source, property);
        }

        /// <summary>
        /// Observes property changes for items in the collection.
        /// </summary>
        /// <typeparam name="TItem">The type of the items in the collection.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="source">The source item to track changes for.</param>
        /// <param name="property">Sample: item => item.SomeProp.SomeNestedProp</param>
        /// <param name="signalInitial">When true a reset is signaled on subscribe.</param>
        public static IObservable<EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>> ObserveItemPropertyChanged<TItem, TProperty>(
            this IReadOnlyObservableCollection<TItem> source,
            Expression<Func<TItem, TProperty>> property,
            bool signalInitial = true)
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

            return Observable.Create<EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>>(o =>
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

                void Handler(TItem? item, object? sender, PropertyChangedEventArgs args, SourceAndValue<INotifyPropertyChanged?, TProperty> sourceAndValue)
                {
                    o.OnNext(
                        new EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>(
                            sender,
                            new ItemPropertyChangedEventArgs<TItem, TProperty>(
                                item,
                                sourceAndValue,
                                args.PropertyName)));
                }
            });
        }

        /// <summary>
        /// Observes property changes for items in the collection.
        /// </summary>
        /// <typeparam name="TItem">The type of the items in the collection.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="source">The source item to track changes for.</param>
        /// <param name="property">Sample: item => item.SomeProp.SomeNestedProp</param>
        /// <param name="signalInitial">When true a reset is signaled on subscribe.</param>
        public static IObservable<PropertyChangedEventArgs> ObserveItemPropertyChangedSlim<TItem, TProperty>(
            this IReadOnlyObservableCollection<TItem> source,
            Expression<Func<TItem, TProperty>> property,
            bool signalInitial = true)
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

            var observable = Observable.Create<PropertyChangedEventArgs>(o =>
            {
                var tracker = ItemsTrackerSlim.Create(source, NotifyingPath.GetOrCreate(property));
                tracker.ItemPropertyChanged += o.OnNext;
                return new CompositeDisposable(2)
                {
                    tracker,
                    Disposable.Create(() => tracker.ItemPropertyChanged -= o.OnNext)
                };
            });

            if (signalInitial)
            {
                return observable.StartWith(CachedEventArgs.StringEmpty);
            }

            return observable;
        }

        /// <summary>
        /// Used for chained subscriptions sample:
        /// source.ItemPropertyChanged(x => x.Name).
        /// </summary>
        /// <typeparam name="TItem">The type of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="source">The source instance.</param>
        /// <param name="property">An expression with the property path.</param>
        /// <returns>An observable that notifies when the property changes.</returns>
        public static IObservable<EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>> ItemPropertyChanged<TItem, TProperty>(
             this IObservable<IReadOnlyObservableCollection<TItem>> source,
             Expression<Func<TItem, TProperty>> property)
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

            return ItemPropertyChanged<IReadOnlyObservableCollection<TItem>, TItem, TProperty>(source, property);
        }

        /// <summary>
        /// Used for chained subscriptions sample:
        /// source.ItemPropertyChangedSlim(x => x.Name).
        /// </summary>
        /// <typeparam name="TItem">The type of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="source">The source instance.</param>
        /// <param name="property">An expression with the property path.</param>
        /// <returns>An observable that notifies when the property changes.</returns>
        public static IObservable<PropertyChangedEventArgs> ItemPropertyChangedSlim<TItem, TProperty>(
             this IObservable<IReadOnlyObservableCollection<TItem>> source,
             Expression<Func<TItem, TProperty>> property)
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

            return ItemPropertyChangedSlim<IReadOnlyObservableCollection<TItem>, TItem, TProperty>(source, property);
        }


        /// <summary>
        /// Used for chained subscriptions sample:
        /// source.ObserveValue(x => x.Collection, true)
        ///       .ItemPropertyChanged(x => x.Name).
        /// </summary>
        /// <typeparam name="TItem">The type of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="source">The source instance.</param>
        /// <param name="property">An expression with the property path.</param>
        /// <returns>An observable that notifies when the property changes.</returns>
        public static IObservable<EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>> ItemPropertyChanged<TItem, TProperty>(
             this IObservable<Maybe<IReadOnlyObservableCollection<TItem>>> source,
             Expression<Func<TItem, TProperty>> property)
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

            return ItemPropertyChanged<IReadOnlyObservableCollection<TItem>, TItem, TProperty>(source, property);
        }

        /// <summary>
        /// Used for chained subscriptions sample:
        /// source.ObserveValue(x => x.Collection, true)
        ///       .ItemPropertyChangedSlim(x => x.Name).
        /// </summary>
        /// <typeparam name="TItem">The type of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="source">The source instance.</param>
        /// <param name="property">An expression with the property path.</param>
        /// <returns>An observable that notifies when the property changes.</returns>
        public static IObservable<PropertyChangedEventArgs> ItemPropertyChangedSlim<TItem, TProperty>(
             this IObservable<Maybe<IReadOnlyObservableCollection<TItem>>> source,
             Expression<Func<TItem, TProperty>> property)
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

            return ItemPropertyChangedSlim<IReadOnlyObservableCollection<TItem>, TItem, TProperty>(source, property);
        }
    }
}