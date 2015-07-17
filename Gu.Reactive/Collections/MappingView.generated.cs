namespace Gu.Reactive
{
    using System;
    using System.Collections.ObjectModel;
    using System.Reactive.Concurrency;
    using Gu.Reactive.Internals;

    public static partial class MappingView
    {
        public static MappingView<TSource, TResult> AsMappingView<TSource, TResult>(
            this ObservableCollection<TSource> source,
            Func<TSource, TResult> selector,
            params IObservable<object>[] triggers)
        {
			Ensure.NotNull(source, "source");
			Ensure.NotNull(selector, "selector");
            return new MappingView<TSource, TResult>(source, selector, null, triggers);
        }

        public static MappingView<TSource, TResult> AsMappingView<TSource, TResult>(
            this ObservableCollection<TSource> source,
            Func<TSource, TResult> selector,
            IScheduler scheduler)
        {
			Ensure.NotNull(source, "source");
			Ensure.NotNull(selector, "selector");
            return new MappingView<TSource, TResult>(source, selector, scheduler);
        }

        public static MappingView<TSource, TResult> AsMappingView<TSource, TResult>(
            this ObservableCollection<TSource> source,
            Func<TSource, int, TResult> selector,
            params IObservable<object>[] triggers)
        {
			Ensure.NotNull(source, "source");
			Ensure.NotNull(selector, "selector");
            return new MappingView<TSource, TResult>(source, selector,  null, triggers);
        }

        public static MappingView<TSource, TResult> AsMappingView<TSource, TResult>(
            this ObservableCollection<TSource> source,
            Func<TSource, int, TResult> selector,
            IScheduler scheduler)
        {
			Ensure.NotNull(source, "source");
			Ensure.NotNull(selector, "selector");
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
			Ensure.NotNull(source, "source");
			Ensure.NotNull(selector, "selector");
			Ensure.NotNull(updater, "updater");
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
			Ensure.NotNull(source, "source");
			Ensure.NotNull(selector, "selector");
			Ensure.NotNull(updater, "updater");
            return new MappingView<TSource, TResult>(source, selector, updater, scheduler);
        }

        public static MappingView<TSource, TResult> AsMappingView<TSource, TResult>(
            this ReadOnlyObservableCollection<TSource> source,
            Func<TSource, TResult> selector,
            params IObservable<object>[] triggers)
        {
			Ensure.NotNull(source, "source");
			Ensure.NotNull(selector, "selector");
            return new MappingView<TSource, TResult>(source, selector,  null, triggers);
        }

        public static MappingView<TSource, TResult> AsMappingView<TSource, TResult>(
            this ReadOnlyObservableCollection<TSource> source,
            Func<TSource, TResult> selector,
            IScheduler scheduler)
        {
			Ensure.NotNull(source, "source");
			Ensure.NotNull(selector, "selector");
            return new MappingView<TSource, TResult>(source, selector, scheduler);
        }

        public static MappingView<TSource, TResult> AsMappingView<TSource, TResult>(
            this ReadOnlyObservableCollection<TSource> source,
            Func<TSource, int, TResult> selector,
            params IObservable<object>[] triggers)
        {
			Ensure.NotNull(source, "source");
			Ensure.NotNull(selector, "selector");
            return new MappingView<TSource, TResult>(source, selector,  null, triggers);
        }

        public static MappingView<TSource, TResult> AsMappingView<TSource, TResult>(
            this ReadOnlyObservableCollection<TSource> source,
            Func<TSource, int, TResult> selector,
            IScheduler scheduler)
        {
			Ensure.NotNull(source, "source");
			Ensure.NotNull(selector, "selector");
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
			Ensure.NotNull(source, "source");
			Ensure.NotNull(selector, "selector");
			Ensure.NotNull(updater, "updater");
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
			Ensure.NotNull(source, "source");
			Ensure.NotNull(selector, "selector");
			Ensure.NotNull(updater, "updater");
            return new MappingView<TSource, TResult>(source, selector, updater, scheduler);
        }

        public static MappingView<TSource, TResult> AsMappingView<TSource, TResult>(
            this IReadOnlyObservableCollection<TSource> source,
            Func<TSource, TResult> selector,
            params IObservable<object>[] triggers)
        {
			Ensure.NotNull(source, "source");
			Ensure.NotNull(selector, "selector");
            return new MappingView<TSource, TResult>(source, selector,  null, triggers);
        }

        public static MappingView<TSource, TResult> AsMappingView<TSource, TResult>(
            this IReadOnlyObservableCollection<TSource> source,
            Func<TSource, TResult> selector,
            IScheduler scheduler)
        {
			Ensure.NotNull(source, "source");
			Ensure.NotNull(selector, "selector");
            return new MappingView<TSource, TResult>(source, selector, scheduler);
        }

        public static MappingView<TSource, TResult> AsMappingView<TSource, TResult>(
            this IReadOnlyObservableCollection<TSource> source,
            Func<TSource, int, TResult> selector,
            params IObservable<object>[] triggers)
        {
			Ensure.NotNull(source, "source");
			Ensure.NotNull(selector, "selector");
            return new MappingView<TSource, TResult>(source, selector,  null, triggers);
        }

        public static MappingView<TSource, TResult> AsMappingView<TSource, TResult>(
            this IReadOnlyObservableCollection<TSource> source,
            Func<TSource, int, TResult> selector,
            IScheduler scheduler)
        {
			Ensure.NotNull(source, "source");
			Ensure.NotNull(selector, "selector");
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
			Ensure.NotNull(source, "source");
			Ensure.NotNull(selector, "selector");
			Ensure.NotNull(updater, "updater");
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
			Ensure.NotNull(source, "source");
			Ensure.NotNull(selector, "selector");
			Ensure.NotNull(updater, "updater");
            return new MappingView<TSource, TResult>(source, selector, updater, scheduler);
        }
    }
}