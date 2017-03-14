namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Concurrency;
    using Gu.Reactive.Internals;

    /// <summary>
    /// Factory methods for creating <see cref="FilteredView{T}"/>
    /// </summary>
    public static partial class FilteredView
    {
        /// <summary>
        /// Create a <see cref="FilteredView{T}"/> view for <paramref name="collection"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <param name="collection">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        /// <param name="trigger">Trigger for when filtering is updated.</param>
        /// <param name="triggers">Triggers for when filtering is updated.</param>
        /// <returns>A <see cref="FilteredView{T}"/></returns>
        public static FilteredView<T> AsFilteredView<T>(
            this IList<T> collection,
            Func<T, bool> filter,
            IScheduler scheduler,
            IObservable<object> trigger,
            params IObservable<object>[] triggers)
        {
            Ensure.NotNull(collection, nameof(collection));
            Ensure.NotNull(filter, nameof(filter));
            Ensure.NotNull(scheduler, nameof(scheduler));
            Ensure.NotNull(trigger, nameof(trigger));
            return new FilteredView<T>(collection, filter, TimeSpan.Zero, scheduler, triggers.Prepend(trigger));
        }

        /// <summary>
        /// Create a <see cref="FilteredView{T}"/> view for <paramref name="collection"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <param name="collection">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="trigger">Trigger for when filtering is updated.</param>
        /// <param name="triggers">Triggers for when filtering is updated.</param>
        /// <returns>A <see cref="FilteredView{T}"/></returns>
        public static FilteredView<T> AsFilteredView<T>(
            this IList<T> collection,
            Func<T, bool> filter,
            IObservable<object> trigger,
            params IObservable<object>[] triggers)
        {
            Ensure.NotNull(collection, nameof(collection));
            Ensure.NotNull(filter, nameof(filter));
            Ensure.NotNull(trigger, nameof(trigger));
            return new FilteredView<T>(
                collection,
                filter,
                TimeSpan.Zero,
                null,
                triggers.Prepend(trigger));
        }

        /// <summary>
        /// Create a <see cref="FilteredView{T}"/> view for <paramref name="collection"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <param name="collection">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="bufferTime">The time to buffer changes in <paramref name="collection"/></param>
        /// <param name="trigger">Trigger for when filtering is updated.</param>
        /// <param name="triggers">Triggers for when filtering is updated.</param>
        /// <returns>A <see cref="FilteredView{T}"/></returns>
        public static FilteredView<T> AsFilteredView<T>(
            this IList<T> collection,
            Func<T, bool> filter,
            TimeSpan bufferTime,
            IObservable<object> trigger,
            params IObservable<object>[] triggers)
        {
            Ensure.NotNull(collection, nameof(collection));
            Ensure.NotNull(filter, nameof(filter));
            Ensure.NotNull(trigger, nameof(trigger));
            return new FilteredView<T>(collection, filter, bufferTime, null, triggers.Prepend(trigger));
        }

        /// <summary>
        /// Create a <see cref="FilteredView{T}"/> view for <paramref name="collection"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <param name="collection">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="bufferTime">The time to buffer changes in <paramref name="collection"/></param>
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        /// <param name="trigger">Trigger for when filtering is updated.</param>
        /// <param name="triggers">Triggers for when filtering is updated.</param>
        /// <returns>A <see cref="FilteredView{T}"/></returns>
        public static FilteredView<T> AsFilteredView<T>(
            this IList<T> collection,
            Func<T, bool> filter,
            TimeSpan bufferTime,
            IScheduler scheduler,
            IObservable<object> trigger,
            params IObservable<object>[] triggers)
        {
            Ensure.NotNull(collection, nameof(collection));
            Ensure.NotNull(filter, nameof(filter));
            Ensure.NotNull(scheduler, nameof(scheduler));
            Ensure.NotNull(trigger, nameof(trigger));
            return new FilteredView<T>(collection, filter, bufferTime, scheduler, triggers.Prepend(trigger));
        }

        private static IObservable<object>[] Prepend(this IObservable<object>[] items, IObservable<object> item)
        {
            if (items == null || items.Length == 0)
            {
                return new[] { item };
            }

            var result = new IObservable<object>[items.Length + 1];
            items[0] = item;
            Array.Copy(items, 0, result, 1, items.Length);
            return result;
        }
    }
}