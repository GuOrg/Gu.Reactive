namespace Gu.Reactive.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive;

    using NUnit.Framework;

    public class NotifyProeprtyChangedExt_ObservePropertyChangedAndValue_Simple
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
            var fake = new FakeInpc();
            fake.ObservePropertyChangedAndValue(x => x.Name, true)
                .Subscribe(_changes.Add);
            Assert.AreEqual(1, _changes.Count);
            Assert.AreEqual(null, _changes.Single().EventArgs.Value);
            Assert.IsTrue(_changes.Single().EventArgs.IsDefaultValue);
        }

        [Test]
        public void CapturesInitialValue()
        {
            var fake = new FakeInpc{Name = "Johan"};
            fake.ObservePropertyChangedAndValue(x => x.Name, true)
                .Subscribe(_changes.Add);
            Assert.AreEqual(1, _changes.Count);
            Assert.AreEqual("Johan", _changes.Single().EventArgs.Value);
            Assert.IsTrue(_changes.Single().EventArgs.IsDefaultValue);
        }

        [Test]
        public void DoesNotSignalOnSubscribe()
        {
            var fake = new FakeInpc { Name = "Johan" };
            fake.ObservePropertyChangedAndValue(x => x.Name, false)
                .Subscribe(_changes.Add);
            CollectionAssert.IsEmpty(_changes);
        }

        [Test]
        public void SignalsOnSourceChanges()
        {
            var fake = new FakeInpc();
            fake.ObservePropertyChangedAndValue(x => x.Name, false)
                .Subscribe(_changes.Add);
            CollectionAssert.IsEmpty(_changes);
            fake.Name = "El Kurro";
            Assert.AreEqual(1, _changes.Count);
            Assert.AreEqual("El Kurro", _changes.Single().EventArgs.Value);
            Assert.IsTrue(_changes.Single().EventArgs.IsDefaultValue);
        }

        [Test]
        public void MemoryLeakNoDisposeTest()
        {
            var fake = new FakeInpc();
            var wr = new WeakReference(fake);
            Assert.IsTrue(wr.IsAlive);
            var subscription = fake.ObservePropertyChangedAndValue(x => x.Name, false)
                                   .Subscribe();
            fake = null;
            GC.Collect();
            Assert.IsFalse(wr.IsAlive);
            var s = subscription.ToString(); // touching it after GC.Collect for no optimizations
        }

        [Test]
        public void MemoryLeakDisposeTest()
        {
            var fake = new FakeInpc();
            var wr = new WeakReference(fake);
            Assert.IsTrue(wr.IsAlive);
            var subscription = fake.ObservePropertyChangedAndValue(x => x.Name, false)
                                   .Subscribe();
            fake = null;
            subscription.Dispose();
            GC.Collect();
            Assert.IsFalse(wr.IsAlive);
            var s = subscription.ToString(); // touching it after GC.Collect for no optimizations
        }
    }
}