namespace Gu.Reactive.Tests.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;

    using Microsoft.Reactive.Testing;

    using Moq;

    using NUnit.Framework;

    public class DeferredRefresherTests
    {
        [TestCaseSource(typeof(RefresherSource))]
        public void BufferTest(RefresherData data)
        {
            var ints = new ObservableCollection<int>();
            var results = new List<Timestamped<IReadOnlyList<NotifyCollectionChangedEventArgs>>>();
            var scheduler = new TestScheduler();
            var observable = DeferredRefresher.Create(Mock.Of<IRefresher>(x=>x.IsRefreshing == false), ints, TimeSpan.FromMilliseconds(10), scheduler, data.SignalInitial)
                                 .Timestamp(scheduler);
            observable.Subscribe(results.Add);

            foreach (var time in data.Times)
            {
                scheduler.Schedule(TimeSpan.FromMilliseconds(time), () => ints.Add(time));
            }

            scheduler.Start();

            Assert.AreEqual(data.Results.Count, results.Count);
            for (int i = 0; i < data.Results.Count; i++)
            {
                var expected = data.Results[i];
                var actual = results[i];
                Assert.AreEqual(expected.Count, actual.Value.Count);
            }

            var secondResults = new List<Timestamped<IReadOnlyList<NotifyCollectionChangedEventArgs>>>();
            observable.Subscribe(secondResults.Add);
            CollectionAssert.IsEmpty(secondResults);
        }

        [Test]
        public void StartWithIfTests()
        {
            var scheduler = new TestScheduler();
            var subject = new Subject<int>();
            var results = new List<Timestamped<int>>();
            var observable = subject.StartWithIf(true, scheduler, 0);
            observable.Timestamp(scheduler)
                      .Throttle(TimeSpan.FromSeconds(0.5), scheduler)
                      .Subscribe(results.Add);
            scheduler.Schedule(TimeSpan.FromSeconds(1), () => subject.OnNext(1));
            scheduler.Schedule(TimeSpan.FromSeconds(2), () => subject.OnNext(1));
            scheduler.Start();
            Assert.AreEqual(3, results.Count);
        }

        public class RefresherSource : List<RefresherData>
        {
            public RefresherSource()
            {
                Add(true, new[] { 1 }, new[] { 0, 1 });
                Add(false, new[] { 1 }, new[] { 1 });

                Add(true, new[] { 15 }, new[] { 0 }, new[] { 15 });
                Add(false, new[] { 15 }, new[] { 15 });

                Add(true, new[] { 1, 2, 3 }, new[] { 0, 1, 2, 3 });
                Add(false, new[] { 1, 2, 3 }, new[] { 1, 2, 3 });

                Add(true, new[] { 1, 2, 3, 15, 16, 35 }, new[] { 0, 1, 2, 3 }, new[] { 15, 16 }, new[] { 35 });
                Add(false, new[] { 1, 2, 3, 15, 16, 35 }, new[] { 1, 2, 3 }, new[] { 15, 16 }, new[] { 35 });
            }

            public void Add(bool signalInitial, int[] times, params int[][] results)
            {
                Add(new RefresherData(signalInitial, times, results));
            }
        }

        public class RefresherData
        {
            public RefresherData(bool signalInitial, int[] times, params int[][] results)
            {
                SignalInitial = signalInitial;
                Times = times;
                Results = results;
            }

            public bool SignalInitial { get; set; }

            public int[] Times { get; private set; }

            public IReadOnlyList<IReadOnlyList<int>> Results { get; private set; }

            public static string ToString(IReadOnlyList<IReadOnlyList<int>> values)
            {
                return string.Join(",", values.Select(x => string.Format(@"{{{0}}}", string.Join(", ", x))));
            }

            public override string ToString()
            {
                var times = string.Join(", ", Times);
                var results = ToString(Results);
                return string.Format("SignalInitial: {0} Times: {1}, Results: {2}", SignalInitial, times, results);
            }
        }
    }
}