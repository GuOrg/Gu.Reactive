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
        [Test]
        public void SignalsInitialNull()
        {
            var changes = new List<EventPattern<PropertyChangedAndValueEventArgs<string>>>();
            var fake = new Fake();
            fake.ObservePropertyChangedWithValue(x => x.Name, true)
                .Subscribe(changes.Add);
            Assert.AreEqual(1, changes.Count);
            Assert.AreEqual(null, changes.Single().EventArgs.Value);
            Assert.AreSame(fake, changes.Single().Sender);
            Assert.IsTrue(changes.Single().EventArgs.HasValue);
        }

        [Test]
        public void SignalsInitialValue()
        {
            var changes = new List<EventPattern<PropertyChangedAndValueEventArgs<string>>>();
            var fake = new Fake { Name = "Johan" };
            fake.ObservePropertyChangedWithValue(x => x.Name, true)
                .Subscribe(changes.Add);
            Assert.AreEqual(1, changes.Count);
            Assert.AreEqual("Johan", changes.Single().EventArgs.Value);
            Assert.AreSame(fake, changes.Single().Sender);
            Assert.IsTrue(changes.Single().EventArgs.HasValue);
        }

        [Test]
        public void DoesNotSignalOnSubscribe()
        {
            var changes = new List<EventPattern<PropertyChangedAndValueEventArgs<string>>>();
            var fake = new Fake { Name = "Johan" };
            fake.ObservePropertyChangedWithValue(x => x.Name, false)
                .Subscribe(changes.Add);
            CollectionAssert.IsEmpty(changes);
        }

        [Test]
        public void SignalsOnSourceChanges()
        {
            var changes = new List<EventPattern<PropertyChangedAndValueEventArgs<string>>>();
            var fake = new Fake();
            fake.ObservePropertyChangedWithValue(x => x.Name, false)
                .Subscribe(changes.Add);
            CollectionAssert.IsEmpty(changes);
            fake.Name = "El Kurro";
            Assert.AreEqual(1, changes.Count);
            Assert.AreEqual("El Kurro", changes.Single().EventArgs.Value);
            Assert.AreSame(fake, changes.Single().Sender);
            Assert.IsTrue(changes.Single().EventArgs.HasValue);
        }

        [Test]
        public void SignalsOnDerivedSourceChanges()
        {
            var changes = new List<EventPattern<PropertyChangedAndValueEventArgs<string>>>();
            var fake = new DerivedFake();
            fake.ObservePropertyChangedWithValue(x => x.Name, false)
                .Subscribe(changes.Add);
            CollectionAssert.IsEmpty(changes);
            fake.Name = "El Kurro";
            Assert.AreEqual(1, changes.Count);
            Assert.AreEqual("El Kurro", changes.Single().EventArgs.Value);
            Assert.AreSame(fake, changes.Single().Sender);
            Assert.IsTrue(changes.Single().EventArgs.HasValue);
        }

        [Test]
        public void MemoryLeakNoDisposeTest()
        {
            var fake = new Fake();
            var wr = new WeakReference(fake);
            Assert.IsTrue(wr.IsAlive);
            var observable = fake.ObservePropertyChangedWithValue(x => x.Name, false);
#pragma warning disable GU0030 // Use using.
            var subscription = observable.Subscribe();
#pragma warning restore GU0030 // Use using.
            GC.KeepAlive(observable);
            GC.KeepAlive(subscription);

            // ReSharper disable once RedundantAssignment
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
            using (var subscription = observable.Subscribe())
            {
                GC.KeepAlive(observable);
                GC.KeepAlive(subscription);

                // ReSharper disable once RedundantAssignment
                fake = null;
            }

            GC.Collect();

            Assert.IsFalse(wr.IsAlive);
        }
    }
}