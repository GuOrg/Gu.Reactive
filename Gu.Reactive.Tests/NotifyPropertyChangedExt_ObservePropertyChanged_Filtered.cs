namespace Gu.Reactive.Tests
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reactive;

    using Gu.Reactive.Tests.Fakes;

    using NUnit.Framework;
    // ReSharper disable once InconsistentNaming
    public class NotifyPropertyChangedExt_ObservePropertyChanged_Filtered
    {
        private List<EventPattern<PropertyChangedEventArgs>> _changes;

        [SetUp]
        public void SetUp()
        {
            _changes = new List<EventPattern<PropertyChangedEventArgs>>();
        }

        [Test]
        public void HandlesNull()
        {
            var fake = new Fake { Name = "1" };
            fake.ObservePropertyChanged(x => x.Name, false)
                .Subscribe(_changes.Add);

            Assert.AreEqual(0, _changes.Count);

            fake.Name = null;
            Assert.AreEqual(1, _changes.Count);
            AssertEventPattern(fake, "Name", _changes.Last());

            fake.Name = "1";
            Assert.AreEqual(2, _changes.Count);
            AssertEventPattern(fake, "Name", _changes.Last());
        }

        [Test]
        public void ReactsTwoPropertiesSameInstance()
        {
            var fake = new Fake { Value = 1 };
            fake.ObservePropertyChanged(x => x.Value, false)
                .Subscribe(_changes.Add);

            fake.ObservePropertyChanged(x => x.IsTrue, false)
                .Subscribe(_changes.Add);
            Assert.AreEqual(0, _changes.Count);

            fake.Value++;
            Assert.AreEqual(1, _changes.Count);
            AssertEventPattern(fake, "Value", _changes.Last());

            fake.IsTrue = !fake.IsTrue;
            Assert.AreEqual(2, _changes.Count);
            AssertEventPattern(fake, "IsTrue", _changes.Last());
        }

        [Test]
        public void ReactsTwoInstances()
        {
            var fake1 = new Fake { Value = 1 };
            fake1.ObservePropertyChanged(x => x.Value, false)
                .Subscribe(_changes.Add);
            var fake2 = new Fake { Value = 1 };
            fake2.ObservePropertyChanged(x => x.Value, false)
                .Subscribe(_changes.Add);
            Assert.AreEqual(0, _changes.Count);

            fake1.Value++;
            Assert.AreEqual(1, _changes.Count);
            AssertEventPattern(fake1, "Value", _changes.Last());

            fake2.Value++;
            Assert.AreEqual(2, _changes.Count);
            AssertEventPattern(fake2, "Value", _changes.Last());
        }

        [Test]
        public void ReactsWhenValueChanges()
        {
            var fake = new Fake { Value = 1 };
            fake.ObservePropertyChanged(x => x.Value, false)
                .Subscribe(_changes.Add);
            Assert.AreEqual(0, _changes.Count);

            fake.Value++;

            Assert.AreEqual(1, _changes.Count);
            AssertEventPattern(fake, "Value", _changes.Last());
        }

        [Test]
        public void DoesNotReactWhenOtherPropertyChanges()
        {
            var fake = new Fake { Value = 1 };
            fake.ObservePropertyChanged(x => x.Value, false)
                .Subscribe(_changes.Add);
            Assert.AreEqual(0, _changes.Count);
            fake.Value++;
            Assert.AreEqual(1, _changes.Count);
            AssertEventPattern(fake, "Value", _changes.Last());

            fake.IsTrue = !fake.IsTrue;

            Assert.AreEqual(1, _changes.Count); // No notification when changing other property
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase("Value")]
        public void ReactsOnStringEmptyOrNull(string propertyName)
        {
            var fake = new Fake { Value = 1 };
            fake.ObservePropertyChanged(x => x.Value, false)
                .Subscribe(_changes.Add);

            Assert.AreEqual(0, _changes.Count);

            fake.OnPropertyChanged(propertyName); // This means all properties changed according to wpf convention

            Assert.AreEqual(1, _changes.Count);
            AssertEventPattern(fake, propertyName, _changes.Last());
        }

        [TestCase(true, 1)]
        [TestCase(false, 0)]
        public void SignalsInitial(bool signalInitial, int expected)
        {
            var fake = new Fake { Value = 1 };

            fake.ObservePropertyChanged(x => x.Value, signalInitial)
                .Subscribe(_changes.Add);

            Assert.AreEqual(expected, _changes.Count);
            if (signalInitial)
            {
                AssertEventPattern(fake, "Value", _changes.Last());
            }
            fake.Value++;
            Assert.AreEqual(expected + 1, _changes.Count); // Double check that we are subscribing
        }

        [Test]
        public void MemoryLeakDisposeTest()
        {
            var fake = new Fake();
            var wr = new WeakReference(fake);
            var observable = fake.ObservePropertyChanged(x => x.IsTrueOrNull);
            var subscription = observable.Subscribe();
            GC.KeepAlive(observable);
            GC.KeepAlive(subscription);

            fake = null;
            subscription.Dispose();
            GC.Collect();

            Assert.IsFalse(wr.IsAlive);
        }

        [Test]
        public void MemoryLeakNoDisposeTest()
        {
            var fake = new Fake();
            var wr = new WeakReference(fake);
            var observable = fake.ObservePropertyChanged(x => x.IsTrueOrNull);
            var subscription = observable.Subscribe();
            GC.KeepAlive(observable);
            GC.KeepAlive(subscription);

            fake = null;
            GC.Collect();

            Assert.IsFalse(wr.IsAlive);
        }

        [Test]
        public void MemoryLeakFilteredNoDisposeTest()
        {
            var fake = new Fake();
            var wr = new WeakReference(fake);
            Assert.IsTrue(wr.IsAlive);
            var observable = fake.ObservePropertyChanged(x => x.Name);
            var subscription = observable.Subscribe();
            GC.KeepAlive(observable);
            GC.KeepAlive(subscription);

            fake = null;
            GC.Collect();
            Assert.IsFalse(wr.IsAlive);
        }

        private static void AssertEventPattern(object sender, string propertyName, EventPattern<PropertyChangedEventArgs> pattern)
        {
            Assert.AreSame(sender, pattern.Sender);
            Assert.AreEqual(propertyName, pattern.EventArgs.PropertyName);
        }
    }
}