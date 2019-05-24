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

    using Gu.Reactive.Internals;

    /// <summary>
    /// Extension methods for <see cref="IObservable{T}"/>.
    /// </summary>
    public static class ObservableExt
    {
        /// <summary>
        /// Author: Brandon Wallace, https://github.com/bman654
        /// http://stackoverflow.com/a/30761373/1069200.
        /// </summary>
        /// <param name="source">Source sequence whose elements will be multicasted through a single shared subscription.</param>
        /// <param name="dueTime">Throttling duration for each element.</param>
        /// <param name="maxTime">Max throttling time.</param>
        public static IObservable<T> Throttle<T>(this IObservable<T> source, TimeSpan dueTime, TimeSpan maxTime)
        {
            return source.Throttle(dueTime, maxTime, DefaultScheduler.Instance);
        }

        /// <summary>
        /// Author: Brandon Wallace, https://github.com/bman654
        /// http://stackoverflow.com/a/30761373/1069200.
        /// </summary>
        /// <param name="source">Source sequence whose elements will be multicasted through a single shared subscription.</param>
        /// <param name="dueTime">Throttling duration for each element.</param>
        /// <param name="maxTime">Max throttling time.</param>
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
        /// Like throttle but returning all elements captured during the throttle time.
        /// </summary>
        /// <param name="source">Source sequence whose elements will be multicasted through a single shared subscription.</param>
        /// <param name="dueTime">Throttling duration for each element.</param>
        public static IObservable<IReadOnlyList<T>> Chunks<T>(this IObservable<T> source, TimeSpan dueTime)
        {
            return source.Chunks(dueTime, DefaultScheduler.Instance);
        }

        /// <summary>
        /// Like throttle but returning all elements captured during the throttle time.
        /// </summary>
        /// <param name="source">Source sequence whose elements will be multicasted through a single shared subscription.</param>
        /// <param name="dueTime">Throttling duration for each element.</param>
        /// <param name="scheduler">Scheduler to run the timers on.</param>
        public static IObservable<IReadOnlyList<T>> Chunks<T>(this IObservable<T> source, TimeSpan dueTime, IScheduler scheduler)
        {
            Ensure.NotNull(source, nameof(source));
            if (dueTime == TimeSpan.Zero)
            {
                return source.Select(x => new[] { x })
                             .ObserveOnOrDefault(scheduler);
            }

            return source.Publish(shared => shared.Buffer(() => shared.Throttle(dueTime, scheduler ?? DefaultScheduler.Instance)))
                         .Cast<IReadOnlyList<T>>();
        }

        /// <summary>
        /// Like throttle but returning all elements captured during the throttle time.
        /// </summary>
        /// <param name="source">Source sequence whose elements will be multicasted through a single shared subscription.</param>
        /// <param name="dueTime">Throttling duration for each element.</param>
        /// <param name="maxTime">Max throttling time.</param>
        public static IObservable<IReadOnlyList<T>> Chunks<T>(this IObservable<T> source, TimeSpan dueTime, TimeSpan maxTime)
        {
            Ensure.NotNull(source, nameof(source));
            if (dueTime == TimeSpan.Zero)
            {
                return source.Select(x => new[] { x });
            }

            return source.Chunks(dueTime, maxTime, DefaultScheduler.Instance);
        }

        /// <summary>
        /// Like throttle but returning all elements captured during the throttle time.
        /// </summary>
        /// <param name="source">Source sequence whose elements will be multicasted through a single shared subscription.</param>
        /// <param name="dueTime">Throttling duration for each element.</param>
        /// <param name="maxTime">Max throttling time.</param>
        /// <param name="scheduler">Scheduler to run the timers on.</param>
        public static IObservable<IReadOnlyList<T>> Chunks<T>(this IObservable<T> source, TimeSpan dueTime, TimeSpan maxTime, IScheduler scheduler)
        {
            Ensure.NotNull(source, nameof(source));
            if (dueTime == TimeSpan.Zero)
            {
                return source.Select(x => new[] { x })
                             .ObserveOnOrDefault(scheduler);
            }

            return source.Publish(shared => shared.Buffer(() => shared.Throttle(dueTime, maxTime, scheduler)))
                         .Cast<IReadOnlyList<T>>();
        }

        /// <summary>
        /// Author: Brandon Wallace, https://github.com/bman654.
        /// </summary>
        /// <param name="source">Source sequence whose elements will be multicasted through a single shared subscription.</param>
        /// <param name="delayTime">The time to delay the repeat.</param>
        public static IObservable<T> RepeatAfterDelay<T>(this IObservable<T> source, TimeSpan delayTime)
        {
            return source.RepeatAfterDelay(delayTime, Scheduler.Default);
        }

        /// <summary>
        /// Author: Brandon Wallace, https://github.com/bman654.
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
        /// Creates an observable with the last two values from <paramref name="source"/>.
        /// </summary>
        /// <typeparam name="T">The type of the messages.</typeparam>
        /// <param name="source">The observable.</param>
        /// <returns>An observable with the last two values from <paramref name="source"/>.</returns>
        public static IObservable<Paired<T>> Pair<T>(this IObservable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return Observable.Create<Paired<T>>(
                o =>
                {
                    var hasPrevious = false;
                    var previous = default(T);
                    return source.Subscribe(
                        x =>
                        {
                            if (hasPrevious)
                            {
                                o.OnNext(new Paired<T>(x, previous));
                            }

                            previous = x;
                            hasPrevious = true;
                        },
                        x => o.OnError(x),
                        () => o.OnCompleted());
                });
        }

        /// <summary>
        /// Turn a <see cref="CancellationToken"/> into an observable
        /// Author: Brandon Wallace, https://github.com/bman654.
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
        /// Turn the observable into a <see cref="IReadOnlyView{T}"/> that can be bound.
        /// </summary>
        public static IReadOnlyView<T> AsReadOnlyView<T>(this IObservable<IMaybe<IEnumerable<T>>> source)
        {
            return new ReadOnlyView<T>(source.Select(x => x.GetValueOrDefault()));
        }

        /// <summary>
        /// Turn the observable into a <see cref="IReadOnlyView{T}"/> that can be bound.
        /// </summary>
        public static IReadOnlyView<T> AsReadOnlyView<T>(this IObservable<Maybe<IEnumerable<T>>> source)
        {
            return new ReadOnlyView<T>(source.Select(x => x.GetValueOrDefault()));
        }

        /// <summary>
        /// Turn the observable into a <see cref="IReadOnlyView{T}"/> that can be bound.
        /// </summary>
        public static IReadOnlyView<T> AsReadOnlyView<T>(this IObservable<IEnumerable<T>> source)
        {
            return new ReadOnlyView<T>(source);
        }

        /// <summary>
        /// Return Observable.Merge if <paramref name="source"/> is not null or empty.
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

        internal static IObservable<T> ObserveOnOrDefault<T>(this IObservable<T> source, IScheduler scheduler)
        {
            if (scheduler == null)
            {
                return source;
            }

            return source.ObserveOn(scheduler);
        }

        private class ReadOnlyView<T> : ReadOnlySerialViewBase<T>
        {
            private readonly IDisposable subscription;

            public ReadOnlyView(IObservable<IEnumerable<T>> source)
                : base(null, TimeSpan.Zero, ImmediateScheduler.Instance, leaveOpen: true)
            {
                this.subscription = source.Subscribe(this.SetSource);
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    this.subscription?.Dispose();
                }

                base.Dispose(disposing);
            }
        }
    }
}
