namespace Gu.Reactive
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;

    public static class FilteredRefresher
    {
        private static readonly IReadOnlyList<NotifyCollectionChangedEventArgs> ResetArgses = new[] { Diff.NotifyCollectionResetEventArgs };

        internal static IObservable<IReadOnlyList<NotifyCollectionChangedEventArgs>> Create(
            IRefresher refresher,
            IEnumerable source,
            TimeSpan bufferTime,
            IEnumerable<IObservable<object>> triggers,
            IScheduler scheduler,
            bool signalInitial)
        {
            var collectionChanges = DeferredRefresher.Create(refresher, source, bufferTime, scheduler, false);
            var triggersChanges = triggers.Merge()
                                          .ThrottleOrDefault(bufferTime, scheduler)
                                          .Select(_ => ResetArgses);
            return Observable.Merge(collectionChanges, triggersChanges)
                             .StartWithIf(signalInitial, scheduler, ResetArgses)
                             .ThrottleOrDefault(bufferTime, scheduler);
        }

        internal static IObservable<T> ThrottleOrDefault<T>(
            this IObservable<T> observable,
            TimeSpan dueTime,
            IScheduler scheduler)
        {
            if (dueTime <= TimeSpan.Zero)
            {
                return observable;
            }
            if (scheduler == null)
            {
                return observable.Throttle(dueTime);
            }
            return observable.Throttle(dueTime, scheduler);
        }
    }
}