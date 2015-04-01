namespace Gu.Reactive.Tests
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reactive;

    using NUnit.Framework;

    public class NotifyPropertyChangedExt_ObservePropertyChanged_Filtered
    {
        private List<EventPattern<PropertyChangedEventArgs>> _changes;

        [SetUp]
        public void SetUp()
        {
            _changes = new List<EventPattern<PropertyChangedEventArgs>>();
        }

        [Test]
        public void ReactsWhenValueChanges()
        {
            var fake = new FakeInpc { Value = 1 };
            fake.ObservePropertyChanged(x => x.Value, false)
                .Subscribe(_changes.Add);
            Assert.AreEqual(0, _changes.Count);

            fake.Value++;

            Assert.AreEqual(1, _changes.Count);
            Assert.AreSame(fake, _changes.Last().Sender);
            Assert.AreEqual("Value", _changes.Last().EventArgs.PropertyName);
        }

        [Test]
        public void DoesNotReactWhenOtherPropertyChanges()
        {
            var fake = new FakeInpc { Value = 1 };
            fake.ObservePropertyChanged(x => x.Value, false)
                .Subscribe(_changes.Add);
            Assert.AreEqual(0, _changes.Count);
            fake.Value++;
            Assert.AreEqual(1, _changes.Count);
            Assert.AreSame(fake, _changes.Last().Sender);
            Assert.AreEqual("Value", _changes.Last().EventArgs.PropertyName);

            fake.IsTrue = !fake.IsTrue;

            Assert.AreEqual(1, _changes.Count); // No notification when changing other property
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase("Value")]
        public void ReactsOnStringEmptyOrNull(string prop)
        {
            var fake = new FakeInpc { Value = 1 };
            fake.ObservePropertyChanged(x => x.Value, false)
                .Subscribe(_changes.Add);

            Assert.AreEqual(0, _changes.Count);

            fake.OnPropertyChanged(prop); // This means all properties changed according to wpf convention

            Assert.AreEqual(1, _changes.Count);
            Assert.AreSame(fake, _changes.Last().Sender);
            Assert.AreEqual(prop, _changes.Last().EventArgs.PropertyName);
        }

        [TestCase(true, 1)]
        [TestCase(false, 0)]
        public void SignalsInitial(bool signalInitial, int expected)
        {
            var fake = new FakeInpc { Value = 1 };

            fake.ObservePropertyChanged(x => x.Value, signalInitial)
                .Subscribe(_changes.Add);

            Assert.AreEqual(expected, _changes.Count);

            fake.Value++;
            Assert.AreEqual(expected + 1, _changes.Count); // Double check that we are subscribing
        }

        [Test]
        public void MemoryLeakDisposeTest()
        {
            var fake = new FakeInpc();
            var wr = new WeakReference(fake);
            var subscription = fake.ObservePropertyChanged(x => x.IsTrueOrNull).Subscribe();
            fake = null;
            subscription.Dispose();
            GC.Collect();
            Assert.IsFalse(wr.IsAlive);
            var s = subscription.ToString(); // touching it after GC.Collect for no optimizations
        }

        [Test]
        public void MemoryLeakNoDisposeTest()
        {
            var fake = new FakeInpc();
            var wr = new WeakReference(fake);
            var subscription = fake.ObservePropertyChanged(x => x.IsTrueOrNull).Subscribe();
            fake = null;
            GC.Collect();
            Assert.IsFalse(wr.IsAlive);
            var s = subscription.ToString(); // touching it after GC.Collect for no optimizations
        }


        [Test]
        public void MemoryLeakFilteredNoDisposeTest()
        {
            var fake = new FakeInpc();
            var wr = new WeakReference(fake);
            Assert.IsTrue(wr.IsAlive);
            var subscription = fake.ObservePropertyChanged(x => x.Name)
                                   .Subscribe();
            fake = null;
            GC.Collect();
            Assert.IsFalse(wr.IsAlive);
            var s = subscription.ToString(); // touching it after GC.Collect for no optimizations
        }

    }
}