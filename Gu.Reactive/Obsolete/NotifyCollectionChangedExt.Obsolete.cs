namespace Gu.Reactive
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Reactive;

    public static partial class NotifyCollectionChangedExt
    {

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
    }
}
