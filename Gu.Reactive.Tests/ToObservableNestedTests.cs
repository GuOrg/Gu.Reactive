namespace Gu.Reactive.Tests
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reactive;

    using NUnit.Framework;

    public class ToObservableNestedTests
    {
        [Test]
        public void TwoLevels()
        {
            var count = 0;
            var fake = new FakeInpc();
            var observable = fake.ToObservable(x => x.Next.Value);
            var disposable = observable.Subscribe(x => count++);
            Assert.AreEqual(1, count);
            fake.Next = new Level();
            Assert.AreEqual(2, count);
            fake.Next.Value = !fake.Next.Value;
            Assert.AreEqual(3, count);
            fake.Next = null;
            Assert.AreEqual(4, count);
        }

        [Test]
        public void TwoLevelsStartingWithValue()
        {
            var count = 0;
            var level = new Level { Next = new Level() };
            level.ToObservable(x => x.Next.Value)
                 .Subscribe(x => count++);
            Assert.AreEqual(1, count);
            level.Next.Value = !level.Next.Value;
            Assert.AreEqual(2, count);
        }

        [Test]
        public void TwoLevelsNotCapturingFirstStartingWithValue()
        {
            var count = 0;
            var level = new Level { Next = new Level() };
            level.ToObservable(x => x.Next.Value, false)
                 .Subscribe(x => count++);
            Assert.AreEqual(0, count);
            level.Next.Value = !level.Next.Value;
            Assert.AreEqual(1, count);
        }

        [TestCase(true, null)]
        [TestCase(null, false)]
        [TestCase(null, true)]
        public void TwoLevelsNullableStartingWithValue(bool? first, bool? other)
        {
            var count = 0;
            var level = new Level { Next = new Level { NullableValue = first } };
            level.ToObservable(x => x.Next.NullableValue)
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
            level.ToObservable(x => x.Next.NullableValue)
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
            var observable = fake.ToObservable(x => x.Next.Next.Value);
            var disposable = observable.Subscribe(x => count++);
            Assert.AreEqual(1, count);
            fake.Next = new Level { Next = new Level() };
            Assert.AreEqual(2, count);
            fake.Next.Next.Value = !fake.Next.Next.Value;
            Assert.AreEqual(3, count);
            fake.Next = null;
            Assert.AreEqual(4, count);
        }

        [TestCase("")]
        [TestCase(null)]
        public void StringEmptyOrNull(string prop)
        {
            int count = 0;
            var next = new Level();
            var fake = new FakeInpc { Next = next };
            var observable = fake.ToObservable(x => x.Next.Value, false);
            var disposable = observable.Subscribe(x => count++);
            fake.OnPropertyChanged(prop);
            Assert.AreEqual(1, count);
            next.OnPropertyChanged(prop);
            Assert.AreEqual(2, count);
        }

        [Test]
        public void ThreeLevelsExisting()
        {
            int count = 0;
            var fake = new FakeInpc { Next = new Level { Next = new Level() } };
            var observable = fake.ToObservable(x => x.Next.Next.Value);
            var disposable = observable.Subscribe(x => count++);
            Assert.AreEqual(1, count);
            fake.Next.Next.Value = !fake.Next.Next.Value;
            Assert.AreEqual(2, count);
        }

        [Test]
        public void Reacts()
        {
            var args = new List<EventPattern<PropertyChangedEventArgs>>();
            var fake = new FakeInpc();
            var observable = fake.ToObservable(x => x.Next.Value);
            observable.Subscribe(args.Add);
            Assert.AreEqual(1, args.Count);
            fake.Next = new Level { Value = false };
            Assert.AreEqual(2, args.Count);
            fake.Next.Value = true;
            Assert.AreEqual(3, args.Count);
            Level level1 = fake.Next;
            fake.Next = null;
            Assert.AreEqual(4, args.Count);
            level1.Value = !level1.Value;
            Assert.AreEqual(4, args.Count);
        }

        [Test]
        public void SignalsInitial()
        {
            int count = 0;
            var fake = new FakeInpc { Prop1 = false, Prop2 = true, Next = new Level { Value = true } };
            var observable = fake.ToObservable(x => x.Next.Value, true); // Default true captures initial value
            var disposable = observable.Subscribe(x => count++);
            Assert.AreEqual(1, count);
            fake.Next.Value = !fake.Next.Value;
            Assert.AreEqual(2, count);
        }

        [Test]
        public void ExplicitNoSignalInitial()
        {
            int count = 0;
            var fake = new FakeInpc { Prop1 = false, Prop2 = true };
            var observable = fake.ToObservable(x => x.Next.Value, false); // Default true captures initial value
            var disposable = observable.Subscribe(x => count++);
            Assert.AreEqual(0, count);
            fake.Next = new Level { Value = true };
            Assert.AreEqual(1, count);
        }

        [Test]
        public void NoCaptureOfInitial()
        {
            int count = 0;
            var fake = new FakeInpc { Prop1 = false, Prop2 = true, Next = new Level { Value = true } };
            var observable = fake.ToObservable(x => x.Next.Value, false);
            var disposable = observable.Subscribe(x => count++);
            Assert.AreEqual(0, count);
            fake.Next.Value = !fake.Next.Value;
            Assert.AreEqual(1, count);
        }

        [Test, Explicit("Does not work on build server")]
        public void MemoryLeakDisposeTest()
        {
            var fake = new FakeInpc { Next = new Level() };
            var wr = new WeakReference(fake);
            var subscription = fake.ToObservable().Subscribe();
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
            var subscription = fake.ToObservable().Subscribe();
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
            var subscription = fake.ToObservable().Subscribe();
            fake = null;
            GC.Collect();
            Assert.IsFalse(wr.IsAlive);
            var s = subscription.ToString(); // touching it after GC.Collect for no optimizations
        }
    }
}