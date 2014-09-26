namespace Gu.Reactive.Tests
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reactive;

    using NUnit.Framework;

    public class ReactiveExtTests
    {
        [Test]
        public void ToTrackingTest()
        {
            var fake = new FakeInpc();
            var strings = new List<string>();
            fake.ToTrackingObservable(x => x.Name, false).Subscribe(x => strings.Add(x.CurrentValue));
            CollectionAssert.AreEqual(new string[] { }, strings);
            fake.Name = "El Kurro";
            CollectionAssert.AreEqual(new string[] { "El Kurro" }, strings);
        }

        [Test]
        public void ToTrackingSampleCurrent()
        {
            var fake = new FakeInpc();
            var strings = new List<string>();
            fake.ToTrackingObservable(x => x.Name, true).Subscribe(x => strings.Add(x.CurrentValue));
            CollectionAssert.AreEqual(new string[] { null }, strings);
            fake.Name = "El Kurro";
            CollectionAssert.AreEqual(new string[] { null, "El Kurro" }, strings);
        }

        [Test]
        public void ToObservableSubscribeTest()
        {
            int count = 0;
            var fake = new FakeInpc();
            var observable = fake.ToObservable();
            var disposable = observable.Subscribe(x => count++);
            fake.Prop1 = !fake.Prop1;
            Assert.AreEqual(1, count);
            disposable.Dispose();
            fake.Prop1 = !fake.Prop1;
            Assert.AreEqual(1, count);
        }

        [Test]
        public void ToFilterObservableSubscribeTest()
        {
            int count = 0;
            var fake = new FakeInpc();
            var observable = fake.ToObservable(x => x.Prop1);
            var disposable = observable.Subscribe(x => count++);
            Assert.AreEqual(1, count);
            fake.Prop1 = !fake.Prop1;
            Assert.AreEqual(2, count);
            fake.Prop2 = !fake.Prop2;
            Assert.AreEqual(2, count);
        }

        [Test]
        public void ToFilterObservableSubscribeTestStringEmptyOrNull()
        {
            int count = 0;
            var fake = new FakeInpc();
            var observable = fake.ToObservable(x => x.Prop1, false);
            var disposable = observable.Subscribe(x => count++);
            fake.OnPropertyChanged("");
            Assert.AreEqual(1, count);
            fake.OnPropertyChanged(null);
            Assert.AreEqual(2, count);
        }

        [Test]
        public void ToFilterNestedObservableSubscribeTestStringEmptyOrNull()
        {
            int count = 0;
            var next = new Level();
            var fake = new FakeInpc { Next = next };
            var observable = fake.ToObservable(x => x.Next.Value, false);
            var disposable = observable.Subscribe(x => count++);
            fake.OnPropertyChanged("");
            Assert.AreEqual(1, count);
            fake.OnPropertyChanged(null);
            Assert.AreEqual(2, count);
            next.OnPropertyChanged("");
            Assert.AreEqual(3, count);
            next.OnPropertyChanged(null);
            Assert.AreEqual(4, count);
        }

        [Test]
        public void ToFilterObservableTwoLevelsSubscribeTest()
        {
            int count = 0;
            var fake = new FakeInpc();
            var observable = fake.ToObservable(x => x.Next.Value);
            var disposable = observable.Subscribe(x => count++);
            Assert.AreEqual(0, count);
            fake.Next = new Level();
            Assert.AreEqual(1, count);
            fake.Next.Value = !fake.Next.Value;
            Assert.AreEqual(2, count);
            fake.Next = null;
            Assert.AreEqual(3, count);
        }

        [Test]
        public void ToFilterObservableThreeLevelsSubscribeTest()
        {
            int count = 0;
            var fake = new FakeInpc();
            var observable = fake.ToObservable(x => x.Next.Next.Value);
            var disposable = observable.Subscribe(x => count++);
            Assert.AreEqual(0, count);
            fake.Next = new Level { Next = new Level() };
            Assert.AreEqual(1, count);
            fake.Next.Next.Value = !fake.Next.Next.Value;
            Assert.AreEqual(2, count);
            fake.Next = null;
            Assert.AreEqual(3, count);
        }

        [Test]
        public void ToFilterObservableThreeLevelsExistingSubscribeTest()
        {
            int count = 0;
            var fake = new FakeInpc { Next = new Level { Next = new Level() } };
            var observable = fake.ToObservable(x => x.Next.Next.Value);
            var disposable = observable.Subscribe(x => count++);
            Assert.AreEqual(0, count);
            fake.Next.Next.Value = !fake.Next.Next.Value;
            Assert.AreEqual(1, count);
        }

        [Test]
        public void ToFilterObservableOnlyChangesSubscribeTest()
        {
            int count = 0;
            var fake = new FakeInpc();
            var observable = fake.ToObservable(x => x.Prop1, false);
            var disposable = observable.Subscribe(x => count++);
            Assert.AreEqual(0, count);
            fake.Prop1 = !fake.Prop1;
            Assert.AreEqual(1, count);
            fake.Prop2 = !fake.Prop2;
            Assert.AreEqual(1, count);
        }

        [Test]
        public void NestedObservableTest()
        {
            var args = new List<EventPattern<PropertyChangedEventArgs>>();
            var fake = new FakeInpc();
            var observable = fake.ToObservable(x => x.Next.Value);
            observable.Subscribe(args.Add);
            fake.Next = new Level();
            Assert.AreEqual(1, args.Count);
            fake.Next.Value = true;
            Assert.AreEqual(2, args.Count);
            Level level1 = fake.Next;
            fake.Next = null;
            Assert.AreEqual(3, args.Count);
            level1.Value = !level1.Value;
            Assert.AreEqual(3, args.Count);
        }

        [Test]
        public void MemoryLeakDisposeTest()
        {
            var fake = new FakeInpc();
            var wr = new WeakReference(fake);
            var subscription = fake.ToObservable().Subscribe();
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
            var subscription = fake.ToObservable().Subscribe();
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
            var subscription = fake.ToObservable(x => x.Name)
                                   .Subscribe();
            fake = null;
            GC.Collect();
            Assert.IsFalse(wr.IsAlive);
            var s = subscription.ToString(); // touching it after GC.Collect for no optimizations
        }

        [Test, Explicit("Have not found a solution to this, captured and kept alive by rx")]
        public void MemoryLeakTrackingNoDisposeTest()
        {
            var fake = new FakeInpc();
            var wr = new WeakReference(fake);
            Assert.IsTrue(wr.IsAlive);
            var subscription = fake.ToTrackingObservable(x => x.Name, false)
                                   .Subscribe();
            fake = null;
            GC.Collect();
            Assert.IsFalse(wr.IsAlive);
            var s = subscription.ToString(); // touching it after GC.Collect for no optimizations
        }

        [Test]
        public void MemoryLeakTrackingDisposeTest()
        {
            var fake = new FakeInpc();
            var wr = new WeakReference(fake);
            Assert.IsTrue(wr.IsAlive);
            var subscription = fake.ToTrackingObservable(x => x.Name, false)
                                   .Subscribe();
            fake = null;
            subscription.Dispose();
            GC.Collect();
            Assert.IsFalse(wr.IsAlive);
            var s = subscription.ToString(); // touching it after GC.Collect for no optimizations
        }
    }
}
