namespace Gu.Reactive
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq.Expressions;

    /// <summary>
    /// Factory methods for creating trackers for min value.
    /// </summary>
    public static class MinTracker
    {
        /// <summary>
        /// Creates a <see cref="MinTracker{TValue}"/> for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TItem">The type of the items in <paramref name="source"/></typeparam>
        /// <typeparam name="TValue">The type of the min value.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="selector">The function used when producing a value from an item.</param>
        /// <param name="whenEmpty">The value to return <paramref name="source"/> is empty.</param>
        /// <param name="trackItemChanges">If true we subscribe to property changes for each item. This is much slower.</param>
        /// <returns>A tracker with Value synced with source.Min()</returns>
        public static MinTracker<TValue> TrackMin<TItem, TValue>(this ObservableCollection<TItem> source, Expression<Func<TItem, TValue>> selector, TValue? whenEmpty, bool trackItemChanges)
            where TItem : class, INotifyPropertyChanged
            where TValue : struct, IComparable<TValue>
        {
            var onItemChanged = trackItemChanges
                                    ? source.ObserveItemPropertyChangedSlim(selector, false)
                                    : null;
            var mapped = source.AsMappingView(selector.Compile(), onItemChanged);
            return new MinTracker<TValue>(mapped, mapped.ObserveCollectionChangedSlim(false), whenEmpty);
        }

        /// <summary>
        /// Creates a <see cref="MinTracker{TValue}"/> for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TValue">The type of the min value.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="whenEmpty">The value to return <paramref name="source"/> is empty.</param>
        /// <returns>A tracker with Value synced with source.Min()</returns>
        public static MinTracker<TValue> TrackMin<TValue>(this ObservableCollection<TValue> source, TValue? whenEmpty)
            where TValue : struct, IComparable<TValue>
        {
            return new MinTracker<TValue>(source, source.ObserveCollectionChangedSlim(false), whenEmpty);
        }

        /// <summary>
        /// Creates a <see cref="MinTracker{TValue}"/> for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TItem">The type of the items in <paramref name="source"/></typeparam>
        /// <typeparam name="TValue">The type of the min value.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="selector">The function used when producing a value from an item.</param>
        /// <param name="whenEmpty">The value to return <paramref name="source"/> is empty.</param>
        /// <param name="trackItemChanges">If true we subscribe to property changes for each item. This is much slower.</param>
        /// <returns>A tracker with Value synced with source.Min()</returns>
        public static MinTracker<TValue> TrackMin<TItem, TValue>(this ReadOnlyObservableCollection<TItem> source, Expression<Func<TItem, TValue>> selector, TValue? whenEmpty, bool trackItemChanges)
            where TItem : class, INotifyPropertyChanged
            where TValue : struct, IComparable<TValue>
        {
            var onItemChanged = trackItemChanges
                                    ? source.ObserveItemPropertyChangedSlim(selector, false)
                                    : null;
            var mapped = source.AsMappingView(selector.Compile(), onItemChanged);
            return new MinTracker<TValue>(mapped, mapped.ObserveCollectionChangedSlim(false), whenEmpty);
        }

        /// <summary>
        /// Creates a <see cref="MinTracker{TValue}"/> for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TValue">The type of the min value.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="whenEmpty">The value to return <paramref name="source"/> is empty.</param>
        /// <returns>A tracker with Value synced with source.Min()</returns>
        public static MinTracker<TValue> TrackMin<TValue>(this ReadOnlyObservableCollection<TValue> source, TValue? whenEmpty)
            where TValue : struct, IComparable<TValue>
        {
            return new MinTracker<TValue>(source, source.ObserveCollectionChangedSlim(false), whenEmpty);
        }

        /// <summary>
        /// Creates a <see cref="MinTracker{TValue}"/> for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TItem">The type of the items in <paramref name="source"/></typeparam>
        /// <typeparam name="TValue">The type of the min value.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="selector">The function used when producing a value from an item.</param>
        /// <param name="whenEmpty">The value to return <paramref name="source"/> is empty.</param>
        /// <param name="trackItemChanges">If true we subscribe to property changes for each item. This is much slower.</param>
        /// <returns>A tracker with Value synced with source.Min()</returns>
        public static MinTracker<TValue> TrackMin<TItem, TValue>(this IReadOnlyObservableCollection<TItem> source, Expression<Func<TItem, TValue>> selector, TValue? whenEmpty, bool trackItemChanges)
            where TItem : class, INotifyPropertyChanged
            where TValue : struct, IComparable<TValue>
        {
            var onItemChanged = trackItemChanges
                                    ? source.ObserveItemPropertyChangedSlim(selector, false)
                                    : null;
            var mapped = source.AsMappingView(selector.Compile(), onItemChanged);
            return new MinTracker<TValue>(mapped, mapped.ObserveCollectionChangedSlim(false), whenEmpty);
        }

        /// <summary>
        /// Creates a <see cref="MinTracker{TValue}"/> for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TValue">The type of the min value.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="whenEmpty">The value to return <paramref name="source"/> is empty.</param>
        /// <returns>A tracker with Value synced with source.Min()</returns>
        public static MinTracker<TValue> TrackMin<TValue>(this IReadOnlyObservableCollection<TValue> source, TValue? whenEmpty)
            where TValue : struct, IComparable<TValue>
        {
            return new MinTracker<TValue>(source, source.ObserveCollectionChangedSlim(false), whenEmpty);
        }
    }
}