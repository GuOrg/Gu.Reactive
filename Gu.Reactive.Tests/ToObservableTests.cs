namespace Gu.Reactive.Tests
{
    using System;
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Linq;

    using NUnit.Framework;

    public class ToObservableTests
    {
        [Test]
        public void Reacts()
        {
            int count = 0;
            var fake = new FakeInpc { Prop1 = false, Prop2 = true };
            var observable = fake.ToObservable();
            var disposable = observable.Subscribe(x => count++);
            Assert.AreEqual(0, count);
            fake.Prop1 = !fake.Prop1;
            Assert.AreEqual(1, count);
            fake.Prop2 = !fake.Prop2;
            Assert.AreEqual(2, count);
        }

        [Test]
        public void StopsListeningOnDispose()
        {
            int count = 0;
            var fake = new FakeInpc { Prop1 = true };
            var observable = fake.ToObservable();
            var disposable = observable.Subscribe(x => count++);
            fake.Prop1 = !fake.Prop1;
            Assert.AreEqual(1, count);
            disposable.Dispose();
            fake.Prop1 = !fake.Prop1;
            Assert.AreEqual(1, count);
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
    }
}
