﻿namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Threading;

    /// <summary>
    /// Extension methods for <see cref="IObservable{T}"/>.
    /// </summary>
    public static class ObservableExt
    {
        /// <summary>
        /// Author: Brandon Wallace, https://github.com/bman654
        /// http://stackoverflow.com/a/30761373/1069200.
        /// </summary>
        /// <typeparam name="T">The type of the items in the observable.</typeparam>
        /// <param name="source">Source sequence whose elements will be multicasted through a single shared subscription.</param>
        /// <param name="dueTime">Throttling duration for each element.</param>
        /// <param name="maxTime">Max throttling time.</param>
        /// <returns>An <see cref="IObservable{T}"/>.</returns>
        public static IObservable<T> Throttle<T>(this IObservable<T> source, TimeSpan dueTime, TimeSpan maxTime)
        {
            return source.Throttle(dueTime, maxTime, DefaultScheduler.Instance);
        }

        /// <summary>
        /// Author: Brandon Wallace, https://github.com/bman654
        /// http://stackoverflow.com/a/30761373/1069200.
        /// </summary>
        /// <typeparam name="T">The type of the items in the observable.</typeparam>
        /// <param name="source">Source sequence whose elements will be multicasted through a single shared subscription.</param>
        /// <param name="dueTime">Throttling duration for each element.</param>
        /// <param name="maxTime">Max throttling time.</param>
        /// <param name="scheduler">Scheduler to run the timers on.</param>
        /// <returns>An <see cref="IObservable{T}"/>.</returns>
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
        /// <typeparam name="T">The type of the items in the observable.</typeparam>
        /// <param name="source">Source sequence whose elements will be multicasted through a single shared subscription.</param>
        /// <param name="dueTime">Throttling duration for each element.</param>
        /// <returns>An <see cref="IObservable{T}"/>.</returns>
        public static IObservable<IReadOnlyList<T>> Chunks<T>(this IObservable<T> source, TimeSpan dueTime)
        {
            return source.Chunks(dueTime, DefaultScheduler.Instance);
        }

        /// <summary>
        /// Like throttle but returning all elements captured during the throttle time.
        /// </summary>
        /// <typeparam name="T">The type of the items in the observable.</typeparam>
        /// <param name="source">Source sequence whose elements will be multicasted through a single shared subscription.</param>
        /// <param name="dueTime">Throttling duration for each element.</param>
        /// <param name="scheduler">Scheduler to run the timers on.</param>
        /// <returns>An <see cref="IObservable{T}"/>.</returns>
        public static IObservable<IReadOnlyList<T>> Chunks<T>(this IObservable<T> source, TimeSpan dueTime, IScheduler scheduler)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

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
        /// <typeparam name="T">The type of the items in the observable.</typeparam>
        /// <param name="source">Source sequence whose elements will be multicasted through a single shared subscription.</param>
        /// <param name="dueTime">Throttling duration for each element.</param>
        /// <param name="maxTime">Max throttling time.</param>
        /// <returns>An <see cref="IObservable{T}"/>.</returns>
        public static IObservable<IReadOnlyList<T>> Chunks<T>(this IObservable<T> source, TimeSpan dueTime, TimeSpan maxTime)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (dueTime == TimeSpan.Zero)
            {
                return source.Select(x => new[] { x });
            }

            return source.Chunks(dueTime, maxTime, DefaultScheduler.Instance);
        }

        /// <summary>
        /// Like throttle but returning all elements captured during the throttle time.
        /// </summary>
        /// <typeparam name="T">The type of the items in the observable.</typeparam>
        /// <param name="source">Source sequence whose elements will be multicasted through a single shared subscription.</param>
        /// <param name="dueTime">Throttling duration for each element.</param>
        /// <param name="maxTime">Max throttling time.</param>
        /// <param name="scheduler">Scheduler to run the timers on.</param>
        /// <returns>An <see cref="IObservable{T}"/>.</returns>
        public static IObservable<IReadOnlyList<T>> Chunks<T>(this IObservable<T> source, TimeSpan dueTime, TimeSpan maxTime, IScheduler scheduler)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

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
        /// <typeparam name="T">The type of the items in the observable.</typeparam>
        /// <param name="source">Source sequence whose elements will be multicasted through a single shared subscription.</param>
        /// <param name="delayTime">The time to delay the repeat.</param>
        /// <returns>An <see cref="IObservable{T}"/>.</returns>
        public static IObservable<T> RepeatAfterDelay<T>(this IObservable<T> source, TimeSpan delayTime)
        {
            return source.RepeatAfterDelay(delayTime, Scheduler.Default);
        }

        /// <summary>
        /// Author: Brandon Wallace, https://github.com/bman654.
        /// </summary>
        /// <typeparam name="T">The type of the items in the observable.</typeparam>
        /// <param name="source">Source sequence whose elements will be multicasted through a single shared subscription.</param>
        /// <param name="delayTime">The time to delay the repeat.</param>
        /// <param name="scheduler">Scheduler to run the timers on.</param>
        /// <returns>An <see cref="IObservable{T}"/>.</returns>
        public static IObservable<T> RepeatAfterDelay<T>(this IObservable<T> source, TimeSpan delayTime, IScheduler scheduler)
        {
            var delay = Observable.Empty<T>().Delay(delayTime, scheduler);
            return source.Concat(delay).Repeat();
        }

        /// <summary>
        /// Creates an observable with the last two values from <paramref name="source"/>.
        /// Starts signaling after the second value.
        /// </summary>
        /// <typeparam name="T">The type of the messages.</typeparam>
        /// <param name="source">The observable.</param>
        /// <returns>An observable with the last two values from <paramref name="source"/>.</returns>
        public static IObservable<WithPrevious<T>> WithPrevious<T>(this IObservable<T> source)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return Observable.Create<WithPrevious<T>>(
                o =>
                {
                    var hasPrevious = false;
                    var previous = default(T)!;
                    return source.Subscribe(
                        x =>
                        {
                            if (hasPrevious)
                            {
                                o.OnNext(new WithPrevious<T>(x, previous));
                            }

                            previous = x;
                            hasPrevious = true;
                        },
                        x => o.OnError(x),
                        () => o.OnCompleted());
                });
        }

        /// <summary>
        /// Creates an observable with the last two values from <paramref name="source"/>.
        /// Starts signaling after the first value, then previous is <see cref="Maybe{T}.None"/>.
        /// </summary>
        /// <typeparam name="T">The type of the messages.</typeparam>
        /// <param name="source">The observable.</param>
        /// <returns>An observable with the last two values from <paramref name="source"/>.</returns>
        public static IObservable<WithMaybePrevious<T>> WithMaybePrevious<T>(this IObservable<T> source)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return Observable.Create<WithMaybePrevious<T>>(
                o =>
                {
                    var hasPrevious = false;
                    var previous = default(T)!;
                    return source.Subscribe(
                        x =>
                        {
                            o.OnNext(new WithMaybePrevious<T>(x, hasPrevious ? Maybe.Some(previous) : Maybe.None<T>()));
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
        /// <param name="token">The <see cref="CancellationToken"/> that cancels the operation.</param>
        /// <returns>An <see cref="IObservable{T}"/>.</returns>
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
        /// <typeparam name="T">The type of the items in the collection.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <returns>An <see cref="IReadOnlyView{T}"/>.</returns>
        public static IReadOnlyView<T> AsReadOnlyView<T>(this IObservable<IMaybe<IEnumerable<T>?>> source)
        {
            return new ReadOnlyView<T>(source.Select(x => x.GetValueOrDefault()));
        }

        /// <summary>
        /// Turn the observable into a <see cref="IReadOnlyView{T}"/> that can be bound.
        /// </summary>
        /// <typeparam name="T">The type of the items in the collection.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <returns>An <see cref="IReadOnlyView{T}"/>.</returns>
        public static IReadOnlyView<T> AsReadOnlyView<T>(this IObservable<Maybe<IEnumerable<T>?>> source)
        {
            return new ReadOnlyView<T>(source.Select(x => x.GetValueOrDefault()));
        }

        /// <summary>
        /// Turn the observable into a <see cref="IReadOnlyView{T}"/> that can be bound.
        /// </summary>
        /// <typeparam name="T">The type of the items in the collection.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <returns>An <see cref="IReadOnlyView{T}"/>.</returns>
        public static IReadOnlyView<T> AsReadOnlyView<T>(this IObservable<IEnumerable<T>?> source)
        {
            return new ReadOnlyView<T>(source);
        }

        /// <summary>
        /// Return Observable.Merge if <paramref name="source"/> is not null or empty.
        /// </summary>
        /// <typeparam name="T">The type of the items in the collection.</typeparam>
        internal static IObservable<T> MergeOrNever<T>(this IEnumerable<IObservable<T>>? source)
        {
            if (source?.Any() == true)
            {
                return Observable.Merge(source);
            }

            return Observable.Never<T>();
        }

        internal static IObservable<T> ObserveOnOrDefault<T>(this IObservable<T> source, IScheduler scheduler)
        {
            if (scheduler is null)
            {
                return source;
            }

            return source.ObserveOn(scheduler);
        }

        private class ReadOnlyView<T> : ReadOnlySerialViewBase<T>
        {
            private readonly IDisposable subscription;

            internal ReadOnlyView(IObservable<IEnumerable<T>?> source)
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
