namespace Gu.Reactive.Tests.NotifyPropertyChangedExt
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive;

    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    // ReSharper disable once InconsistentNaming
    public class ObservePropertyChangedAndValue_SimpleTests
    {
        private List<EventPattern<PropertyChangedAndValueEventArgs<string>>> changes;

        [SetUp]
        public void SetUp()
        {
            this.changes = new List<EventPattern<PropertyChangedAndValueEventArgs<string>>>();
        }

        [Test]
        public void SignalsInitialNull()
        {
            var fake = new Fake();
            fake.ObservePropertyChangedWithValue(x => x.Name, true)
                .Subscribe(this.changes.Add);
            Assert.AreEqual(1, this.changes.Count);
            Assert.AreEqual(null, this.changes.Single().EventArgs.Value);
            Assert.AreSame(fake, this.changes.Single().Sender);
            Assert.IsTrue(this.changes.Single().EventArgs.HasValue);
        }

        [Test]
        public void SignalsInitialValue()
        {
            var fake = new Fake { Name = "Johan" };
            fake.ObservePropertyChangedWithValue(x => x.Name, true)
                .Subscribe(this.changes.Add);
            Assert.AreEqual(1, this.changes.Count);
            Assert.AreEqual("Johan", this.changes.Single().EventArgs.Value);
            Assert.AreSame(fake, this.changes.Single().Sender);
            Assert.IsTrue(this.changes.Single().EventArgs.HasValue);
        }

        [Test]
        public void DoesNotSignalOnSubscribe()
        {
            var fake = new Fake { Name = "Johan" };
            fake.ObservePropertyChangedWithValue(x => x.Name, false)
                .Subscribe(this.changes.Add);
            CollectionAssert.IsEmpty(this.changes);
        }

        [Test]
        public void SignalsOnSourceChanges()
        {
            var fake = new Fake();
            fake.ObservePropertyChangedWithValue(x => x.Name, false)
                .Subscribe(this.changes.Add);
            CollectionAssert.IsEmpty(this.changes);
            fake.Name = "El Kurro";
            Assert.AreEqual(1, this.changes.Count);
            Assert.AreEqual("El Kurro", this.changes.Single().EventArgs.Value);
            Assert.AreSame(fake, this.changes.Single().Sender);
            Assert.IsTrue(this.changes.Single().EventArgs.HasValue);
        }

        [Test]
        public void SignalsOnDerivedSourceChanges()
        {
            var fake = new DerivedFake();
            fake.ObservePropertyChangedWithValue(x => x.Name, false)
                .Subscribe(this.changes.Add);
            CollectionAssert.IsEmpty(this.changes);
            fake.Name = "El Kurro";
            Assert.AreEqual(1, this.changes.Count);
            Assert.AreEqual("El Kurro", this.changes.Single().EventArgs.Value);
            Assert.AreSame(fake, this.changes.Single().Sender);
            Assert.IsTrue(this.changes.Single().EventArgs.HasValue);
        }

        [Test]
        public void MemoryLeakNoDisposeTest()
        {
            var fake = new Fake();
            var wr = new WeakReference(fake);
            Assert.IsTrue(wr.IsAlive);
            var observable = fake.ObservePropertyChangedWithValue(x => x.Name, false);
            var subscription = observable.Subscribe();
            GC.KeepAlive(observable);
            GC.KeepAlive(subscription);

            fake = null;
            GC.Collect();

            Assert.IsFalse(wr.IsAlive);
        }

        [Test]
        public void MemoryLeakDisposeTest()
        {
            var fake = new Fake();
            var wr = new WeakReference(fake);
            Assert.IsTrue(wr.IsAlive);
            var observable = fake.ObservePropertyChangedWithValue(x => x.Name, false);
            var subscription = observable
                                   .Subscribe();
            GC.KeepAlive(observable);
            GC.KeepAlive(subscription);

            fake = null;
            subscription.Dispose();
            GC.Collect();

            Assert.IsFalse(wr.IsAlive);
        }
    }
}