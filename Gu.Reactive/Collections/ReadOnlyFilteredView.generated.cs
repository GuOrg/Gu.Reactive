namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Reactive.Concurrency;
    using Gu.Reactive.Internals;

    public static partial class ReadOnlyFilteredView
    {
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this ObservableCollection<T> collection,
            Func<T, bool> filter,
            params IObservable<object>[] triggers)
        {
			Ensure.NotNull(collection, "collection");
            return new ReadOnlyFilteredView<T>(collection, filter, TimeSpan.Zero, null, triggers);
        }

        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this ObservableCollection<T> collection,
            Func<T, bool> filter,
            TimeSpan bufferTime,
            params IObservable<object>[] triggers)
        {
			Ensure.NotNull(collection, "collection");
            Ensure.NotNull(filter, "filter");
            return new ReadOnlyFilteredView<T>(collection, filter, bufferTime, null, triggers);
        }

        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this ObservableCollection<T> collection,
            Func<T, bool> filter,
            IScheduler scheduler,
            params IObservable<object>[] triggers)
        {
		    Ensure.NotNull(collection, "collection");
            Ensure.NotNull(filter, "filter");
            Ensure.NotNull(scheduler, "scheduler");
            return new ReadOnlyFilteredView<T>(collection, filter, TimeSpan.Zero, scheduler, triggers);
        }
       
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this ObservableCollection<T> collection,
            Func<T, bool> filter,
            TimeSpan bufferTime,
            IScheduler scheduler,
            params IObservable<object>[] triggers)
        {
		    Ensure.NotNull(collection, "collection");
            Ensure.NotNull(filter, "filter");
            Ensure.NotNull(scheduler, "scheduler");
            return new ReadOnlyFilteredView<T>(collection, filter, bufferTime, scheduler, triggers);
        }

        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this ReadOnlyObservableCollection<T> collection,
            Func<T, bool> filter,
            params IObservable<object>[] triggers)
        {
			Ensure.NotNull(collection, "collection");
            return new ReadOnlyFilteredView<T>(collection, filter, TimeSpan.Zero, null, triggers);
        }

        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this ReadOnlyObservableCollection<T> collection,
            Func<T, bool> filter,
            TimeSpan bufferTime,
            params IObservable<object>[] triggers)
        {
			Ensure.NotNull(collection, "collection");
            Ensure.NotNull(filter, "filter");
            return new ReadOnlyFilteredView<T>(collection, filter, bufferTime, null, triggers);
        }

        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this ReadOnlyObservableCollection<T> collection,
            Func<T, bool> filter,
            IScheduler scheduler,
            params IObservable<object>[] triggers)
        {
		    Ensure.NotNull(collection, "collection");
            Ensure.NotNull(filter, "filter");
            Ensure.NotNull(scheduler, "scheduler");
            return new ReadOnlyFilteredView<T>(collection, filter, TimeSpan.Zero, scheduler, triggers);
        }
       
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this ReadOnlyObservableCollection<T> collection,
            Func<T, bool> filter,
            TimeSpan bufferTime,
            IScheduler scheduler,
            params IObservable<object>[] triggers)
        {
		    Ensure.NotNull(collection, "collection");
            Ensure.NotNull(filter, "filter");
            Ensure.NotNull(scheduler, "scheduler");
            return new ReadOnlyFilteredView<T>(collection, filter, bufferTime, scheduler, triggers);
        }

        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this IReadOnlyObservableCollection<T> collection,
            Func<T, bool> filter,
            params IObservable<object>[] triggers)
        {
			Ensure.NotNull(collection, "collection");
            return new ReadOnlyFilteredView<T>(collection, filter, TimeSpan.Zero, null, triggers);
        }

        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this IReadOnlyObservableCollection<T> collection,
            Func<T, bool> filter,
            TimeSpan bufferTime,
            params IObservable<object>[] triggers)
        {
			Ensure.NotNull(collection, "collection");
            Ensure.NotNull(filter, "filter");
            return new ReadOnlyFilteredView<T>(collection, filter, bufferTime, null, triggers);
        }

        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this IReadOnlyObservableCollection<T> collection,
            Func<T, bool> filter,
            IScheduler scheduler,
            params IObservable<object>[] triggers)
        {
		    Ensure.NotNull(collection, "collection");
            Ensure.NotNull(filter, "filter");
            Ensure.NotNull(scheduler, "scheduler");
            return new ReadOnlyFilteredView<T>(collection, filter, TimeSpan.Zero, scheduler, triggers);
        }
       
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this IReadOnlyObservableCollection<T> collection,
            Func<T, bool> filter,
            TimeSpan bufferTime,
            IScheduler scheduler,
            params IObservable<object>[] triggers)
        {
		    Ensure.NotNull(collection, "collection");
            Ensure.NotNull(filter, "filter");
            Ensure.NotNull(scheduler, "scheduler");
            return new ReadOnlyFilteredView<T>(collection, filter, bufferTime, scheduler, triggers);
        }

        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this IEnumerable<T> collection,
            Func<T, bool> filter,
            IEnumerable<IObservable<object>> triggers)
        {
			Ensure.NotNull(collection, "collection");
            Ensure.NotNull(filter, "filter");
			Ensure.NotNull(triggers, "triggers");
            return new ReadOnlyFilteredView<T>(collection, filter, TimeSpan.Zero, null, triggers);
        }

        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this IEnumerable<T> collection,
            Func<T, bool> filter,
            TimeSpan bufferTime,
            IEnumerable<IObservable<object>> triggers)
        {
			Ensure.NotNull(collection, "collection");
            Ensure.NotNull(filter, "filter");
			Ensure.NotNull(triggers, "triggers");
            return new ReadOnlyFilteredView<T>(collection, filter, bufferTime, null, triggers);
        }

        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this IEnumerable<T> collection,
            Func<T, bool> filter,
            IScheduler scheduler,
            IEnumerable<IObservable<object>> triggers)
        {
		    Ensure.NotNull(collection, "collection");
            Ensure.NotNull(filter, "filter");
            Ensure.NotNull(scheduler, "scheduler");
			Ensure.NotNull(triggers, "triggers");
            return new ReadOnlyFilteredView<T>(collection, filter, TimeSpan.Zero, scheduler, triggers);
        }
       
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this IEnumerable<T> collection,
            Func<T, bool> filter,
            TimeSpan bufferTime,
            IScheduler scheduler,
            IEnumerable<IObservable<object>> triggers)
        {
		    Ensure.NotNull(collection, "collection");
            Ensure.NotNull(filter, "filter");
            Ensure.NotNull(scheduler, "scheduler");
			Ensure.NotNull(triggers, "triggers");
            return new ReadOnlyFilteredView<T>(collection, filter, bufferTime, scheduler, triggers);
        }
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this IEnumerable<T> collection,
            Func<T, bool> filter,
            IObservable<object> trigger)
        {
			Ensure.NotNull(collection, "collection");
            Ensure.NotNull(filter, "filter");
			Ensure.NotNull(trigger, "trigger");
            return new ReadOnlyFilteredView<T>(collection, filter, TimeSpan.Zero, null, trigger);
        }

        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this IEnumerable<T> collection,
            Func<T, bool> filter,
            TimeSpan bufferTime,
            IObservable<object> trigger)
        {
			Ensure.NotNull(collection, "collection");
            Ensure.NotNull(filter, "filter");
			Ensure.NotNull(trigger, "trigger");
            return new ReadOnlyFilteredView<T>(collection, filter, bufferTime, null, trigger);
        }

        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this IEnumerable<T> collection,
            Func<T, bool> filter,
            IScheduler scheduler,
            IObservable<object> trigger)
        {
		    Ensure.NotNull(collection, "collection");
            Ensure.NotNull(filter, "filter");
            Ensure.NotNull(scheduler, "scheduler");
			Ensure.NotNull(trigger, "trigger");
            return new ReadOnlyFilteredView<T>(collection, filter, TimeSpan.Zero, scheduler, trigger);
        }
       
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this IEnumerable<T> collection,
            Func<T, bool> filter,
            TimeSpan bufferTime,
            IScheduler scheduler,
            IObservable<object> trigger)
        {
		    Ensure.NotNull(collection, "collection");
            Ensure.NotNull(filter, "filter");
            Ensure.NotNull(scheduler, "scheduler");
			Ensure.NotNull(trigger, "trigger");
            return new ReadOnlyFilteredView<T>(collection, filter, bufferTime, scheduler, trigger);
        }
    }
}