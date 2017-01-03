namespace Gu.Reactive
{
    using System;
    using System.Collections.ObjectModel;
    using System.Reactive.Concurrency;
    using Gu.Reactive.Internals;

    /// <summary>
    /// Factory methods for creating <see cref="MappingView{TSource, TResult}"/>
    /// </summary>
    public static partial class MappingView
    {
        /// <summary>
        /// Create a <see cref="MappingView{TSource, TResult}"/> for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the collection.</typeparam>
        /// <typeparam name="TResult">The type of the elements in the resulting collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="selector">The function mapping an element of type <typeparamref name="TSource"/> to <typeparamref name="TResult"/>.</param>
        /// <param name="triggers">Additional triggers for when mapping is updated.</param>
        /// <returns>A <see cref="FilteredView{T}"/></returns>
        public static MappingView<TSource, TResult> AsMappingView<TSource, TResult>(
            this ObservableCollection<TSource> source,
            Func<TSource, TResult> selector,
            params IObservable<object>[] triggers)
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(selector, nameof(selector));
            return new MappingView<TSource, TResult>(source, selector,  null, triggers);
        }

        /// <summary>
        /// Create a <see cref="MappingView{TSource, TResult}"/> for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the collection.</typeparam>
        /// <typeparam name="TResult">The type of the elements in the resulting collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="selector">The function mapping an element of type <typeparamref name="TSource"/> to <typeparamref name="TResult"/>.</param>
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        /// <returns>A <see cref="FilteredView{T}"/></returns>
        public static MappingView<TSource, TResult> AsMappingView<TSource, TResult>(
            this ObservableCollection<TSource> source,
            Func<TSource, TResult> selector,
            IScheduler scheduler)
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(selector, nameof(selector));
            return new MappingView<TSource, TResult>(source, selector, scheduler);
        }

        /// <summary>
        /// Create a <see cref="MappingView{TSource, TResult}"/> for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the collection.</typeparam>
        /// <typeparam name="TResult">The type of the elements in the resulting collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="selector">
        /// The function mapping an element of type <typeparamref name="TSource"/> to <typeparamref name="TResult"/>.</param>
        /// The second parameter is the index of the element.
        /// <param name="triggers">Additional triggers for when mapping is updated.</param>
        /// <returns>A <see cref="FilteredView{T}"/></returns>
        public static MappingView<TSource, TResult> AsMappingView<TSource, TResult>(
            this ObservableCollection<TSource> source,
            Func<TSource, int, TResult> selector,
            params IObservable<object>[] triggers)
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(selector, nameof(selector));
            return new MappingView<TSource, TResult>(source, selector,  null, triggers);
        }

        /// <summary>
        /// Create a <see cref="MappingView{TSource, TResult}"/> for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the collection.</typeparam>
        /// <typeparam name="TResult">The type of the elements in the resulting collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="selector">
        /// The function mapping an element of type <typeparamref name="TSource"/> to <typeparamref name="TResult"/>.</param>
        /// The second parameter is the index of the element.
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        /// <returns>A <see cref="FilteredView{T}"/></returns>
        public static MappingView<TSource, TResult> AsMappingView<TSource, TResult>(
            this ObservableCollection<TSource> source,
            Func<TSource, int, TResult> selector,
            IScheduler scheduler)
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(selector, nameof(selector));
            return new MappingView<TSource, TResult>(source, selector, scheduler, null);
        }

        public static MappingView<TSource, TResult> AsMappingView<TSource, TResult>(
            this ObservableCollection<TSource> source,
            Func<TSource, int, TResult> selector,
            Func<TResult, int, TResult> updater,
            params IObservable<object>[] triggers)
            where TSource : class
            where TResult : class
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(selector, nameof(selector));
            Ensure.NotNull(updater, nameof(updater));
            return new MappingView<TSource, TResult>(source, selector, updater, null, triggers);
        }

        public static MappingView<TSource, TResult> AsMappingView<TSource, TResult>(
            this ObservableCollection<TSource> source,
            Func<TSource, int, TResult> selector,
            Func<TResult, int, TResult> updater,
            IScheduler scheduler)
            where TSource : class
            where TResult : class
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(selector, nameof(selector));
            Ensure.NotNull(updater, nameof(updater));
            return new MappingView<TSource, TResult>(source, selector, updater, scheduler);
        }

        /// <summary>
        /// Create a <see cref="MappingView{TSource, TResult}"/> for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the collection.</typeparam>
        /// <typeparam name="TResult">The type of the elements in the resulting collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="selector">The function mapping an element of type <typeparamref name="TSource"/> to <typeparamref name="TResult"/>.</param>
        /// <param name="triggers">Additional triggers for when mapping is updated.</param>
        /// <returns>A <see cref="FilteredView{T}"/></returns>
        public static MappingView<TSource, TResult> AsMappingView<TSource, TResult>(
            this ReadOnlyObservableCollection<TSource> source,
            Func<TSource, TResult> selector,
            params IObservable<object>[] triggers)
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(selector, nameof(selector));
            return new MappingView<TSource, TResult>(source, selector,  null, triggers);
        }

        /// <summary>
        /// Create a <see cref="MappingView{TSource, TResult}"/> for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the collection.</typeparam>
        /// <typeparam name="TResult">The type of the elements in the resulting collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="selector">The function mapping an element of type <typeparamref name="TSource"/> to <typeparamref name="TResult"/>.</param>
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        /// <returns>A <see cref="FilteredView{T}"/></returns>
        public static MappingView<TSource, TResult> AsMappingView<TSource, TResult>(
            this ReadOnlyObservableCollection<TSource> source,
            Func<TSource, TResult> selector,
            IScheduler scheduler)
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(selector, nameof(selector));
            return new MappingView<TSource, TResult>(source, selector, scheduler);
        }

        /// <summary>
        /// Create a <see cref="MappingView{TSource, TResult}"/> for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the collection.</typeparam>
        /// <typeparam name="TResult">The type of the elements in the resulting collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="selector">
        /// The function mapping an element of type <typeparamref name="TSource"/> to <typeparamref name="TResult"/>.</param>
        /// The second parameter is the index of the element.
        /// <param name="triggers">Additional triggers for when mapping is updated.</param>
        /// <returns>A <see cref="FilteredView{T}"/></returns>
        public static MappingView<TSource, TResult> AsMappingView<TSource, TResult>(
            this ReadOnlyObservableCollection<TSource> source,
            Func<TSource, int, TResult> selector,
            params IObservable<object>[] triggers)
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(selector, nameof(selector));
            return new MappingView<TSource, TResult>(source, selector,  null, triggers);
        }

        /// <summary>
        /// Create a <see cref="MappingView{TSource, TResult}"/> for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the collection.</typeparam>
        /// <typeparam name="TResult">The type of the elements in the resulting collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="selector">
        /// The function mapping an element of type <typeparamref name="TSource"/> to <typeparamref name="TResult"/>.</param>
        /// The second parameter is the index of the element.
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        /// <returns>A <see cref="FilteredView{T}"/></returns>
        public static MappingView<TSource, TResult> AsMappingView<TSource, TResult>(
            this ReadOnlyObservableCollection<TSource> source,
            Func<TSource, int, TResult> selector,
            IScheduler scheduler)
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(selector, nameof(selector));
            return new MappingView<TSource, TResult>(source, selector, scheduler, null);
        }

        public static MappingView<TSource, TResult> AsMappingView<TSource, TResult>(
            this ReadOnlyObservableCollection<TSource> source,
            Func<TSource, int, TResult> selector,
            Func<TResult, int, TResult> updater,
            params IObservable<object>[] triggers)
            where TSource : class
            where TResult : class
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(selector, nameof(selector));
            Ensure.NotNull(updater, nameof(updater));
            return new MappingView<TSource, TResult>(source, selector, updater, null, triggers);
        }

        public static MappingView<TSource, TResult> AsMappingView<TSource, TResult>(
            this ReadOnlyObservableCollection<TSource> source,
            Func<TSource, int, TResult> selector,
            Func<TResult, int, TResult> updater,
            IScheduler scheduler)
            where TSource : class
            where TResult : class
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(selector, nameof(selector));
            Ensure.NotNull(updater, nameof(updater));
            return new MappingView<TSource, TResult>(source, selector, updater, scheduler);
        }

        /// <summary>
        /// Create a <see cref="MappingView{TSource, TResult}"/> for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the collection.</typeparam>
        /// <typeparam name="TResult">The type of the elements in the resulting collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="selector">The function mapping an element of type <typeparamref name="TSource"/> to <typeparamref name="TResult"/>.</param>
        /// <param name="triggers">Additional triggers for when mapping is updated.</param>
        /// <returns>A <see cref="FilteredView{T}"/></returns>
        public static MappingView<TSource, TResult> AsMappingView<TSource, TResult>(
            this IReadOnlyObservableCollection<TSource> source,
            Func<TSource, TResult> selector,
            params IObservable<object>[] triggers)
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(selector, nameof(selector));
            return new MappingView<TSource, TResult>(source, selector,  null, triggers);
        }

        /// <summary>
        /// Create a <see cref="MappingView{TSource, TResult}"/> for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the collection.</typeparam>
        /// <typeparam name="TResult">The type of the elements in the resulting collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="selector">The function mapping an element of type <typeparamref name="TSource"/> to <typeparamref name="TResult"/>.</param>
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        /// <returns>A <see cref="FilteredView{T}"/></returns>
        public static MappingView<TSource, TResult> AsMappingView<TSource, TResult>(
            this IReadOnlyObservableCollection<TSource> source,
            Func<TSource, TResult> selector,
            IScheduler scheduler)
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(selector, nameof(selector));
            return new MappingView<TSource, TResult>(source, selector, scheduler);
        }

        /// <summary>
        /// Create a <see cref="MappingView{TSource, TResult}"/> for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the collection.</typeparam>
        /// <typeparam name="TResult">The type of the elements in the resulting collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="selector">
        /// The function mapping an element of type <typeparamref name="TSource"/> to <typeparamref name="TResult"/>.</param>
        /// The second parameter is the index of the element.
        /// <param name="triggers">Additional triggers for when mapping is updated.</param>
        /// <returns>A <see cref="FilteredView{T}"/></returns>
        public static MappingView<TSource, TResult> AsMappingView<TSource, TResult>(
            this IReadOnlyObservableCollection<TSource> source,
            Func<TSource, int, TResult> selector,
            params IObservable<object>[] triggers)
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(selector, nameof(selector));
            return new MappingView<TSource, TResult>(source, selector,  null, triggers);
        }

        /// <summary>
        /// Create a <see cref="MappingView{TSource, TResult}"/> for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the collection.</typeparam>
        /// <typeparam name="TResult">The type of the elements in the resulting collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="selector">
        /// The function mapping an element of type <typeparamref name="TSource"/> to <typeparamref name="TResult"/>.</param>
        /// The second parameter is the index of the element.
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        /// <returns>A <see cref="FilteredView{T}"/></returns>
        public static MappingView<TSource, TResult> AsMappingView<TSource, TResult>(
            this IReadOnlyObservableCollection<TSource> source,
            Func<TSource, int, TResult> selector,
            IScheduler scheduler)
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(selector, nameof(selector));
            return new MappingView<TSource, TResult>(source, selector, scheduler, null);
        }

        public static MappingView<TSource, TResult> AsMappingView<TSource, TResult>(
            this IReadOnlyObservableCollection<TSource> source,
            Func<TSource, int, TResult> selector,
            Func<TResult, int, TResult> updater,
            params IObservable<object>[] triggers)
            where TSource : class
            where TResult : class
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(selector, nameof(selector));
            Ensure.NotNull(updater, nameof(updater));
            return new MappingView<TSource, TResult>(source, selector, updater, null, triggers);
        }

        public static MappingView<TSource, TResult> AsMappingView<TSource, TResult>(
            this IReadOnlyObservableCollection<TSource> source,
            Func<TSource, int, TResult> selector,
            Func<TResult, int, TResult> updater,
            IScheduler scheduler)
            where TSource : class
            where TResult : class
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(selector, nameof(selector));
            Ensure.NotNull(updater, nameof(updater));
            return new MappingView<TSource, TResult>(source, selector, updater, scheduler);
        }
    }
}