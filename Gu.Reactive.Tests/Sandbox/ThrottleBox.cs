namespace Gu.Reactive.Tests.Sandbox
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Text;
    using Microsoft.Reactive.Testing;
    using NUnit.Framework;

    public class ThrottleBox
    {
        private const int _THROTTLE = 2;
        private const int _TIMEOUT = 5;
        private const int _COMPLETE = 100000;

        [TestCaseSource(typeof(ThrottleSource))]
        public void ThrottleW(ThrottleData data)
        {
            var source = data.Source;
            var scheduler = data.Scheduler;
            var observer = data.Observer;
            IObservable<int> throttled = source.ThrottleW(TimeSpan.FromTicks(_THROTTLE), TimeSpan.FromTicks(_TIMEOUT), scheduler);

            throttled.Subscribe(observer);

            // start the clock
            scheduler.Start();
            Dump(data.Expected, observer.Messages);
            CollectionAssert.AreEqual(data.Expected, observer.Messages);
        }

        [TestCaseSource(typeof(ThrottleSource))]
        public void ThrottleWJ(ThrottleData data)
        {
            var source = data.Source;
            var scheduler = data.Scheduler;
            var observer = data.Observer;
            IObservable<int> throttled = source.ThrottleWJ(TimeSpan.FromTicks(_THROTTLE), TimeSpan.FromTicks(_TIMEOUT), scheduler);

            throttled.Subscribe(observer);

            // start the clock
            scheduler.Start();
            Dump(data.Expected, observer.Messages);
            CollectionAssert.AreEqual(data.Expected, observer.Messages);
        }

        [TestCaseSource(typeof(ThrottleSource))]
        public void ThrottleG(ThrottleData data)
        {
            var source = data.Source;
            var scheduler = data.Scheduler;
            var observer = data.Observer;
            IObservable<int> throttled = source.ThrottleG(TimeSpan.FromTicks(_THROTTLE), TimeSpan.FromTicks(_TIMEOUT), scheduler);

            throttled.Subscribe(observer);

            // start the clock
            scheduler.Start();
            Dump(data.Expected, observer.Messages);
            CollectionAssert.AreEqual(data.Expected, observer.Messages);
        }

        private void Dump(IList<Recorded<Notification<int>>> expected, IList<Recorded<Notification<int>>> messages)
        {
            for (int i = 0; i < Math.Max(expected.Count, messages.Count); i++)
            {
                Console.WriteLine("{0}, {1}", ToString(expected, i), ToString(messages, i));
            }
        }

        private string ToString(IList<Recorded<Notification<int>>> list, int index)
        {
            if (list.Count <= index)
            {
                return "";
            }
            var recorded = list[index];
            if (recorded.Value.Kind == NotificationKind.OnCompleted)
            {
                return " end  ";
            }
            return string.Format("{{{0}, {1}}}", recorded.Value.Value, recorded.Time);
        }

        public class ThrottleSource : List<ThrottleData>
        {
            public ThrottleSource()
            {
                Add(new ThrottleData(new[] { 1, 2 }, new[,] { { 2, 4 } }));
                Add(new ThrottleData(new[] { 1, 4 }, new[,] { { 1, 3 }, { 4, 6 } }));
                Add(new ThrottleData(new[] { 1, 2, 3, 4 }, new[,] { { 4, 6 } }));
                Add(new ThrottleData(new[] { 1, 2, 3, 4, 5 }, new[,] { { 5, 6 } }));
                Add(new ThrottleData(new[] { 1, 2, 3, 4, 5, 6, 7 }, new[,] { { 6, 6 }, { 7, 9 } }));
                Add(new ThrottleData(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 }, new[,] { { 6, 6 }, { 12, 12 } }));
            }
        }

        public class ThrottleData
        {
            public readonly TestScheduler Scheduler;
            public readonly Recorded<Notification<int>>[] CompletedEvent;
            public readonly ITestableObserver<int> Observer;
            public readonly IObservable<int> Source;
            public readonly IList<Recorded<Notification<int>>> Expected;

            public ThrottleData(int[] pattern, int[,] expectedPattern)
            {
                Scheduler = new TestScheduler();
                CompletedEvent = new[] { ReactiveTest.OnCompleted(_COMPLETE, _COMPLETE) };
                Observer = Scheduler.CreateObserver<int>();
                Source = Scheduler.CreateColdObservable(pattern.Select(v => ReactiveTest.OnNext(v, v))
                                                               .Concat(CompletedEvent)
                                                               .ToArray()); ;
                Pattern = pattern;
                ExpectedPattern = new int[expectedPattern.GetLength(0)];
                ExpectedTimes = new int[expectedPattern.GetLength(0)];
                for (int i = 0; i < expectedPattern.GetLength(0); i++)
                {
                    ExpectedPattern[i] = expectedPattern[i, 0];
                    ExpectedTimes[i] = expectedPattern[i, 1];
                }
                Expected = ExpectedPattern.Zip(ExpectedTimes, (v, t) => ReactiveTest.OnNext(t, v)).Concat(CompletedEvent).ToList();
            }

            public int[] Pattern { get; private set; }
           
            public int[] ExpectedPattern { get; private set; }
            
            public int[] ExpectedTimes { get; private set; }

            public override string ToString()
            {
                var stringBuilder = new StringBuilder();
                for (int i = 0; i < ExpectedPattern.Length; i++)
                {
                    stringBuilder.AppendFormat("{{{0}, {1}}}", ExpectedPattern[i], ExpectedTimes[i]);
                    if (i < (ExpectedPattern.Length - 1))
                    {
                        stringBuilder.Append(", ");
                    }
                }
                return string.Format("{0} -> {1}", string.Join(", ", Pattern), stringBuilder.ToString());
            }
        }
    }

    public static class ObservableExt
    {
        public static IObservable<T> ThrottleW<T>(this IObservable<T> source, TimeSpan dueTime, TimeSpan maxTime, IScheduler scheduler = null)
        {
            return source.Publish(p => p
                    .Window(() =>
                    {
                        // close the window when p slows down for dueTime
                        // or when it exceeds maxTime.
                        // do not start throttling or the maxTime timer
                        // until the first p of the new window arrives
                        var throttleTimer = p.Throttle(dueTime, scheduler ?? Scheduler.Default);
                        var timeoutTimer = p.Delay(maxTime, scheduler ?? Scheduler.Default);
                        // signal when either timer signals
                        return throttleTimer.Amb(timeoutTimer);
                    })
                    .SelectMany(w => w.TakeLast(1)));
        }

        public static IObservable<T> ThrottleWJ<T>(this IObservable<T> source, TimeSpan dueTime, TimeSpan maxTime, IScheduler scheduler = null)
        {
            return source.Publish(p => p
                    .Window(() =>
                    {
                        // close the window when p slows down for dueTime
                        // or when it exceeds maxTime.
                        // do not start throttling or the maxTime timer
                        // until the first p of the new window arrives
                        var throttleTimer = p.Throttle(dueTime, scheduler ?? Scheduler.Default);
                        var timeoutTimer = p.Delay(maxTime, scheduler ?? Scheduler.Default);
                        // signal when either timer signals
                        return throttleTimer.Amb(timeoutTimer);
                    })
                    .SelectMany(w => w.TakeLast(1)));
        }

        public static IObservable<T> ThrottleG<T>(this IObservable<T> source, TimeSpan dueTime, TimeSpan maxTime, IScheduler scheduler = null)
        {
            return source
                .GroupByUntil(
                    t => 0, // they all get the same key
                    t => t, // the element is the element
                    g =>
                    {
                        // expire the group when it slows down for dueTime
                        // or when it exceeds maxTime
                        return g
                            .Throttle(dueTime, scheduler ?? Scheduler.Default)
                            .Timeout(maxTime, Observable.Empty<T>(), scheduler ?? Scheduler.Default);
                    })
                .SelectMany(g => g.LastAsync());
        }
    }
}
