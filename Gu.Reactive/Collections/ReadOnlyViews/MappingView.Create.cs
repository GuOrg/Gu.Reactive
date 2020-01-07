// ReSharper disable PossibleMultipleEnumeration
namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Concurrency;

    using Gu.Reactive.Internals;

    /// <summary>
    /// Factory methods for creating <see cref="MappingView{TSource,TResult}"/>.
    /// </summary>
    public static partial class MappingView
    {
        /// <summary>
        /// Create a <see cref="MappingView{TSource, TResult}"/> for <paramref name="source"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the collection.</typeparam>
        /// <typeparam name="TResult">The type of the elements in the resulting collection.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="selector">The function mapping an element of type <typeparamref name="TSource"/> to <typeparamref name="TResult"/>.</param>
        /// <param name="triggers">Additional triggers for when mapping is updated.</param>
        /// <returns>A <see cref="MappingView{TSource, TResult}"/>.</returns>
        public static MappingView<TSource, TResult> Create<TSource, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, TResult> selector,
            params IObservable<object>[] triggers)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (selector is null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            return new MappingView<TSource, TResult>(source, Mapper.Create(selector), TimeSpan.Zero, scheduler: null, leaveOpen: true, triggers: triggers);
        }

        /// <summary>
        /// Create a <see cref="MappingView{TSource, TResult}"/> for <paramref name="source"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the collection.</typeparam>
        /// <typeparam name="TResult">The type of the elements in the resulting collection.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="selector">The function mapping an element of type <typeparamref name="TSource"/> to <typeparamref name="TResult"/>.</param>
        /// <param name="leaveOpen">True means that the <paramref name="source"/> is not disposed when this instance is disposed.</param>
        /// <param name="triggers">Additional triggers for when mapping is updated.</param>
        /// <returns>A <see cref="MappingView{TSource, TResult}"/>.</returns>
        public static MappingView<TSource, TResult> Create<TSource, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, TResult> selector,
            bool leaveOpen,
            params IObservable<object>[] triggers)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (selector is null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            return new MappingView<TSource, TResult>(source, Mapper.Create(selector), TimeSpan.Zero, scheduler: null, leaveOpen: leaveOpen, triggers: triggers);
        }

        /// <summary>
        /// Create a <see cref="MappingView{TSource, TResult}"/> for <paramref name="source"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the collection.</typeparam>
        /// <typeparam name="TResult">The type of the elements in the resulting collection.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="selector">The function mapping an element of type <typeparamref name="TSource"/> to <typeparamref name="TResult"/>.</param>
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        /// <param name="leaveOpen">True means that the <paramref name="source"/> is not disposed when this instance is disposed.</param>
        /// <param name="triggers">Additional triggers for when mapping is updated.</param>
        /// <returns>A <see cref="MappingView{TSource, TResult}"/>.</returns>
        public static MappingView<TSource, TResult> Create<TSource, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, TResult> selector,
            IScheduler scheduler = null,
            bool leaveOpen = true,
            params IObservable<object>[] triggers)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (selector is null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            return new MappingView<TSource, TResult>(source, Mapper.Create(selector), System.TimeSpan.Zero, scheduler, leaveOpen, triggers);
        }

        /// <summary>
        /// Create a <see cref="MappingView{TSource, TResult}"/> for <paramref name="source"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the collection.</typeparam>
        /// <typeparam name="TResult">The type of the elements in the resulting collection.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="selector">The function mapping an element of type <typeparamref name="TSource"/> to <typeparamref name="TResult"/>.</param>
        /// <param name="bufferTime">The time to buffer changes before updating and notifying.</param>
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        /// <param name="leaveOpen">True means that the <paramref name="source"/> is not disposed when this instance is disposed.</param>
        /// <param name="triggers">Additional triggers for when mapping is updated.</param>
        /// <returns>A <see cref="MappingView{TSource, TResult}"/>.</returns>
        public static MappingView<TSource, TResult> Create<TSource, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, TResult> selector,
            TimeSpan bufferTime,
            IScheduler scheduler = null,
            bool leaveOpen = true,
            params IObservable<object>[] triggers)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (selector is null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            return new MappingView<TSource, TResult>(source, Mapper.Create(selector), bufferTime, scheduler, leaveOpen, triggers);
        }

        /// <summary>
        /// Create a <see cref="MappingView{TSource, TResult}"/> for <paramref name="source"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the collection.</typeparam>
        /// <typeparam name="TResult">The type of the elements in the resulting collection.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="selector">The function mapping an element of type <typeparamref name="TSource"/> to <typeparamref name="TResult"/>.</param>
        /// <param name="onRemove">An action to perform when an item is removed from the collection. Typically it will be x => x.Dispose().</param>
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        /// <param name="leaveOpen">True means that the <paramref name="source"/> is not disposed when this instance is disposed.</param>
        /// <param name="triggers">Additional triggers for when mapping is updated.</param>
        /// <returns>A <see cref="MappingView{TSource, TResult}"/>.</returns>
        public static MappingView<TSource, TResult> Create<TSource, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, TResult> selector,
            Action<TResult> onRemove,
            IScheduler scheduler = null,
            bool leaveOpen = true,
            params IObservable<object>[] triggers)
            where TResult : class
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (selector is null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            if (onRemove is null)
            {
                throw new ArgumentNullException(nameof(onRemove));
            }

            return new MappingView<TSource, TResult>(source, Mapper.Create(selector, onRemove), System.TimeSpan.Zero, scheduler, leaveOpen, triggers);
        }

        /// <summary>
        /// Create a <see cref="MappingView{TSource, TResult}"/> for <paramref name="source"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the collection.</typeparam>
        /// <typeparam name="TResult">The type of the elements in the resulting collection.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="selector">The function mapping an element of type <typeparamref name="TSource"/> to <typeparamref name="TResult"/>.</param>
        /// <param name="onRemove">An action to perform when an item is removed from the collection. Typically it will be x => x.Dispose().</param>
        /// <param name="bufferTime">The time to buffer changes before updating and notifying.</param>
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        /// <param name="leaveOpen">True means that the <paramref name="source"/> is not disposed when this instance is disposed.</param>
        /// <param name="triggers">Additional triggers for when mapping is updated.</param>
        /// <returns>A <see cref="MappingView{TSource, TResult}"/>.</returns>
        public static MappingView<TSource, TResult> Create<TSource, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, TResult> selector,
            Action<TResult> onRemove,
            TimeSpan bufferTime,
            IScheduler scheduler = null,
            bool leaveOpen = true,
            params IObservable<object>[] triggers)
            where TResult : class
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (selector is null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            if (onRemove is null)
            {
                throw new ArgumentNullException(nameof(onRemove));
            }

            return new MappingView<TSource, TResult>(source, Mapper.Create(selector, onRemove), bufferTime, scheduler, leaveOpen, triggers);
        }

        /// <summary>
        /// Create a <see cref="MappingView{TSource, TResult}"/> for <paramref name="source"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the collection.</typeparam>
        /// <typeparam name="TResult">The type of the elements in the resulting collection.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="selector">
        /// The function mapping an element of type <typeparamref name="TSource"/> to <typeparamref name="TResult"/>.
        /// The second parameter is the index of the element.
        /// </param>
        /// <param name="updater">
        /// The function updating an element for which the index changed.
        /// The second parameter is the index of the element.
        /// </param>
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        /// <param name="leaveOpen">True means that the <paramref name="source"/> is not disposed when this instance is disposed.</param>
        /// <param name="triggers">Additional triggers for when mapping is updated.</param>
        /// <returns>A <see cref="MappingView{TSource, TResult}"/>.</returns>
        public static MappingView<TSource, TResult> Create<TSource, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, int, TResult> selector,
            Func<TResult, int, TResult> updater,
            IScheduler scheduler = null,
            bool leaveOpen = true,
            params IObservable<object>[] triggers)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (selector is null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            if (updater is null)
            {
                throw new ArgumentNullException(nameof(updater));
            }

            return new MappingView<TSource, TResult>(source, Mapper.Create(selector, updater), System.TimeSpan.Zero, scheduler, leaveOpen, triggers);
        }

        /// <summary>
        /// Create a <see cref="MappingView{TSource, TResult}"/> for <paramref name="source"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the collection.</typeparam>
        /// <typeparam name="TResult">The type of the elements in the resulting collection.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="selector">
        /// The function mapping an element of type <typeparamref name="TSource"/> to <typeparamref name="TResult"/>.
        /// The second parameter is the index of the element.
        /// </param>
        /// <param name="updater">
        /// The function updating an element for which the index changed.
        /// The second parameter is the index of the element.
        /// </param>
        /// <param name="bufferTime">The time to buffer changes before updating and notifying.</param>
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        /// <param name="leaveOpen">True means that the <paramref name="source"/> is not disposed when this instance is disposed.</param>
        /// <param name="triggers">Additional triggers for when mapping is updated.</param>
        /// <returns>A <see cref="MappingView{TSource, TResult}"/>.</returns>
        public static MappingView<TSource, TResult> Create<TSource, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, int, TResult> selector,
            Func<TResult, int, TResult> updater,
            TimeSpan bufferTime,
            IScheduler scheduler = null,
            bool leaveOpen = true,
            params IObservable<object>[] triggers)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (selector is null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            if (updater is null)
            {
                throw new ArgumentNullException(nameof(updater));
            }

            return new MappingView<TSource, TResult>(source, Mapper.Create(selector, updater), bufferTime, scheduler, leaveOpen, triggers);
        }

        /// <summary>
        /// Create a <see cref="MappingView{TSource, TResult}"/> for <paramref name="source"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the collection.</typeparam>
        /// <typeparam name="TResult">The type of the elements in the resulting collection.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="selector">
        /// The function mapping an element of type <typeparamref name="TSource"/> to <typeparamref name="TResult"/>.
        /// The second parameter is the index of the element.
        /// </param>
        /// <param name="updater">
        /// The function updating an element for which the index changed.
        /// The second parameter is the index of the element.
        /// </param>
        /// <param name="onRemove">An action to perform when an item is removed from the collection. Typically it will be x => x.Dispose().</param>
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        /// <param name="leaveOpen">True means that the <paramref name="source"/> is not disposed when this instance is disposed.</param>
        /// <param name="triggers">Additional triggers for when mapping is updated.</param>
        /// <returns>A <see cref="MappingView{TSource, TResult}"/>.</returns>
        public static MappingView<TSource, TResult> Create<TSource, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, int, TResult> selector,
            Func<TResult, int, TResult> updater,
            Action<TResult> onRemove,
            IScheduler scheduler = null,
            bool leaveOpen = true,
            params IObservable<object>[] triggers)
            where TResult : class
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (selector is null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            if (updater is null)
            {
                throw new ArgumentNullException(nameof(updater));
            }

            return new MappingView<TSource, TResult>(source, Mapper.Create(selector, updater, onRemove), System.TimeSpan.Zero, scheduler, leaveOpen, triggers);
        }

        /// <summary>
        /// Create a <see cref="MappingView{TSource, TResult}"/> for <paramref name="source"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the collection.</typeparam>
        /// <typeparam name="TResult">The type of the elements in the resulting collection.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="selector">
        /// The function mapping an element of type <typeparamref name="TSource"/> to <typeparamref name="TResult"/>.
        /// The second parameter is the index of the element.
        /// </param>
        /// <param name="updater">
        /// The function updating an element for which the index changed.
        /// The second parameter is the index of the element.
        /// </param>
        /// <param name="onRemove">An action to perform when an item is removed from the collection. Typically it will be x => x.Dispose().</param>
        /// <param name="bufferTime">The time to buffer changes before updating and notifying.</param>
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        /// <param name="leaveOpen">True means that the <paramref name="source"/> is not disposed when this instance is disposed.</param>
        /// <param name="triggers">Additional triggers for when mapping is updated.</param>
        /// <returns>A <see cref="MappingView{TSource, TResult}"/>.</returns>
        public static MappingView<TSource, TResult> Create<TSource, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, int, TResult> selector,
            Func<TResult, int, TResult> updater,
            Action<TResult> onRemove,
            TimeSpan bufferTime,
            IScheduler scheduler = null,
            bool leaveOpen = true,
            params IObservable<object>[] triggers)
            where TResult : class
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (selector is null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            if (updater is null)
            {
                throw new ArgumentNullException(nameof(updater));
            }

            return new MappingView<TSource, TResult>(source, Mapper.Create(selector, updater, onRemove), bufferTime, scheduler, leaveOpen, triggers);
        }
    }
}
