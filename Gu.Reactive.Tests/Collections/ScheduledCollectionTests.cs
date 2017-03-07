namespace Gu.Reactive.Tests.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using Gu.Reactive.Tests.Helpers;
    using NUnit.Framework;

    public class ScheduledCollectionTests
    {
        [Test]
        public void Add()
        {
            var reference = new ObservableCollection<int>();
            using (var expectedChanges = reference.SubscribeAll())
            {
                var scheduledCollection = new ScheduledCollection<int>();
                using (var actualChanges = scheduledCollection.SubscribeAll())
                {
                    reference.Add(1);
                    scheduledCollection.Add(1);
                    CollectionAssert.AreEqual(reference, scheduledCollection);
                    CollectionAssert.AreEqual(expectedChanges, actualChanges, EventArgsComparer.Default);

                    reference.Add(2);
                    scheduledCollection.Add(2);
                    CollectionAssert.AreEqual(reference, scheduledCollection);
                    CollectionAssert.AreEqual(expectedChanges, actualChanges, EventArgsComparer.Default);
                }
            }
        }

        [Test]
        public void AddRange()
        {
            var scheduledCollection = new ScheduledCollection<int>();
            using (var actualChanges = scheduledCollection.SubscribeAll())
            {
                var expectedChanges = new List<EventArgs>(CachedEventArgs.ResetEventArgsCollection);
                scheduledCollection.AddRange(new[] { 1, 2 });
                CollectionAssert.AreEqual(new[] { 1, 2 }, scheduledCollection);
                CollectionAssert.AreEqual(expectedChanges, actualChanges, EventArgsComparer.Default);

                scheduledCollection.AddRange(new[] { 3, 4 });
                CollectionAssert.AreEqual(new[] { 1, 2, 3, 4 }, scheduledCollection);
                expectedChanges.AddRange(CachedEventArgs.ResetEventArgsCollection);
                CollectionAssert.AreEqual(expectedChanges, actualChanges, EventArgsComparer.Default);
            }
        }

        [Test]
        public void AddRangeSingle()
        {
            var reference = new ObservableCollection<int>();
            using (var expectedChanges = reference.SubscribeAll())
            {
                var scheduledCollection = new ScheduledCollection<int>();
                using (var actualChanges = scheduledCollection.SubscribeAll())
                {
                    reference.Add(1);
                    scheduledCollection.AddRange(new[] { 1 });
                    CollectionAssert.AreEqual(reference, scheduledCollection);
                    CollectionAssert.AreEqual(expectedChanges, actualChanges, EventArgsComparer.Default);

                    reference.Add(2);
                    scheduledCollection.AddRange(new[] { 2 });
                    CollectionAssert.AreEqual(reference, scheduledCollection);
                    CollectionAssert.AreEqual(expectedChanges, actualChanges, EventArgsComparer.Default);
                }
            }
        }

        [Test]
        public void Remove()
        {
            var reference = new ObservableCollection<int> { 1, 2, 3 };
            using (var expectedChanges = reference.SubscribeAll())
            {
                var scheduledCollection = new ScheduledCollection<int>(reference);
                using (var actualChanges = scheduledCollection.SubscribeAll())
                {
                    reference.Remove(1);
                    scheduledCollection.Remove(1);
                    CollectionAssert.AreEqual(reference, scheduledCollection);
                    CollectionAssert.AreEqual(expectedChanges, actualChanges, EventArgsComparer.Default);

                    reference.Remove(2);
                    scheduledCollection.Remove(2);
                    CollectionAssert.AreEqual(reference, scheduledCollection);
                    CollectionAssert.AreEqual(expectedChanges, actualChanges, EventArgsComparer.Default);
                }
            }
        }

        [Test]
        public void RemoveRange()
        {
            var scheduledCollection = new ScheduledCollection<int> { 1, 2, 3, 4 };
            using (var actualChanges = scheduledCollection.SubscribeAll())
            {
                var expectedChanges = new List<EventArgs>(CachedEventArgs.ResetEventArgsCollection);

                scheduledCollection.RemoveRange(new[] { 1, 2 });
                CollectionAssert.AreEqual(new[] { 3, 4 }, scheduledCollection);
                CollectionAssert.AreEqual(expectedChanges, actualChanges, EventArgsComparer.Default);

                scheduledCollection.RemoveRange(new[] { 3, 4 });
                CollectionAssert.IsEmpty(scheduledCollection);
                expectedChanges.AddRange(CachedEventArgs.ResetEventArgsCollection);
                CollectionAssert.AreEqual(expectedChanges, actualChanges, EventArgsComparer.Default);
            }
        }

        [Test]
        public void RemoveRangeSingle()
        {
            var reference = new ObservableCollection<int> { 1, 2, 3 };
            using (var expectedChanges = reference.SubscribeAll())
            {
                var scheduledCollection = new ScheduledCollection<int>(reference);
                using (var actualChanges = scheduledCollection.SubscribeAll())
                {
                    reference.Remove(1);
                    scheduledCollection.RemoveRange(new[] { 1 });
                    CollectionAssert.AreEqual(reference, scheduledCollection);
                    CollectionAssert.AreEqual(expectedChanges, actualChanges, EventArgsComparer.Default);

                    reference.Remove(2);
                    scheduledCollection.RemoveRange(new[] { 2 });
                    CollectionAssert.AreEqual(reference, scheduledCollection);
                    CollectionAssert.AreEqual(expectedChanges, actualChanges, EventArgsComparer.Default);
                }
            }
        }

        [Test]
        public async Task ThreadTest()
        {
            var ints = new ScheduledCollection<int>();
            await Task.WhenAll(Enumerable.Range(0, 8)
                                         .Select(x =>
                                                     Task.Run(() =>
                                                     {
                                                         if (x % 2 == 0)
                                                         {
                                                             for (var i = 0; i < 1000; i++)
                                                             {
                                                                 ints.Remove(i);
                                                             }
                                                         }
                                                         else
                                                         {
                                                             for (var i = 0; i < 1000; i++)
                                                             {
                                                                 ints.Add(i);
                                                             }
                                                         }
                                                     })));
        }
    }
}