namespace Gu.Reactive.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Subjects;

    using Microsoft.Reactive.Testing;

    using NUnit.Framework;

    public partial class ObservableExtTests
    {
        public class Chunks
        {
            [TestCase(new[] { 1 })]
            [TestCase(new[] { 1, 2 })]
            public void ChunksWithTimeAndScheduler(int[] values)
            {
                using (var subject = new Subject<int>())
                {
                    var scheduler = new TestScheduler();
                    var list = new List<IReadOnlyList<int>>();
                    using (subject.Chunks(TimeSpan.FromMilliseconds(20), scheduler)
                                  .Subscribe(list.Add))
                    {
                        for (var i = 1; i < 5; i++)
                        {
                            foreach (var value in values)
                            {
                                subject.OnNext(value);
                            }

                            scheduler.Start();
                            Assert.AreEqual(i, list.Count);
                            CollectionAssert.AreEqual(values, list.Last());
                        }
                    }
                }
            }

            [TestCase(new[] { 1 })]
            [TestCase(new[] { 1, 2 })]
            public void ChunksWithTimeZeroAndScheduler(int[] values)
            {
                using (var subject = new Subject<int>())
                {
                    var scheduler = new TestScheduler();
                    var list = new List<IReadOnlyList<int>>();
                    using (subject.Chunks(TimeSpan.Zero, scheduler)
                                  .Subscribe(list.Add))
                    {
                        foreach (var value in values)
                        {
                            subject.OnNext(value);
                        }

                        scheduler.Start();
                        Assert.AreEqual(values.Length, list.Count);
                        for (var i = 0; i < values.Length; i++)
                        {
                            CollectionAssert.AreEqual(new[] { values[i] }, list[i]);
                        }
                    }
                }
            }

            [TestCase(new[] { 1 })]
            [TestCase(new[] { 1, 2 })]
            public void ChunksWithTimeAndMaxTimeScheduler(int[] values)
            {
                using (var subject = new Subject<int>())
                {
                    var scheduler = new TestScheduler();
                    var list = new List<IReadOnlyList<int>>();
                    using (subject.Chunks(TimeSpan.FromMilliseconds(20), TimeSpan.FromMilliseconds(100), scheduler)
                                  .Subscribe(list.Add))
                    {
                        for (var i = 1; i < 5; i++)
                        {
                            foreach (var value in values)
                            {
                                subject.OnNext(value);
                            }

                            scheduler.Start();
                            Assert.AreEqual(i, list.Count);
                            CollectionAssert.AreEqual(values, list.Last());
                        }
                    }
                }
            }
        }
    }
}