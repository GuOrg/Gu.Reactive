namespace Gu.Reactive.Tests.NotifyPropertyChangedExt
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reactive;

    using Gu.Reactive.Tests.Helpers;

    using Moq;

    using NUnit.Framework;

    // ReSharper disable once InconsistentNaming
    public class ObservePropertyChanged_FilteredTests
    {
        private List<EventPattern<PropertyChangedEventArgs>> changes;

        [SetUp]
        public void SetUp()
        {
            this.changes = new List<EventPattern<PropertyChangedEventArgs>>();
        }

        [Test]
        public void ReactsOnMock()
        {
            var mock = new Mock<IReadOnlyObservableCollection<int>>();
            mock.Object.ObservePropertyChanged(x => x.Count, false)
                .Subscribe(this.changes.Add);
            Assert.AreEqual(0, this.changes.Count);

            mock.Raise(x => x.PropertyChanged += null, new PropertyChangedEventArgs("Count"));
            Assert.AreEqual(1, this.changes.Count);
        }

        [Test]
        public void HandlesNull()
        {
            var fake = new Fake { Name = "1" };
            fake.ObservePropertyChanged(x => x.Name, false)
                .Subscribe(this.changes.Add);

            Assert.AreEqual(0, this.changes.Count);

            fake.Name = null;
            Assert.AreEqual(1, this.changes.Count);
            AssertEventPattern(fake, "Name", this.changes.Last());

            fake.Name = "1";
            Assert.AreEqual(2, this.changes.Count);
            AssertEventPattern(fake, "Name", this.changes.Last());
        }

        [Test]
        public void ReactsTwoPropertiesSameInstance()
        {
            var fake = new Fake { Value = 1 };
            fake.ObservePropertyChanged(x => x.Value, false)
                .Subscribe(this.changes.Add);

            fake.ObservePropertyChanged(x => x.IsTrue, false)
                .Subscribe(this.changes.Add);
            Assert.AreEqual(0, this.changes.Count);

            fake.Value++;
            Assert.AreEqual(1, this.changes.Count);
            AssertEventPattern(fake, "Value", this.changes.Last());

            fake.IsTrue = !fake.IsTrue;
            Assert.AreEqual(2, this.changes.Count);
            AssertEventPattern(fake, "IsTrue", this.changes.Last());
        }

        [Test]
        public void ReactsTwoInstances()
        {
            var fake1 = new Fake { Value = 1 };
            fake1.ObservePropertyChanged(x => x.Value, false)
                .Subscribe(this.changes.Add);
            var fake2 = new Fake { Value = 1 };
            fake2.ObservePropertyChanged(x => x.Value, false)
                .Subscribe(this.changes.Add);
            Assert.AreEqual(0, this.changes.Count);

            fake1.Value++;
            Assert.AreEqual(1, this.changes.Count);
            AssertEventPattern(fake1, "Value", this.changes.Last());

            fake2.Value++;
            Assert.AreEqual(2, this.changes.Count);
            AssertEventPattern(fake2, "Value", this.changes.Last());
        }

        [Test]
        public void ReactsWhenValueChanges()
        {
            var fake = new Fake { Value = 1 };
            fake.ObservePropertyChanged(x => x.Value, false)
                .Subscribe(this.changes.Add);
            Assert.AreEqual(0, this.changes.Count);

            fake.Value++;

            Assert.AreEqual(1, this.changes.Count);
            AssertEventPattern(fake, "Value", this.changes.Last());
        }

        [Test]
        public void DoesNotReactWhenOtherPropertyChanges()
        {
            var fake = new Fake { Value = 1 };
            fake.ObservePropertyChanged(x => x.Value, false)
                .Subscribe(this.changes.Add);
            Assert.AreEqual(0, this.changes.Count);
            fake.Value++;
            Assert.AreEqual(1, this.changes.Count);
            AssertEventPattern(fake, "Value", this.changes.Last());

            fake.IsTrue = !fake.IsTrue;

            Assert.AreEqual(1, this.changes.Count); // No notification when changing other property
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase("Value")]
        public void ReactsOnStringEmptyOrNull(string propertyName)
        {
            var fake = new Fake { Value = 1 };
            fake.ObservePropertyChanged(x => x.Value, false)
                .Subscribe(this.changes.Add);

            Assert.AreEqual(0, this.changes.Count);

            fake.OnPropertyChanged(propertyName); // This means all properties changed according to wpf convention

            Assert.AreEqual(1, this.changes.Count);
            AssertEventPattern(fake, propertyName, this.changes.Last());
        }

        [TestCase(true, 1)]
        [TestCase(false, 0)]
        public void SignalsInitial(bool signalInitial, int expected)
        {
            var fake = new Fake { Value = 1 };

            fake.ObservePropertyChanged(x => x.Value, signalInitial)
                .Subscribe(this.changes.Add);

            Assert.AreEqual(expected, this.changes.Count);
            if (signalInitial)
            {
                AssertEventPattern(fake, "Value", this.changes.Last());
            }

            fake.Value++;
            Assert.AreEqual(expected + 1, this.changes.Count); // Double check that we are subscribing
        }

        [Test]
        public void MemoryLeakDisposeTest()
        {
            var fake = new Fake();
            var wr = new WeakReference(fake);
            var observable = fake.ObservePropertyChanged(x => x.IsTrueOrNull);
            using (var subscription = observable.Subscribe())
            {
                GC.KeepAlive(observable);
                GC.KeepAlive(subscription);

                fake = null;
            }

            GC.Collect();
            Assert.IsFalse(wr.IsAlive);
        }

        [Test]
        public void MemoryLeakNoDisposeTest()
        {
            var fake = new Fake();
            var wr = new WeakReference(fake);
            var observable = fake.ObservePropertyChanged(x => x.IsTrueOrNull);
#pragma warning disable GU0030 // Use using.
            var subscription = observable.Subscribe();
#pragma warning restore GU0030 // Use using.
            GC.KeepAlive(observable);
            GC.KeepAlive(subscription);

            fake = null;
            GC.Collect();

            Assert.IsFalse(wr.IsAlive);
        }

        [Test]
        public void MemoryLeakFilteredNoDisposeTest()
        {
            var fake = new Fake();
            var wr = new WeakReference(fake);
            Assert.IsTrue(wr.IsAlive);
            var observable = fake.ObservePropertyChanged(x => x.Name);
#pragma warning disable GU0030 // Use using.
            var subscription = observable.Subscribe();
#pragma warning restore GU0030 // Use using.
            GC.KeepAlive(observable);
            GC.KeepAlive(subscription);

            fake = null;
            GC.Collect();
            Assert.IsFalse(wr.IsAlive);
        }

        private static void AssertEventPattern(object sender, string propertyName, EventPattern<PropertyChangedEventArgs> pattern)
        {
            Assert.AreSame(sender, pattern.Sender);
            Assert.AreEqual(propertyName, pattern.EventArgs.PropertyName);
        }
    }
}