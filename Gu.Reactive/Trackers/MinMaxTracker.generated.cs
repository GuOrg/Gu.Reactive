#nullable enable
namespace Gu.Reactive
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq.Expressions;

    /// <summary>
    /// Factory methods for creating trackers for max value.
    /// </summary>
    public static class MinMaxTracker
    {
        /// <summary>
        /// Creates a <see cref="MinMaxTracker{TValue}"/> for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TItem">The type of the items in <paramref name="source"/></typeparam>
        /// <typeparam name="TValue">The type of the max value.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="selector">The function used when producing a value from an item.</param>
        /// <returns>A tracker with Max synced with source.Max() and min with source.Max()</returns>
        public static MinMaxTracker<TValue> TrackMinMax<TItem, TValue>(this ObservableCollection<TItem> source, Expression<Func<TItem, TValue>> selector)
            where TItem : class, INotifyPropertyChanged
            where TValue : struct, IComparable<TValue>
        {
            return new MinMaxTracker<TValue>(new NestedChanges<ObservableCollection<TItem>, TItem, TValue>(source, selector));
        }

        /// <summary>
        /// Creates a <see cref="MinMaxTracker{TValue}"/> for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TValue">The type of the max value.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <returns>A tracker with Max synced with source.Max() and min with source.Max()</returns>
        public static MinMaxTracker<TValue> TrackMinMax<TValue>(this ObservableCollection<TValue> source)
            where TValue : struct, IComparable<TValue>
        {
            return new MinMaxTracker<TValue>(new SimpleChanges<ObservableCollection<TValue>, TValue>(source));
        }

        /// <summary>
        /// Creates a <see cref="MinMaxTracker{TValue}"/> for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TItem">The type of the items in <paramref name="source"/></typeparam>
        /// <typeparam name="TValue">The type of the max value.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="selector">The function used when producing a value from an item.</param>
        /// <returns>A tracker with Max synced with source.Max() and min with source.Max()</returns>
        public static MinMaxTracker<TValue> TrackMinMax<TItem, TValue>(this ReadOnlyObservableCollection<TItem> source, Expression<Func<TItem, TValue>> selector)
            where TItem : class, INotifyPropertyChanged
            where TValue : struct, IComparable<TValue>
        {
            return new MinMaxTracker<TValue>(new NestedChanges<ReadOnlyObservableCollection<TItem>, TItem, TValue>(source, selector));
        }

        /// <summary>
        /// Creates a <see cref="MinMaxTracker{TValue}"/> for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TValue">The type of the max value.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <returns>A tracker with Max synced with source.Max() and min with source.Max()</returns>
        public static MinMaxTracker<TValue> TrackMinMax<TValue>(this ReadOnlyObservableCollection<TValue> source)
            where TValue : struct, IComparable<TValue>
        {
            return new MinMaxTracker<TValue>(new SimpleChanges<ReadOnlyObservableCollection<TValue>, TValue>(source));
        }

        /// <summary>
        /// Creates a <see cref="MinMaxTracker{TValue}"/> for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TItem">The type of the items in <paramref name="source"/></typeparam>
        /// <typeparam name="TValue">The type of the max value.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="selector">The function used when producing a value from an item.</param>
        /// <returns>A tracker with Max synced with source.Max() and min with source.Max()</returns>
        public static MinMaxTracker<TValue> TrackMinMax<TItem, TValue>(this IReadOnlyObservableCollection<TItem> source, Expression<Func<TItem, TValue>> selector)
            where TItem : class, INotifyPropertyChanged
            where TValue : struct, IComparable<TValue>
        {
            return new MinMaxTracker<TValue>(new NestedChanges<IReadOnlyObservableCollection<TItem>, TItem, TValue>(source, selector));
        }

        /// <summary>
        /// Creates a <see cref="MinMaxTracker{TValue}"/> for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TValue">The type of the max value.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <returns>A tracker with Max synced with source.Max() and min with source.Max()</returns>
        public static MinMaxTracker<TValue> TrackMinMax<TValue>(this IReadOnlyObservableCollection<TValue> source)
            where TValue : struct, IComparable<TValue>
        {
            return new MinMaxTracker<TValue>(new SimpleChanges<IReadOnlyObservableCollection<TValue>, TValue>(source));
        }
    }
}