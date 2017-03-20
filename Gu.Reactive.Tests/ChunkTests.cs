namespace Gu.Reactive.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Subjects;
    using Microsoft.Reactive.Testing;
    using NUnit.Framework;

    public class ChunkTests
    {
        [Test]
        public void OneChange()
        {
            var scheduler = new TestScheduler();
            var chunk = new Chunk<int>(TimeSpan.Zero, scheduler);
            using (var subject = new Subject<int>())
            {
                var list = new List<IReadOnlyList<int>>();
                using (subject.Slide(chunk)
                              .Subscribe(x => list.Add(x.ToArray())))
                {
                    CollectionAssert.IsEmpty(list);

                    subject.OnNext(1);
                    CollectionAssert.AreEqual(new[] { 1 }, list.Single());
                }
            }
        }

        [Test]
        public void OneChangeThrottled()
        {
            var scheduler = new TestScheduler();
            var chunk = new Chunk<int>(TimeSpan.FromMilliseconds(100), scheduler);
            using (var subject = new Subject<int>())
            {
                var list = new List<IReadOnlyList<int>>();
                using (subject.Slide(chunk)
                              .Subscribe(x => list.Add(x.ToArray())))
                {
                    CollectionAssert.IsEmpty(list);

                    subject.OnNext(1);
                    CollectionAssert.IsEmpty(list);
                    scheduler.Start();
                    CollectionAssert.AreEqual(new[] { 1 }, list.Single());
                }
            }
        }

        [Test]
        public void TwoChanges()
        {
            var scheduler = new TestScheduler();
            var chunk = new Chunk<int>(TimeSpan.FromMilliseconds(100), scheduler);
            using (var subject = new Subject<int>())
            {
                var list = new List<IReadOnlyList<int>>();
                using (subject.Slide(chunk)
                              .Subscribe(x => list.Add(x.ToArray())))
                {
                    CollectionAssert.IsEmpty(list);

                    subject.OnNext(1);
                    subject.OnNext(2);
                    CollectionAssert.IsEmpty(list);
                    scheduler.Start();
                    CollectionAssert.AreEqual(new[] { 1, 2 }, list.Single());
                }
            }
        }


        [Test]
        public void DoesNotLoseChangesWhenChangingBufferTime()
        {
            var scheduler = new TestScheduler();
            var chunk = new Chunk<int>(TimeSpan.FromMilliseconds(100), scheduler);
            using (var subject = new Subject<int>())
            {
                var list = new List<IReadOnlyList<int>>();
                using (subject.Slide(chunk)
                              .Subscribe(x => list.Add(x.ToArray())))
                {
                    CollectionAssert.IsEmpty(list);

                    subject.OnNext(1);
                    CollectionAssert.IsEmpty(list);

                    chunk.BufferTime = TimeSpan.FromMilliseconds(200);
                    scheduler.Start();
                    CollectionAssert.AreEqual(new[] { 1 }, list.Single());
                }
            }
        }
    }
}