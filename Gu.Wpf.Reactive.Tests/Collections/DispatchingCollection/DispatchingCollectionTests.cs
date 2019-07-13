namespace Gu.Wpf.Reactive.Tests.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    using Gu.Reactive;
    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    public static class DispatchingCollectionTests
    {
        [Test]
        public static void Add()
        {
            var reference = new ObservableCollection<int>();
            using (var expected = reference.SubscribeAll())
            {
                var batchCollection = new DispatchingCollection<int>();
                using (var actual = batchCollection.SubscribeAll())
                {
                    reference.Add(1);
                    batchCollection.Add(1);
                    CollectionAssert.AreEqual(reference, batchCollection);
                    CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);

                    reference.Add(2);
                    batchCollection.Add(2);
                    CollectionAssert.AreEqual(reference, batchCollection);
                    CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                }
            }
        }

        [Test]
        public static void AddRange()
        {
            var batchCollection = new DispatchingCollection<int>();
            using (var actual = batchCollection.SubscribeAll())
            {
                var expectedChanges = new List<EventArgs>
                                          {
                                              CachedEventArgs.CountPropertyChanged,
                                              CachedEventArgs.IndexerPropertyChanged,
                                              CachedEventArgs.NotifyCollectionReset,
                                          };
                batchCollection.AddRange(new[] { 1, 2 });
                CollectionAssert.AreEqual(new[] { 1, 2 }, batchCollection);
                CollectionAssert.AreEqual(expectedChanges, actual, EventArgsComparer.Default);

                batchCollection.AddRange(new[] { 3, 4 });
                CollectionAssert.AreEqual(new[] { 1, 2, 3, 4 }, batchCollection);
                expectedChanges.AddRange(
                    new EventArgs[]
                        {
                            CachedEventArgs.CountPropertyChanged,
                            CachedEventArgs.IndexerPropertyChanged,
                            CachedEventArgs.NotifyCollectionReset,
                        });
                CollectionAssert.AreEqual(expectedChanges, actual, EventArgsComparer.Default);
            }
        }

        [Test]
        public static void AddRangeSingle()
        {
            var reference = new ObservableCollection<int>();
            using (var expected = reference.SubscribeAll())
            {
                var batchCollection = new DispatchingCollection<int>();
                using (var actual = batchCollection.SubscribeAll())
                {
                    reference.Add(1);
                    batchCollection.AddRange(new[] { 1 });
                    CollectionAssert.AreEqual(reference, batchCollection);
                    CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);

                    reference.Add(2);
                    batchCollection.AddRange(new[] { 2 });
                    CollectionAssert.AreEqual(reference, batchCollection);
                    CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                }
            }
        }

        [Test]
        public static void Remove()
        {
            var reference = new ObservableCollection<int> { 1, 2, 3 };
            using (var expected = reference.SubscribeAll())
            {
                var batchCollection = new DispatchingCollection<int>(reference);
                using (var actual = batchCollection.SubscribeAll())
                {
                    reference.Remove(1);
                    batchCollection.Remove(1);
                    CollectionAssert.AreEqual(reference, batchCollection);
                    CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);

                    reference.Remove(2);
                    batchCollection.Remove(2);
                    CollectionAssert.AreEqual(reference, batchCollection);
                    CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                }
            }
        }

        [Test]
        public static void RemoveRange()
        {
            var batchCollection = new DispatchingCollection<int> { 1, 2, 3, 4 };
            using (var actual = batchCollection.SubscribeAll())
            {
                var expectedChanges = new List<EventArgs>
                                          {
                                              CachedEventArgs.CountPropertyChanged,
                                              CachedEventArgs.IndexerPropertyChanged,
                                              CachedEventArgs.NotifyCollectionReset,
                                          };

                batchCollection.RemoveRange(new[] { 1, 2 });
                CollectionAssert.AreEqual(new[] { 3, 4 }, batchCollection);
                CollectionAssert.AreEqual(expectedChanges, actual, EventArgsComparer.Default);

                batchCollection.RemoveRange(new[] { 3, 4 });
                CollectionAssert.IsEmpty(batchCollection);
                expectedChanges.AddRange(
                    new EventArgs[]
                        {
                            CachedEventArgs.CountPropertyChanged,
                            CachedEventArgs.IndexerPropertyChanged,
                            CachedEventArgs.NotifyCollectionReset,
                        });
                CollectionAssert.AreEqual(expectedChanges, actual, EventArgsComparer.Default);
            }
        }

        [Test]
        public static void RemoveRangeSingle()
        {
            var reference = new ObservableCollection<int> { 1, 2, 3 };
            using (var expected = reference.SubscribeAll())
            {
                var batchCollection = new DispatchingCollection<int>(reference);
                using (var actual = batchCollection.SubscribeAll())
                {
                    reference.Remove(1);
                    batchCollection.RemoveRange(new[] { 1 });
                    CollectionAssert.AreEqual(reference, batchCollection);
                    CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);

                    reference.Remove(2);
                    batchCollection.RemoveRange(new[] { 2 });
                    CollectionAssert.AreEqual(reference, batchCollection);
                    CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                }
            }
        }

        [Test]
        public static void SerializeRountrip()
        {
            var binaryFormatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                var ints = new DispatchingCollection<int> { 1, 2 };
                binaryFormatter.Serialize(stream, ints);
                stream.Position = 0;
                var roundtripped = (DispatchingCollection<int>)binaryFormatter.Deserialize(stream);
                Assert.AreEqual(ints, roundtripped);
            }
        }
    }
}
