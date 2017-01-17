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
        private List<EventPattern<PropertyChangedAndValueEventArgs<string>>> changes;

        [SetUp]
        public void SetUp()
        {
            this.changes = new List<EventPattern<PropertyChangedAndValueEventArgs<string>>>();
        }

        [Test]
        [Explicit("Implement")]
        public void TypedEventargsTest()
        {
        }

        [TestCase("")]
        [TestCase(null)]
        public void DoesNotReactOnStringEmptyOrNullFromRootWithoutValue(string prop)
        {
            var fake = new Fake();
            fake.ObservePropertyChangedWithValue(x => x.Next.Name, false)
                .Subscribe(this.changes.Add);
            Assert.AreEqual(0, this.changes.Count);
            fake.OnPropertyChanged(prop); // This means all properties changed according to wpf convention
            Assert.AreEqual(0, this.changes.Count);
        }

        [TestCase("")]
        [TestCase(null)]
        public void ReactsOnStringEmptyOrNullFromRootWithValue(string prop)
        {
            var fake = new Fake { Next = new Level { Next = new Level { Name = "Johan" } } };
            fake.ObservePropertyChangedWithValue(x => x.Next.Next.Name, false)
                .Subscribe(this.changes.Add);
            Assert.AreEqual(0, this.changes.Count);
            fake.OnPropertyChanged(prop); // This means all properties changed according to wpf convention
            Assert.AreEqual(1, this.changes.Count);
            Assert.IsTrue(this.changes.Single().EventArgs.HasValue);
            Assert.AreEqual("Johan", this.changes.Single().EventArgs.Value);
            Assert.AreSame(fake.Next.Next, this.changes.Single().Sender);
            Assert.AreEqual(prop, this.changes.Last().EventArgs.PropertyName);
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase("Name")]
        public void ReactsOnStringEmptyOrNullWithValue(string prop)
        {
            var fake = new Fake { Next = new Level { Next = new Level { Name = "Johan" } } };
            fake.ObservePropertyChangedWithValue(x => x.Next.Next.Name, false)
                .Subscribe(this.changes.Add);
            Assert.AreEqual(0, this.changes.Count);
            fake.Next.Next.OnPropertyChanged(prop); // This means all properties changed according to wpf convention
            Assert.AreEqual(1, this.changes.Count);
            Assert.IsTrue(this.changes.Single().EventArgs.HasValue);
            Assert.AreEqual("Johan", this.changes.Single().EventArgs.Value);
            Assert.AreSame(fake.Next.Next, this.changes.Single().Sender);
            Assert.AreEqual(prop, this.changes.Last().EventArgs.PropertyName);
        }

        [Test]
        public void SignalsInitialNull()
        {
            var fake = new Fake();
            fake.ObservePropertyChangedWithValue(x => x.Next.Name, true)
                .Subscribe(this.changes.Add);
            Assert.AreEqual(1, this.changes.Count);
            Assert.AreEqual(null, this.changes.Single().EventArgs.Value);
            Assert.AreSame(null, this.changes.Single().Sender);
            Assert.IsFalse(this.changes.Single().EventArgs.HasValue);
            Assert.AreEqual("Name", this.changes.Last().EventArgs.PropertyName);
        }

        [Test]
        public void CapturesInitialValue()
        {
            var fake = new Fake { Next = new Level { Name = "Johan" } };
            fake.ObservePropertyChangedWithValue(x => x.Next.Name, true)
                .Subscribe(this.changes.Add);
            Assert.AreEqual(1, this.changes.Count);
            Assert.AreEqual("Johan", this.changes.Last().EventArgs.Value);
            Assert.AreEqual("Name", this.changes.Last().EventArgs.PropertyName);
            Assert.AreSame(fake.Next, this.changes.Last().Sender);
            Assert.IsTrue(this.changes.Single().EventArgs.HasValue);
        }

        [Test]
        public void DoesNotSignalOnSubscribe()
        {
            var fake = new Fake { Next = new Level { Name = "Johan" } };
            fake.ObservePropertyChangedWithValue(x => x.Next.Name, false)
                .Subscribe(this.changes.Add);
            CollectionAssert.IsEmpty(this.changes);
        }

        [Test]
        public void SignalsOnSourceChanges()
        {
            var fake = new Fake();
            fake.ObservePropertyChangedWithValue(x => x.Next.Name, false)
                .Subscribe(this.changes.Add);
            CollectionAssert.IsEmpty(this.changes);
            fake.Next = new Level();
            CollectionAssert.IsEmpty(this.changes);
            fake.Next.Name = "El Kurro";
            Assert.AreEqual("El Kurro", this.changes.Single().EventArgs.Value);
            Assert.IsTrue(this.changes.Single().EventArgs.HasValue);
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
            var subscription = observable.Subscribe();
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