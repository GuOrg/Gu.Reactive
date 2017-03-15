namespace Gu.Reactive
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Reactive.Linq;

    /// <summary>
    /// Factory methods for creating trackers for max value.
    /// </summary>
    public static class MaxTracker
    {
        /// <summary>
        /// Creates a <see cref="MaxTracker{TValue}"/> for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TItem">The type of the items in <paramref name="source"/></typeparam>
        /// <typeparam name="TValue">The type of the max value.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="selector">The function used when producing a value from an item.</param>
        /// <returns>A tracker with Value synced with source.Max()</returns>
        public static MaxTracker<TValue> TrackMax<TItem, TValue>(this ObservableCollection<TItem> source, Expression<Func<TItem, TValue>> selector)
            where TItem : class, INotifyPropertyChanged
            where TValue : struct, IComparable<TValue>
        {
            return new MaxTracker<TValue>(new NestedChanges<ObservableCollection<TItem>, TItem, TValue>(source, selector));
        }

        /// <summary>
        /// Creates a <see cref="MaxTracker{TValue}"/> for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TValue">The type of the max value.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <returns>A tracker with Value synced with source.Max()</returns>
        public static MaxTracker<TValue> TrackMax<TValue>(this ObservableCollection<TValue> source)
            where TValue : struct, IComparable<TValue>
        {
            return new MaxTracker<TValue>(new SimpleChanges<ObservableCollection<TValue>, TValue>(source));
        }

        /// <summary>
        /// Creates a <see cref="MaxTracker{TValue}"/> for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TItem">The type of the items in <paramref name="source"/></typeparam>
        /// <typeparam name="TValue">The type of the max value.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="selector">The function used when producing a value from an item.</param>
        /// <returns>A tracker with Value synced with source.Max()</returns>
        public static MaxTracker<TValue> TrackMax<TItem, TValue>(this ReadOnlyObservableCollection<TItem> source, Expression<Func<TItem, TValue>> selector)
            where TItem : class, INotifyPropertyChanged
            where TValue : struct, IComparable<TValue>
        {
            return new MaxTracker<TValue>(new NestedChanges<ReadOnlyObservableCollection<TItem>, TItem, TValue>(source, selector));
        }

        /// <summary>
        /// Creates a <see cref="MaxTracker{TValue}"/> for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TValue">The type of the max value.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <returns>A tracker with Value synced with source.Max()</returns>
        public static MaxTracker<TValue> TrackMax<TValue>(this ReadOnlyObservableCollection<TValue> source)
            where TValue : struct, IComparable<TValue>
        {
            return new MaxTracker<TValue>(new SimpleChanges<ReadOnlyObservableCollection<TValue>, TValue>(source));
        }

        /// <summary>
        /// Creates a <see cref="MaxTracker{TValue}"/> for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TItem">The type of the items in <paramref name="source"/></typeparam>
        /// <typeparam name="TValue">The type of the max value.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="selector">The function used when producing a value from an item.</param>
        /// <returns>A tracker with Value synced with source.Max()</returns>
        public static MaxTracker<TValue> TrackMax<TItem, TValue>(this IReadOnlyObservableCollection<TItem> source, Expression<Func<TItem, TValue>> selector)
            where TItem : class, INotifyPropertyChanged
            where TValue : struct, IComparable<TValue>
        {
            return new MaxTracker<TValue>(new NestedChanges<IReadOnlyObservableCollection<TItem>, TItem, TValue>(source, selector));
        }

        /// <summary>
        /// Creates a <see cref="MaxTracker{TValue}"/> for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TValue">The type of the max value.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <returns>A tracker with Value synced with source.Max()</returns>
        public static MaxTracker<TValue> TrackMax<TValue>(this IReadOnlyObservableCollection<TValue> source)
            where TValue : struct, IComparable<TValue>
        {
            return new MaxTracker<TValue>(new SimpleChanges<IReadOnlyObservableCollection<TValue>, TValue>(source));
        }
    }
}