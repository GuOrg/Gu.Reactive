namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Concurrency;

    /// <summary>
    /// Factory methods fro creating <see cref="ReadOnlyFilteredView{T}"/>.
    /// </summary>
    public static partial class ReadOnlyFilteredView
    {
        /// <summary>
        /// Create a filtered view for <paramref name="source"/>.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source collection.</typeparam>
        /// <param name="source">The source observable.</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/>.</returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this IObservable<IEnumerable<T>> source,
            Func<T, bool> filter)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (filter is null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            return source.AsReadOnlyView()
                         .AsReadOnlyFilteredView(filter, leaveOpen: false);
        }

        /// <summary>
        /// Create a filtered view for <paramref name="source"/>.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source collection.</typeparam>
        /// <param name="source">The source observable.</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/>.</returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this IObservable<IMaybe<IEnumerable<T>>> source,
            Func<T, bool> filter)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (filter is null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            return source.AsReadOnlyView()
                         .AsReadOnlyFilteredView(filter, leaveOpen: false);
        }

        /// <summary>
        /// Create a filtered view for <paramref name="source"/>.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source collection.</typeparam>
        /// <param name="source">The source observable.</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/>.</returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this IObservable<Maybe<IEnumerable<T>>> source,
            Func<T, bool> filter)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (filter is null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            return source.AsReadOnlyView()
                         .AsReadOnlyFilteredView(filter, leaveOpen: false);
        }

        /// <summary>
        /// Create a filtered view for <paramref name="source"/>.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source collection.</typeparam>
        /// <param name="source">The source observable.</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/>.</returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this IObservable<IEnumerable<T>> source,
            Func<T, bool> filter,
            IScheduler scheduler)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (filter is null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            return source.AsReadOnlyView()
                         .AsReadOnlyFilteredView(filter, scheduler, leaveOpen: false);
        }

        /// <summary>
        /// Create a filtered view for <paramref name="source"/>.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source collection.</typeparam>
        /// <param name="source">The source observable.</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/>.</returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this IObservable<IMaybe<IEnumerable<T>>> source,
            Func<T, bool> filter,
            IScheduler scheduler)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (filter is null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            return source.AsReadOnlyView()
                         .AsReadOnlyFilteredView(filter, scheduler, leaveOpen: false);
        }

        /// <summary>
        /// Create a filtered view for <paramref name="source"/>.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source collection.</typeparam>
        /// <param name="source">The source observable.</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/>.</returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this IObservable<Maybe<IEnumerable<T>>> source,
            Func<T, bool> filter,
            IScheduler scheduler)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (filter is null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            return source.AsReadOnlyView()
                         .AsReadOnlyFilteredView(filter, scheduler, leaveOpen: false);
        }
    }
}
