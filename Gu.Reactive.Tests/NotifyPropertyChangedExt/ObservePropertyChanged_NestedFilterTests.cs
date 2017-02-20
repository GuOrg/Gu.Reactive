// ReSharper disable All
namespace Gu.Reactive.Tests.NotifyPropertyChangedExt
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reactive;

    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    // ReSharper disable once InconsistentNaming
    public class ObservePropertyChanged_NestedFilterTests
    {
        [Test]
        public void ReactsTwoPropertiesSameInstance()
        {
            var changes = new List<EventPattern<PropertyChangedEventArgs>>();
            var fake = new Fake { Next = new Level { Value = 1 } };
            fake.ObservePropertyChanged(x => x.Next.Value, false)
                .Subscribe(changes.Add);

            fake.ObservePropertyChanged(x => x.Next.IsTrue, false)
                .Subscribe(changes.Add);
            Assert.AreEqual(0, changes.Count);

            fake.Next.Value++;
            Assert.AreEqual(1, changes.Count);
            AssertEventPattern(fake.Next, "Value", changes.Last());

            fake.Next.IsTrue = !fake.IsTrue;
            Assert.AreEqual(2, changes.Count);
            AssertEventPattern(fake.Next, "IsTrue", changes.Last());
        }

        [Test]
        public void ReactsTwoInstancesSameProperty()
        {
            var changes = new List<EventPattern<PropertyChangedEventArgs>>();
            var fake1 = new Fake { Next = new Level() };
            fake1.ObservePropertyChanged(x => x.Next.Value, false)
                .Subscribe(changes.Add);
            var fake2 = new Fake { Next = new Level() };
            fake2.ObservePropertyChanged(x => x.Next.Value, false)
                .Subscribe(changes.Add);
            Assert.AreEqual(0, changes.Count);

            fake1.Next.Value++;
            Assert.AreEqual(1, changes.Count);
            AssertEventPattern(fake1.Next, "Value", changes.Last());

            fake2.Next.Value++;
            Assert.AreEqual(2, changes.Count);
            AssertEventPattern(fake2.Next, "Value", changes.Last());
        }

        [Test]
        public void TwoSubscriptionsOneObservable()
        {
            var changes1 = new List<EventPattern<PropertyChangedEventArgs>>();
            var changes2 = new List<EventPattern<PropertyChangedEventArgs>>();
            var fake = new Fake { Next = new Level() };
            var observable = fake.ObservePropertyChanged(x => x.Next.IsTrue, false);
            using (observable.Subscribe(changes1.Add))
            {
                using (observable.Subscribe(changes2.Add))
                {
                    Assert.AreEqual(0, changes1.Count);
                    Assert.AreEqual(0, changes2.Count);

                    fake.Next.IsTrue = !fake.Next.IsTrue;
                    Assert.AreEqual(1, changes1.Count);
                    Assert.AreEqual(1, changes2.Count);
                    AssertEventPattern(fake.Next, "IsTrue", changes1.Last());
                    AssertEventPattern(fake.Next, "IsTrue", changes2.Last());

                    fake.Next.IsTrue = !fake.Next.IsTrue;
                    Assert.AreEqual(2, changes1.Count);
                    Assert.AreEqual(2, changes2.Count);
                    AssertEventPattern(fake.Next, "IsTrue", changes1.Last());
                    AssertEventPattern(fake.Next, "IsTrue", changes2.Last());

                    fake.Next = null;
                    Assert.AreEqual(3, changes1.Count);
                    Assert.AreEqual(3, changes2.Count);
                    AssertEventPattern(null, "IsTrue", changes1.Last());
                    AssertEventPattern(null, "IsTrue", changes2.Last());
                }
            }
        }

        [Test]
        public void ThrowsOnStructInPath()
        {
            var fake = new Fake();
            var exception = Assert.Throws<ArgumentException>(() => fake.ObservePropertyChanged(x => x.StructLevel.Name));
            var expected = "Property path cannot have structs in it. Copy by value will make subscribing error prone. Also mutable struct much?\r\n" +
                           "The type StructLevel is a value type not so x.StructLevel will not notify when it changes.\r\n" +
                           "The path is: x => x.StructLevel.Name\r\n" +
                           "Parameter name: path";
            Assert.AreEqual(expected, exception.Message);
        }

        [Test]
        public void ThrowsOnNotInpcInPath()
        {
            var fake = new Fake();
            var exception = Assert.Throws<ArgumentException>(() => fake.ObservePropertyChanged(x => x.Name.Length));
            var expected = "All levels in the path must implement INotifyPropertyChanged.\r\n" +
                           "The type string does not so x.Name will not notify when it changes.\r\n" +
                           "The path is: x => x.Name.Length\r\n" +
                           "Parameter name: path";
            Assert.AreEqual(expected, exception.Message);
        }

        [Test]
        public void ThrowsOnMethodInPath()
        {
            var fake = new Fake();
            var exception = Assert.Throws<ArgumentException>(() => fake.ObservePropertyChanged(x => x.Method().Name));
            Assert.AreEqual("Expected path to be properties only. Was x.Method().Name", exception.Message);
        }

        [TestCase(true, 1)]
        [TestCase(false, 0)]
        public void SignalsInitialWhenHasValue(bool signalInitial, int expected)
        {
            var changes = new List<EventPattern<PropertyChangedEventArgs>>();
            var fake = new Fake { Next = new Level() };
            fake.ObservePropertyChanged(x => x.Next.Value, signalInitial)
                .Subscribe(changes.Add);

            Assert.AreEqual(expected, changes.Count);
            if (expected == 1)
            {
                AssertEventPattern(fake.Next, "Value", changes.Last());
            }

            fake.Next.Value++;
            Assert.AreEqual(expected + 1, changes.Count); // Double check that we are subscribing
        }

        [TestCase(true, 1)]
        [TestCase(false, 0)]
        public void SignalsInitialWhenHasValueGeneric(bool signalInitial, int expected)
        {
            var changes = new List<EventPattern<PropertyChangedEventArgs>>();
            var fake = new Fake<int> { Next = new Level<int>() };
            fake.ObservePropertyChanged(x => x.Next.Value, signalInitial)
                .Subscribe(changes.Add);

            Assert.AreEqual(expected, changes.Count);
            if (expected == 1)
            {
                AssertEventPattern(fake.Next, "Value", changes.Last());
            }

            fake.Next.Value++;
            Assert.AreEqual(expected + 1, changes.Count); // Double check that we are subscribing
        }

        [TestCase(true, 1)]
        [TestCase(false, 0)]
        public void SignalsInitialWhenNoValue(bool signalInitial, int expected)
        {
            var changes = new List<EventPattern<PropertyChangedEventArgs>>();
            var fake = new Fake();
            fake.ObservePropertyChanged(x => x.Next.Value, signalInitial)
                .Subscribe(changes.Add);

            Assert.AreEqual(expected, changes.Count);
            if (expected == 1)
            {
                AssertEventPattern(null, "Value", changes.Last());
            }

            fake.Next = new Level();
            Assert.AreEqual(expected + 1, changes.Count); // Double check that we are subscribing
        }

        [TestCase(true, 1)]
        [TestCase(false, 0)]
        public void SignalsInitialWhenNoValueGeneric(bool signalInitial, int expected)
        {
            var changes = new List<EventPattern<PropertyChangedEventArgs>>();
            var fake = new Fake<int>();
            fake.ObservePropertyChanged(x => x.Next.Value, signalInitial)
                .Subscribe(changes.Add);

            Assert.AreEqual(expected, changes.Count);
            if (expected == 1)
            {
                AssertEventPattern(null, "Value", changes.Last());
            }

            fake.Next = new Level<int>();
            Assert.AreEqual(expected + 1, changes.Count); // Double check that we are subscribing
        }

        [TestCase("")]
        [TestCase(null)]
        public void DoesNotReactToStringEmptyOrNullFromRootWhenNull(string propertyName)
        {
            var changes = new List<EventPattern<PropertyChangedEventArgs>>();
            var fake = new Fake();
            var observable = fake.ObservePropertyChanged(x => x.Next.IsTrue, false);
            using (observable.Subscribe(changes.Add))
            {
                Assert.AreEqual(0, changes.Count);
                fake.OnPropertyChanged(propertyName); // This means all properties changed according to wpf convention
                Assert.AreEqual(0, changes.Count);
            }
        }

        [TestCase("")]
        [TestCase(null)]
        public void DoesReactToStringEmptyOrNullFromRootWhenNotNull(string propertyName)
        {
            var changes = new List<EventPattern<PropertyChangedEventArgs>>();
            var fake = new Fake { Next = new Level() };
            var observable = fake.ObservePropertyChanged(x => x.Next.IsTrue, false);
            using (var disposable = observable.Subscribe(changes.Add))
            {
                Assert.AreEqual(0, changes.Count);

                fake.OnPropertyChanged(propertyName); // This means all properties changed according to wpf convention
                Assert.AreEqual(1, changes.Count);
                AssertEventPattern(fake.Next, propertyName, changes.Last());
            }
        }

        [TestCase("")]
        [TestCase(null)]
        public void ReactsOnStringEmptyOrNullFromSource(string propertyName)
        {
            var changes = new List<EventPattern<PropertyChangedEventArgs>>();
            var fake = new Fake { Next = new Level() };
            var observable = fake.ObservePropertyChanged(x => x.Next.IsTrue, false);
            using (observable.Subscribe(changes.Add))
            {
                Assert.AreEqual(0, changes.Count);

                fake.Next.OnPropertyChanged(propertyName); // This means all properties changed according to wpf convention

                Assert.AreEqual(1, changes.Count);
                Assert.AreEqual(fake.Next, changes.Last().Sender);
                Assert.AreEqual(propertyName, changes.Last().EventArgs.PropertyName);
            }
        }

        [Test]
        public void TwoLevelsReacts()
        {
            var changes = new List<EventPattern<PropertyChangedEventArgs>>();
            var fake = new Fake();
            fake.ObservePropertyChanged(x => x.Next.IsTrue, true)
                .Subscribe(changes.Add);
            Assert.AreEqual(1, changes.Count);
            Assert.AreSame(null, changes.Single().Sender);
            Assert.AreEqual("IsTrue", changes.Last().EventArgs.PropertyName);

            fake.Next = new Level();
            Assert.AreEqual(2, changes.Count);
            Assert.AreSame(fake.Next, changes.Last().Sender);
            Assert.AreEqual("IsTrue", changes.Last().EventArgs.PropertyName);

            fake.Next.IsTrue = !fake.Next.IsTrue;
            Assert.AreEqual(3, changes.Count);
            Assert.AreSame(fake.Next, changes.Last().Sender);
            Assert.AreEqual("IsTrue", changes.Last().EventArgs.PropertyName);
        }

        [Test]
        public void TwoLevelsReactsGeneric()
        {
            var changes = new List<EventPattern<PropertyChangedEventArgs>>();
            var fake = new Fake<int>();
            fake.ObservePropertyChanged(x => x.Next.Value, true)
                .Subscribe(changes.Add);
            Assert.AreEqual(1, changes.Count);
            Assert.AreSame(null, changes.Single().Sender);
            Assert.AreEqual("Value", changes.Last().EventArgs.PropertyName);

            fake.Next = new Level<int>();
            Assert.AreEqual(2, changes.Count);
            Assert.AreSame(fake.Next, changes.Last().Sender);
            Assert.AreEqual("Value", changes.Last().EventArgs.PropertyName);

            fake.Next.Value++;
            Assert.AreEqual(3, changes.Count);
            Assert.AreSame(fake.Next, changes.Last().Sender);
            Assert.AreEqual("Value", changes.Last().EventArgs.PropertyName);
        }

        [Test]
        public void TwoLevelsRootChangesFromValueToNull()
        {
            var changes = new List<EventPattern<PropertyChangedEventArgs>>();
            var fake = new Fake { Next = new Level() };
            fake.ObservePropertyChanged(x => x.Next.IsTrue, true)
                .Subscribe(changes.Add);
            Assert.AreEqual(1, changes.Count);
            Assert.AreSame(fake.Next, changes.Single().Sender);
            Assert.AreEqual("IsTrue", changes.Last().EventArgs.PropertyName);

            fake.Next = null;
            Assert.AreEqual(2, changes.Count);
            Assert.AreSame(null, changes.Last().Sender);
            Assert.AreEqual("IsTrue", changes.Last().EventArgs.PropertyName);
        }

        [Test]
        public void TwoLevelsRootChangesFromNullToValue()
        {
            var changes = new List<EventPattern<PropertyChangedEventArgs>>();
            var fake = new Fake();
            fake.ObservePropertyChanged(x => x.Next.IsTrue, true)
                .Subscribe(changes.Add);
            Assert.AreEqual(1, changes.Count);
            Assert.AreSame(null, changes.Single().Sender);
            Assert.AreEqual("IsTrue", changes.Last().EventArgs.PropertyName);

            fake.Next = new Level();
            Assert.AreEqual(2, changes.Count);
            Assert.AreSame(fake.Next, changes.Last().Sender);
            Assert.AreEqual("IsTrue", changes.Last().EventArgs.PropertyName);
        }

        [Test]
        public void TwoLevelsStartingWithValue()
        {
            var changes = new List<EventPattern<PropertyChangedEventArgs>>();
            var fake = new Fake { Next = new Level() };
            fake.ObservePropertyChanged(x => x.Next.IsTrue, true)
                .Subscribe(changes.Add);
            Assert.AreEqual(1, changes.Count);
            Assert.AreSame(fake.Next, changes.Last().Sender);
            Assert.AreEqual("IsTrue", changes.Last().EventArgs.PropertyName);

            fake.Next.IsTrue = !fake.Next.IsTrue;
            Assert.AreEqual(2, changes.Count);
            Assert.AreSame(fake.Next, changes.Last().Sender);
            Assert.AreEqual("IsTrue", changes.Last().EventArgs.PropertyName);
        }

        [Test]
        public void TwoLevelsStartingWithoutValue()
        {
            var changes = new List<EventPattern<PropertyChangedEventArgs>>();
            var fake = new Fake();
            fake.ObservePropertyChanged(x => x.Next.IsTrue, true)
                .Subscribe(changes.Add);
            Assert.AreEqual(1, changes.Count);
            Assert.AreSame(fake.Next, changes.Last().Sender);
            Assert.AreEqual("IsTrue", changes.Last().EventArgs.PropertyName);

            fake.Next = new Level();
            Assert.AreEqual(2, changes.Count);
            Assert.AreSame(fake.Next, changes.Last().Sender);
            Assert.AreEqual("IsTrue", changes.Last().EventArgs.PropertyName);

            fake.Next.IsTrue = !fake.Next.IsTrue;
            Assert.AreEqual(3, changes.Count);
            Assert.AreSame(fake.Next, changes.Last().Sender);
            Assert.AreEqual("IsTrue", changes.Last().EventArgs.PropertyName);
        }

        [TestCase(true, null)]
        [TestCase(null, false)]
        [TestCase(null, true)]
        public void TwoLevelsNullableStartingWithValue(bool? first, bool? other)
        {
            var changes = new List<EventPattern<PropertyChangedEventArgs>>();
            var fake = new Fake { Next = new Level { IsTrueOrNull = first } };
            fake.ObservePropertyChanged(x => x.Next.IsTrueOrNull, true)
                .Subscribe(changes.Add);
            Assert.AreEqual(1, changes.Count);
            Assert.AreSame(fake.Next, changes.Last().Sender);
            Assert.AreEqual("IsTrueOrNull", changes.Last().EventArgs.PropertyName);

            fake.Next.IsTrueOrNull = other;

            Assert.AreEqual(2, changes.Count);
            Assert.AreSame(fake.Next, changes.Last().Sender);
            Assert.AreEqual("IsTrueOrNull", changes.Last().EventArgs.PropertyName);
        }

        [Test]
        public void ThreeLevelsStartingWithNull()
        {
            var changes = new List<EventPattern<PropertyChangedEventArgs>>();
            var fake = new Fake();
            var observable = fake.ObservePropertyChanged(x => x.Next.Next.IsTrue);
            using (observable.Subscribe(changes.Add))
            {
                Assert.AreEqual(1, changes.Count);
                fake.Next = new Level { Next = new Level() };
                Assert.AreEqual(2, changes.Count);
                Assert.AreSame(fake.Next.Next, changes.Last().Sender);
                Assert.AreEqual("IsTrue", changes.Last().EventArgs.PropertyName);

                fake.Next.Next.IsTrue = !fake.Next.Next.IsTrue;
                Assert.AreEqual(3, changes.Count);
                Assert.AreSame(fake.Next.Next, changes.Last().Sender);
                Assert.AreEqual("IsTrue", changes.Last().EventArgs.PropertyName);

                fake.Next = null;
                Assert.AreEqual(4, changes.Count);
                Assert.AreSame(null, changes.Last().Sender);
                Assert.AreEqual("IsTrue", changes.Last().EventArgs.PropertyName);
            }
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase("Next")]
        public void FirstInPathSignalsEventWhenHasValue(string propertyName)
        {
            var changes = new List<EventPattern<PropertyChangedEventArgs>>();
            var next = new Level();
            var fake = new Fake { Next = next };
            fake.ObservePropertyChanged(x => x.Next.IsTrue, false)
                .Subscribe(changes.Add);
            fake.OnPropertyChanged(propertyName);
            Assert.AreEqual(1, changes.Count);
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase("IsTrue")]
        public void LastInPathSignalsEvent(string propertyName)
        {
            var changes = new List<EventPattern<PropertyChangedEventArgs>>();
            var next = new Level();
            var fake = new Fake { Next = next };
            var observable = fake.ObservePropertyChanged(x => x.Next.IsTrue, false);
            using (observable.Subscribe(changes.Add))
            {
                fake.Next.OnPropertyChanged(propertyName);
                Assert.AreEqual(1, changes.Count);
                Assert.AreSame(fake.Next, changes.Last().Sender);
                Assert.AreEqual(propertyName, changes.Last().EventArgs.PropertyName);
            }
        }

        [Test]
        public void ThreeLevelsExisting()
        {
            var changes = new List<EventPattern<PropertyChangedEventArgs>>();
            var fake = new Fake { Next = new Level { Next = new Level() } };
            var observable = fake.ObservePropertyChanged(x => x.Next.Next.IsTrue);
            using (observable.Subscribe(changes.Add))
            {
                Assert.AreEqual(1, changes.Count);
                Assert.AreSame(fake.Next.Next, changes.Last().Sender);
                Assert.AreEqual("IsTrue", changes.Last().EventArgs.PropertyName);

                fake.Next.Next.IsTrue = !fake.Next.Next.IsTrue;
                Assert.AreEqual(2, changes.Count);
                Assert.AreSame(fake.Next.Next, changes.Last().Sender);
                Assert.AreEqual("IsTrue", changes.Last().EventArgs.PropertyName);
            }
        }

        [Test]
        public void ThreeLevelsExistingLevelBecomesNull()
        {
            var changes = new List<EventPattern<PropertyChangedEventArgs>>();
            var fake = new Fake { Next = new Level { Next = new Level() } };
            var observable = fake.ObservePropertyChanged(x => x.Next.Next.IsTrue);
            using (observable.Subscribe(changes.Add))
            {
                Assert.AreEqual(1, changes.Count);
                Assert.AreSame(fake.Next.Next, changes.Last().Sender);
                Assert.AreEqual("IsTrue", changes.Last().EventArgs.PropertyName);

                fake.Next = null;
                Assert.AreEqual(2, changes.Count);
                Assert.AreSame(null, changes.Last().Sender);
                Assert.AreEqual("IsTrue", changes.Last().EventArgs.PropertyName);
            }
        }

        [Test]
        public void ThreeLevelsExistingLevelBecomesNew()
        {
            var changes = new List<EventPattern<PropertyChangedEventArgs>>();
            var fake = new Fake { Next = new Level { Next = new Level() } };
            var observable = fake.ObservePropertyChanged(x => x.Next.Next.IsTrue);
            using (observable.Subscribe(changes.Add))
            {
                Assert.AreEqual(1, changes.Count);
                Assert.AreSame(fake.Next.Next, changes.Last().Sender);
                Assert.AreEqual("IsTrue", changes.Last().EventArgs.PropertyName);

                fake.Next = new Level() { Next = new Level() };
                Assert.AreEqual(2, changes.Count);
                Assert.AreSame(fake.Next.Next, changes.Last().Sender);
                Assert.AreEqual("IsTrue", changes.Last().EventArgs.PropertyName);
            }
        }

        [Test]
        public void Reacts()
        {
            var changes = new List<EventPattern<PropertyChangedEventArgs>>();
            var fake = new Fake();
            var observable = fake.ObservePropertyChanged(x => x.Next.IsTrue);
            observable.Subscribe(changes.Add);
            Assert.AreEqual(1, changes.Count);
            Assert.AreSame(fake.Next, changes.Last().Sender);
            Assert.AreEqual("IsTrue", changes.Last().EventArgs.PropertyName);

            fake.Next = new Level { IsTrue = false };
            Assert.AreEqual(2, changes.Count);
            Assert.AreSame(fake.Next, changes.Last().Sender);
            Assert.AreEqual("IsTrue", changes.Last().EventArgs.PropertyName);

            fake.Next.IsTrue = true;
            Assert.AreEqual(3, changes.Count);
            Assert.AreSame(fake.Next, changes.Last().Sender);
            Assert.AreEqual("IsTrue", changes.Last().EventArgs.PropertyName);

            Level level1 = fake.Next;
            fake.Next = null;
            Assert.AreEqual(4, changes.Count);
            Assert.AreSame(fake.Next, changes.Last().Sender);
            Assert.AreEqual("IsTrue", changes.Last().EventArgs.PropertyName);

            level1.IsTrue = !level1.IsTrue;
            Assert.AreEqual(4, changes.Count);
            Assert.AreSame(fake.Next, changes.Last().Sender);
            Assert.AreEqual("IsTrue", changes.Last().EventArgs.PropertyName);
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
            //// http://stackoverflow.com/a/579001/1069200
            using (var subscription = observable.Subscribe())
            {
                GC.KeepAlive(observable);
                GC.KeepAlive(subscription);
            }

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
            //// http://stackoverflow.com/a/579001/1069200
            using (var subscription = observable.Subscribe())
            {
                GC.KeepAlive(observable);
                GC.KeepAlive(subscription);
            }

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
#pragma warning disable GU0030 // Use using.
            var subscription = observable.Subscribe();
#pragma warning restore GU0030 // Use using.
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
            //// http://stackoverflow.com/a/579001/1069200
#pragma warning disable GU0030 // Use using.
            var subscription = observable.Subscribe();
#pragma warning restore GU0030 // Use using.
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