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
			Ensure.NotNull(collection, nameof(collection));
            return new ReadOnlyFilteredView<T>(collection, filter, TimeSpan.Zero, null, triggers);
        }

        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this ObservableCollection<T> collection,
            Func<T, bool> filter,
            TimeSpan bufferTime,
            params IObservable<object>[] triggers)
        {
			Ensure.NotNull(collection, nameof(collection));
            Ensure.NotNull(filter, nameof(filter));
            return new ReadOnlyFilteredView<T>(collection, filter, bufferTime, null, triggers);
        }

        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this ObservableCollection<T> collection,
            Func<T, bool> filter,
            IScheduler scheduler,
            params IObservable<object>[] triggers)
        {
		    Ensure.NotNull(collection, nameof(collection));
            Ensure.NotNull(filter, nameof(filter));
            Ensure.NotNull(scheduler, nameof(scheduler));
            return new ReadOnlyFilteredView<T>(collection, filter, TimeSpan.Zero, scheduler, triggers);
        }
       
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this ObservableCollection<T> collection,
            Func<T, bool> filter,
            TimeSpan bufferTime,
            IScheduler scheduler,
            params IObservable<object>[] triggers)
        {
		    Ensure.NotNull(collection, nameof(collection));
            Ensure.NotNull(filter, nameof(filter));
            Ensure.NotNull(scheduler, nameof(scheduler));
            return new ReadOnlyFilteredView<T>(collection, filter, bufferTime, scheduler, triggers);
        }

        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this ReadOnlyObservableCollection<T> collection,
            Func<T, bool> filter,
            params IObservable<object>[] triggers)
        {
			Ensure.NotNull(collection, nameof(collection));
            return new ReadOnlyFilteredView<T>(collection, filter, TimeSpan.Zero, null, triggers);
        }

        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this ReadOnlyObservableCollection<T> collection,
            Func<T, bool> filter,
            TimeSpan bufferTime,
            params IObservable<object>[] triggers)
        {
			Ensure.NotNull(collection, nameof(collection));
            Ensure.NotNull(filter, nameof(filter));
            return new ReadOnlyFilteredView<T>(collection, filter, bufferTime, null, triggers);
        }

        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this ReadOnlyObservableCollection<T> collection,
            Func<T, bool> filter,
            IScheduler scheduler,
            params IObservable<object>[] triggers)
        {
		    Ensure.NotNull(collection, nameof(collection));
            Ensure.NotNull(filter, nameof(filter));
            Ensure.NotNull(scheduler, nameof(scheduler));
            return new ReadOnlyFilteredView<T>(collection, filter, TimeSpan.Zero, scheduler, triggers);
        }
       
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this ReadOnlyObservableCollection<T> collection,
            Func<T, bool> filter,
            TimeSpan bufferTime,
            IScheduler scheduler,
            params IObservable<object>[] triggers)
        {
		    Ensure.NotNull(collection, nameof(collection));
            Ensure.NotNull(filter, nameof(filter));
            Ensure.NotNull(scheduler, nameof(scheduler));
            return new ReadOnlyFilteredView<T>(collection, filter, bufferTime, scheduler, triggers);
        }

        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this IReadOnlyObservableCollection<T> collection,
            Func<T, bool> filter,
            params IObservable<object>[] triggers)
        {
			Ensure.NotNull(collection, nameof(collection));
            return new ReadOnlyFilteredView<T>(collection, filter, TimeSpan.Zero, null, triggers);
        }

        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this IReadOnlyObservableCollection<T> collection,
            Func<T, bool> filter,
            TimeSpan bufferTime,
            params IObservable<object>[] triggers)
        {
			Ensure.NotNull(collection, nameof(collection));
            Ensure.NotNull(filter, nameof(filter));
            return new ReadOnlyFilteredView<T>(collection, filter, bufferTime, null, triggers);
        }

        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this IReadOnlyObservableCollection<T> collection,
            Func<T, bool> filter,
            IScheduler scheduler,
            params IObservable<object>[] triggers)
        {
		    Ensure.NotNull(collection, nameof(collection));
            Ensure.NotNull(filter, nameof(filter));
            Ensure.NotNull(scheduler, nameof(scheduler));
            return new ReadOnlyFilteredView<T>(collection, filter, TimeSpan.Zero, scheduler, triggers);
        }
       
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this IReadOnlyObservableCollection<T> collection,
            Func<T, bool> filter,
            TimeSpan bufferTime,
            IScheduler scheduler,
            params IObservable<object>[] triggers)
        {
		    Ensure.NotNull(collection, nameof(collection));
            Ensure.NotNull(filter, nameof(filter));
            Ensure.NotNull(scheduler, nameof(scheduler));
            return new ReadOnlyFilteredView<T>(collection, filter, bufferTime, scheduler, triggers);
        }

        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this IEnumerable<T> collection,
            Func<T, bool> filter,
            IEnumerable<IObservable<object>> triggers)
        {
			Ensure.NotNull(collection, nameof(collection));
            Ensure.NotNull(filter, nameof(filter));
			Ensure.NotNull(triggers, nameof(triggers));
            return new ReadOnlyFilteredView<T>(collection, filter, TimeSpan.Zero, null, triggers);
        }

        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this IEnumerable<T> collection,
            Func<T, bool> filter,
            TimeSpan bufferTime,
            IEnumerable<IObservable<object>> triggers)
        {
			Ensure.NotNull(collection, nameof(collection));
            Ensure.NotNull(filter, nameof(filter));
			Ensure.NotNull(triggers, nameof(triggers));
            return new ReadOnlyFilteredView<T>(collection, filter, bufferTime, null, triggers);
        }

        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this IEnumerable<T> collection,
            Func<T, bool> filter,
            IScheduler scheduler,
            IEnumerable<IObservable<object>> triggers)
        {
		    Ensure.NotNull(collection, nameof(collection));
            Ensure.NotNull(filter, nameof(filter));
            Ensure.NotNull(scheduler, nameof(scheduler));
			Ensure.NotNull(triggers, nameof(triggers));
            return new ReadOnlyFilteredView<T>(collection, filter, TimeSpan.Zero, scheduler, triggers);
        }
       
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this IEnumerable<T> collection,
            Func<T, bool> filter,
            TimeSpan bufferTime,
            IScheduler scheduler,
            IEnumerable<IObservable<object>> triggers)
        {
		    Ensure.NotNull(collection, nameof(collection));
            Ensure.NotNull(filter, nameof(filter));
            Ensure.NotNull(scheduler, nameof(scheduler));
			Ensure.NotNull(triggers, nameof(triggers));
            return new ReadOnlyFilteredView<T>(collection, filter, bufferTime, scheduler, triggers);
        }
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this IEnumerable<T> collection,
            Func<T, bool> filter,
            IObservable<object> trigger)
        {
			Ensure.NotNull(collection, nameof(collection));
            Ensure.NotNull(filter, nameof(filter));
			Ensure.NotNull(trigger, nameof(trigger));
            return new ReadOnlyFilteredView<T>(collection, filter, TimeSpan.Zero, null, trigger);
        }

        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this IEnumerable<T> collection,
            Func<T, bool> filter,
            TimeSpan bufferTime,
            IObservable<object> trigger)
        {
			Ensure.NotNull(collection, nameof(collection));
            Ensure.NotNull(filter, nameof(filter));
			Ensure.NotNull(trigger, nameof(trigger));
            return new ReadOnlyFilteredView<T>(collection, filter, bufferTime, null, trigger);
        }

        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this IEnumerable<T> collection,
            Func<T, bool> filter,
            IScheduler scheduler,
            IObservable<object> trigger)
        {
		    Ensure.NotNull(collection, nameof(collection));
            Ensure.NotNull(filter, nameof(filter));
            Ensure.NotNull(scheduler, nameof(scheduler));
			Ensure.NotNull(trigger, nameof(trigger));
            return new ReadOnlyFilteredView<T>(collection, filter, TimeSpan.Zero, scheduler, trigger);
        }
       
        public static ReadOnlyFilteredView<T> AsReadOnlyFilteredView<T>(
            this IEnumerable<T> collection,
            Func<T, bool> filter,
            TimeSpan bufferTime,
            IScheduler scheduler,
            IObservable<object> trigger)
        {
		    Ensure.NotNull(collection, nameof(collection));
            Ensure.NotNull(filter, nameof(filter));
            Ensure.NotNull(scheduler, nameof(scheduler));
			Ensure.NotNull(trigger, nameof(trigger));
            return new ReadOnlyFilteredView<T>(collection, filter, bufferTime, scheduler, trigger);
        }
    }
}