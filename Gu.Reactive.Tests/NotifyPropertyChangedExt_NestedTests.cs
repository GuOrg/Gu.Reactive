namespace Gu.Reactive.Tests
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reactive;

    using NUnit.Framework;

    public class NotifyPropertyChangedExt_NestedTests
    {
        [Test]
        public void TwoLevels()
        {
            var count = 0;
            var fake = new FakeInpc();
            var observable = fake.ObservePropertyChanged(x => x.Next.IsTrue);
            var disposable = observable.Subscribe(x => count++);
            Assert.AreEqual(1, count);
            fake.Next = new Level();
            Assert.AreEqual(2, count);
            fake.Next.IsTrue = !fake.Next.IsTrue;
            Assert.AreEqual(3, count);
            fake.Next = null;
            Assert.AreEqual(4, count);
        }

        [Test]
        public void TwoLevelsStartingWithValue()
        {
            var count = 0;
            var level = new Level { Next = new Level() };
            level.ObservePropertyChanged(x => x.Next.IsTrue)
                 .Subscribe(x => count++);
            Assert.AreEqual(1, count);
            level.Next.IsTrue = !level.Next.IsTrue;
            Assert.AreEqual(2, count);
        }

        [Test]
        public void TwoLevelsNotCapturingFirstStartingWithValue()
        {
            var count = 0;
            var level = new Level { Next = new Level() };
            level.ObservePropertyChanged(x => x.Next.IsTrue, false)
                 .Subscribe(x => count++);
            Assert.AreEqual(0, count);
            level.Next.IsTrue = !level.Next.IsTrue;
            Assert.AreEqual(1, count);
        }

        [TestCase(true, null)]
        [TestCase(null, false)]
        [TestCase(null, true)]
        public void TwoLevelsNullableStartingWithValue(bool? first, bool? other)
        {
            var count = 0;
            var level = new Level { Next = new Level { NullableValue = first } };
            level.ObservePropertyChanged(x => x.Next.NullableValue)
                 .Subscribe(x => count++);
            Assert.AreEqual(1, count);
            level.Next.NullableValue = other;
            Assert.AreEqual(2, count);
        }

        [Test]
        public void TwoLevelsNullableStartingWithValueChangesToNull()
        {
            var count = 0;
            var level = new Level { Next = new Level { NullableValue = true } };
            level.ObservePropertyChanged(x => x.Next.NullableValue)
                 .Subscribe(x => count++);
            Assert.AreEqual(1, count);
            level.Next.NullableValue = null;
            Assert.AreEqual(2, count);
        }

        [Test]
        public void ThreeLevelsStartingWithNull()
        {
            int count = 0;
            var fake = new FakeInpc();
            var observable = fake.ObservePropertyChanged(x => x.Next.Next.IsTrue);
            var disposable = observable.Subscribe(x => count++);
            Assert.AreEqual(1, count);
            fake.Next = new Level { Next = new Level() };
            Assert.AreEqual(2, count);
            fake.Next.Next.IsTrue = !fake.Next.Next.IsTrue;
            Assert.AreEqual(3, count);
            fake.Next = null;
            Assert.AreEqual(4, count);
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase("IsTrue")]
        public void FirstInPathSignalsEvent(string eventargsPropertyName)
        {
            int count = 0;
            var next = new Level();
            var fake = new FakeInpc { Next = next };
            var observable = fake.ObservePropertyChanged(x => x.Next.IsTrue, false);
            var disposable = observable.Subscribe(x => count++);
            fake.OnPropertyChanged(eventargsPropertyName);
            Assert.AreEqual(0, count);
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase("IsTrue")]
        public void LastInPathSignalsEvent(string eventargsPropertyName)
        {
            int count = 0;
            var next = new Level();
            var fake = new FakeInpc { Next = next };
            var observable = fake.ObservePropertyChanged(x => x.Next.IsTrue, false);
            var disposable = observable.Subscribe(x => count++);
            fake.Next.OnPropertyChanged(eventargsPropertyName);
            Assert.AreEqual(1, count);
        }

        [Test]
        public void ThreeLevelsExisting()
        {
            int count = 0;
            var fake = new FakeInpc { Next = new Level { Next = new Level() } };
            var observable = fake.ObservePropertyChanged(x => x.Next.Next.IsTrue);
            var disposable = observable.Subscribe(x => count++);
            Assert.AreEqual(1, count);
            fake.Next.Next.IsTrue = !fake.Next.Next.IsTrue;
            Assert.AreEqual(2, count);
        }

        [Test]
        public void Reacts()
        {
            var args = new List<EventPattern<PropertyChangedEventArgs>>();
            var fake = new FakeInpc();
            var observable = fake.ObservePropertyChanged(x => x.Next.IsTrue);
            observable.Subscribe(args.Add);
            Assert.AreEqual(1, args.Count);
            fake.Next = new Level { IsTrue = false };
            Assert.AreEqual(2, args.Count);
            fake.Next.IsTrue = true;
            Assert.AreEqual(3, args.Count);
            Level level1 = fake.Next;
            fake.Next = null;
            Assert.AreEqual(4, args.Count);
            level1.IsTrue = !level1.IsTrue;
            Assert.AreEqual(4, args.Count);
        }

        [Test]
        public void SignalsInitial()
        {
            int count = 0;
            var fake = new FakeInpc { IsTrueOrNull = false, IsTrue = true, Next = new Level { IsTrue = true } };
            var observable = fake.ObservePropertyChanged(x => x.Next.IsTrue, true); // Default true captures initial value
            var disposable = observable.Subscribe(x => count++);
            Assert.AreEqual(1, count);
            fake.Next.IsTrue = !fake.Next.IsTrue;
            Assert.AreEqual(2, count);
        }

        [Test]
        public void ExplicitNoSignalInitial()
        {
            int count = 0;
            var fake = new FakeInpc { IsTrueOrNull = false, IsTrue = true };
            var observable = fake.ObservePropertyChanged(x => x.Next.IsTrue, false); // Default true captures initial value
            var disposable = observable.Subscribe(x => count++);
            Assert.AreEqual(0, count);
            fake.Next = new Level { IsTrue = true };
            Assert.AreEqual(1, count);
        }

        [Test]
        public void NoCaptureOfInitial()
        {
            int count = 0;
            var fake = new FakeInpc { IsTrueOrNull = false, IsTrue = true, Next = new Level { IsTrue = true } };
            var observable = fake.ObservePropertyChanged(x => x.Next.IsTrue, false);
            var disposable = observable.Subscribe(x => count++);
            Assert.AreEqual(0, count);
            fake.Next.IsTrue = !fake.Next.IsTrue;
            Assert.AreEqual(1, count);
        }

        [Test, Explicit("Does not work on build server")]
        public void MemoryLeakDisposeTest()
        {
            var fake = new FakeInpc { Next = new Level() };
            var wr = new WeakReference(fake);
            var subscription = fake.ObservePropertyChanged().Subscribe();
            fake = null;
            subscription.Dispose();
            GC.Collect();
            Assert.IsFalse(wr.IsAlive);
            var s = subscription.ToString(); // touching it after GC.Collect for no optimizations
        }

        [Test, Explicit("Does not work on build server")]
        public void MemoryLeakLevelNoDisposeTest()
        {
            var level = new Level();
            var fake = new FakeInpc { Next = level };
            var wr = new WeakReference(level);
            var subscription = fake.ObservePropertyChanged().Subscribe();
            fake = null;
            GC.Collect();
            Assert.IsFalse(wr.IsAlive);
            var s = subscription.ToString(); // touching it after GC.Collect for no optimizations
        }

        [Test, Explicit("Does not work on build server")]
        public void MemoryLeakRootNoDisposeTest()
        {
            var level = new Level();
            var fake = new FakeInpc { Next = level };
            var wr = new WeakReference(fake);
            var subscription = fake.ObservePropertyChanged().Subscribe();
            fake = null;
            GC.Collect();
            Assert.IsFalse(wr.IsAlive);
            var s = subscription.ToString(); // touching it after GC.Collect for no optimizations
        }
    }
}