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
        /// Observes propertychanges for items in the collection.
        /// </summary>
        /// <typeparam name="TItem">The type of the items in the collection</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="source">The sopurce item to track changes for.</param>
        /// <param name="property">Sample: item => item.SomeProp.SomeNestedProp</param>
        /// <param name="signalInitial">When true a reset is singaled on subscribe.</param>
        public static IObservable<EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>> ObserveItemPropertyChanged<TItem, TProperty>(
            this ObservableCollection<TItem> source,
            Expression<Func<TItem, TProperty>> property,
            bool signalInitial = true)
            where TItem : class, INotifyPropertyChanged
        {
            return Observable.Create<EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>>(
                    o =>
                        new ItemsTracker<ObservableCollection<TItem>, TItem, TProperty>(
                            source,
                            NotifyingPath.GetOrCreate(property),
                            o,
                            signalInitial));
        }

        /// <summary>
        /// Used for chained subscriptions sample:
        /// source.ObservePropertyChangedWithValue(x => x.Collection, true)
        ///       .ItemPropertyChanged(x => x.Name)
        /// </summary>
        /// <typeparam name="TItem">The type of <paramref name="source"/></typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="source">The source instance.</param>
        /// <param name="property">An expression with the property path.</param>
        /// <returns>An observable that notifies when the property changes.</returns>
        public static IObservable<EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>> ItemPropertyChanged<TItem, TProperty>(
             this IObservable<EventPattern<PropertyChangedAndValueEventArgs<ObservableCollection<TItem>>>> source,
             Expression<Func<TItem, TProperty>> property)
             where TItem : class, INotifyPropertyChanged
        {
            return Observable.Create<EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>>(
                o =>
                    {
                        var tracker = new ItemsTracker<ObservableCollection<TItem>, TItem, TProperty>(
                            null,
                            NotifyingPath.GetOrCreate(property),
                            o,
                            false);
                        var subscription = source.Subscribe(x => tracker.UpdateSource(x.EventArgs.Value));
                        return new CompositeDisposable(2) { tracker, subscription };
                    });
        }

        /// <summary>
        /// Observes propertychanges for items in the collection.
        /// </summary>
        /// <typeparam name="TItem">The type of the items in the collection</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="source">The sopurce item to track changes for.</param>
        /// <param name="property">Sample: item => item.SomeProp.SomeNestedProp</param>
        /// <param name="signalInitial">When true a reset is singaled on subscribe.</param>
        public static IObservable<EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>> ObserveItemPropertyChanged<TItem, TProperty>(
            this ReadOnlyObservableCollection<TItem> source,
            Expression<Func<TItem, TProperty>> property,
            bool signalInitial = true)
            where TItem : class, INotifyPropertyChanged
        {
            return Observable.Create<EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>>(
                    o =>
                        new ItemsTracker<ReadOnlyObservableCollection<TItem>, TItem, TProperty>(
                            source,
                            NotifyingPath.GetOrCreate(property),
                            o,
                            signalInitial));
        }

        /// <summary>
        /// Used for chained subscriptions sample:
        /// source.ObservePropertyChangedWithValue(x => x.Collection, true)
        ///       .ItemPropertyChanged(x => x.Name)
        /// </summary>
        /// <typeparam name="TItem">The type of <paramref name="source"/></typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="source">The source instance.</param>
        /// <param name="property">An expression with the property path.</param>
        /// <returns>An observable that notifies when the property changes.</returns>
        public static IObservable<EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>> ItemPropertyChanged<TItem, TProperty>(
             this IObservable<EventPattern<PropertyChangedAndValueEventArgs<ReadOnlyObservableCollection<TItem>>>> source,
             Expression<Func<TItem, TProperty>> property)
             where TItem : class, INotifyPropertyChanged
        {
            return Observable.Create<EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>>(
                o =>
                    {
                        var tracker = new ItemsTracker<ReadOnlyObservableCollection<TItem>, TItem, TProperty>(
                            null,
                            NotifyingPath.GetOrCreate(property),
                            o,
                            false);
                        var subscription = source.Subscribe(x => tracker.UpdateSource(x.EventArgs.Value));
                        return new CompositeDisposable(2) { tracker, subscription };
                    });
        }

        /// <summary>
        /// Observes propertychanges for items in the collection.
        /// </summary>
        /// <typeparam name="TItem">The type of the items in the collection</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="source">The sopurce item to track changes for.</param>
        /// <param name="property">Sample: item => item.SomeProp.SomeNestedProp</param>
        /// <param name="signalInitial">When true a reset is singaled on subscribe.</param>
        public static IObservable<EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>> ObserveItemPropertyChanged<TItem, TProperty>(
            this IReadOnlyObservableCollection<TItem> source,
            Expression<Func<TItem, TProperty>> property,
            bool signalInitial = true)
            where TItem : class, INotifyPropertyChanged
        {
            return Observable.Create<EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>>(
                    o =>
                        new ItemsTracker<IReadOnlyObservableCollection<TItem>, TItem, TProperty>(
                            source,
                            NotifyingPath.GetOrCreate(property),
                            o,
                            signalInitial));
        }

        /// <summary>
        /// Used for chained subscriptions sample:
        /// source.ObservePropertyChangedWithValue(x => x.Collection, true)
        ///       .ItemPropertyChanged(x => x.Name)
        /// </summary>
        /// <typeparam name="TItem">The type of <paramref name="source"/></typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="source">The source instance.</param>
        /// <param name="property">An expression with the property path.</param>
        /// <returns>An observable that notifies when the property changes.</returns>
        public static IObservable<EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>> ItemPropertyChanged<TItem, TProperty>(
             this IObservable<EventPattern<PropertyChangedAndValueEventArgs<IReadOnlyObservableCollection<TItem>>>> source,
             Expression<Func<TItem, TProperty>> property)
             where TItem : class, INotifyPropertyChanged
        {
            return Observable.Create<EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>>(
                o =>
                    {
                        var tracker = new ItemsTracker<IReadOnlyObservableCollection<TItem>, TItem, TProperty>(
                            null,
                            NotifyingPath.GetOrCreate(property),
                            o,
                            false);
                        var subscription = source.Subscribe(x => tracker.UpdateSource(x.EventArgs.Value));
                        return new CompositeDisposable(2) { tracker, subscription };
                    });
        }
    }
}