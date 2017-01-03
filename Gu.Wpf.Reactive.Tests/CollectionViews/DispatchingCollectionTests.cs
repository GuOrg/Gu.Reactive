namespace Gu.Wpf.Reactive.Tests.CollectionViews
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    using Gu.Reactive;
    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    public class DispatchingCollectionTests
    {
        [Test]
        public void Add()
        {
            var reference = new ObservableCollection<int>();
            var expectedChanges = reference.SubscribeAll();

            var batchCollection = new DispatchingCollection<int>();
            var actualChanges = batchCollection.SubscribeAll();

            reference.Add(1);
            batchCollection.Add(1);
            CollectionAssert.AreEqual(reference, batchCollection);
            CollectionAssert.AreEqual(expectedChanges, actualChanges, EventArgsComparer.Default);

            reference.Add(2);
            batchCollection.Add(2);
            CollectionAssert.AreEqual(reference, batchCollection);
            CollectionAssert.AreEqual(expectedChanges, actualChanges, EventArgsComparer.Default);
        }

        [Test]
        public void AddRange()
        {
            var batchCollection = new DispatchingCollection<int>();
            var actualChanges = batchCollection.SubscribeAll();
            var expectedChanges = new List<EventArgs>(Diff.ResetEventArgsCollection);
            batchCollection.AddRange(new[] { 1, 2 });
            CollectionAssert.AreEqual(new[] { 1, 2 }, batchCollection);
            CollectionAssert.AreEqual(expectedChanges, actualChanges, EventArgsComparer.Default);

            batchCollection.AddRange(new[] { 3, 4 });
            CollectionAssert.AreEqual(new[] { 1, 2, 3, 4 }, batchCollection);
            expectedChanges.AddRange(Diff.ResetEventArgsCollection);
            CollectionAssert.AreEqual(expectedChanges, actualChanges, EventArgsComparer.Default);
        }

        [Test]
        public void AddRangeSingle()
        {
            var reference = new ObservableCollection<int>();
            var expectedChanges = reference.SubscribeAll();

            var batchCollection = new DispatchingCollection<int>();
            var actualChanges = batchCollection.SubscribeAll();

            reference.Add(1);
            batchCollection.AddRange(new[] { 1 });
            CollectionAssert.AreEqual(reference, batchCollection);
            CollectionAssert.AreEqual(expectedChanges, actualChanges, EventArgsComparer.Default);

            reference.Add(2);
            batchCollection.AddRange(new[] { 2 });
            CollectionAssert.AreEqual(reference, batchCollection);
            CollectionAssert.AreEqual(expectedChanges, actualChanges, EventArgsComparer.Default);
        }

        [Test]
        public void Remove()
        {
            var reference = new ObservableCollection<int> { 1, 2, 3 };
            var expectedChanges = reference.SubscribeAll();

            var batchCollection = new DispatchingCollection<int>(reference);
            var actualChanges = batchCollection.SubscribeAll();

            reference.Remove(1);
            batchCollection.Remove(1);
            CollectionAssert.AreEqual(reference, batchCollection);
            CollectionAssert.AreEqual(expectedChanges, actualChanges, EventArgsComparer.Default);

            reference.Remove(2);
            batchCollection.Remove(2);
            CollectionAssert.AreEqual(reference, batchCollection);
            CollectionAssert.AreEqual(expectedChanges, actualChanges, EventArgsComparer.Default);
        }

        [Test]
        public void RemoveRange()
        {
            var batchCollection = new DispatchingCollection<int> { 1, 2, 3, 4 };
            var actualChanges = batchCollection.SubscribeAll();
            var expectedChanges = new List<EventArgs>(Diff.ResetEventArgsCollection);

            batchCollection.RemoveRange(new[] { 1, 2 });
            CollectionAssert.AreEqual(new[] { 3, 4 }, batchCollection);
            CollectionAssert.AreEqual(expectedChanges, actualChanges, EventArgsComparer.Default);

            batchCollection.RemoveRange(new[] { 3, 4 });
            CollectionAssert.IsEmpty(batchCollection);
            expectedChanges.AddRange(Diff.ResetEventArgsCollection);
            CollectionAssert.AreEqual(expectedChanges, actualChanges, EventArgsComparer.Default);
        }

        [Test]
        public void RemoveRangeSingle()
        {
            var reference = new ObservableCollection<int> { 1, 2, 3 };
            var expectedChanges = reference.SubscribeAll();

            var batchCollection = new DispatchingCollection<int>(reference);
            var actualChanges = batchCollection.SubscribeAll();

            reference.Remove(1);
            batchCollection.RemoveRange(new[] { 1 });
            CollectionAssert.AreEqual(reference, batchCollection);
            CollectionAssert.AreEqual(expectedChanges, actualChanges, EventArgsComparer.Default);

            reference.Remove(2);
            batchCollection.RemoveRange(new[] { 2 });
            CollectionAssert.AreEqual(reference, batchCollection);
            CollectionAssert.AreEqual(expectedChanges, actualChanges, EventArgsComparer.Default);
        }

        [Test]
        public void SerializeRountrip()
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