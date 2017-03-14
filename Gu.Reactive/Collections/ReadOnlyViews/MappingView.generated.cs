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
        /// <returns>A <see cref="MappingView{TSource, TResult}"/></returns>
        public static MappingView<TSource, TResult> AsMappingView<TSource, TResult>(
            this ObservableCollection<TSource> source,
            Func<TSource, TResult> selector,
            params IObservable<object>[] triggers)
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(selector, nameof(selector));
            return new MappingView<TSource, TResult>(source, null, Mapper.Create(selector), triggers);
        }

        /// <summary>
        /// Create a <see cref="MappingView{TSource, TResult}"/> for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the collection.</typeparam>
        /// <typeparam name="TResult">The type of the elements in the resulting collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="selector">The function mapping an element of type <typeparamref name="TSource"/> to <typeparamref name="TResult"/>.</param>
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        /// <param name="triggers">Additional triggers for when mapping is updated.</param>
        /// <returns>A <see cref="MappingView{TSource, TResult}"/></returns>
        public static MappingView<TSource, TResult> AsMappingView<TSource, TResult>(
            this ObservableCollection<TSource> source,
            Func<TSource, TResult> selector,
            IScheduler scheduler,
            params IObservable<object>[] triggers)
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(selector, nameof(selector));
            return new MappingView<TSource, TResult>(source, scheduler, Mapper.Create(selector), triggers);
        }

        /// <summary>
        /// Create a <see cref="MappingView{TSource, TResult}"/> for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the collection.</typeparam>
        /// <typeparam name="TResult">The type of the elements in the resulting collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="selector">The function mapping an element of type <typeparamref name="TSource"/> to <typeparamref name="TResult"/>.</param>
        /// <param name="onRemove">An action to perform when an item is removed from the collection. Typically it will be x => x.Dispose()</param>
        /// <param name="triggers">Additional triggers for when mapping is updated.</param>
        /// <returns>A <see cref="MappingView{TSource, TResult}"/></returns>
        public static MappingView<TSource, TResult> AsMappingView<TSource, TResult>(
            this ObservableCollection<TSource> source,
            Func<TSource, TResult> selector,
            Action<TResult> onRemove,
            params IObservable<object>[] triggers)
            where TResult : class
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(selector, nameof(selector));
            Ensure.NotNull(onRemove, nameof(onRemove));
            return new MappingView<TSource, TResult>(source, null, Mapper.Create(selector, onRemove), triggers);
        }

        /// <summary>
        /// Create a <see cref="MappingView{TSource, TResult}"/> for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the collection.</typeparam>
        /// <typeparam name="TResult">The type of the elements in the resulting collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="selector">The function mapping an element of type <typeparamref name="TSource"/> to <typeparamref name="TResult"/>.</param>
        /// <param name="onRemove">An action to perform when an item is removed from the collection. Typically it will be x => x.Dispose()</param>
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        /// <param name="triggers">Additional triggers for when mapping is updated.</param>
        /// <returns>A <see cref="MappingView{TSource, TResult}"/></returns>
        public static MappingView<TSource, TResult> AsMappingView<TSource, TResult>(
            this ObservableCollection<TSource> source,
            Func<TSource, TResult> selector,
            Action<TResult> onRemove,
            IScheduler scheduler,
            params IObservable<object>[] triggers)
            where TResult : class
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(selector, nameof(selector));
            Ensure.NotNull(onRemove, nameof(onRemove));
            return new MappingView<TSource, TResult>(source, scheduler, Mapper.Create(selector, onRemove), triggers);
        }

        /// <summary>
        /// Create a <see cref="MappingView{TSource, TResult}"/> for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the collection.</typeparam>
        /// <typeparam name="TResult">The type of the elements in the resulting collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="selector">
        /// The function mapping an element of type <typeparamref name="TSource"/> to <typeparamref name="TResult"/>.
        /// The second parameter is the index of the element.
        /// </param>
        /// <param name="updater">
        /// The function updating an element for which the index changed.
        /// The second parameter is the index of the element.
        /// </param>
        /// <param name="triggers">Additional triggers for when mapping is updated.</param>
        /// <returns>A <see cref="MappingView{TSource, TResult}"/></returns>
        public static MappingView<TSource, TResult> AsMappingView<TSource, TResult>(
            this ObservableCollection<TSource> source,
            Func<TSource, int, TResult> selector,
            Func<TResult, int, TResult> updater,
            params IObservable<object>[] triggers)
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(selector, nameof(selector));
            Ensure.NotNull(updater, nameof(updater));
            return new MappingView<TSource, TResult>(source, null, Mapper.Create(selector, updater), triggers);
        }

        /// <summary>
        /// Create a <see cref="MappingView{TSource, TResult}"/> for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the collection.</typeparam>
        /// <typeparam name="TResult">The type of the elements in the resulting collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="selector">
        /// The function mapping an element of type <typeparamref name="TSource"/> to <typeparamref name="TResult"/>.
        /// The second parameter is the index of the element.
        /// </param>
        /// <param name="updater">
        /// The function updating an element for which the index changed.
        /// The second parameter is the index of the element.
        /// </param>
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        /// <param name="triggers">Additional triggers for when mapping is updated.</param>
        /// <returns>A <see cref="MappingView{TSource, TResult}"/></returns>
        public static MappingView<TSource, TResult> AsMappingView<TSource, TResult>(
            this ObservableCollection<TSource> source,
            Func<TSource, int, TResult> selector,
            Func<TResult, int, TResult> updater,
            IScheduler scheduler,
            params IObservable<object>[] triggers)
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(selector, nameof(selector));
            Ensure.NotNull(updater, nameof(updater));
            return new MappingView<TSource, TResult>(source, scheduler, Mapper.Create(selector, updater), triggers);
        }

        /// <summary>
        /// Create a <see cref="MappingView{TSource, TResult}"/> for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the collection.</typeparam>
        /// <typeparam name="TResult">The type of the elements in the resulting collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="selector">
        /// The function mapping an element of type <typeparamref name="TSource"/> to <typeparamref name="TResult"/>.
        /// The second parameter is the index of the element.
        /// </param>
        /// <param name="updater">
        /// The function updating an element for which the index changed.
        /// The second parameter is the index of the element.
        /// </param>
        /// <param name="onRemove">An action to perform when an item is removed from the collection. Typically it will be x => x.Dispose()</param>
        /// <param name="triggers">Additional triggers for when mapping is updated.</param>
        /// <returns>A <see cref="MappingView{TSource, TResult}"/></returns>
        public static MappingView<TSource, TResult> AsMappingView<TSource, TResult>(
            this ObservableCollection<TSource> source,
            Func<TSource, int, TResult> selector,
            Func<TResult, int, TResult> updater,
            Action<TResult> onRemove,
            params IObservable<object>[] triggers)
            where TResult : class
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(selector, nameof(selector));
            Ensure.NotNull(updater, nameof(updater));
            return new MappingView<TSource, TResult>(source, null, Mapper.Create(selector, updater, onRemove), triggers);
        }

        /// <summary>
        /// Create a <see cref="MappingView{TSource, TResult}"/> for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the collection.</typeparam>
        /// <typeparam name="TResult">The type of the elements in the resulting collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="selector">
        /// The function mapping an element of type <typeparamref name="TSource"/> to <typeparamref name="TResult"/>.
        /// The second parameter is the index of the element.
        /// </param>
        /// <param name="updater">
        /// The function updating an element for which the index changed.
        /// The second parameter is the index of the element.
        /// </param>
        /// <param name="onRemove">An action to perform when an item is removed from the collection. Typically it will be x => x.Dispose()</param>
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        /// <param name="triggers">Additional triggers for when mapping is updated.</param>
        /// <returns>A <see cref="MappingView{TSource, TResult}"/></returns>
        public static MappingView<TSource, TResult> AsMappingView<TSource, TResult>(
            this ObservableCollection<TSource> source,
            Func<TSource, int, TResult> selector,
            Func<TResult, int, TResult> updater,
            Action<TResult> onRemove,
            IScheduler scheduler,
            params IObservable<object>[] triggers)
            where TResult : class
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(selector, nameof(selector));
            Ensure.NotNull(updater, nameof(updater));
            return new MappingView<TSource, TResult>(source, scheduler, Mapper.Create(selector, updater, onRemove), triggers);
        }

        /// <summary>
        /// Create a <see cref="MappingView{TSource, TResult}"/> for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the collection.</typeparam>
        /// <typeparam name="TResult">The type of the elements in the resulting collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="selector">The function mapping an element of type <typeparamref name="TSource"/> to <typeparamref name="TResult"/>.</param>
        /// <param name="triggers">Additional triggers for when mapping is updated.</param>
        /// <returns>A <see cref="MappingView{TSource, TResult}"/></returns>
        public static MappingView<TSource, TResult> AsMappingView<TSource, TResult>(
            this ReadOnlyObservableCollection<TSource> source,
            Func<TSource, TResult> selector,
            params IObservable<object>[] triggers)
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(selector, nameof(selector));
            return new MappingView<TSource, TResult>(source, null, Mapper.Create(selector), triggers);
        }

        /// <summary>
        /// Create a <see cref="MappingView{TSource, TResult}"/> for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the collection.</typeparam>
        /// <typeparam name="TResult">The type of the elements in the resulting collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="selector">The function mapping an element of type <typeparamref name="TSource"/> to <typeparamref name="TResult"/>.</param>
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        /// <param name="triggers">Additional triggers for when mapping is updated.</param>
        /// <returns>A <see cref="MappingView{TSource, TResult}"/></returns>
        public static MappingView<TSource, TResult> AsMappingView<TSource, TResult>(
            this ReadOnlyObservableCollection<TSource> source,
            Func<TSource, TResult> selector,
            IScheduler scheduler,
            params IObservable<object>[] triggers)
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(selector, nameof(selector));
            return new MappingView<TSource, TResult>(source, scheduler, Mapper.Create(selector), triggers);
        }

        /// <summary>
        /// Create a <see cref="MappingView{TSource, TResult}"/> for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the collection.</typeparam>
        /// <typeparam name="TResult">The type of the elements in the resulting collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="selector">The function mapping an element of type <typeparamref name="TSource"/> to <typeparamref name="TResult"/>.</param>
        /// <param name="onRemove">An action to perform when an item is removed from the collection. Typically it will be x => x.Dispose()</param>
        /// <param name="triggers">Additional triggers for when mapping is updated.</param>
        /// <returns>A <see cref="MappingView{TSource, TResult}"/></returns>
        public static MappingView<TSource, TResult> AsMappingView<TSource, TResult>(
            this ReadOnlyObservableCollection<TSource> source,
            Func<TSource, TResult> selector,
            Action<TResult> onRemove,
            params IObservable<object>[] triggers)
            where TResult : class
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(selector, nameof(selector));
            Ensure.NotNull(onRemove, nameof(onRemove));
            return new MappingView<TSource, TResult>(source, null, Mapper.Create(selector, onRemove), triggers);
        }

        /// <summary>
        /// Create a <see cref="MappingView{TSource, TResult}"/> for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the collection.</typeparam>
        /// <typeparam name="TResult">The type of the elements in the resulting collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="selector">The function mapping an element of type <typeparamref name="TSource"/> to <typeparamref name="TResult"/>.</param>
        /// <param name="onRemove">An action to perform when an item is removed from the collection. Typically it will be x => x.Dispose()</param>
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        /// <param name="triggers">Additional triggers for when mapping is updated.</param>
        /// <returns>A <see cref="MappingView{TSource, TResult}"/></returns>
        public static MappingView<TSource, TResult> AsMappingView<TSource, TResult>(
            this ReadOnlyObservableCollection<TSource> source,
            Func<TSource, TResult> selector,
            Action<TResult> onRemove,
            IScheduler scheduler,
            params IObservable<object>[] triggers)
            where TResult : class
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(selector, nameof(selector));
            Ensure.NotNull(onRemove, nameof(onRemove));
            return new MappingView<TSource, TResult>(source, scheduler, Mapper.Create(selector, onRemove), triggers);
        }

        /// <summary>
        /// Create a <see cref="MappingView{TSource, TResult}"/> for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the collection.</typeparam>
        /// <typeparam name="TResult">The type of the elements in the resulting collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="selector">
        /// The function mapping an element of type <typeparamref name="TSource"/> to <typeparamref name="TResult"/>.
        /// The second parameter is the index of the element.
        /// </param>
        /// <param name="updater">
        /// The function updating an element for which the index changed.
        /// The second parameter is the index of the element.
        /// </param>
        /// <param name="triggers">Additional triggers for when mapping is updated.</param>
        /// <returns>A <see cref="MappingView{TSource, TResult}"/></returns>
        public static MappingView<TSource, TResult> AsMappingView<TSource, TResult>(
            this ReadOnlyObservableCollection<TSource> source,
            Func<TSource, int, TResult> selector,
            Func<TResult, int, TResult> updater,
            params IObservable<object>[] triggers)
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(selector, nameof(selector));
            Ensure.NotNull(updater, nameof(updater));
            return new MappingView<TSource, TResult>(source, null, Mapper.Create(selector, updater), triggers);
        }

        /// <summary>
        /// Create a <see cref="MappingView{TSource, TResult}"/> for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the collection.</typeparam>
        /// <typeparam name="TResult">The type of the elements in the resulting collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="selector">
        /// The function mapping an element of type <typeparamref name="TSource"/> to <typeparamref name="TResult"/>.
        /// The second parameter is the index of the element.
        /// </param>
        /// <param name="updater">
        /// The function updating an element for which the index changed.
        /// The second parameter is the index of the element.
        /// </param>
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        /// <param name="triggers">Additional triggers for when mapping is updated.</param>
        /// <returns>A <see cref="MappingView{TSource, TResult}"/></returns>
        public static MappingView<TSource, TResult> AsMappingView<TSource, TResult>(
            this ReadOnlyObservableCollection<TSource> source,
            Func<TSource, int, TResult> selector,
            Func<TResult, int, TResult> updater,
            IScheduler scheduler,
            params IObservable<object>[] triggers)
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(selector, nameof(selector));
            Ensure.NotNull(updater, nameof(updater));
            return new MappingView<TSource, TResult>(source, scheduler, Mapper.Create(selector, updater), triggers);
        }

        /// <summary>
        /// Create a <see cref="MappingView{TSource, TResult}"/> for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the collection.</typeparam>
        /// <typeparam name="TResult">The type of the elements in the resulting collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="selector">
        /// The function mapping an element of type <typeparamref name="TSource"/> to <typeparamref name="TResult"/>.
        /// The second parameter is the index of the element.
        /// </param>
        /// <param name="updater">
        /// The function updating an element for which the index changed.
        /// The second parameter is the index of the element.
        /// </param>
        /// <param name="onRemove">An action to perform when an item is removed from the collection. Typically it will be x => x.Dispose()</param>
        /// <param name="triggers">Additional triggers for when mapping is updated.</param>
        /// <returns>A <see cref="MappingView{TSource, TResult}"/></returns>
        public static MappingView<TSource, TResult> AsMappingView<TSource, TResult>(
            this ReadOnlyObservableCollection<TSource> source,
            Func<TSource, int, TResult> selector,
            Func<TResult, int, TResult> updater,
            Action<TResult> onRemove,
            params IObservable<object>[] triggers)
            where TResult : class
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(selector, nameof(selector));
            Ensure.NotNull(updater, nameof(updater));
            return new MappingView<TSource, TResult>(source, null, Mapper.Create(selector, updater, onRemove), triggers);
        }

        /// <summary>
        /// Create a <see cref="MappingView{TSource, TResult}"/> for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the collection.</typeparam>
        /// <typeparam name="TResult">The type of the elements in the resulting collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="selector">
        /// The function mapping an element of type <typeparamref name="TSource"/> to <typeparamref name="TResult"/>.
        /// The second parameter is the index of the element.
        /// </param>
        /// <param name="updater">
        /// The function updating an element for which the index changed.
        /// The second parameter is the index of the element.
        /// </param>
        /// <param name="onRemove">An action to perform when an item is removed from the collection. Typically it will be x => x.Dispose()</param>
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        /// <param name="triggers">Additional triggers for when mapping is updated.</param>
        /// <returns>A <see cref="MappingView{TSource, TResult}"/></returns>
        public static MappingView<TSource, TResult> AsMappingView<TSource, TResult>(
            this ReadOnlyObservableCollection<TSource> source,
            Func<TSource, int, TResult> selector,
            Func<TResult, int, TResult> updater,
            Action<TResult> onRemove,
            IScheduler scheduler,
            params IObservable<object>[] triggers)
            where TResult : class
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(selector, nameof(selector));
            Ensure.NotNull(updater, nameof(updater));
            return new MappingView<TSource, TResult>(source, scheduler, Mapper.Create(selector, updater, onRemove), triggers);
        }

        /// <summary>
        /// Create a <see cref="MappingView{TSource, TResult}"/> for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the collection.</typeparam>
        /// <typeparam name="TResult">The type of the elements in the resulting collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="selector">The function mapping an element of type <typeparamref name="TSource"/> to <typeparamref name="TResult"/>.</param>
        /// <param name="triggers">Additional triggers for when mapping is updated.</param>
        /// <returns>A <see cref="MappingView{TSource, TResult}"/></returns>
        public static MappingView<TSource, TResult> AsMappingView<TSource, TResult>(
            this IReadOnlyObservableCollection<TSource> source,
            Func<TSource, TResult> selector,
            params IObservable<object>[] triggers)
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(selector, nameof(selector));
            return new MappingView<TSource, TResult>(source, null, Mapper.Create(selector), triggers);
        }

        /// <summary>
        /// Create a <see cref="MappingView{TSource, TResult}"/> for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the collection.</typeparam>
        /// <typeparam name="TResult">The type of the elements in the resulting collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="selector">The function mapping an element of type <typeparamref name="TSource"/> to <typeparamref name="TResult"/>.</param>
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        /// <param name="triggers">Additional triggers for when mapping is updated.</param>
        /// <returns>A <see cref="MappingView{TSource, TResult}"/></returns>
        public static MappingView<TSource, TResult> AsMappingView<TSource, TResult>(
            this IReadOnlyObservableCollection<TSource> source,
            Func<TSource, TResult> selector,
            IScheduler scheduler,
            params IObservable<object>[] triggers)
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(selector, nameof(selector));
            return new MappingView<TSource, TResult>(source, scheduler, Mapper.Create(selector), triggers);
        }

        /// <summary>
        /// Create a <see cref="MappingView{TSource, TResult}"/> for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the collection.</typeparam>
        /// <typeparam name="TResult">The type of the elements in the resulting collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="selector">The function mapping an element of type <typeparamref name="TSource"/> to <typeparamref name="TResult"/>.</param>
        /// <param name="onRemove">An action to perform when an item is removed from the collection. Typically it will be x => x.Dispose()</param>
        /// <param name="triggers">Additional triggers for when mapping is updated.</param>
        /// <returns>A <see cref="MappingView{TSource, TResult}"/></returns>
        public static MappingView<TSource, TResult> AsMappingView<TSource, TResult>(
            this IReadOnlyObservableCollection<TSource> source,
            Func<TSource, TResult> selector,
            Action<TResult> onRemove,
            params IObservable<object>[] triggers)
            where TResult : class
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(selector, nameof(selector));
            Ensure.NotNull(onRemove, nameof(onRemove));
            return new MappingView<TSource, TResult>(source, null, Mapper.Create(selector, onRemove), triggers);
        }

        /// <summary>
        /// Create a <see cref="MappingView{TSource, TResult}"/> for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the collection.</typeparam>
        /// <typeparam name="TResult">The type of the elements in the resulting collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="selector">The function mapping an element of type <typeparamref name="TSource"/> to <typeparamref name="TResult"/>.</param>
        /// <param name="onRemove">An action to perform when an item is removed from the collection. Typically it will be x => x.Dispose()</param>
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        /// <param name="triggers">Additional triggers for when mapping is updated.</param>
        /// <returns>A <see cref="MappingView{TSource, TResult}"/></returns>
        public static MappingView<TSource, TResult> AsMappingView<TSource, TResult>(
            this IReadOnlyObservableCollection<TSource> source,
            Func<TSource, TResult> selector,
            Action<TResult> onRemove,
            IScheduler scheduler,
            params IObservable<object>[] triggers)
            where TResult : class
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(selector, nameof(selector));
            Ensure.NotNull(onRemove, nameof(onRemove));
            return new MappingView<TSource, TResult>(source, scheduler, Mapper.Create(selector, onRemove), triggers);
        }

        /// <summary>
        /// Create a <see cref="MappingView{TSource, TResult}"/> for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the collection.</typeparam>
        /// <typeparam name="TResult">The type of the elements in the resulting collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="selector">
        /// The function mapping an element of type <typeparamref name="TSource"/> to <typeparamref name="TResult"/>.
        /// The second parameter is the index of the element.
        /// </param>
        /// <param name="updater">
        /// The function updating an element for which the index changed.
        /// The second parameter is the index of the element.
        /// </param>
        /// <param name="triggers">Additional triggers for when mapping is updated.</param>
        /// <returns>A <see cref="MappingView{TSource, TResult}"/></returns>
        public static MappingView<TSource, TResult> AsMappingView<TSource, TResult>(
            this IReadOnlyObservableCollection<TSource> source,
            Func<TSource, int, TResult> selector,
            Func<TResult, int, TResult> updater,
            params IObservable<object>[] triggers)
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(selector, nameof(selector));
            Ensure.NotNull(updater, nameof(updater));
            return new MappingView<TSource, TResult>(source, null, Mapper.Create(selector, updater), triggers);
        }

        /// <summary>
        /// Create a <see cref="MappingView{TSource, TResult}"/> for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the collection.</typeparam>
        /// <typeparam name="TResult">The type of the elements in the resulting collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="selector">
        /// The function mapping an element of type <typeparamref name="TSource"/> to <typeparamref name="TResult"/>.
        /// The second parameter is the index of the element.
        /// </param>
        /// <param name="updater">
        /// The function updating an element for which the index changed.
        /// The second parameter is the index of the element.
        /// </param>
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        /// <param name="triggers">Additional triggers for when mapping is updated.</param>
        /// <returns>A <see cref="MappingView{TSource, TResult}"/></returns>
        public static MappingView<TSource, TResult> AsMappingView<TSource, TResult>(
            this IReadOnlyObservableCollection<TSource> source,
            Func<TSource, int, TResult> selector,
            Func<TResult, int, TResult> updater,
            IScheduler scheduler,
            params IObservable<object>[] triggers)
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(selector, nameof(selector));
            Ensure.NotNull(updater, nameof(updater));
            return new MappingView<TSource, TResult>(source, scheduler, Mapper.Create(selector, updater), triggers);
        }

        /// <summary>
        /// Create a <see cref="MappingView{TSource, TResult}"/> for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the collection.</typeparam>
        /// <typeparam name="TResult">The type of the elements in the resulting collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="selector">
        /// The function mapping an element of type <typeparamref name="TSource"/> to <typeparamref name="TResult"/>.
        /// The second parameter is the index of the element.
        /// </param>
        /// <param name="updater">
        /// The function updating an element for which the index changed.
        /// The second parameter is the index of the element.
        /// </param>
        /// <param name="onRemove">An action to perform when an item is removed from the collection. Typically it will be x => x.Dispose()</param>
        /// <param name="triggers">Additional triggers for when mapping is updated.</param>
        /// <returns>A <see cref="MappingView{TSource, TResult}"/></returns>
        public static MappingView<TSource, TResult> AsMappingView<TSource, TResult>(
            this IReadOnlyObservableCollection<TSource> source,
            Func<TSource, int, TResult> selector,
            Func<TResult, int, TResult> updater,
            Action<TResult> onRemove,
            params IObservable<object>[] triggers)
            where TResult : class
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(selector, nameof(selector));
            Ensure.NotNull(updater, nameof(updater));
            return new MappingView<TSource, TResult>(source, null, Mapper.Create(selector, updater, onRemove), triggers);
        }

        /// <summary>
        /// Create a <see cref="MappingView{TSource, TResult}"/> for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the collection.</typeparam>
        /// <typeparam name="TResult">The type of the elements in the resulting collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="selector">
        /// The function mapping an element of type <typeparamref name="TSource"/> to <typeparamref name="TResult"/>.
        /// The second parameter is the index of the element.
        /// </param>
        /// <param name="updater">
        /// The function updating an element for which the index changed.
        /// The second parameter is the index of the element.
        /// </param>
        /// <param name="onRemove">An action to perform when an item is removed from the collection. Typically it will be x => x.Dispose()</param>
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        /// <param name="triggers">Additional triggers for when mapping is updated.</param>
        /// <returns>A <see cref="MappingView{TSource, TResult}"/></returns>
        public static MappingView<TSource, TResult> AsMappingView<TSource, TResult>(
            this IReadOnlyObservableCollection<TSource> source,
            Func<TSource, int, TResult> selector,
            Func<TResult, int, TResult> updater,
            Action<TResult> onRemove,
            IScheduler scheduler,
            params IObservable<object>[] triggers)
            where TResult : class
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(selector, nameof(selector));
            Ensure.NotNull(updater, nameof(updater));
            return new MappingView<TSource, TResult>(source, scheduler, Mapper.Create(selector, updater, onRemove), triggers);
        }
    }
}