namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Concurrency;
    using Gu.Reactive.Internals;

    public static partial class FilteredView
    {
        public static FilteredView<T> AsFilteredView<T>(
            this IList<T> collection,
            Func<T, bool> filter,
            IScheduler scheduler,
            IObservable<object> trigger,
            params IObservable<object>[] triggers)
        {
            Ensure.NotNull(collection, "collection");
            Ensure.NotNull(filter, "filter");
            Ensure.NotNull(scheduler, "scheduler");
            Ensure.NotNull(trigger, "trigger");
            return new FilteredView<T>(collection, filter, TimeSpan.Zero, scheduler, triggers.Prepend(trigger));
        }

        public static FilteredView<T> AsFilteredView<T>(
            this IList<T> collection,
            Func<T, bool> filter,
            IObservable<object> trigger,
            params IObservable<object>[] triggers)
        {
            Ensure.NotNull(collection, "collection");
            Ensure.NotNull(filter, "filter");
            Ensure.NotNull(trigger, "trigger");
            return new FilteredView<T>(
                collection,
                filter,
                TimeSpan.Zero,
                null,
                triggers.Prepend(trigger));
        }

        public static FilteredView<T> AsFilteredView<T>(
            this IList<T> collection,
            Func<T, bool> filter,
            TimeSpan bufferTime,
            IObservable<object> trigger,
            params IObservable<object>[] triggers)
        {
            Ensure.NotNull(collection, "collection");
            Ensure.NotNull(filter, "filter");
            Ensure.NotNull(trigger, "trigger");
            return new FilteredView<T>(collection, filter, bufferTime, null, triggers.Prepend(trigger));
        }

        public static FilteredView<T> AsFilteredView<T>(
            this IList<T> collection,
            Func<T, bool> filter,
            TimeSpan bufferTime,
            IScheduler scheduler,
            IObservable<object> trigger,
            params IObservable<object>[] triggers)
        {
            Ensure.NotNull(collection, "collection");
            Ensure.NotNull(filter, "filter");
            Ensure.NotNull(scheduler, "scheduler");
            Ensure.NotNull(trigger, "trigger");
            return new FilteredView<T>(collection, filter, bufferTime, scheduler, triggers.Prepend(trigger));
        }

        private static IObservable<object>[] Prepend(this IObservable<object>[] items, IObservable<object> item)
        {
            if (items == null || items.Length == 0)
            {
                return new[] { item };
            }
            var result = new IObservable<object>[items.Length + 1];
            items[0] = item;
            Array.Copy(items, 0, result, 1, items.Length);
            return result;
        }
    }
}