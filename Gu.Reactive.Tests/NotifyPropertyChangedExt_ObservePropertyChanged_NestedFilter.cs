namespace Gu.Reactive.Tests
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reactive;

    using Gu.Reactive.Tests.Fakes;

    using NUnit.Framework;
    // ReSharper disable once InconsistentNaming
    public class NotifyPropertyChangedExt_ObservePropertyChanged_NestedFilter
    {
        private List<EventPattern<PropertyChangedEventArgs>> _changes;

        [SetUp]
        public void SetUp()
        {
            _changes = new List<EventPattern<PropertyChangedEventArgs>>();
        }

        [Test]
        public void ReactsTwoPropertiesSameInstance()
        {
            var fake = new Fake { Next = new Level { Value = 1 } };
            fake.ObservePropertyChanged(x => x.Next.Value, false)
                .Subscribe(_changes.Add);

            fake.ObservePropertyChanged(x => x.Next.IsTrue, false)
                .Subscribe(_changes.Add);
            Assert.AreEqual(0, _changes.Count);

            fake.Next.Value++;
            Assert.AreEqual(1, _changes.Count);
            AssertEventPattern(fake.Next, "Value", _changes.Last());

            fake.Next.IsTrue = !fake.IsTrue;
            Assert.AreEqual(2, _changes.Count);
            AssertEventPattern(fake.Next, "IsTrue", _changes.Last());
        }

        [Test]
        public void ReactsTwoInstancesSameProperty()
        {
            var fake1 = new Fake { Next = new Level() };
            fake1.ObservePropertyChanged(x => x.Next.Value, false)
                .Subscribe(_changes.Add);
            var fake2 = new Fake { Next = new Level() };
            fake2.ObservePropertyChanged(x => x.Next.Value, false)
                .Subscribe(_changes.Add);
            Assert.AreEqual(0, _changes.Count);

            fake1.Next.Value++;
            Assert.AreEqual(1, _changes.Count);
            AssertEventPattern(fake1.Next, "Value", _changes.Last());

            fake2.Next.Value++;
            Assert.AreEqual(2, _changes.Count);
            AssertEventPattern(fake2.Next, "Value", _changes.Last());
        }

        [Test]
        public void ThrowsOnStructInPath()
        {
            var fake = new Fake();
            Assert.Throws<ArgumentException>(() => fake.ObservePropertyChanged(x => x.StructLevel.Name));
        }

        [Test]
        public void ThrowsOnNotInpcInPath()
        {
            var fake = new Fake();
            Assert.Throws<ArgumentException>(() => fake.ObservePropertyChanged(x => x.Name.Length));
        }

        [Test]
        public void ThrowsOnMethodInPath()
        {
            var fake = new Fake();
            Assert.Throws<InvalidCastException>(() => fake.ObservePropertyChanged(x => x.Method().Name)); // Leaving it InvalidCast here
        }

        [TestCase(true, 1)]
        [TestCase(false, 0)]
        public void SignalsInitialWhenHasValue(bool signalInitial, int expected)
        {
            var fake = new Fake { Next = new Level() };
            fake.ObservePropertyChanged(x => x.Next.Value, signalInitial)
                .Subscribe(_changes.Add);

            Assert.AreEqual(expected, _changes.Count);
            if (expected == 1)
            {
                AssertEventPattern(fake.Next, "Value", _changes.Last());
            }

            fake.Next.Value++;
            Assert.AreEqual(expected + 1, _changes.Count); // Double check that we are subscribing
        }

        [TestCase(true, 1)]
        [TestCase(false, 0)]
        public void SignalsInitialWhenNoValue(bool signalInitial, int expected)
        {
            var fake = new Fake();
            fake.ObservePropertyChanged(x => x.Next.Value, signalInitial)
                .Subscribe(_changes.Add);

            Assert.AreEqual(expected, _changes.Count);
            if (expected == 1)
            {
                AssertEventPattern(null, "Value", _changes.Last());
            }

            fake.Next = new Level();
            Assert.AreEqual(expected + 1, _changes.Count); // Double check that we are subscribing
        }

        [TestCase("")]
        [TestCase(null)]
        public void DoesNotReactsnStringEmptyOrNullFromRootWhenNull(string propertyName)
        {
            var fake = new Fake();
            var observable = fake.ObservePropertyChanged(x => x.Next.IsTrue, false);
            var disposable = observable.Subscribe(_changes.Add);
            Assert.AreEqual(0, _changes.Count);

            fake.OnPropertyChanged(propertyName); // This means all properties changed according to wpf convention
            Assert.AreEqual(0, _changes.Count);
        }

        [TestCase("")]
        [TestCase(null)]
        public void DoesReactsOnStringEmptyOrNullFromRootWhenNotNull(string propertyName)
        {
            var fake = new Fake { Next = new Level() };
            var observable = fake.ObservePropertyChanged(x => x.Next.IsTrue, false);
            var disposable = observable.Subscribe(_changes.Add);
            Assert.AreEqual(0, _changes.Count);

            fake.OnPropertyChanged(propertyName); // This means all properties changed according to wpf convention
            Assert.AreEqual(1, _changes.Count);
            AssertEventPattern(fake.Next, propertyName, _changes.Last());
        }

        [TestCase("")]
        [TestCase(null)]
        public void ReactsOnStringEmptyOrNullFromSource(string propertyName)
        {
            var fake = new Fake { Next = new Level() };
            var observable = fake.ObservePropertyChanged(x => x.Next.IsTrue, false);
            var disposable = observable.Subscribe(_changes.Add);
            Assert.AreEqual(0, _changes.Count);

            fake.Next.OnPropertyChanged(propertyName); // This means all properties changed according to wpf convention

            Assert.AreEqual(1, _changes.Count);
            Assert.AreEqual(fake.Next, _changes.Last().Sender);
            Assert.AreEqual(propertyName, _changes.Last().EventArgs.PropertyName);
        }

        [Test]
        public void TwoLevelsReacts()
        {
            var fake = new Fake();
            fake.ObservePropertyChanged(x => x.Next.IsTrue, true)
                .Subscribe(_changes.Add);
            Assert.AreEqual(1, _changes.Count);
            Assert.AreSame(null, _changes.Single().Sender);
            Assert.AreEqual("IsTrue", _changes.Last().EventArgs.PropertyName);

            fake.Next = new Level();
            Assert.AreEqual(2, _changes.Count);
            Assert.AreSame(fake.Next, _changes.Last().Sender);
            Assert.AreEqual("IsTrue", _changes.Last().EventArgs.PropertyName);

            fake.Next.IsTrue = !fake.Next.IsTrue;
            Assert.AreEqual(3, _changes.Count);
            Assert.AreSame(fake.Next, _changes.Last().Sender);
            Assert.AreEqual("IsTrue", _changes.Last().EventArgs.PropertyName);
        }

        [Test]
        public void TwoLevelsRootChangesFromValueToNull()
        {
            var fake = new Fake { Next = new Level() };
            fake.ObservePropertyChanged(x => x.Next.IsTrue, true)
                .Subscribe(_changes.Add);
            Assert.AreEqual(1, _changes.Count);
            Assert.AreSame(fake.Next, _changes.Single().Sender);
            Assert.AreEqual("IsTrue", _changes.Last().EventArgs.PropertyName);

            fake.Next = null;
            Assert.AreEqual(2, _changes.Count);
            Assert.AreSame(null, _changes.Last().Sender);
            Assert.AreEqual("IsTrue", _changes.Last().EventArgs.PropertyName);
        }

        [Test]
        public void TwoLevelsRootChangesFromNullToValue()
        {
            var fake = new Fake();
            fake.ObservePropertyChanged(x => x.Next.IsTrue, true)
                .Subscribe(_changes.Add);
            Assert.AreEqual(1, _changes.Count);
            Assert.AreSame(null, _changes.Single().Sender);
            Assert.AreEqual("IsTrue", _changes.Last().EventArgs.PropertyName);

            fake.Next = new Level();
            Assert.AreEqual(2, _changes.Count);
            Assert.AreSame(fake.Next, _changes.Last().Sender);
            Assert.AreEqual("IsTrue", _changes.Last().EventArgs.PropertyName);
        }

        [Test]
        public void TwoLevelsStartingWithValue()
        {
            var fake = new Fake { Next = new Level() };
            fake.ObservePropertyChanged(x => x.Next.IsTrue, true)
                .Subscribe(_changes.Add);
            Assert.AreEqual(1, _changes.Count);
            Assert.AreSame(fake.Next, _changes.Last().Sender);
            Assert.AreEqual("IsTrue", _changes.Last().EventArgs.PropertyName);

            fake.Next.IsTrue = !fake.Next.IsTrue;
            Assert.AreEqual(2, _changes.Count);
            Assert.AreSame(fake.Next, _changes.Last().Sender);
            Assert.AreEqual("IsTrue", _changes.Last().EventArgs.PropertyName);
        }

        [Test]
        public void TwoLevelsStartingWithoutValue()
        {
            var fake = new Fake();
            fake.ObservePropertyChanged(x => x.Next.IsTrue, true)
                .Subscribe(_changes.Add);
            Assert.AreEqual(1, _changes.Count);
            Assert.AreSame(fake.Next, _changes.Last().Sender);
            Assert.AreEqual("IsTrue", _changes.Last().EventArgs.PropertyName);

            fake.Next = new Level();
            Assert.AreEqual(2, _changes.Count);
            Assert.AreSame(fake.Next, _changes.Last().Sender);
            Assert.AreEqual("IsTrue", _changes.Last().EventArgs.PropertyName);

            fake.Next.IsTrue = !fake.Next.IsTrue;
            Assert.AreEqual(3, _changes.Count);
            Assert.AreSame(fake.Next, _changes.Last().Sender);
            Assert.AreEqual("IsTrue", _changes.Last().EventArgs.PropertyName);
        }

        [TestCase(true, null)]
        [TestCase(null, false)]
        [TestCase(null, true)]
        public void TwoLevelsNullableStartingWithValue(bool? first, bool? other)
        {
            var fake = new Fake { Next = new Level { IsTrueOrNull = first } };
            fake.ObservePropertyChanged(x => x.Next.IsTrueOrNull, true)
                .Subscribe(_changes.Add);
            Assert.AreEqual(1, _changes.Count);
            Assert.AreSame(fake.Next, _changes.Last().Sender);
            Assert.AreEqual("IsTrueOrNull", _changes.Last().EventArgs.PropertyName);

            fake.Next.IsTrueOrNull = other;

            Assert.AreEqual(2, _changes.Count);
            Assert.AreSame(fake.Next, _changes.Last().Sender);
            Assert.AreEqual("IsTrueOrNull", _changes.Last().EventArgs.PropertyName);
        }

        [Test]
        public void ThreeLevelsStartingWithNull()
        {
            var fake = new Fake();
            var observable = fake.ObservePropertyChanged(x => x.Next.Next.IsTrue);
            var disposable = observable.Subscribe(_changes.Add);
            Assert.AreEqual(1, _changes.Count);
            fake.Next = new Level { Next = new Level() };
            Assert.AreEqual(2, _changes.Count);
            Assert.AreSame(fake.Next.Next, _changes.Last().Sender);
            Assert.AreEqual("IsTrue", _changes.Last().EventArgs.PropertyName);

            fake.Next.Next.IsTrue = !fake.Next.Next.IsTrue;
            Assert.AreEqual(3, _changes.Count);
            Assert.AreSame(fake.Next.Next, _changes.Last().Sender);
            Assert.AreEqual("IsTrue", _changes.Last().EventArgs.PropertyName);

            fake.Next = null;
            Assert.AreEqual(4, _changes.Count);
            Assert.AreSame(null, _changes.Last().Sender);
            Assert.AreEqual("IsTrue", _changes.Last().EventArgs.PropertyName);
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase("Next")]
        public void FirstInPathSignalsEventWhenHasValue(string propertyName)
        {
            var next = new Level();
            var fake = new Fake { Next = next };
            fake.ObservePropertyChanged(x => x.Next.IsTrue, false)
                .Subscribe(_changes.Add);
            fake.OnPropertyChanged(propertyName);
            Assert.AreEqual(1, _changes.Count);
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase("IsTrue")]
        public void LastInPathSignalsEvent(string propertyName)
        {
            var next = new Level();
            var fake = new Fake { Next = next };
            var observable = fake.ObservePropertyChanged(x => x.Next.IsTrue, false);
            var disposable = observable.Subscribe(_changes.Add);
            fake.Next.OnPropertyChanged(propertyName);
            Assert.AreEqual(1, _changes.Count);
            Assert.AreSame(fake.Next, _changes.Last().Sender);
            Assert.AreEqual(propertyName, _changes.Last().EventArgs.PropertyName);
        }

        [Test]
        public void ThreeLevelsExisting()
        {
            var fake = new Fake { Next = new Level { Next = new Level() } };
            var observable = fake.ObservePropertyChanged(x => x.Next.Next.IsTrue);
            var disposable = observable.Subscribe(_changes.Add);
            Assert.AreEqual(1, _changes.Count);
            Assert.AreSame(fake.Next.Next, _changes.Last().Sender);
            Assert.AreEqual("IsTrue", _changes.Last().EventArgs.PropertyName);

            fake.Next.Next.IsTrue = !fake.Next.Next.IsTrue;
            Assert.AreEqual(2, _changes.Count);
            Assert.AreSame(fake.Next.Next, _changes.Last().Sender);
            Assert.AreEqual("IsTrue", _changes.Last().EventArgs.PropertyName);
        }

        [Test]
        public void ThreeLevelsExistingLevelBecomesNull()
        {
            var fake = new Fake { Next = new Level { Next = new Level() } };
            var observable = fake.ObservePropertyChanged(x => x.Next.Next.IsTrue);
            var disposable = observable.Subscribe(_changes.Add);
            Assert.AreEqual(1, _changes.Count);
            Assert.AreSame(fake.Next.Next, _changes.Last().Sender);
            Assert.AreEqual("IsTrue", _changes.Last().EventArgs.PropertyName);

            fake.Next = null;
            Assert.AreEqual(2, _changes.Count);
            Assert.AreSame(null, _changes.Last().Sender);
            Assert.AreEqual("IsTrue", _changes.Last().EventArgs.PropertyName);
        }

        [Test]
        public void ThreeLevelsExistingLevelBecomesNew()
        {
            var fake = new Fake { Next = new Level { Next = new Level() } };
            var observable = fake.ObservePropertyChanged(x => x.Next.Next.IsTrue);
            var disposable = observable.Subscribe(_changes.Add);
            Assert.AreEqual(1, _changes.Count);
            Assert.AreSame(fake.Next.Next, _changes.Last().Sender);
            Assert.AreEqual("IsTrue", _changes.Last().EventArgs.PropertyName);

            fake.Next = new Level() { Next = new Level() };
            Assert.AreEqual(2, _changes.Count);
            Assert.AreSame(fake.Next.Next, _changes.Last().Sender);
            Assert.AreEqual("IsTrue", _changes.Last().EventArgs.PropertyName);
        }

        [Test]
        public void Reacts()
        {
            var fake = new Fake();
            var observable = fake.ObservePropertyChanged(x => x.Next.IsTrue);
            observable.Subscribe(_changes.Add);
            Assert.AreEqual(1, _changes.Count);
            Assert.AreSame(fake.Next, _changes.Last().Sender);
            Assert.AreEqual("IsTrue", _changes.Last().EventArgs.PropertyName);

            fake.Next = new Level { IsTrue = false };
            Assert.AreEqual(2, _changes.Count);
            Assert.AreSame(fake.Next, _changes.Last().Sender);
            Assert.AreEqual("IsTrue", _changes.Last().EventArgs.PropertyName);

            fake.Next.IsTrue = true;
            Assert.AreEqual(3, _changes.Count);
            Assert.AreSame(fake.Next, _changes.Last().Sender);
            Assert.AreEqual("IsTrue", _changes.Last().EventArgs.PropertyName);

            Level level1 = fake.Next;
            fake.Next = null;
            Assert.AreEqual(4, _changes.Count);
            Assert.AreSame(fake.Next, _changes.Last().Sender);
            Assert.AreEqual("IsTrue", _changes.Last().EventArgs.PropertyName);

            level1.IsTrue = !level1.IsTrue;
            Assert.AreEqual(4, _changes.Count);
            Assert.AreSame(fake.Next, _changes.Last().Sender);
            Assert.AreEqual("IsTrue", _changes.Last().EventArgs.PropertyName);
        }

        [Test]
        public void MemoryLeakRootDisposeTest()
        {
            var rootRef = new WeakReference(null);
            var levelRef = new WeakReference(null);
            IObservable<EventPattern<PropertyChangedAndValueEventArgs<string>>> observable = null;
            new Action(
                () =>
                {
                    var level = new Level();
                    var fake = new Fake { Next = level };
                    rootRef.Target = fake;
                    levelRef.Target = fake.Next;
                    Assert.IsTrue(rootRef.IsAlive);
                    observable = fake.ObservePropertyChangedWithValue(x => x.Next.Name, false);
                })();
            // http://stackoverflow.com/a/579001/1069200
            var subscription = observable.Subscribe();
            GC.KeepAlive(observable);
            GC.KeepAlive(subscription);

            subscription.Dispose();
            GC.Collect();
            Assert.IsFalse(rootRef.IsAlive);
            Assert.IsFalse(levelRef.IsAlive);
        }

        [Test]
        public void MemoryLeakLevelDisposeTest()
        {
            WeakReference wr = new WeakReference(null);
            IObservable<EventPattern<PropertyChangedAndValueEventArgs<string>>> observable = null;
            new Action(
                () =>
                {
                    var level = new Level();
                    var fake = new Fake { Next = level };
                    wr.Target = fake.Next;
                    Assert.IsTrue(wr.IsAlive);
                    observable = fake.ObservePropertyChangedWithValue(x => x.Next.Name, false);
                })();
            // http://stackoverflow.com/a/579001/1069200

            var subscription = observable.Subscribe();
            GC.KeepAlive(observable);
            GC.KeepAlive(subscription);

            subscription.Dispose();
            GC.Collect();
            Assert.IsFalse(wr.IsAlive);
        }

        [Test]
        public void MemoryLeakLevelNoDisposeTest()
        {
            var level = new Level();
            var fake = new Fake { Next = level };
            var wr = new WeakReference(level);
            var observable = fake.ObservePropertyChanged(x => x.Next.Value);
            var subscription = observable.Subscribe();
            GC.KeepAlive(observable);
            GC.KeepAlive(subscription);

            level = null;
            fake.Next = null;
            GC.Collect();

            Assert.IsFalse(wr.IsAlive);
        }

        [Test]
        public void MemoryLeakRootNoDisposeTest()
        {
            var rootRef = new WeakReference(null);
            var levelRef = new WeakReference(null);
            IObservable<EventPattern<PropertyChangedAndValueEventArgs<string>>> observable = null;
            new Action(
                () =>
                {
                    var level = new Level();
                    var fake = new Fake { Next = level };
                    rootRef.Target = fake;
                    levelRef.Target = fake.Next;
                    Assert.IsTrue(rootRef.IsAlive);
                    observable = fake.ObservePropertyChangedWithValue(x => x.Next.Name, false);
                })();
            // http://stackoverflow.com/a/579001/1069200
            var subscription = observable.Subscribe();
            GC.KeepAlive(observable);
            GC.KeepAlive(subscription);

            GC.Collect();

            Assert.IsFalse(rootRef.IsAlive);
            Assert.IsFalse(levelRef.IsAlive);
        }

        private static void AssertEventPattern(object sender, string propertyName, EventPattern<PropertyChangedEventArgs> pattern)
        {
            Assert.AreSame(sender, pattern.Sender);
            Assert.AreEqual(propertyName, pattern.EventArgs.PropertyName);
        }
    }
}