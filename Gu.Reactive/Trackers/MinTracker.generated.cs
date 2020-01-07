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
        /// <returns>A tracker with Value synced with source.Min()</returns>
        public static MinTracker<TValue> TrackMin<TItem, TValue>(this ObservableCollection<TItem> source, Expression<Func<TItem, TValue>> selector)
            where TItem : class, INotifyPropertyChanged
            where TValue : struct, IComparable<TValue>
        {
            return new MinTracker<TValue>(new NestedChanges<ObservableCollection<TItem>, TItem, TValue>(source, selector));
        }

        /// <summary>
        /// Creates a <see cref="MinTracker{TValue}"/> for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TValue">The type of the min value.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <returns>A tracker with Value synced with source.Min()</returns>
        public static MinTracker<TValue> TrackMin<TValue>(this ObservableCollection<TValue> source)
            where TValue : struct, IComparable<TValue>
        {
            return new MinTracker<TValue>(new SimpleChanges<ObservableCollection<TValue>, TValue>(source));
        }

        /// <summary>
        /// Creates a <see cref="MinTracker{TValue}"/> for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TItem">The type of the items in <paramref name="source"/></typeparam>
        /// <typeparam name="TValue">The type of the min value.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="selector">The function used when producing a value from an item.</param>
        /// <returns>A tracker with Value synced with source.Min()</returns>
        public static MinTracker<TValue> TrackMin<TItem, TValue>(this ReadOnlyObservableCollection<TItem> source, Expression<Func<TItem, TValue>> selector)
            where TItem : class, INotifyPropertyChanged
            where TValue : struct, IComparable<TValue>
        {
            return new MinTracker<TValue>(new NestedChanges<ReadOnlyObservableCollection<TItem>, TItem, TValue>(source, selector));
        }

        /// <summary>
        /// Creates a <see cref="MinTracker{TValue}"/> for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TValue">The type of the min value.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <returns>A tracker with Value synced with source.Min()</returns>
        public static MinTracker<TValue> TrackMin<TValue>(this ReadOnlyObservableCollection<TValue> source)
            where TValue : struct, IComparable<TValue>
        {
            return new MinTracker<TValue>(new SimpleChanges<ReadOnlyObservableCollection<TValue>, TValue>(source));
        }

        /// <summary>
        /// Creates a <see cref="MinTracker{TValue}"/> for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TItem">The type of the items in <paramref name="source"/></typeparam>
        /// <typeparam name="TValue">The type of the min value.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="selector">The function used when producing a value from an item.</param>
        /// <returns>A tracker with Value synced with source.Min()</returns>
        public static MinTracker<TValue> TrackMin<TItem, TValue>(this IReadOnlyObservableCollection<TItem> source, Expression<Func<TItem, TValue>> selector)
            where TItem : class, INotifyPropertyChanged
            where TValue : struct, IComparable<TValue>
        {
            return new MinTracker<TValue>(new NestedChanges<IReadOnlyObservableCollection<TItem>, TItem, TValue>(source, selector));
        }

        /// <summary>
        /// Creates a <see cref="MinTracker{TValue}"/> for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TValue">The type of the min value.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <returns>A tracker with Value synced with source.Min()</returns>
        public static MinTracker<TValue> TrackMin<TValue>(this IReadOnlyObservableCollection<TValue> source)
            where TValue : struct, IComparable<TValue>
        {
            return new MinTracker<TValue>(new SimpleChanges<IReadOnlyObservableCollection<TValue>, TValue>(source));
        }
    }
}