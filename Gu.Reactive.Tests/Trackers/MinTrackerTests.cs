namespace Gu.Reactive.Tests.Trackers
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using NUnit.Framework;

    public class MinTracker_PrimitiveTests
    {
        [Test]
        public void InitializesWithValues()
        {
            var ints = new ObservableCollection<int>(new[] { 1, 2, 3 });
            var tracker = ints.TrackMin(-1);
            Assert.AreEqual(1, tracker.Value);
        }

        [Test]
        public void InitializesWhenEmpty()
        {
            var ints = new ObservableCollection<int>(new int[0]);
            var tracker = ints.TrackMin(-1);
            Assert.AreEqual(-1, tracker.Value);
        }

        [TestCase(0, -2, -2, 1)]
        [TestCase(0, 2, 2, 1)]
        [TestCase(0, 3, 2, 1)]
        [TestCase(1, 3, 1, 0)]
        public void Replace(int index, int value, int expectedValue, int expectedCount)
        {
            var ints = new ObservableCollection<int>(new[] { 1, 2, 3 });
            var tracker = ints.TrackMin(-1);
            Assert.AreEqual(1, tracker.Value);
            int count = 0;
            tracker.ObservePropertyChanged(x => x.Value, false)
                   .Subscribe(_ => count++);
            ints[index] = value;
            Assert.AreEqual(expectedValue, tracker.Value);
            Assert.AreEqual(expectedCount, count);
        }

        [Test]
        public void ReactsAndNotifiesOnSourceChanges()
        {
            var ints = new ObservableCollection<int>(new[] { 1, 2, 3 });
            var tracker = MinTracker.TrackMin(ints, -1);
            Assert.AreEqual(1, tracker.Value);
            int count = 0;
            tracker.ObservePropertyChanged(x => x.Value, false)
                   .Subscribe(_ => count++);
            ints.Remove(2);
            Assert.AreEqual(0, count);
            Assert.AreEqual(1, tracker.Value);

            ints.Remove(1);
            Assert.AreEqual(1, count);
            Assert.AreEqual(3, tracker.Value);

            ints.Remove(3);
            Assert.AreEqual(2, count);
            Assert.AreEqual(-1, tracker.Value);

            ints.Add(2);
            Assert.AreEqual(3, count);
            Assert.AreEqual(2, tracker.Value);
        }
    }
}
