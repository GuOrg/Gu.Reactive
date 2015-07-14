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
    using System.Reactive.Linq;

    using Gu.Reactive.Internals;

    public static partial class NotifyCollectionChangedExt
    {
        /// <summary>
        /// Observes propertychanges for items in the collection.
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <typeparam name="TItem">The type of the items in the collection</typeparam>
        /// <param name="source"></param>
        /// <param name="property">Sample: item => item.SomeProp.SomeNestedProp</param>
        /// <param name="signalInitial">When true a reset is singaled on subscribe.</param>
        /// <returns></returns>
        public static IObservable<EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>> ObserveItemPropertyChanged<TItem, TProperty>(
            this ObservableCollection<TItem> source,
            Expression<Func<TItem, TProperty>> property,
            bool signalInitial = true)
            where TItem : class, INotifyPropertyChanged
        {
            return new ItemsObservable<ObservableCollection<TItem>, TItem, TProperty>(source, property, signalInitial);
        }

        /// <summary>
        /// Used for chained subscriptions sample:
        /// source.ObservePropertyChangedWithValue(x => x.Collection, true)
        ///       .ItemPropertyChanged(x => x.Name)
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="source"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static IObservable<EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>> ItemPropertyChanged<TItem, TProperty>(
             this IObservable<EventPattern<PropertyChangedAndValueEventArgs<ObservableCollection<TItem>>>> source,
             Expression<Func<TItem, TProperty>> property)
             where TItem : class, INotifyPropertyChanged
        {
            var observable = new ItemsObservable<ObservableCollection<TItem>, TItem, TProperty>(source, property);
            return observable;
        }

        /// <summary>
        /// Observes propertychanges for items in the collection.
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <typeparam name="TItem">The type of the items in the collection</typeparam>
        /// <param name="source"></param>
        /// <param name="property">Sample: item => item.SomeProp.SomeNestedProp</param>
        /// <param name="signalInitial">When true a reset is singaled on subscribe.</param>
        /// <returns></returns>
        public static IObservable<EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>> ObserveItemPropertyChanged<TItem, TProperty>(
            this ReadOnlyObservableCollection<TItem> source,
            Expression<Func<TItem, TProperty>> property,
            bool signalInitial = true)
            where TItem : class, INotifyPropertyChanged
        {
            return new ItemsObservable<ReadOnlyObservableCollection<TItem>, TItem, TProperty>(source, property, signalInitial);
        }

        /// <summary>
        /// Used for chained subscriptions sample:
        /// source.ObservePropertyChangedWithValue(x => x.Collection, true)
        ///       .ItemPropertyChanged(x => x.Name)
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="source"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static IObservable<EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>> ItemPropertyChanged<TItem, TProperty>(
             this IObservable<EventPattern<PropertyChangedAndValueEventArgs<ReadOnlyObservableCollection<TItem>>>> source,
             Expression<Func<TItem, TProperty>> property)
             where TItem : class, INotifyPropertyChanged
        {
            var observable = new ItemsObservable<ReadOnlyObservableCollection<TItem>, TItem, TProperty>(source, property);
            return observable;
        }

        /// <summary>
        /// Observes propertychanges for items in the collection.
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <typeparam name="TItem">The type of the items in the collection</typeparam>
        /// <param name="source"></param>
        /// <param name="property">Sample: item => item.SomeProp.SomeNestedProp</param>
        /// <param name="signalInitial">When true a reset is singaled on subscribe.</param>
        /// <returns></returns>
        public static IObservable<EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>> ObserveItemPropertyChanged<TItem, TProperty>(
            this IReadOnlyObservableCollection<TItem> source,
            Expression<Func<TItem, TProperty>> property,
            bool signalInitial = true)
            where TItem : class, INotifyPropertyChanged
        {
            return new ItemsObservable<IReadOnlyObservableCollection<TItem>, TItem, TProperty>(source, property, signalInitial);
        }

        /// <summary>
        /// Used for chained subscriptions sample:
        /// source.ObservePropertyChangedWithValue(x => x.Collection, true)
        ///       .ItemPropertyChanged(x => x.Name)
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="source"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static IObservable<EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>> ItemPropertyChanged<TItem, TProperty>(
             this IObservable<EventPattern<PropertyChangedAndValueEventArgs<IReadOnlyObservableCollection<TItem>>>> source,
             Expression<Func<TItem, TProperty>> property)
             where TItem : class, INotifyPropertyChanged
        {
            var observable = new ItemsObservable<IReadOnlyObservableCollection<TItem>, TItem, TProperty>(source, property);
            return observable;
        }
    }
}