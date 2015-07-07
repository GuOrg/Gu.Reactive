namespace Gu.Reactive
{
    using System;
    using System.Reactive;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Threading;

    public static class ObservableExt
    {
        public static IObservable<T> Throttle<T>(this IObservable<T> source, TimeSpan dueTime, TimeSpan maxTime)
        {
            return source.Throttle(dueTime, maxTime, Scheduler.Default);
        }

        /// <summary>
        /// Author: Brandon Wallace, https://github.com/bman654
        /// http://stackoverflow.com/a/30761373/1069200
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="dueTime">Throttling duration for each element</param>
        /// <param name="maxTime">Max throttling time</param>
        /// <param name="scheduler"></param>
        /// <returns></returns>
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

        public static IObservable<T> RepeatAfterDelay<T>(this IObservable<T> source, TimeSpan delayTime)
        {
            return source.RepeatAfterDelay(delayTime, Scheduler.Default);
        }

        /// <summary>
        /// Author: Brandon Wallace, https://github.com/bman654
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="delayTime"></param>
        /// <param name="scheduler"></param>
        /// <returns></returns>
        public static IObservable<T> RepeatAfterDelay<T>(this IObservable<T> source, TimeSpan delayTime, IScheduler scheduler)
        {
            var delay = Observable.Empty<T>().Delay(delayTime, scheduler);
            return Observable.Concat(source, delay).Repeat();
        }

        /// <summary>
        /// Author: Brandon Wallace, https://github.com/bman654
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static IObservable<Unit> AsObservable(this CancellationToken token)
        {
            return Observable.Create<Unit>(observer =>
            {
                return token.Register(() =>
                {
                    observer.OnNext(Unit.Default);
                    observer.OnCompleted();
                });
            });
        }
    }
}
