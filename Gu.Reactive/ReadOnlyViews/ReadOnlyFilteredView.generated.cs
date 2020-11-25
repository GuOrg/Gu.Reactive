#nullable enable
#pragma warning disable RS0026 // Do not add multiple public overloads with optional parameters
#pragma warning disable RS0027 // Public API with optional parameter(s) should have the most parameters amongst its public overloads
namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Reactive.Concurrency;

    /// <summary>
    /// Factory methods for creating <see cref="ReadOnlyFilteredView{T}"/>
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
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this ObservableCollection<T> source,
            Func<T, bool> filter,
            params IObservable<object?>[] triggers)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (filter is null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            return new ReadOnlyFilteredView<T>(source, filter, TimeSpan.Zero, null, false, triggers);
        }

        /// <summary>
        /// Create a filtered view for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="leaveOpen">True means that the <paramref name="source"/> is not disposed when this instance is disposed.</param>
        /// <param name="triggers">Triggers for when filtering is updated.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/></returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this ObservableCollection<T> source,
            Func<T, bool> filter,
            bool leaveOpen,
            params IObservable<object?>[] triggers)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (filter is null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            return new ReadOnlyFilteredView<T>(source, filter, TimeSpan.Zero, null, leaveOpen, triggers);
        }

        /// <summary>
        /// Create a <see cref="ReadOnlyFilteredView{T}"/> view for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="bufferTime">The time to buffer changes in <paramref name="source"/></param>
        /// <param name="triggers">Triggers for when filtering is updated.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/></returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this ObservableCollection<T> source,
            Func<T, bool> filter,
            TimeSpan bufferTime,
            params IObservable<object?>[] triggers)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (filter is null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            return new ReadOnlyFilteredView<T>(source, filter, bufferTime, null, false, triggers);
        }

        /// <summary>
        /// Create a <see cref="ReadOnlyFilteredView{T}"/> view for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="bufferTime">The time to buffer changes in <paramref name="source"/></param>
        /// <param name="leaveOpen">True means that the <paramref name="source"/> is not disposed when this instance is disposed.</param>
        /// <param name="triggers">Triggers for when filtering is updated.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/></returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this ObservableCollection<T> source,
            Func<T, bool> filter,
            TimeSpan bufferTime,
            bool leaveOpen,
            params IObservable<object?>[] triggers)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (filter is null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            return new ReadOnlyFilteredView<T>(source, filter, bufferTime, null, leaveOpen, triggers);
        }

        /// <summary>
        /// Create a <see cref="ReadOnlyFilteredView{T}"/> view for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        /// <param name="triggers">Triggers for when filtering is updated.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/></returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this ObservableCollection<T> source,
            Func<T, bool> filter,
            IScheduler scheduler,
            params IObservable<object?>[] triggers)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (filter is null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            if (scheduler is null)
            {
                throw new ArgumentNullException(nameof(scheduler));
            }

            return new ReadOnlyFilteredView<T>(source, filter, TimeSpan.Zero, scheduler, false, triggers);
        }

        /// <summary>
        /// Create a <see cref="ReadOnlyFilteredView{T}"/> view for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        /// <param name="leaveOpen">True means that the <paramref name="source"/> is not disposed when this instance is disposed.</param>
        /// <param name="triggers">Triggers for when filtering is updated.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/></returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this ObservableCollection<T> source,
            Func<T, bool> filter,
            IScheduler scheduler,
            bool leaveOpen,
            params IObservable<object?>[] triggers)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (filter is null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            if (scheduler is null)
            {
                throw new ArgumentNullException(nameof(scheduler));
            }

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
        /// <param name="triggers">Triggers for when filtering is updated.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/></returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this ObservableCollection<T> source,
            Func<T, bool> filter,
            TimeSpan bufferTime,
            IScheduler scheduler,
            params IObservable<object?>[] triggers)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (filter is null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            if (scheduler is null)
            {
                throw new ArgumentNullException(nameof(scheduler));
            }

            return new ReadOnlyFilteredView<T>(source, filter, bufferTime, scheduler, false, triggers);
        }

        /// <summary>
        /// Create a <see cref="ReadOnlyFilteredView{T}"/> view for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="bufferTime">The time to buffer changes in <paramref name="source"/></param>
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        /// <param name="leaveOpen">True means that the <paramref name="source"/> is not disposed when this instance is disposed.</param>
        /// <param name="triggers">Triggers for when filtering is updated.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/></returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this ObservableCollection<T> source,
            Func<T, bool> filter,
            TimeSpan bufferTime,
            IScheduler scheduler,
            bool leaveOpen,
            params IObservable<object?>[] triggers)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (filter is null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            if (scheduler is null)
            {
                throw new ArgumentNullException(nameof(scheduler));
            }

            return new ReadOnlyFilteredView<T>(source, filter, bufferTime, scheduler, leaveOpen, triggers);
        }

        /// <summary>
        /// Create a filtered view for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="observableFactory">Factory for creating observables for the items.</param>
        /// <param name="leaveOpen">True means that the <paramref name="source"/> is not disposed when this instance is disposed.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/></returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this ObservableCollection<T> source,
            Func<T, bool> filter,
            Func<T, IObservable<object?>> observableFactory,
            bool leaveOpen = false)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (filter is null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            return new ReadOnlyFilteredView<T>(source, filter, observableFactory, TimeSpan.Zero, null, leaveOpen);
        }

        /// <summary>
        /// Create a filtered view for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="observableFactory">Factory for creating observables for the items.</param>
        /// <param name="bufferTime">The time to buffer changes in <paramref name="source"/></param>
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        /// <param name="leaveOpen">True means that the <paramref name="source"/> is not disposed when this instance is disposed.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/></returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this ObservableCollection<T> source,
            Func<T, bool> filter,
            Func<T, IObservable<object?>> observableFactory,
            TimeSpan bufferTime,
            IScheduler scheduler,
            bool leaveOpen)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (filter is null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            if (scheduler is null)
            {
                throw new ArgumentNullException(nameof(scheduler));
            }

            return new ReadOnlyFilteredView<T>(source, filter, observableFactory, bufferTime, scheduler, leaveOpen);
        }

        /// <summary>
        /// Create a filtered view for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="triggers">Triggers for when filtering is updated.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/></returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this ReadOnlyObservableCollection<T> source,
            Func<T, bool> filter,
            params IObservable<object?>[] triggers)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (filter is null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            return new ReadOnlyFilteredView<T>(source, filter, TimeSpan.Zero, null, false, triggers);
        }

        /// <summary>
        /// Create a filtered view for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="leaveOpen">True means that the <paramref name="source"/> is not disposed when this instance is disposed.</param>
        /// <param name="triggers">Triggers for when filtering is updated.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/></returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this ReadOnlyObservableCollection<T> source,
            Func<T, bool> filter,
            bool leaveOpen,
            params IObservable<object?>[] triggers)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (filter is null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            return new ReadOnlyFilteredView<T>(source, filter, TimeSpan.Zero, null, leaveOpen, triggers);
        }

        /// <summary>
        /// Create a <see cref="ReadOnlyFilteredView{T}"/> view for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="bufferTime">The time to buffer changes in <paramref name="source"/></param>
        /// <param name="triggers">Triggers for when filtering is updated.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/></returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this ReadOnlyObservableCollection<T> source,
            Func<T, bool> filter,
            TimeSpan bufferTime,
            params IObservable<object?>[] triggers)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (filter is null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            return new ReadOnlyFilteredView<T>(source, filter, bufferTime, null, false, triggers);
        }

        /// <summary>
        /// Create a <see cref="ReadOnlyFilteredView{T}"/> view for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="bufferTime">The time to buffer changes in <paramref name="source"/></param>
        /// <param name="leaveOpen">True means that the <paramref name="source"/> is not disposed when this instance is disposed.</param>
        /// <param name="triggers">Triggers for when filtering is updated.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/></returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this ReadOnlyObservableCollection<T> source,
            Func<T, bool> filter,
            TimeSpan bufferTime,
            bool leaveOpen,
            params IObservable<object?>[] triggers)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (filter is null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            return new ReadOnlyFilteredView<T>(source, filter, bufferTime, null, leaveOpen, triggers);
        }

        /// <summary>
        /// Create a <see cref="ReadOnlyFilteredView{T}"/> view for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        /// <param name="triggers">Triggers for when filtering is updated.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/></returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this ReadOnlyObservableCollection<T> source,
            Func<T, bool> filter,
            IScheduler scheduler,
            params IObservable<object?>[] triggers)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (filter is null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            if (scheduler is null)
            {
                throw new ArgumentNullException(nameof(scheduler));
            }

            return new ReadOnlyFilteredView<T>(source, filter, TimeSpan.Zero, scheduler, false, triggers);
        }

        /// <summary>
        /// Create a <see cref="ReadOnlyFilteredView{T}"/> view for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        /// <param name="leaveOpen">True means that the <paramref name="source"/> is not disposed when this instance is disposed.</param>
        /// <param name="triggers">Triggers for when filtering is updated.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/></returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this ReadOnlyObservableCollection<T> source,
            Func<T, bool> filter,
            IScheduler scheduler,
            bool leaveOpen,
            params IObservable<object?>[] triggers)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (filter is null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            if (scheduler is null)
            {
                throw new ArgumentNullException(nameof(scheduler));
            }

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
        /// <param name="triggers">Triggers for when filtering is updated.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/></returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this ReadOnlyObservableCollection<T> source,
            Func<T, bool> filter,
            TimeSpan bufferTime,
            IScheduler scheduler,
            params IObservable<object?>[] triggers)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (filter is null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            if (scheduler is null)
            {
                throw new ArgumentNullException(nameof(scheduler));
            }

            return new ReadOnlyFilteredView<T>(source, filter, bufferTime, scheduler, false, triggers);
        }

        /// <summary>
        /// Create a <see cref="ReadOnlyFilteredView{T}"/> view for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="bufferTime">The time to buffer changes in <paramref name="source"/></param>
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        /// <param name="leaveOpen">True means that the <paramref name="source"/> is not disposed when this instance is disposed.</param>
        /// <param name="triggers">Triggers for when filtering is updated.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/></returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this ReadOnlyObservableCollection<T> source,
            Func<T, bool> filter,
            TimeSpan bufferTime,
            IScheduler scheduler,
            bool leaveOpen,
            params IObservable<object?>[] triggers)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (filter is null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            if (scheduler is null)
            {
                throw new ArgumentNullException(nameof(scheduler));
            }

            return new ReadOnlyFilteredView<T>(source, filter, bufferTime, scheduler, leaveOpen, triggers);
        }

        /// <summary>
        /// Create a filtered view for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="observableFactory">Factory for creating observables for the items.</param>
        /// <param name="leaveOpen">True means that the <paramref name="source"/> is not disposed when this instance is disposed.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/></returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this ReadOnlyObservableCollection<T> source,
            Func<T, bool> filter,
            Func<T, IObservable<object?>> observableFactory,
            bool leaveOpen = false)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (filter is null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            return new ReadOnlyFilteredView<T>(source, filter, observableFactory, TimeSpan.Zero, null, leaveOpen);
        }

        /// <summary>
        /// Create a filtered view for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="observableFactory">Factory for creating observables for the items.</param>
        /// <param name="bufferTime">The time to buffer changes in <paramref name="source"/></param>
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        /// <param name="leaveOpen">True means that the <paramref name="source"/> is not disposed when this instance is disposed.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/></returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this ReadOnlyObservableCollection<T> source,
            Func<T, bool> filter,
            Func<T, IObservable<object?>> observableFactory,
            TimeSpan bufferTime,
            IScheduler scheduler,
            bool leaveOpen)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (filter is null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            if (scheduler is null)
            {
                throw new ArgumentNullException(nameof(scheduler));
            }

            return new ReadOnlyFilteredView<T>(source, filter, observableFactory, bufferTime, scheduler, leaveOpen);
        }

        /// <summary>
        /// Create a filtered view for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="triggers">Triggers for when filtering is updated.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/></returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this IReadOnlyObservableCollection<T> source,
            Func<T, bool> filter,
            params IObservable<object?>[] triggers)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (filter is null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            return new ReadOnlyFilteredView<T>(source, filter, TimeSpan.Zero, null, false, triggers);
        }

        /// <summary>
        /// Create a filtered view for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="leaveOpen">True means that the <paramref name="source"/> is not disposed when this instance is disposed.</param>
        /// <param name="triggers">Triggers for when filtering is updated.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/></returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this IReadOnlyObservableCollection<T> source,
            Func<T, bool> filter,
            bool leaveOpen,
            params IObservable<object?>[] triggers)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (filter is null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            return new ReadOnlyFilteredView<T>(source, filter, TimeSpan.Zero, null, leaveOpen, triggers);
        }

        /// <summary>
        /// Create a <see cref="ReadOnlyFilteredView{T}"/> view for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="bufferTime">The time to buffer changes in <paramref name="source"/></param>
        /// <param name="triggers">Triggers for when filtering is updated.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/></returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this IReadOnlyObservableCollection<T> source,
            Func<T, bool> filter,
            TimeSpan bufferTime,
            params IObservable<object?>[] triggers)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (filter is null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            return new ReadOnlyFilteredView<T>(source, filter, bufferTime, null, false, triggers);
        }

        /// <summary>
        /// Create a <see cref="ReadOnlyFilteredView{T}"/> view for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="bufferTime">The time to buffer changes in <paramref name="source"/></param>
        /// <param name="leaveOpen">True means that the <paramref name="source"/> is not disposed when this instance is disposed.</param>
        /// <param name="triggers">Triggers for when filtering is updated.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/></returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this IReadOnlyObservableCollection<T> source,
            Func<T, bool> filter,
            TimeSpan bufferTime,
            bool leaveOpen,
            params IObservable<object?>[] triggers)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (filter is null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            return new ReadOnlyFilteredView<T>(source, filter, bufferTime, null, leaveOpen, triggers);
        }

        /// <summary>
        /// Create a <see cref="ReadOnlyFilteredView{T}"/> view for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        /// <param name="triggers">Triggers for when filtering is updated.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/></returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this IReadOnlyObservableCollection<T> source,
            Func<T, bool> filter,
            IScheduler scheduler,
            params IObservable<object?>[] triggers)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (filter is null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            if (scheduler is null)
            {
                throw new ArgumentNullException(nameof(scheduler));
            }

            return new ReadOnlyFilteredView<T>(source, filter, TimeSpan.Zero, scheduler, false, triggers);
        }

        /// <summary>
        /// Create a <see cref="ReadOnlyFilteredView{T}"/> view for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        /// <param name="leaveOpen">True means that the <paramref name="source"/> is not disposed when this instance is disposed.</param>
        /// <param name="triggers">Triggers for when filtering is updated.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/></returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this IReadOnlyObservableCollection<T> source,
            Func<T, bool> filter,
            IScheduler scheduler,
            bool leaveOpen,
            params IObservable<object?>[] triggers)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (filter is null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            if (scheduler is null)
            {
                throw new ArgumentNullException(nameof(scheduler));
            }

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
        /// <param name="triggers">Triggers for when filtering is updated.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/></returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this IReadOnlyObservableCollection<T> source,
            Func<T, bool> filter,
            TimeSpan bufferTime,
            IScheduler scheduler,
            params IObservable<object?>[] triggers)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (filter is null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            if (scheduler is null)
            {
                throw new ArgumentNullException(nameof(scheduler));
            }

            return new ReadOnlyFilteredView<T>(source, filter, bufferTime, scheduler, false, triggers);
        }

        /// <summary>
        /// Create a <see cref="ReadOnlyFilteredView{T}"/> view for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="bufferTime">The time to buffer changes in <paramref name="source"/></param>
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        /// <param name="leaveOpen">True means that the <paramref name="source"/> is not disposed when this instance is disposed.</param>
        /// <param name="triggers">Triggers for when filtering is updated.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/></returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this IReadOnlyObservableCollection<T> source,
            Func<T, bool> filter,
            TimeSpan bufferTime,
            IScheduler scheduler,
            bool leaveOpen,
            params IObservable<object?>[] triggers)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (filter is null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            if (scheduler is null)
            {
                throw new ArgumentNullException(nameof(scheduler));
            }

            return new ReadOnlyFilteredView<T>(source, filter, bufferTime, scheduler, leaveOpen, triggers);
        }

        /// <summary>
        /// Create a filtered view for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="observableFactory">Factory for creating observables for the items.</param>
        /// <param name="leaveOpen">True means that the <paramref name="source"/> is not disposed when this instance is disposed.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/></returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this IReadOnlyObservableCollection<T> source,
            Func<T, bool> filter,
            Func<T, IObservable<object?>> observableFactory,
            bool leaveOpen = false)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (filter is null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            return new ReadOnlyFilteredView<T>(source, filter, observableFactory, TimeSpan.Zero, null, leaveOpen);
        }

        /// <summary>
        /// Create a filtered view for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="observableFactory">Factory for creating observables for the items.</param>
        /// <param name="bufferTime">The time to buffer changes in <paramref name="source"/></param>
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        /// <param name="leaveOpen">True means that the <paramref name="source"/> is not disposed when this instance is disposed.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/></returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this IReadOnlyObservableCollection<T> source,
            Func<T, bool> filter,
            Func<T, IObservable<object?>> observableFactory,
            TimeSpan bufferTime,
            IScheduler scheduler,
            bool leaveOpen)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (filter is null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            if (scheduler is null)
            {
                throw new ArgumentNullException(nameof(scheduler));
            }

            return new ReadOnlyFilteredView<T>(source, filter, observableFactory, bufferTime, scheduler, leaveOpen);
        }

        /// <summary>
        /// Create a filtered view for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="triggers">Trigger for when filtering is updated.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/></returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this IEnumerable<T> source,
            Func<T, bool> filter,
            IEnumerable<IObservable<object?>> triggers)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (filter is null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            if (triggers is null)
            {
                throw new ArgumentNullException(nameof(triggers));
            }

            return new ReadOnlyFilteredView<T>(source, filter, TimeSpan.Zero, null, false, triggers);
        }

        /// <summary>
        /// Create a filtered view for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="leaveOpen">True means that the <paramref name="source"/> is not disposed when this instance is disposed.</param>
        /// <param name="triggers">Trigger for when filtering is updated.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/></returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this IEnumerable<T> source,
            Func<T, bool> filter,
            bool leaveOpen,
            IEnumerable<IObservable<object?>> triggers)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (filter is null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            if (triggers is null)
            {
                throw new ArgumentNullException(nameof(triggers));
            }

            return new ReadOnlyFilteredView<T>(source, filter, TimeSpan.Zero, null, leaveOpen, triggers);
        }

        /// <summary>
        /// Create a filtered view for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="bufferTime">The time to buffer changes in <paramref name="source"/></param>
        /// <param name="triggers">Trigger for when filtering is updated.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/></returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this IEnumerable<T> source,
            Func<T, bool> filter,
            TimeSpan bufferTime,
            IEnumerable<IObservable<object?>> triggers)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (filter is null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            if (triggers is null)
            {
                throw new ArgumentNullException(nameof(triggers));
            }

            return new ReadOnlyFilteredView<T>(source, filter, bufferTime, null, false, triggers);
        }

        /// <summary>
        /// Create a filtered view for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="bufferTime">The time to buffer changes in <paramref name="source"/></param>
        /// <param name="leaveOpen">True means that the <paramref name="source"/> is not disposed when this instance is disposed.</param>
        /// <param name="triggers">Trigger for when filtering is updated.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/></returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this IEnumerable<T> source,
            Func<T, bool> filter,
            TimeSpan bufferTime,
            bool leaveOpen,
            IEnumerable<IObservable<object?>> triggers)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (filter is null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            if (triggers is null)
            {
                throw new ArgumentNullException(nameof(triggers));
            }

            return new ReadOnlyFilteredView<T>(source, filter, bufferTime, null, leaveOpen, triggers);
        }

        /// <summary>
        /// Create a filtered view for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        /// <param name="triggers">Trigger for when filtering is updated.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/></returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this IEnumerable<T> source,
            Func<T, bool> filter,
            IScheduler scheduler,
            IEnumerable<IObservable<object?>> triggers)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (filter is null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            if (scheduler is null)
            {
                throw new ArgumentNullException(nameof(scheduler));
            }

            if (triggers is null)
            {
                throw new ArgumentNullException(nameof(triggers));
            }

            return new ReadOnlyFilteredView<T>(source, filter, TimeSpan.Zero, scheduler, false, triggers);
        }

        /// <summary>
        /// Create a filtered view for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        /// <param name="leaveOpen">True means that the <paramref name="source"/> is not disposed when this instance is disposed.</param>
        /// <param name="triggers">Trigger for when filtering is updated.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/></returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this IEnumerable<T> source,
            Func<T, bool> filter,
            IScheduler scheduler,
            bool leaveOpen,
            IEnumerable<IObservable<object?>> triggers)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (filter is null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            if (scheduler is null)
            {
                throw new ArgumentNullException(nameof(scheduler));
            }

            if (triggers is null)
            {
                throw new ArgumentNullException(nameof(triggers));
            }

            return new ReadOnlyFilteredView<T>(source, filter, TimeSpan.Zero, scheduler, leaveOpen, triggers);
        }

        /// <summary>
        /// Create a filtered view for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="bufferTime">The time to buffer changes in <paramref name="source"/></param>
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        /// <param name="triggers">Trigger for when filtering is updated.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/></returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this IEnumerable<T> source,
            Func<T, bool> filter,
            TimeSpan bufferTime,
            IScheduler scheduler,
            IEnumerable<IObservable<object?>> triggers)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (filter is null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            if (scheduler is null)
            {
                throw new ArgumentNullException(nameof(scheduler));
            }

            if (triggers is null)
            {
                throw new ArgumentNullException(nameof(triggers));
            }

            return new ReadOnlyFilteredView<T>(source, filter, bufferTime, scheduler, false, triggers);
        }

        /// <summary>
        /// Create a filtered view for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="bufferTime">The time to buffer changes in <paramref name="source"/></param>
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        /// <param name="leaveOpen">True means that the <paramref name="source"/> is not disposed when this instance is disposed.</param>
        /// <param name="triggers">Trigger for when filtering is updated.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/></returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this IEnumerable<T> source,
            Func<T, bool> filter,
            TimeSpan bufferTime,
            IScheduler scheduler,
            bool leaveOpen,
            IEnumerable<IObservable<object?>> triggers)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (filter is null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            if (scheduler is null)
            {
                throw new ArgumentNullException(nameof(scheduler));
            }

            if (triggers is null)
            {
                throw new ArgumentNullException(nameof(triggers));
            }

            return new ReadOnlyFilteredView<T>(source, filter, bufferTime, scheduler, leaveOpen, triggers);
        }

        /// <summary>
        /// Create a filtered view for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="trigger">Trigger for when filtering is updated.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/></returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this IEnumerable<T> source,
            Func<T, bool> filter,
            IObservable<object?> trigger)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (filter is null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            if (trigger is null)
            {
                throw new ArgumentNullException(nameof(trigger));
            }

            return new ReadOnlyFilteredView<T>(source, filter, TimeSpan.Zero, null, false, new[] { trigger });
        }

        /// <summary>
        /// Create a filtered view for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="leaveOpen">True means that the <paramref name="source"/> is not disposed when this instance is disposed.</param>
        /// <param name="trigger">Trigger for when filtering is updated.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/></returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this IEnumerable<T> source,
            Func<T, bool> filter,
            bool leaveOpen,
            IObservable<object?> trigger)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (filter is null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            if (trigger is null)
            {
                throw new ArgumentNullException(nameof(trigger));
            }

            return new ReadOnlyFilteredView<T>(source, filter, TimeSpan.Zero, null, leaveOpen, new[] { trigger });
        }

        /// <summary>
        /// Create a filtered view for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="bufferTime">The time to buffer changes in <paramref name="source"/></param>
        /// <param name="trigger">Trigger for when filtering is updated.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/></returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this IEnumerable<T> source,
            Func<T, bool> filter,
            TimeSpan bufferTime,
            IObservable<object?> trigger)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (filter is null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            if (trigger is null)
            {
                throw new ArgumentNullException(nameof(trigger));
            }

            return new ReadOnlyFilteredView<T>(source, filter, bufferTime, null, false, new[] { trigger });
        }

        /// <summary>
        /// Create a filtered view for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="bufferTime">The time to buffer changes in <paramref name="source"/></param>
        /// <param name="leaveOpen">True means that the <paramref name="source"/> is not disposed when this instance is disposed.</param>
        /// <param name="trigger">Trigger for when filtering is updated.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/></returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this IEnumerable<T> source,
            Func<T, bool> filter,
            TimeSpan bufferTime,
            bool leaveOpen,
            IObservable<object?> trigger)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (filter is null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            if (trigger is null)
            {
                throw new ArgumentNullException(nameof(trigger));
            }

            return new ReadOnlyFilteredView<T>(source, filter, bufferTime, null, leaveOpen, new[] { trigger });
        }

        /// <summary>
        /// Create a filtered view for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        /// <param name="trigger">Trigger for when filtering is updated.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/></returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this IEnumerable<T> source,
            Func<T, bool> filter,
            IScheduler scheduler,
            IObservable<object?> trigger)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (filter is null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            if (scheduler is null)
            {
                throw new ArgumentNullException(nameof(scheduler));
            }

            if (trigger is null)
            {
                throw new ArgumentNullException(nameof(trigger));
            }

            return new ReadOnlyFilteredView<T>(source, filter, TimeSpan.Zero, scheduler, false, new[] { trigger });
        }

        /// <summary>
        /// Create a filtered view for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        /// <param name="leaveOpen">True means that the <paramref name="source"/> is not disposed when this instance is disposed.</param>
        /// <param name="trigger">Trigger for when filtering is updated.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/></returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this IEnumerable<T> source,
            Func<T, bool> filter,
            IScheduler scheduler,
            bool leaveOpen,
            IObservable<object?> trigger)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (filter is null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            if (scheduler is null)
            {
                throw new ArgumentNullException(nameof(scheduler));
            }

            if (trigger is null)
            {
                throw new ArgumentNullException(nameof(trigger));
            }

            return new ReadOnlyFilteredView<T>(source, filter, TimeSpan.Zero, scheduler, leaveOpen, new[] { trigger });
        }

        /// <summary>
        /// Create a filtered view for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="bufferTime">The time to buffer changes in <paramref name="source"/></param>
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        /// <param name="trigger">Trigger for when filtering is updated.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/></returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this IEnumerable<T> source,
            Func<T, bool> filter,
            TimeSpan bufferTime,
            IScheduler scheduler,
            IObservable<object?> trigger)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (filter is null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            if (scheduler is null)
            {
                throw new ArgumentNullException(nameof(scheduler));
            }

            if (trigger is null)
            {
                throw new ArgumentNullException(nameof(trigger));
            }

            return new ReadOnlyFilteredView<T>(source, filter, bufferTime, scheduler, false, new[] { trigger });
        }

        /// <summary>
        /// Create a filtered view for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="bufferTime">The time to buffer changes in <paramref name="source"/></param>
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        /// <param name="leaveOpen">True means that the <paramref name="source"/> is not disposed when this instance is disposed.</param>
        /// <param name="trigger">Trigger for when filtering is updated.</param>
        /// <returns>A <see cref="ReadOnlyFilteredView{T}"/></returns>
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this IEnumerable<T> source,
            Func<T, bool> filter,
            TimeSpan bufferTime,
            IScheduler scheduler,
            bool leaveOpen,
            IObservable<object?> trigger)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (filter is null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            if (scheduler is null)
            {
                throw new ArgumentNullException(nameof(scheduler));
            }

            if (trigger is null)
            {
                throw new ArgumentNullException(nameof(trigger));
            }

            return new ReadOnlyFilteredView<T>(source, filter, bufferTime, scheduler, leaveOpen, new[] { trigger });
        }
    }
}