namespace Gu.Reactive
{
    using System;
    using System.Collections.ObjectModel;
    using System.Reactive.Concurrency;
    using Gu.Reactive.Internals;

    public static partial class MappingView
    {
        public static MappingView<TSource, TResult> AsMappingView<TSource, TResult>(
            this IObservableCollection<TSource> source,
            Func<TSource, TResult> selector,
            params IObservable<object>[] triggers) where TResult : class
        {
			Ensure.NotNull(source, "source");
			Ensure.NotNull(selector, "selector");
            return new MappingView<TSource, TResult>(source, selector, null);
        }

        public static MappingView<TSource, TResult> AsMappingView<TSource, TResult>(
            this IObservableCollection<TSource> source,
            Func<TSource, TResult> selector,
            IScheduler scheduler) where TResult : class
        {
			Ensure.NotNull(source, "source");
			Ensure.NotNull(selector, "selector");
            return new MappingView<TSource, TResult>(source, selector, scheduler);
        }

        public static MappingView<TSource, TResult> AsMappingView<TSource, TResult>(
            this IReadOnlyObservableCollection<TSource> source,
            Func<TSource, TResult> selector,
            params IObservable<object>[] triggers) where TResult : class
        {
			Ensure.NotNull(source, "source");
			Ensure.NotNull(selector, "selector");
            return new MappingView<TSource, TResult>(source, selector, null);
        }

        public static MappingView<TSource, TResult> AsMappingView<TSource, TResult>(
            this IReadOnlyObservableCollection<TSource> source,
            Func<TSource, TResult> selector,
            IScheduler scheduler) where TResult : class
        {
			Ensure.NotNull(source, "source");
			Ensure.NotNull(selector, "selector");
            return new MappingView<TSource, TResult>(source, selector, scheduler);
        }

        public static MappingView<TSource, TResult> AsMappingView<TSource, TResult>(
            this ObservableCollection<TSource> source,
            Func<TSource, TResult> selector,
            params IObservable<object>[] triggers) where TResult : class
        {
			Ensure.NotNull(source, "source");
			Ensure.NotNull(selector, "selector");
            return new MappingView<TSource, TResult>(source, selector, null);
        }

        public static MappingView<TSource, TResult> AsMappingView<TSource, TResult>(
            this ObservableCollection<TSource> source,
            Func<TSource, TResult> selector,
            IScheduler scheduler) where TResult : class
        {
			Ensure.NotNull(source, "source");
			Ensure.NotNull(selector, "selector");
            return new MappingView<TSource, TResult>(source, selector, scheduler);
        }

        public static MappingView<TSource, TResult> AsMappingView<TSource, TResult>(
            this ReadOnlyObservableCollection<TSource> source,
            Func<TSource, TResult> selector,
            params IObservable<object>[] triggers) where TResult : class
        {
			Ensure.NotNull(source, "source");
			Ensure.NotNull(selector, "selector");
            return new MappingView<TSource, TResult>(source, selector, null);
        }

        public static MappingView<TSource, TResult> AsMappingView<TSource, TResult>(
            this ReadOnlyObservableCollection<TSource> source,
            Func<TSource, TResult> selector,
            IScheduler scheduler) where TResult : class
        {
			Ensure.NotNull(source, "source");
			Ensure.NotNull(selector, "selector");
            return new MappingView<TSource, TResult>(source, selector, scheduler);
        }
    }
}