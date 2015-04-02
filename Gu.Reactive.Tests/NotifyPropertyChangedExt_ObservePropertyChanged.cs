namespace Gu.Reactive.Tests
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reactive;

    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    public class NotifyPropertyChangedExt_ObservePropertyChanged
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
            fake.OnPropertyChanged("Value");
            Assert.AreEqual(1, _changes.Count);
            Assert.AreSame(fake, _changes.Last().Sender);
            Assert.AreEqual("Value", _changes.Last().EventArgs.PropertyName);
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
            Assert.AreSame(fake, _changes.Last().Sender);
            Assert.AreEqual("Value", _changes.Last().EventArgs.PropertyName);
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
            Assert.AreSame(fake1, _changes.Last().Sender);
            Assert.AreEqual("Value", _changes.Last().EventArgs.PropertyName);

            fake2.Value++;
            Assert.AreEqual(2, _changes.Count);
            Assert.AreSame(fake2, _changes.Last().Sender);
            Assert.AreEqual("Value", _changes.Last().EventArgs.PropertyName);
        }

        [Test]
        public void ReactsNullable()
        {
            var fake = new Fake { IsTrueOrNull = null};
            var observable = fake.ObservePropertyChanged();
            var disposable = observable.Subscribe(_changes.Add);
            Assert.AreEqual(0, _changes.Count);
            fake.IsTrueOrNull = true;
            Assert.AreEqual(1, _changes.Count);
            Assert.AreSame(fake, _changes.Last().Sender);
            Assert.AreEqual("IsTrueOrNull", _changes.Last().EventArgs.PropertyName);

            fake.IsTrueOrNull = null;
            Assert.AreEqual(2, _changes.Count);
            Assert.AreSame(fake, _changes.Last().Sender);
            Assert.AreEqual("IsTrueOrNull", _changes.Last().EventArgs.PropertyName);
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
            var subscription = fake.ObservePropertyChanged().Subscribe();
            fake = null;
            subscription.Dispose();
            GC.Collect();
            Assert.IsFalse(wr.IsAlive);
            var s = subscription.ToString(); // touching it after GC.Collect for no optimizations
        }

        [Test]
        public void MemoryLeakNoDisposeTest()
        {
            var fake = new Fake();
            var wr = new WeakReference(fake);
            var subscription = fake.ObservePropertyChanged().Subscribe();
            fake = null;
            GC.Collect();
            Assert.IsFalse(wr.IsAlive);
            var s = subscription.ToString(); // touching it after GC.Collect for no optimizations
        }
    }
}
