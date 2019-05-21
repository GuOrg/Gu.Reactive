namespace Gu.Reactive.Tests.Trackers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using NUnit.Framework;

    public partial class MinMaxTrackerTests
    {
        public class Simple
        {
            [Test]
            public void InitializesWithValues()
            {
                var ints = new ObservableCollection<int> { 1, 2, 3 };
                using (var tracker = ints.TrackMinMax())
                {
                    Assert.AreEqual(1, tracker.Min);
                    Assert.AreEqual(3, tracker.Max);
                }
            }

            [Test]
            public void InitializesWhenEmpty()
            {
                var ints = new ObservableCollection<int>(Array.Empty<int>());
                using (var tracker = ints.TrackMinMax())
                {
                    Assert.AreEqual(null, tracker.Min);
                    Assert.AreEqual(null, tracker.Max);
                }
            }

            [TestCase(0, -2, -2, 3, new[] { "Min" })]
            [TestCase(0, 2, 2, 3, new[] { "Min" })]
            [TestCase(0, 3, 2, 3, new[] { "Min" })]
            [TestCase(1, 3, 1, 3, new string[0])]
            [TestCase(0, 4, 2, 4, new[] { "Max", "Min" })]
            [TestCase(2, -3, -3, 2, new[] { "Min", "Max" })]
            [TestCase(1, 4, 1, 4, new[] { "Max" })]
            [TestCase(2, 2, 1, 2, new[] { "Max" })]
            public void Replace(int index, int value, int expectedMin, int expectedMax, string[] expected)
            {
                var ints = new ObservableCollection<int> { 1, 2, 3 };
                using (var tracker = ints.TrackMinMax())
                {
                    Assert.AreEqual(1, tracker.Min);
                    Assert.AreEqual(3, tracker.Max);

                    var actual = new List<PropertyChangedEventArgs>();
                    using (tracker.ObservePropertyChangedSlim()
                                  .Subscribe(actual.Add))
                    {
                        ints[index] = value;
                        Assert.AreEqual(expectedMin, tracker.Min);
                        Assert.AreEqual(expectedMax, tracker.Max);
                        CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));
                    }
                }
            }

            [Test]
            public void ReactsAndNotifiesOnSourceChanges()
            {
                var ints = new ObservableCollection<int> { 1, 2, 3 };
                using (var tracker = ints.TrackMinMax())
                {
                    Assert.AreEqual(1, tracker.Min);
                    Assert.AreEqual(3, tracker.Max);

                    var actual = new List<PropertyChangedEventArgs>();
                    using (tracker.ObservePropertyChangedSlim()
                                  .Subscribe(actual.Add))
                    {
                        ints.Remove(2);
                        Assert.AreEqual(1, tracker.Min);
                        Assert.AreEqual(3, tracker.Max);
                        CollectionAssert.IsEmpty(actual);

                        ints.Remove(1);
                        Assert.AreEqual(3, tracker.Min);
                        Assert.AreEqual(3, tracker.Max);
                        CollectionAssert.AreEqual(new[] { "Min" }, actual.Select(x => x.PropertyName));

                        ints.Remove(3);
                        Assert.AreEqual(null, tracker.Min);
                        Assert.AreEqual(null, tracker.Max);
                        CollectionAssert.AreEqual(new[] { "Min", "Min", "Max" }, actual.Select(x => x.PropertyName));

                        ints.Add(2);
                        Assert.AreEqual(2, tracker.Min);
                        Assert.AreEqual(2, tracker.Max);
                        CollectionAssert.AreEqual(new[] { "Min", "Min", "Max", "Min", "Max" }, actual.Select(x => x.PropertyName));
                    }
                }
            }
        }
    }
}
