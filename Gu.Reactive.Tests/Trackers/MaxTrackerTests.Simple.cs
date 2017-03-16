namespace Gu.Reactive.Tests.Trackers
{
    using System;
    using System.Collections.ObjectModel;
    using NUnit.Framework;

    public partial class MaxTrackerTests
    {
        public class Simple
        {
            [Test]
            public void InitializesWithValues()
            {
                var ints = new ObservableCollection<int> { 1, 2, 3 };
                using (var tracker = ints.TrackMax())
                {
                    Assert.AreEqual(3, tracker.Value);
                }
            }

            [Test]
            public void InitializesWhenEmpty()
            {
                var ints = new ObservableCollection<int>();
                using (var tracker = ints.TrackMax())
                {
                    Assert.AreEqual(null, tracker.Value);
                }
            }

            [TestCase(0, 4, 4, 1)]
            [TestCase(2, 4, 4, 1)]
            [TestCase(0, 4, 4, 1)]
            [TestCase(1, 3, 3, 0)]
            public void Replace(int index, int value, int expectedValue, int expectedCount)
            {
                var ints = new ObservableCollection<int> { 1, 2, 3 };
                int count;
                using (var tracker = ints.TrackMax())
                {
                    Assert.AreEqual(3, tracker.Value);
                    count = 0;
                    using (tracker.ObservePropertyChangedSlim(x => x.Value, false)
                                  .Subscribe(_ => count++))
                    {
                        ints[index] = value;
                        Assert.AreEqual(expectedValue, tracker.Value);
                        Assert.AreEqual(expectedCount, count);
                    }
                }
            }

            [Test]
            public void ReactsAndNotifiesOnSourceChanges()
            {
                var ints = new ObservableCollection<int> { 1, 2, 3 };
                using (var tracker = ints.TrackMax())
                {
                    Assert.AreEqual(3, tracker.Value);
                    int count = 0;
                    using (tracker.ObservePropertyChangedSlim(x => x.Value, false)
                                  .Subscribe(_ => count++))
                    {
                        ints.Remove(1);
                        Assert.AreEqual(0, count);
                        Assert.AreEqual(3, tracker.Value);

                        ints.Remove(3);
                        Assert.AreEqual(1, count);
                        Assert.AreEqual(2, tracker.Value);

                        ints.Remove(2);
                        Assert.AreEqual(2, count);
                        Assert.AreEqual(null, tracker.Value);

                        ints.Add(2);
                        Assert.AreEqual(3, count);
                        Assert.AreEqual(2, tracker.Value);
                    }
                }
            }
        }
    }
}