namespace Gu.Reactive
{
    using System;
    using System.Collections.ObjectModel;
    using System.Reactive.Concurrency;
    using Gu.Reactive.Internals;

    public static partial class FilteredView
    {
        public static FilteredView<T> AsFilteredView<T>(
            this ObservableCollection<T> collection,
            Func<T, bool> filter,
            params IObservable<object>[] triggers)
        {
			Ensure.NotNull(collection, nameof(collection));
            return new FilteredView<T>(collection, filter, TimeSpan.Zero, null, triggers);
        }

        public static FilteredView<T> AsFilteredView<T>(
            this ObservableCollection<T> collection,
            Func<T, bool> filter,
            TimeSpan bufferTime,
            params IObservable<object>[] triggers)
        {
			Ensure.NotNull(collection, nameof(collection));
            Ensure.NotNull(filter, nameof(filter));
            return new FilteredView<T>(collection, filter, bufferTime, null, triggers);
        }

        public static FilteredView<T> AsFilteredView<T>(
            this ObservableCollection<T> collection,
            Func<T, bool> filter,
            IScheduler scheduler,
            params IObservable<object>[] triggers)
        {
		    Ensure.NotNull(collection, nameof(collection));
            Ensure.NotNull(filter, nameof(filter));
            Ensure.NotNull(scheduler, nameof(scheduler));
            return new FilteredView<T>(collection, filter, TimeSpan.Zero, scheduler, triggers);
        }

        public static FilteredView<T> AsFilteredView<T>(
            this ObservableCollection<T> collection,
            Func<T, bool> filter,
            TimeSpan bufferTime,
            IScheduler scheduler,
            params IObservable<object>[] triggers)
        {
		    Ensure.NotNull(collection, nameof(collection));
            Ensure.NotNull(filter, nameof(filter));
            Ensure.NotNull(scheduler, nameof(scheduler));
            return new FilteredView<T>(collection, filter, bufferTime, scheduler, triggers);
        }

        public static FilteredView<T> AsFilteredView<T>(
            this IObservableCollection<T> collection,
            Func<T, bool> filter,
            params IObservable<object>[] triggers)
        {
			Ensure.NotNull(collection, nameof(collection));
            return new FilteredView<T>(collection, filter, TimeSpan.Zero, null, triggers);
        }

        public static FilteredView<T> AsFilteredView<T>(
            this IObservableCollection<T> collection,
            Func<T, bool> filter,
            TimeSpan bufferTime,
            params IObservable<object>[] triggers)
        {
			Ensure.NotNull(collection, nameof(collection));
            Ensure.NotNull(filter, nameof(filter));
            return new FilteredView<T>(collection, filter, bufferTime, null, triggers);
        }

        public static FilteredView<T> AsFilteredView<T>(
            this IObservableCollection<T> collection,
            Func<T, bool> filter,
            IScheduler scheduler,
            params IObservable<object>[] triggers)
        {
		    Ensure.NotNull(collection, nameof(collection));
            Ensure.NotNull(filter, nameof(filter));
            Ensure.NotNull(scheduler, nameof(scheduler));
            return new FilteredView<T>(collection, filter, TimeSpan.Zero, scheduler, triggers);
        }

        public static FilteredView<T> AsFilteredView<T>(
            this IObservableCollection<T> collection,
            Func<T, bool> filter,
            TimeSpan bufferTime,
            IScheduler scheduler,
            params IObservable<object>[] triggers)
        {
		    Ensure.NotNull(collection, nameof(collection));
            Ensure.NotNull(filter, nameof(filter));
            Ensure.NotNull(scheduler, nameof(scheduler));
            return new FilteredView<T>(collection, filter, bufferTime, scheduler, triggers);
        }
    }
}