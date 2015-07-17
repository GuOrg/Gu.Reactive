namespace Gu.Reactive
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;

    public static partial class MaxTracker
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <param name="whenEmpty"></param>
        /// <param name="trackItemChanges">If true we subscribe to property changes for each item. This is much slower.</param>
        /// <returns>A tracker with Value synced with source.Max()</returns>
        public static MaxTracker<TValue> TrackMax<TItem, TValue>(this ObservableCollection<TItem> source, Expression<Func<TItem, TValue>> selector, TValue? whenEmpty, bool trackItemChanges)
            where TItem : class, INotifyPropertyChanged
            where TValue : struct, IComparable<TValue>
        {
            var onItemChanged = trackItemChanges
                                    ? source.ObserveItemPropertyChanged(selector, false)
                                    : null;
            var mapped = source.AsMappingView(selector.Compile(), onItemChanged);
            return new MaxTracker<TValue>(mapped, mapped.ObserveCollectionChangedSlim(false), whenEmpty);
        }

        public static MaxTracker<TValue> TrackMax<TValue>(this ObservableCollection<TValue> source, TValue? whenEmpty)
            where TValue : struct, IComparable<TValue>
        {
            return new MaxTracker<TValue>(source, source.ObserveCollectionChangedSlim(false), whenEmpty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <param name="whenEmpty"></param>
        /// <param name="trackItemChanges">If true we subscribe to property changes for each item. This is much slower.</param>
        /// <returns>A tracker with Value synced with source.Max()</returns>
        public static MaxTracker<TValue> TrackMax<TItem, TValue>(this ReadOnlyObservableCollection<TItem> source, Expression<Func<TItem, TValue>> selector, TValue? whenEmpty, bool trackItemChanges)
            where TItem : class, INotifyPropertyChanged
            where TValue : struct, IComparable<TValue>
        {
            var onItemChanged = trackItemChanges
                                    ? source.ObserveItemPropertyChanged(selector, false)
                                    : null;
            var mapped = source.AsMappingView(selector.Compile(), onItemChanged);
            return new MaxTracker<TValue>(mapped, mapped.ObserveCollectionChangedSlim(false), whenEmpty);
        }

        public static MaxTracker<TValue> TrackMax<TValue>(this ReadOnlyObservableCollection<TValue> source, TValue? whenEmpty)
            where TValue : struct, IComparable<TValue>
        {
            return new MaxTracker<TValue>(source, source.ObserveCollectionChangedSlim(false), whenEmpty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <param name="whenEmpty"></param>
        /// <param name="trackItemChanges">If true we subscribe to property changes for each item. This is much slower.</param>
        /// <returns>A tracker with Value synced with source.Max()</returns>
        public static MaxTracker<TValue> TrackMax<TItem, TValue>(this IReadOnlyObservableCollection<TItem> source, Expression<Func<TItem, TValue>> selector, TValue? whenEmpty, bool trackItemChanges)
            where TItem : class, INotifyPropertyChanged
            where TValue : struct, IComparable<TValue>
        {
            var onItemChanged = trackItemChanges
                                    ? source.ObserveItemPropertyChanged(selector, false)
                                    : null;
            var mapped = source.AsMappingView(selector.Compile(), onItemChanged);
            return new MaxTracker<TValue>(mapped, mapped.ObserveCollectionChangedSlim(false), whenEmpty);
        }

        public static MaxTracker<TValue> TrackMax<TValue>(this IReadOnlyObservableCollection<TValue> source, TValue? whenEmpty)
            where TValue : struct, IComparable<TValue>
        {
            return new MaxTracker<TValue>(source, source.ObserveCollectionChangedSlim(false), whenEmpty);
        }
    }
}