namespace Gu.Reactive.Tests.NotifyProeprtyChangedExt
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive;

    using Gu.Reactive.Tests.Fakes;

    using NUnit.Framework;

    // ReSharper disable once InconsistentNaming
    public class ObservePropertyChangedAndValue_SimpleTests
    {
        private List<EventPattern<PropertyChangedAndValueEventArgs<string>>> _changes;

        [SetUp]
        public void SetUp()
        {
            _changes = new List<EventPattern<PropertyChangedAndValueEventArgs<string>>>();
        }

        [Test]
        public void SignalsInitialNull()
        {
            var fake = new Fake();
            fake.ObservePropertyChangedWithValue(x => x.Name, true)
                .Subscribe(_changes.Add);
            Assert.AreEqual(1, _changes.Count);
            Assert.AreEqual(null, _changes.Single().EventArgs.Value);
            Assert.AreSame(fake, _changes.Single().Sender);
            Assert.IsTrue(_changes.Single().EventArgs.HasValue);
        }

        [Test]
        public void SignalsInitialValue()
        {
            var fake = new Fake{Name = "Johan"};
            fake.ObservePropertyChangedWithValue(x => x.Name, true)
                .Subscribe(_changes.Add);
            Assert.AreEqual(1, _changes.Count);
            Assert.AreEqual("Johan", _changes.Single().EventArgs.Value);
            Assert.AreSame(fake, _changes.Single().Sender);
            Assert.IsTrue(_changes.Single().EventArgs.HasValue);
        }

        [Test]
        public void DoesNotSignalOnSubscribe()
        {
            var fake = new Fake { Name = "Johan" };
            fake.ObservePropertyChangedWithValue(x => x.Name, false)
                .Subscribe(_changes.Add);
            CollectionAssert.IsEmpty(_changes);
        }

        [Test]
        public void SignalsOnSourceChanges()
        {
            var fake = new Fake();
            fake.ObservePropertyChangedWithValue(x => x.Name, false)
                .Subscribe(_changes.Add);
            CollectionAssert.IsEmpty(_changes);
            fake.Name = "El Kurro";
            Assert.AreEqual(1, _changes.Count);
            Assert.AreEqual("El Kurro", _changes.Single().EventArgs.Value);
            Assert.AreSame(fake, _changes.Single().Sender);
            Assert.IsTrue(_changes.Single().EventArgs.HasValue);
        }

        [Test]
        public void MemoryLeakNoDisposeTest()
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