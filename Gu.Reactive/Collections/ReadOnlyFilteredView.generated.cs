namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Reactive.Concurrency;
    using Gu.Reactive.Internals;
    using Gu.Reactive.Internals.Ensure;

    /// <summary>
    /// Factory methods for creating <see cref="ReadOnlyFilteredView{T}"/>
    /// </summary>
    public static partial class ReadOnlyFilteredView
    {
        /// <summary>
        /// Create a filtered view for <paramref name="collection"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <param name="collection">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="triggers">Triggers for when filtering is updated.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/></returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this ObservableCollection<T> collection,
            Func<T, bool> filter,
            params IObservable<object>[] triggers)
        {
            Ensure.NotNull(collection, nameof(collection));
            return new ReadOnlyFilteredView<T>(collection, filter, TimeSpan.Zero, null, triggers);
        }

        /// <summary>
        /// Create a <see cref="ReadOnlyFilteredView{T}"/> view for <paramref name="collection"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <param name="collection">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="bufferTime">The time to buffer changes in <paramref name="collection"/></param>
        /// <param name="triggers">Triggers for when filtering is updated.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/></returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this ObservableCollection<T> collection,
            Func<T, bool> filter,
            TimeSpan bufferTime,
            params IObservable<object>[] triggers)
        {
            Ensure.NotNull(collection, nameof(collection));
            Ensure.NotNull(filter, nameof(filter));
            return new ReadOnlyFilteredView<T>(collection, filter, bufferTime, null, triggers);
        }

        /// <summary>
        /// Create a <see cref="ReadOnlyFilteredView{T}"/> view for <paramref name="collection"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <param name="collection">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        /// <param name="triggers">Triggers for when filtering is updated.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/></returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this ObservableCollection<T> collection,
            Func<T, bool> filter,
            IScheduler scheduler,
            params IObservable<object>[] triggers)
        {
            Ensure.NotNull(collection, nameof(collection));
            Ensure.NotNull(filter, nameof(filter));
            Ensure.NotNull(scheduler, nameof(scheduler));
            return new ReadOnlyFilteredView<T>(collection, filter, TimeSpan.Zero, scheduler, triggers);
        }

        /// <summary>
        /// Create a <see cref="ReadOnlyFilteredView{T}"/> view for <paramref name="collection"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <param name="collection">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="bufferTime">The time to buffer changes in <paramref name="collection"/></param>
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        /// <param name="triggers">Triggers for when filtering is updated.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/></returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this ObservableCollection<T> collection,
            Func<T, bool> filter,
            TimeSpan bufferTime,
            IScheduler scheduler,
            params IObservable<object>[] triggers)
        {
            Ensure.NotNull(collection, nameof(collection));
            Ensure.NotNull(filter, nameof(filter));
            Ensure.NotNull(scheduler, nameof(scheduler));
            return new ReadOnlyFilteredView<T>(collection, filter, bufferTime, scheduler, triggers);
        }

        /// <summary>
        /// Create a filtered view for <paramref name="collection"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <param name="collection">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="triggers">Triggers for when filtering is updated.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/></returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this ReadOnlyObservableCollection<T> collection,
            Func<T, bool> filter,
            params IObservable<object>[] triggers)
        {
            Ensure.NotNull(collection, nameof(collection));
            return new ReadOnlyFilteredView<T>(collection, filter, TimeSpan.Zero, null, triggers);
        }

        /// <summary>
        /// Create a <see cref="ReadOnlyFilteredView{T}"/> view for <paramref name="collection"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <param name="collection">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="bufferTime">The time to buffer changes in <paramref name="collection"/></param>
        /// <param name="triggers">Triggers for when filtering is updated.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/></returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this ReadOnlyObservableCollection<T> collection,
            Func<T, bool> filter,
            TimeSpan bufferTime,
            params IObservable<object>[] triggers)
        {
            Ensure.NotNull(collection, nameof(collection));
            Ensure.NotNull(filter, nameof(filter));
            return new ReadOnlyFilteredView<T>(collection, filter, bufferTime, null, triggers);
        }

        /// <summary>
        /// Create a <see cref="ReadOnlyFilteredView{T}"/> view for <paramref name="collection"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <param name="collection">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        /// <param name="triggers">Triggers for when filtering is updated.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/></returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this ReadOnlyObservableCollection<T> collection,
            Func<T, bool> filter,
            IScheduler scheduler,
            params IObservable<object>[] triggers)
        {
            Ensure.NotNull(collection, nameof(collection));
            Ensure.NotNull(filter, nameof(filter));
            Ensure.NotNull(scheduler, nameof(scheduler));
            return new ReadOnlyFilteredView<T>(collection, filter, TimeSpan.Zero, scheduler, triggers);
        }

        /// <summary>
        /// Create a <see cref="ReadOnlyFilteredView{T}"/> view for <paramref name="collection"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <param name="collection">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="bufferTime">The time to buffer changes in <paramref name="collection"/></param>
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        /// <param name="triggers">Triggers for when filtering is updated.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/></returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this ReadOnlyObservableCollection<T> collection,
            Func<T, bool> filter,
            TimeSpan bufferTime,
            IScheduler scheduler,
            params IObservable<object>[] triggers)
        {
            Ensure.NotNull(collection, nameof(collection));
            Ensure.NotNull(filter, nameof(filter));
            Ensure.NotNull(scheduler, nameof(scheduler));
            return new ReadOnlyFilteredView<T>(collection, filter, bufferTime, scheduler, triggers);
        }

        /// <summary>
        /// Create a filtered view for <paramref name="collection"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <param name="collection">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="triggers">Triggers for when filtering is updated.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/></returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this IReadOnlyObservableCollection<T> collection,
            Func<T, bool> filter,
            params IObservable<object>[] triggers)
        {
            Ensure.NotNull(collection, nameof(collection));
            return new ReadOnlyFilteredView<T>(collection, filter, TimeSpan.Zero, null, triggers);
        }

        /// <summary>
        /// Create a <see cref="ReadOnlyFilteredView{T}"/> view for <paramref name="collection"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <param name="collection">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="bufferTime">The time to buffer changes in <paramref name="collection"/></param>
        /// <param name="triggers">Triggers for when filtering is updated.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/></returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this IReadOnlyObservableCollection<T> collection,
            Func<T, bool> filter,
            TimeSpan bufferTime,
            params IObservable<object>[] triggers)
        {
            Ensure.NotNull(collection, nameof(collection));
            Ensure.NotNull(filter, nameof(filter));
            return new ReadOnlyFilteredView<T>(collection, filter, bufferTime, null, triggers);
        }

        /// <summary>
        /// Create a <see cref="ReadOnlyFilteredView{T}"/> view for <paramref name="collection"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <param name="collection">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        /// <param name="triggers">Triggers for when filtering is updated.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/></returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this IReadOnlyObservableCollection<T> collection,
            Func<T, bool> filter,
            IScheduler scheduler,
            params IObservable<object>[] triggers)
        {
            Ensure.NotNull(collection, nameof(collection));
            Ensure.NotNull(filter, nameof(filter));
            Ensure.NotNull(scheduler, nameof(scheduler));
            return new ReadOnlyFilteredView<T>(collection, filter, TimeSpan.Zero, scheduler, triggers);
        }

        /// <summary>
        /// Create a <see cref="ReadOnlyFilteredView{T}"/> view for <paramref name="collection"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <param name="collection">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="bufferTime">The time to buffer changes in <paramref name="collection"/></param>
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        /// <param name="triggers">Triggers for when filtering is updated.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/></returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this IReadOnlyObservableCollection<T> collection,
            Func<T, bool> filter,
            TimeSpan bufferTime,
            IScheduler scheduler,
            params IObservable<object>[] triggers)
        {
            Ensure.NotNull(collection, nameof(collection));
            Ensure.NotNull(filter, nameof(filter));
            Ensure.NotNull(scheduler, nameof(scheduler));
            return new ReadOnlyFilteredView<T>(collection, filter, bufferTime, scheduler, triggers);
        }

        /// <summary>
        /// Create a filtered view for <paramref name="collection"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <param name="collection">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="triggers">Trigger for when filtering is updated.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/></returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this IEnumerable<T> collection,
            Func<T, bool> filter,
            IEnumerable<IObservable<object>> triggers)
        {
            Ensure.NotNull(collection, nameof(collection));
            Ensure.NotNull(filter, nameof(filter));
            Ensure.NotNull(triggers, "triggers");
            return new ReadOnlyFilteredView<T>(collection, filter, TimeSpan.Zero, null, triggers);
        }

        /// <summary>
        /// Create a filtered view for <paramref name="collection"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <param name="collection">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="bufferTime">The time to buffer changes in <paramref name="collection"/></param>
        /// <param name="triggers">Trigger for when filtering is updated.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/></returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this IEnumerable<T> collection,
            Func<T, bool> filter,
            TimeSpan bufferTime,
            IEnumerable<IObservable<object>> triggers)
        {
            Ensure.NotNull(collection, nameof(collection));
            Ensure.NotNull(filter, nameof(filter));
            Ensure.NotNull(triggers, "triggers");
            return new ReadOnlyFilteredView<T>(collection, filter, bufferTime, null, triggers);
        }

        /// <summary>
        /// Create a filtered view for <paramref name="collection"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <param name="collection">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        /// <param name="triggers">Trigger for when filtering is updated.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/></returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this IEnumerable<T> collection,
            Func<T, bool> filter,
            IScheduler scheduler,
            IEnumerable<IObservable<object>> triggers)
        {
            Ensure.NotNull(collection, nameof(collection));
            Ensure.NotNull(filter, nameof(filter));
            Ensure.NotNull(scheduler, nameof(scheduler));
            Ensure.NotNull(triggers, "triggers");
            return new ReadOnlyFilteredView<T>(collection, filter, TimeSpan.Zero, scheduler, triggers);
        }

        /// <summary>
        /// Create a filtered view for <paramref name="collection"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <param name="collection">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="bufferTime">The time to buffer changes in <paramref name="collection"/></param>
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        /// <param name="triggers">Trigger for when filtering is updated.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/></returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this IEnumerable<T> collection,
            Func<T, bool> filter,
            TimeSpan bufferTime,
            IScheduler scheduler,
            IEnumerable<IObservable<object>> triggers)
        {
            Ensure.NotNull(collection, nameof(collection));
            Ensure.NotNull(filter, nameof(filter));
            Ensure.NotNull(scheduler, nameof(scheduler));
            Ensure.NotNull(triggers, "triggers");
            return new ReadOnlyFilteredView<T>(collection, filter, bufferTime, scheduler, triggers);
        }

        /// <summary>
        /// Create a filtered view for <paramref name="collection"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <param name="collection">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="trigger">Trigger for when filtering is updated.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/></returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this IEnumerable<T> collection,
            Func<T, bool> filter,
            IObservable<object> trigger)
        {
            Ensure.NotNull(collection, nameof(collection));
            Ensure.NotNull(filter, nameof(filter));
            Ensure.NotNull(trigger, "trigger");
            return new ReadOnlyFilteredView<T>(collection, filter, TimeSpan.Zero, null, trigger);
        }

        /// <summary>
        /// Create a filtered view for <paramref name="collection"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <param name="collection">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="bufferTime">The time to buffer changes in <paramref name="collection"/></param>
        /// <param name="trigger">Trigger for when filtering is updated.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/></returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this IEnumerable<T> collection,
            Func<T, bool> filter,
            TimeSpan bufferTime,
            IObservable<object> trigger)
        {
            Ensure.NotNull(collection, nameof(collection));
            Ensure.NotNull(filter, nameof(filter));
            Ensure.NotNull(trigger, "trigger");
            return new ReadOnlyFilteredView<T>(collection, filter, bufferTime, null, trigger);
        }

        /// <summary>
        /// Create a filtered view for <paramref name="collection"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <param name="collection">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        /// <param name="trigger">Trigger for when filtering is updated.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/></returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this IEnumerable<T> collection,
            Func<T, bool> filter,
            IScheduler scheduler,
            IObservable<object> trigger)
        {
            Ensure.NotNull(collection, nameof(collection));
            Ensure.NotNull(filter, nameof(filter));
            Ensure.NotNull(scheduler, nameof(scheduler));
            Ensure.NotNull(trigger, "trigger");
            return new ReadOnlyFilteredView<T>(collection, filter, TimeSpan.Zero, scheduler, trigger);
        }

        /// <summary>
        /// Create a filtered view for <paramref name="collection"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <param name="collection">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="bufferTime">The time to buffer changes in <paramref name="collection"/></param>
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        /// <param name="trigger">Trigger for when filtering is updated.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/></returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this IEnumerable<T> collection,
            Func<T, bool> filter,
            TimeSpan bufferTime,
            IScheduler scheduler,
            IObservable<object> trigger)
        {
            Ensure.NotNull(collection, nameof(collection));
            Ensure.NotNull(filter, nameof(filter));
            Ensure.NotNull(scheduler, nameof(scheduler));
            Ensure.NotNull(trigger, "trigger");
            return new ReadOnlyFilteredView<T>(collection, filter, bufferTime, scheduler, trigger);
        }
    }
}