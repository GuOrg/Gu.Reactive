// ReSharper disable PossibleMultipleEnumeration
namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Concurrency;

    using Gu.Reactive.Internals;

    /// <summary>
    /// Factory methods fro creating <see cref="ReadOnlyFilteredView{T}"/>
    /// </summary>
    public static partial class ReadOnlyFilteredView
    {
        /// <summary>
        /// Create a filtered view for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="triggers">Triggers for when filtering is updated.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/></returns>
        public static ReadOnlyFilteredView<T> Create<T>(
            IEnumerable<T> source,
            Func<T, bool> filter,
            params IObservable<object>[] triggers)
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(filter, nameof(filter));
            return new ReadOnlyFilteredView<T>(source, filter, TimeSpan.Zero, null, true, triggers);
        }

        /// <summary>
        /// Create a filtered view for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="leaveOpen">True means that the <paramref name="source"/> is not disposed when this instance is diposed.</param>
        /// <param name="triggers">Triggers for when filtering is updated.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/></returns>
        public static ReadOnlyFilteredView<T> Create<T>(
            IEnumerable<T> source,
            Func<T, bool> filter,
            bool leaveOpen,
            params IObservable<object>[] triggers)
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(filter, nameof(filter));
            return new ReadOnlyFilteredView<T>(source, filter, TimeSpan.Zero, null, leaveOpen, triggers);
        }

        /// <summary>
        /// Create a <see cref="ReadOnlyFilteredView{T}"/> view for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        /// <param name="leaveOpen">True means that the <paramref name="source"/> is not disposed when this instance is diposed.</param>
        /// <param name="triggers">Triggers for when filtering is updated.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/></returns>
        public static ReadOnlyFilteredView<T> Create<T>(
            IEnumerable<T> source,
            Func<T, bool> filter,
            IScheduler scheduler,
            bool leaveOpen,
            params IObservable<object>[] triggers)
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(filter, nameof(filter));
            Ensure.NotNull(scheduler, nameof(scheduler));
            return new ReadOnlyFilteredView<T>(source, filter, TimeSpan.Zero, scheduler, leaveOpen, triggers);
        }

        /// <summary>
        /// Create a <see cref="ReadOnlyFilteredView{T}"/> view for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="bufferTime">The time to buffer changes in <paramref name="source"/></param>
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        /// <param name="leaveOpen">True means that the <paramref name="source"/> is not disposed when this instance is diposed.</param>
        /// <param name="triggers">Triggers for when filtering is updated.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/></returns>
        public static ReadOnlyFilteredView<T> Create<T>(
            IEnumerable<T> source,
            Func<T, bool> filter,
            TimeSpan bufferTime,
            IScheduler scheduler,
            bool leaveOpen,
            params IObservable<object>[] triggers)
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(filter, nameof(filter));
            Ensure.NotNull(scheduler, nameof(scheduler));
            return new ReadOnlyFilteredView<T>(source, filter, bufferTime, scheduler, leaveOpen, triggers);
        }
    }
}