namespace Gu.Reactive.Tests
{
    using System;

    using NUnit.Framework;

    public class NotifyPropertyChangedExt_FilteredTests
    {
        [Test]
        public void Reacts()
        {
            int count = 0;
            var fake = new FakeInpc { Prop1 = false, Prop2 = true };
            var observable = fake.ObservePropertyChanged(x => x.Prop1, false);
            var disposable = observable.Subscribe(x => count++);
            Assert.AreEqual(0, count);
            fake.Prop1 = !fake.Prop1;
            Assert.AreEqual(1, count);
            fake.Prop2 = !fake.Prop2; // No notification when changing other property
            Assert.AreEqual(1, count);
        }


        [TestCase("")]
        [TestCase(null)]
        public void ReactsOnStringEmptyOrNull(string prop)
        {
            int count = 0;
            var fake = new FakeInpc();
            var observable = fake.ObservePropertyChanged(x => x.Prop1, false);
            var disposable = observable.Subscribe(x => count++);
            Assert.AreEqual(0, count);
            fake.OnPropertyChanged(prop); // This means all properties changed according to wpf convention
            Assert.AreEqual(1, count);
        }

        [Test]
        public void SignalsInitial()
        {
            int count = 0;
            var fake = new FakeInpc { Prop1 = false, Prop2 = true };
            var observable = fake.ObservePropertyChanged(x => x.Prop1, true); // Default true captures initial value
            var disposable = observable.Subscribe(x => count++);
            Assert.AreEqual(1, count);
            fake.Prop1 = !fake.Prop1;
            Assert.AreEqual(2, count);
        }

        [Test]
        public void ExplicitNoSignalInitial()
        {
            int count = 0;
            var fake = new FakeInpc { Prop1 = false, Prop2 = true };
            var observable = fake.ObservePropertyChanged(x => x.Prop1, false);
            var disposable = observable.Subscribe(x => count++);
            Assert.AreEqual(0, count);
            fake.Prop1 = !fake.Prop1;
            Assert.AreEqual(1, count);
        }

        [Test]
        public void MemoryLeakDisposeTest()
        {
            var fake = new FakeInpc();
            var wr = new WeakReference(fake);
            var subscription = fake.ObservePropertyChanged(x => x.Prop1).Subscribe();
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
            var subscription = fake.ObservePropertyChanged(x => x.Prop1).Subscribe();
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
            var subscription = fake.ObservePropertyChanged(x => x.Name)
                                   .Subscribe();
            fake = null;
            GC.Collect();
            Assert.IsFalse(wr.IsAlive);
            var s = subscription.ToString(); // touching it after GC.Collect for no optimizations
        }

    }
}