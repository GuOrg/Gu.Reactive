namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Threading;

    /// <summary>
    /// Extension methods for <see cref="IObservable{T}"/>
    /// </summary>
    public static class ObservableExt
    {
        /// <summary>
        /// Author: Brandon Wallace, https://github.com/bman654
        /// http://stackoverflow.com/a/30761373/1069200
        /// </summary>
        /// <param name="source">Source sequence whose elements will be multicasted through a single shared subscription.</param>
        /// <param name="dueTime">Throttling duration for each element</param>
        /// <param name="maxTime">Max throttling time</param>
        public static IObservable<T> Throttle<T>(this IObservable<T> source, TimeSpan dueTime, TimeSpan maxTime)
        {
            return source.Throttle(dueTime, maxTime, Scheduler.Default);
        }

        /// <summary>
        /// Author: Brandon Wallace, https://github.com/bman654
        /// http://stackoverflow.com/a/30761373/1069200
        /// </summary>
        /// <param name="source">Source sequence whose elements will be multicasted through a single shared subscription.</param>
        /// <param name="dueTime">Throttling duration for each element</param>
        /// <param name="maxTime">Max throttling time</param>
        /// <param name="scheduler">Scheduler to run the timers on.</param>
        public static IObservable<T> Throttle<T>(this IObservable<T> source, TimeSpan dueTime, TimeSpan maxTime, IScheduler scheduler)
        {
            return source.Publish(p => p
                    .Window(() =>
                    {
                        // close the window when p slows down for throttle
                        // or when it exceeds maxTime.
                        // do not start throttling or the maxTime timer
                        // until the first p of the new window arrives
                        var throttleTimer = p.Throttle(dueTime, scheduler);
                        var timeoutTimer = p.Delay(maxTime, scheduler);

                        // signal when either timer signals
                        return throttleTimer.Amb(timeoutTimer);
                    })
                    .SelectMany(w => w.TakeLast(1)));
        }

        /// <summary>
        /// Author: Brandon Wallace, https://github.com/bman654
        /// </summary>
        /// <param name="source">Source sequence whose elements will be multicasted through a single shared subscription.</param>
        /// <param name="delayTime">The time to delay the repeat.</param>
        public static IObservable<T> RepeatAfterDelay<T>(this IObservable<T> source, TimeSpan delayTime)
        {
            return source.RepeatAfterDelay(delayTime, Scheduler.Default);
        }

        /// <summary>
        /// Author: Brandon Wallace, https://github.com/bman654
        /// </summary>
        /// <param name="source">Source sequence whose elements will be multicasted through a single shared subscription.</param>
        /// <param name="delayTime">The time to delay the repeat.</param>
        /// <param name="scheduler">Scheduler to run the timers on.</param>
        public static IObservable<T> RepeatAfterDelay<T>(this IObservable<T> source, TimeSpan delayTime, IScheduler scheduler)
        {
            var delay = Observable.Empty<T>().Delay(delayTime, scheduler);
            return Observable.Concat(source, delay).Repeat();
        }

        /// <summary>
        /// Turn a <see cref="CancellationToken"/> into an observable
        /// Author: Brandon Wallace, https://github.com/bman654
        /// </summary>
        public static IObservable<Unit> AsObservable(this CancellationToken token)
        {
            return Observable.Create<Unit>(observer =>
                {
                    return token.Register(
                        () =>
                            {
                                observer.OnNext(Unit.Default);
                                observer.OnCompleted();
                            });
                });
        }

        /// <summary>
        /// Return Observable.Merge if <paramref name="source"/> is not null or empty
        /// </summary>
        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        internal static IObservable<T> MergeOrNever<T>(this IEnumerable<IObservable<T>> source)
        {
            if (source?.Any() == true)
            {
                return Observable.Merge(source);
            }

            return Observable.Never<T>();
        }
    }
}
