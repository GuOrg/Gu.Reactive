namespace Gu.Reactive.Tests
{
    using System;
    using System.Collections.Generic;

    using NUnit.Framework;

    public class ToTrackingObservableTests
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
        public void ToTrackingCaptureInitial()
        {
            var fake = new FakeInpc();
            var strings = new List<string>();
            fake.ToTrackingObservable(x => x.Name, true).Subscribe(x => strings.Add(x.CurrentValue));
            CollectionAssert.AreEqual(new string[] { null }, strings);
            fake.Name = "El Kurro";
            CollectionAssert.AreEqual(new string[] { null, "El Kurro" }, strings);
        }

        [Test, Explicit("Have not found a solution to this, captured and kept alive by rx")]
        public void MemoryLeakNoDisposeTest()
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
        public void MemoryLeakDisposeTest()
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