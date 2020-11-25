namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Reactive;
    using System.Reactive.Linq;

    /// <summary>
    /// Obsolete overloads.
    /// </summary>
    public static partial class NotifyCollectionChangedExt
    {
        /// <summary>
        /// Observes property changes for items of the collection.
        /// </summary>
        /// <typeparam name="TCollection">The source collection type.</typeparam>
        /// <typeparam name="TItem">The type of the items in the collection.</typeparam>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="property">The expression specifying the property path.</param>
        /// <returns>An <see cref="IObservable{T}"/>.</returns>
        [Obsolete("Removing this as it is messy and hard to use. Use ObserveValue instead.")]
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
        /// Used for chained subscriptions sample:
        /// source.ObservePropertyChangedWithValue(x => x.Collection, true)
        ///       .ItemPropertyChanged(x => x.Name).
        /// </summary>
        /// <typeparam name="TItem">The type of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="source">The source instance.</param>
        /// <param name="property">An expression with the property path.</param>
        /// <returns>An observable that notifies when the property changes.</returns>
        [Obsolete("Removing this as it is messy and hard to use. Use ObserveValue instead.")]
        public static IObservable<EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>> ItemPropertyChanged<TItem, TProperty>(
            this IObservable<EventPattern<PropertyChangedAndValueEventArgs<ObservableCollection<TItem>>>> source,
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
        /// source.ObservePropertyChangedWithValue(x => x.Collection, true)
        ///       .ItemPropertyChanged(x => x.Name).
        /// </summary>
        /// <typeparam name="TItem">The type of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="source">The source instance.</param>
        /// <param name="property">An expression with the property path.</param>
        /// <returns>An observable that notifies when the property changes.</returns>
        [Obsolete("Removing this as it is messy and hard to use. Use ObserveValue instead.")]
        public static IObservable<EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>> ItemPropertyChanged<TItem, TProperty>(
            this IObservable<EventPattern<PropertyChangedAndValueEventArgs<ReadOnlyObservableCollection<TItem>>>> source,
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
        /// source.ObservePropertyChangedWithValue(x => x.Collection, true)
        ///       .ItemPropertyChanged(x => x.Name).
        /// </summary>
        /// <typeparam name="TItem">The type of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="source">The source instance.</param>
        /// <param name="property">An expression with the property path.</param>
        /// <returns>An observable that notifies when the property changes.</returns>
        [Obsolete("Removing this as it is messy and hard to use. Use ObserveValue instead.")]
        public static IObservable<EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>> ItemPropertyChanged<TItem, TProperty>(
            this IObservable<EventPattern<PropertyChangedAndValueEventArgs<IReadOnlyObservableCollection<TItem>>>> source,
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

        [Obsolete("Removing this as it is messy and hard to use. Use ObserveValue instead.")]
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
    }
}
