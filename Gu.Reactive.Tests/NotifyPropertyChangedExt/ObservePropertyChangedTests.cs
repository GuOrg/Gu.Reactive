namespace Gu.Reactive.Tests.NotifyPropertyChangedExt
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reactive;

    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    public class ObservePropertyChangedTests
    {
        private List<EventPattern<PropertyChangedEventArgs>> _changes;

        [SetUp]
        public void SetUp()
        {
            _changes = new List<EventPattern<PropertyChangedEventArgs>>();
        }

        [Test]
        public void DoesNotSIgnalSubscribe()
        {
            var fake = new Fake { Value = 1 };
            var observable = fake.ObservePropertyChanged();
            var disposable = observable.Subscribe(_changes.Add);
            Assert.AreEqual(0, _changes.Count);
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase("Value")]
        public void ReactsOnStringEmptyOrNull(string prop)
        {
            var fake = new Fake { Value = 1 };
            fake.ObservePropertyChanged()
                .Subscribe(_changes.Add);
            Assert.AreEqual(0, _changes.Count);
            fake.OnPropertyChanged(prop); // This means all properties changed according to wpf convention
            Assert.AreEqual(1, _changes.Count);
        }

        [Test]
        public void ReactsOnEvent()
        {
            var fake = new Fake { Value = 1 };
            fake.ObservePropertyChanged()
                .Subscribe(_changes.Add);
            Assert.AreEqual(0, _changes.Count);
            fake.OnPropertyChanged("SomeProp");
            Assert.AreEqual(1, _changes.Count);
            AssertRx.AreEqual(fake, "SomeProp", _changes.Last());
        }

        [Test]
        public void ReactsValue()
        {
            var fake = new Fake { Value = 1 };
            fake.ObservePropertyChanged()
                .Subscribe(_changes.Add);
            Assert.AreEqual(0, _changes.Count);
            fake.Value++;
            Assert.AreEqual(1, _changes.Count);
            AssertRx.AreEqual(fake, "Value", _changes.Last());
        }

        [Test]
        public void ReactsTwoInstancesValue()
        {
            var fake1 = new Fake { Value = 1 };
            fake1.ObservePropertyChanged()
                .Subscribe(_changes.Add);
            var fake2 = new Fake { Value = 1 };
            fake2.ObservePropertyChanged()
                .Subscribe(_changes.Add);
            Assert.AreEqual(0, _changes.Count);

            fake1.Value++;
            Assert.AreEqual(1, _changes.Count);
            AssertRx.AreEqual(fake1, "Value", _changes.Last());

            fake2.Value++;
            Assert.AreEqual(2, _changes.Count);
            AssertRx.AreEqual(fake2, "Value", _changes.Last());
        }

        [Test]
        public void ReactsNullable()
        {
            var fake = new Fake { IsTrueOrNull = null };
            var observable = fake.ObservePropertyChanged();
            var disposable = observable.Subscribe(_changes.Add);

            Assert.AreEqual(0, _changes.Count);

            fake.IsTrueOrNull = true;
            Assert.AreEqual(1, _changes.Count);
            AssertRx.AreEqual(fake, "IsTrueOrNull", _changes.Last());

            fake.IsTrueOrNull = null;
            Assert.AreEqual(2, _changes.Count);
            AssertRx.AreEqual(fake, "IsTrueOrNull", _changes.Last());
        }

        [Test]
        public void StopsListeningOnDispose()
        {
            var fake = new Fake { IsTrue = true };
            var observable = fake.ObservePropertyChanged();
            var disposable = observable.Subscribe(_changes.Add);
            fake.IsTrue = !fake.IsTrue;
            Assert.AreEqual(1, _changes.Count);

            disposable.Dispose();
            fake.IsTrue = !fake.IsTrue;

            Assert.AreEqual(1, _changes.Count);
        }

        [Test]
        public void MemoryLeakDisposeTest()
        {
            var fake = new Fake();
            var wr = new WeakReference(fake);
            var observable = fake.ObservePropertyChanged();
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
            var observable = fake.ObservePropertyChanged();
            var subscription = observable.Subscribe();
            GC.KeepAlive(observable);
            GC.KeepAlive(subscription);

            fake = null;
            GC.Collect();

            Assert.IsFalse(wr.IsAlive);
        }
    }
}
