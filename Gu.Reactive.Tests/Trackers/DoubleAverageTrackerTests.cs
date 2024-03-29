namespace Gu.Reactive.Tests.Trackers
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;

    using NUnit.Framework;

    public class DoubleAverageTrackerTests
    {
        [Test]
        public void InitializesWithValues()
        {
            var source = new ObservableCollection<double> { 1.0, 2, 3 };
            using var tracker = new DoubleAverageTracker(source);
            Assert.AreEqual(source.Average(), tracker.Value);
        }

        [Test]
        public void InitializesWhenEmpty()
        {
            var source = new ObservableCollection<double>();
            using var tracker = new DoubleAverageTracker(source);
            Assert.AreEqual(null, tracker.Value);
        }

        [Test]
        public void ReactsAndNotifiesOnSourceChanges()
        {
            var source = new ObservableCollection<double> { 1.0, 2, 3 };
            using var tracker = new DoubleAverageTracker(source);
            Assert.AreEqual(source.Average(), tracker.Value);
            var count = 0;
            using (tracker.ObservePropertyChangedSlim(x => x.Value, signalInitial: false)
                          .Subscribe(_ => count++))
            {
                source.Remove(2);
                Assert.AreEqual(0, count);
                Assert.AreEqual(source.Average(), tracker.Value);

                source.Remove(1);
                Assert.AreEqual(1, count);
                Assert.AreEqual(source.Average(), tracker.Value);

                source.Remove(3);
                Assert.AreEqual(2, count);
                Assert.AreEqual(null, tracker.Value);

                source.Add(2);
                Assert.AreEqual(3, count);
                Assert.AreEqual(source.Average(), tracker.Value);
            }
        }
    }
}
