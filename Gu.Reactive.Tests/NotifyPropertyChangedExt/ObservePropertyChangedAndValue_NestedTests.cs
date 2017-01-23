// ReSharper disable All
namespace Gu.Reactive.Tests.NotifyPropertyChangedExt
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive;

    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    // ReSharper disable once InconsistentNaming
    public class ObservePropertyChangedAndValue_NestedTests
    {
        [Test]
        [Explicit("Implement")]
        public void TypedEventargsTest()
        {
        }

        [TestCase("")]
        [TestCase(null)]
        public void DoesNotReactOnStringEmptyOrNullFromRootWithoutValue(string prop)
        {
            var changes = new List<EventPattern<PropertyChangedAndValueEventArgs<string>>>();
            var fake = new Fake();
            fake.ObservePropertyChangedWithValue(x => x.Next.Name, false)
                .Subscribe(changes.Add);
            Assert.AreEqual(0, changes.Count);
            fake.OnPropertyChanged(prop); // This means all properties changed according to wpf convention
            Assert.AreEqual(0, changes.Count);
        }

        [TestCase("")]
        [TestCase(null)]
        public void ReactsOnStringEmptyOrNullFromRootWithValue(string prop)
        {
            var changes = new List<EventPattern<PropertyChangedAndValueEventArgs<string>>>();
            var fake = new Fake { Next = new Level { Next = new Level { Name = "Johan" } } };
            fake.ObservePropertyChangedWithValue(x => x.Next.Next.Name, false)
                .Subscribe(changes.Add);
            Assert.AreEqual(0, changes.Count);
            fake.OnPropertyChanged(prop); // This means all properties changed according to wpf convention
            Assert.AreEqual(1, changes.Count);
            Assert.IsTrue(changes.Single().EventArgs.HasValue);
            Assert.AreEqual("Johan", changes.Single().EventArgs.Value);
            Assert.AreSame(fake.Next.Next, changes.Single().Sender);
            Assert.AreEqual(prop, changes.Last().EventArgs.PropertyName);
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase("Name")]
        public void ReactsOnStringEmptyOrNullWithValue(string prop)
        {
            var changes = new List<EventPattern<PropertyChangedAndValueEventArgs<string>>>();
            var fake = new Fake { Next = new Level { Next = new Level { Name = "Johan" } } };
            fake.ObservePropertyChangedWithValue(x => x.Next.Next.Name, false)
                .Subscribe(changes.Add);
            Assert.AreEqual(0, changes.Count);
            fake.Next.Next.OnPropertyChanged(prop); // This means all properties changed according to wpf convention
            Assert.AreEqual(1, changes.Count);
            Assert.IsTrue(changes.Single().EventArgs.HasValue);
            Assert.AreEqual("Johan", changes.Single().EventArgs.Value);
            Assert.AreSame(fake.Next.Next, changes.Single().Sender);
            Assert.AreEqual(prop, changes.Last().EventArgs.PropertyName);
        }

        [Test]
        public void TwoSubscriptionsOneObservable()
        {
            var changes1 = new List<EventPattern<PropertyChangedAndValueEventArgs<string>>>();
            var changes2 = new List<EventPattern<PropertyChangedAndValueEventArgs<string>>>();
            var fake = new Fake { Next = new Level { Name = "" } };
            var observable = fake.ObservePropertyChangedWithValue(x => x.Next.Name, false);
            using (observable.Subscribe(changes1.Add))
            {
                using (observable.Subscribe(changes2.Add))
                {
                    Assert.AreEqual(0, changes1.Count);
                    Assert.AreEqual(0, changes2.Count);

                    fake.Next.Name += "a";
                    Assert.AreEqual(1, changes1.Count);
                    Assert.AreEqual(1, changes2.Count);

                    fake.Next.Name += "a";
                    Assert.AreEqual(2, changes1.Count);
                    Assert.AreEqual(2, changes2.Count);

                    fake.Next = null;
                    Assert.AreEqual(3, changes1.Count);
                    Assert.AreEqual(3, changes2.Count);
                }
            }
        }

        [Test]
        public void SignalsInitialNull()
        {
            var changes = new List<EventPattern<PropertyChangedAndValueEventArgs<string>>>();
            var fake = new Fake();
            fake.ObservePropertyChangedWithValue(x => x.Next.Name, true)
                .Subscribe(changes.Add);
            Assert.AreEqual(1, changes.Count);
            Assert.AreEqual(null, changes.Single().EventArgs.Value);
            Assert.AreSame(null, changes.Single().Sender);
            Assert.IsFalse(changes.Single().EventArgs.HasValue);
            Assert.AreEqual("Name", changes.Last().EventArgs.PropertyName);
        }

        [Test]
        public void CapturesInitialValue()
        {
            var changes = new List<EventPattern<PropertyChangedAndValueEventArgs<string>>>();
            var fake = new Fake { Next = new Level { Name = "Johan" } };
            fake.ObservePropertyChangedWithValue(x => x.Next.Name, true)
                .Subscribe(changes.Add);
            Assert.AreEqual(1, changes.Count);
            Assert.AreEqual("Johan", changes.Last().EventArgs.Value);
            Assert.AreEqual("Name", changes.Last().EventArgs.PropertyName);
            Assert.AreSame(fake.Next, changes.Last().Sender);
            Assert.IsTrue(changes.Single().EventArgs.HasValue);
        }

        [Test]
        public void DoesNotSignalOnSubscribe()
        {
            var changes = new List<EventPattern<PropertyChangedAndValueEventArgs<string>>>();
            var fake = new Fake { Next = new Level { Name = "Johan" } };
            fake.ObservePropertyChangedWithValue(x => x.Next.Name, false)
                .Subscribe(changes.Add);
            CollectionAssert.IsEmpty(changes);
        }

        [Test]
        public void SignalsOnSourceChanges()
        {
            var changes = new List<EventPattern<PropertyChangedAndValueEventArgs<string>>>();
            var fake = new Fake();
            fake.ObservePropertyChangedWithValue(x => x.Next.Name, false)
                .Subscribe(changes.Add);
            CollectionAssert.IsEmpty(changes);
            fake.Next = new Level();
            CollectionAssert.IsEmpty(changes);
            fake.Next.Name = "El Kurro";
            Assert.AreEqual("El Kurro", changes.Single().EventArgs.Value);
            Assert.IsTrue(changes.Single().EventArgs.HasValue);
        }

        [Test]
        public void MemoryLeakRootNoDisposeTest()
        {
            WeakReference wr = new WeakReference(null);
            IObservable<EventPattern<PropertyChangedAndValueEventArgs<string>>> observable = null;
            new Action(
                () =>
                {
                    var fake = new Fake { Next = new Level() };
                    wr.Target = fake;
                    Assert.IsTrue(wr.IsAlive);
                    observable = fake.ObservePropertyChangedWithValue(x => x.Next.Name, false);
                })();
            //// http://stackoverflow.com/a/579001/1069200
#pragma warning disable GU0030 // Use using.
            var subscription = observable.Subscribe();
#pragma warning restore GU0030 // Use using.
            GC.KeepAlive(observable);
            GC.KeepAlive(subscription);
            GC.Collect();

            Assert.IsFalse(wr.IsAlive);
        }

        [Test]
        public void MemoryLeakRootDisposeTest()
        {
            WeakReference wr = new WeakReference(null);
            IObservable<EventPattern<PropertyChangedAndValueEventArgs<string>>> observable = null;
            new Action(
                () =>
                {
                    var fake = new Fake { Next = new Level() };
                    wr.Target = fake;
                    Assert.IsTrue(wr.IsAlive);
                    observable = fake.ObservePropertyChangedWithValue(x => x.Next.Name, false);
                })();
            //// http://stackoverflow.com/a/579001/1069200
#pragma warning disable GU0030 // Use using.
            var subscription = observable.Subscribe();
#pragma warning restore GU0030 // Use using.
            GC.KeepAlive(observable);
            GC.KeepAlive(subscription);

            subscription.Dispose();
            GC.Collect();

            Assert.IsFalse(wr.IsAlive);
            Assert.IsNotNull(subscription); // touching it after GC.Collect for no optimizations
        }

        [Test]
        public void MemoryLeakLevelNoDisposeTest()
        {
            WeakReference wr = new WeakReference(null);
            IObservable<EventPattern<PropertyChangedAndValueEventArgs<string>>> observable = null;
            new Action(
                () =>
                {
                    var fake = new Fake { Next = new Level() };
                    wr.Target = fake.Next;
                    Assert.IsTrue(wr.IsAlive);
                    observable = fake.ObservePropertyChangedWithValue(x => x.Next.Name, false);
                })();
            //// http://stackoverflow.com/a/579001/1069200
#pragma warning disable GU0030 // Use using.
            var subscription = observable.Subscribe();
#pragma warning restore GU0030 // Use using.
            GC.KeepAlive(observable);
            GC.KeepAlive(subscription);

            GC.Collect();

            Assert.IsFalse(wr.IsAlive);
            Assert.IsNotNull(subscription); // touching it after GC.Collect for no optimizations
        }

        [Test]
        public void MemoryLeakLevelDisposeTest()
        {
            WeakReference wr = new WeakReference(null);
            IObservable<EventPattern<PropertyChangedAndValueEventArgs<string>>> observable = null;
            new Action(
                () =>
                {
                    var fake = new Fake { Next = new Level() };
                    wr.Target = fake.Next;
                    Assert.IsTrue(wr.IsAlive);
                    observable = fake.ObservePropertyChangedWithValue(x => x.Next.Name, false);
                })();
            //// http://stackoverflow.com/a/579001/1069200
            using (var subscription = observable.Subscribe())
            {
                GC.KeepAlive(observable);
                GC.KeepAlive(subscription);
            }

            GC.Collect();
            Assert.IsFalse(wr.IsAlive);
        }
    }
}