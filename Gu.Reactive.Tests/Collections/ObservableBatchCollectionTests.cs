namespace Gu.Reactive.Tests.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization.Formatters.Binary;

    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    public class ObservableBatchCollectionTests
    {
        [Test]
        public void Add()
        {
            var reference = new ObservableCollection<int>();
            using (var expectedChanges = reference.SubscribeAll())
            {
                var batchCollection = new ObservableBatchCollection<int>();
                using (var actualChanges = batchCollection.SubscribeAll())
                {
                    reference.Add(1);
                    batchCollection.Add(1);
                    CollectionAssert.AreEqual(reference, batchCollection);
                    CollectionAssert.AreEqual(expectedChanges, actualChanges, EventArgsComparer.Default);

                    reference.Add(2);
                    batchCollection.Add(2);
                    CollectionAssert.AreEqual(reference, batchCollection);
                    CollectionAssert.AreEqual(expectedChanges, actualChanges, EventArgsComparer.Default);
                }
            }
        }

        [Test]
        public void AddRange()
        {
            var batchCollection = new ObservableBatchCollection<int>();
            using (var actualChanges = batchCollection.SubscribeAll())
            {
                var expectedChanges = new List<EventArgs>
                                          {
                                              CachedEventArgs.CountPropertyChanged,
                                              CachedEventArgs.IndexerPropertyChanged,
                                              CachedEventArgs.NotifyCollectionReset
                                          };
                batchCollection.AddRange(new[] { 1, 2 });
                CollectionAssert.AreEqual(new[] { 1, 2 }, batchCollection);
                CollectionAssert.AreEqual(expectedChanges, actualChanges, EventArgsComparer.Default);

                batchCollection.AddRange(new[] { 3, 4 });
                CollectionAssert.AreEqual(new[] { 1, 2, 3, 4 }, batchCollection);
                expectedChanges.AddRange(
                    new EventArgs[]
                        {
                            CachedEventArgs.CountPropertyChanged,
                            CachedEventArgs.IndexerPropertyChanged,
                            CachedEventArgs.NotifyCollectionReset
                        });
                CollectionAssert.AreEqual(expectedChanges, actualChanges, EventArgsComparer.Default);
            }
        }

        [Test]
        public void AddRangeSingle()
        {
            var reference = new ObservableCollection<int>();
            using (var expectedChanges = reference.SubscribeAll())
            {
                var batchCollection = new ObservableBatchCollection<int>();
                using (var actualChanges = batchCollection.SubscribeAll())
                {
                    reference.Add(1);
                    batchCollection.AddRange(new[] { 1 });
                    CollectionAssert.AreEqual(reference, batchCollection);
                    CollectionAssert.AreEqual(expectedChanges, actualChanges, EventArgsComparer.Default);

                    reference.Add(2);
                    batchCollection.AddRange(new[] { 2 });
                    CollectionAssert.AreEqual(reference, batchCollection);
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
                var batchCollection = new ObservableBatchCollection<int>(reference);
                using (var actualChanges = batchCollection.SubscribeAll())
                {
                    reference.Remove(1);
                    batchCollection.Remove(1);
                    CollectionAssert.AreEqual(reference, batchCollection);
                    CollectionAssert.AreEqual(expectedChanges, actualChanges, EventArgsComparer.Default);

                    reference.Remove(2);
                    batchCollection.Remove(2);
                    CollectionAssert.AreEqual(reference, batchCollection);
                    CollectionAssert.AreEqual(expectedChanges, actualChanges, EventArgsComparer.Default);
                }
            }
        }

        [Test]
        public void RemoveRange()
        {
            var batchCollection = new ObservableBatchCollection<int> { 1, 2, 3, 4 };
            using (var actualChanges = batchCollection.SubscribeAll())
            {
                var expectedChanges = new List<EventArgs>
                                          {
                                              CachedEventArgs.CountPropertyChanged,
                                              CachedEventArgs.IndexerPropertyChanged,
                                              CachedEventArgs.NotifyCollectionReset
                                          };

                batchCollection.RemoveRange(new[] { 1, 2 });
                CollectionAssert.AreEqual(new[] { 3, 4 }, batchCollection);
                CollectionAssert.AreEqual(expectedChanges, actualChanges, EventArgsComparer.Default);

                batchCollection.RemoveRange(new[] { 3, 4 });
                CollectionAssert.IsEmpty(batchCollection);
                expectedChanges.AddRange(
                    new EventArgs[]
                        {
                            CachedEventArgs.CountPropertyChanged,
                            CachedEventArgs.IndexerPropertyChanged,
                            CachedEventArgs.NotifyCollectionReset
                        });
                CollectionAssert.AreEqual(expectedChanges, actualChanges, EventArgsComparer.Default);
            }
        }

        [Test]
        public void RemoveAll()
        {
            var batchCollection = new ObservableBatchCollection<int> { 1, 2, 3, 4 };
            var reference = batchCollection.ToList();
            using (var actualChanges = batchCollection.SubscribeAll())
            {
                var expectedChanges = new List<EventArgs>
                                      {
                                          CachedEventArgs.CountPropertyChanged,
                                          CachedEventArgs.IndexerPropertyChanged,
                                          CachedEventArgs.NotifyCollectionReset
                                      };

                Assert.AreEqual(reference.RemoveAll(x => x % 2 == 0), batchCollection.RemoveAll(x => x % 2 == 0));
                CollectionAssert.AreEqual(reference, batchCollection);
                CollectionAssert.AreEqual(expectedChanges, actualChanges, EventArgsComparer.Default);
            }
        }

        [Test]
        public void RemoveRangeSingle()
        {
            var reference = new ObservableCollection<int> { 1, 2, 3 };
            using (var expectedChanges = reference.SubscribeAll())
            {
                var batchCollection = new ObservableBatchCollection<int>(reference);
                using (var actualChanges = batchCollection.SubscribeAll())
                {
                    reference.Remove(1);
                    batchCollection.RemoveRange(new[] { 1 });
                    CollectionAssert.AreEqual(reference, batchCollection);
                    CollectionAssert.AreEqual(expectedChanges, actualChanges, EventArgsComparer.Default);

                    reference.Remove(2);
                    batchCollection.RemoveRange(new[] { 2 });
                    CollectionAssert.AreEqual(reference, batchCollection);
                    CollectionAssert.AreEqual(expectedChanges, actualChanges, EventArgsComparer.Default);
                }
            }
        }

        [Test]
        public void ResetTo()
        {
            var batchCollection = new ObservableBatchCollection<int> { 1, 2, 3 };
            using (var actualChanges = batchCollection.SubscribeAll())
            {
                batchCollection.ResetTo(new[] { 4, 5 });
                CollectionAssert.AreEqual(new[] { 4, 5 }, batchCollection);
                var expectedChanges = new List<EventArgs>
                                      {
                                          CachedEventArgs.CountPropertyChanged,
                                          CachedEventArgs.IndexerPropertyChanged,
                                          CachedEventArgs.NotifyCollectionReset
                                      };
                CollectionAssert.AreEqual(expectedChanges, actualChanges, EventArgsComparer.Default);
            }
        }

        [Test]
        public void SerializeRountrip()
        {
            var binaryFormatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                var ints = new ObservableBatchCollection<int> { 1, 2 };
                binaryFormatter.Serialize(stream, ints);
                stream.Position = 0;
                var roundtripped = (ObservableBatchCollection<int>)binaryFormatter.Deserialize(stream);
                Assert.AreEqual(ints, roundtripped);
            }
        }
    }
}
