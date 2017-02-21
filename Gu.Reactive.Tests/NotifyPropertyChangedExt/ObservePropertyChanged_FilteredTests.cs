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
        [Test]
        public void ReactsOnMock()
        {
            var changes = new List<EventPattern<PropertyChangedEventArgs>>();
            var mock = new Mock<IReadOnlyObservableCollection<int>>();
            mock.Object.ObservePropertyChanged(x => x.Count, false)
                .Subscribe(changes.Add);
            Assert.AreEqual(0, changes.Count);

            mock.Raise(x => x.PropertyChanged += null, new PropertyChangedEventArgs("Count"));
            Assert.AreEqual(1, changes.Count);
        }

        [Test]
        public void HandlesNull()
        {
            var changes = new List<EventPattern<PropertyChangedEventArgs>>();
            var fake = new Fake { Name = "1" };
            fake.ObservePropertyChanged(x => x.Name, false)
                .Subscribe(changes.Add);

            Assert.AreEqual(0, changes.Count);

            fake.Name = null;
            Assert.AreEqual(1, changes.Count);
            AssertEventPattern(fake, "Name", changes.Last());

            fake.Name = "1";
            Assert.AreEqual(2, changes.Count);
            AssertEventPattern(fake, "Name", changes.Last());
        }

        [Test]
        public void ReactsTwoPropertiesSameInstance()
        {
            var changes = new List<EventPattern<PropertyChangedEventArgs>>();
            var fake = new Fake { Value = 1 };
            fake.ObservePropertyChanged(x => x.Value, false)
                .Subscribe(changes.Add);

            fake.ObservePropertyChanged(x => x.IsTrue, false)
                .Subscribe(changes.Add);
            Assert.AreEqual(0, changes.Count);

            fake.Value++;
            Assert.AreEqual(1, changes.Count);
            AssertEventPattern(fake, "Value", changes.Last());

            fake.IsTrue = !fake.IsTrue;
            Assert.AreEqual(2, changes.Count);
            AssertEventPattern(fake, "IsTrue", changes.Last());
        }

        [Test]
        public void ReactsTwoInstances()
        {
            var changes = new List<EventPattern<PropertyChangedEventArgs>>();
            var fake1 = new Fake { Value = 1 };
            fake1.ObservePropertyChanged(x => x.Value, false)
                .Subscribe(changes.Add);
            var fake2 = new Fake { Value = 1 };
            fake2.ObservePropertyChanged(x => x.Value, false)
                .Subscribe(changes.Add);
            Assert.AreEqual(0, changes.Count);

            fake1.Value++;
            Assert.AreEqual(1, changes.Count);
            AssertEventPattern(fake1, "Value", changes.Last());

            fake2.Value++;
            Assert.AreEqual(2, changes.Count);
            AssertEventPattern(fake2, "Value", changes.Last());
        }

        [Test]
        public void TwoSubscriptionsOneObservable()
        {
            var changes1 = new List<EventPattern<PropertyChangedEventArgs>>();
            var changes2 = new List<EventPattern<PropertyChangedEventArgs>>();
            var fake = new Fake { Value = 1 };
            var observable = fake.ObservePropertyChanged(x => x.IsTrue, false);
            using (observable.Subscribe(changes1.Add))
            {
                using (observable.Subscribe(changes2.Add))
                {
                    Assert.AreEqual(0, changes1.Count);
                    Assert.AreEqual(0, changes2.Count);

                    fake.IsTrue = !fake.IsTrue;
                    Assert.AreEqual(1, changes1.Count);
                    Assert.AreEqual(1, changes2.Count);
                    AssertEventPattern(fake, "IsTrue", changes1.Last());
                    AssertEventPattern(fake, "IsTrue", changes2.Last());

                    fake.IsTrue = !fake.IsTrue;
                    Assert.AreEqual(2, changes1.Count);
                    Assert.AreEqual(2, changes2.Count);
                    AssertEventPattern(fake, "IsTrue", changes1.Last());
                    AssertEventPattern(fake, "IsTrue", changes2.Last());
                }
            }
        }

        [Test]
        public void ReactsWhenValueChanges()
        {
            var changes = new List<EventPattern<PropertyChangedEventArgs>>();
            var fake = new Fake { Value = 1 };
            fake.ObservePropertyChanged(x => x.Value, false)
                .Subscribe(changes.Add);
            Assert.AreEqual(0, changes.Count);

            fake.Value++;

            Assert.AreEqual(1, changes.Count);
            AssertEventPattern(fake, "Value", changes.Last());
        }

        [Test]
        public void ReactsWhenValueChangesGeneric()
        {
            var changes = new List<EventPattern<PropertyChangedEventArgs>>();
            var fake = new Fake<int> { Value = 1 };
            fake.ObservePropertyChanged(x => x.Value, false)
                .Subscribe(changes.Add);
            Assert.AreEqual(0, changes.Count);

            fake.Value++;

            Assert.AreEqual(1, changes.Count);
            AssertEventPattern(fake, "Value", changes.Last());
        }

        [Test]
        public void DoesNotReactWhenOtherPropertyChanges()
        {
            var changes = new List<EventPattern<PropertyChangedEventArgs>>();
            var fake = new Fake { Value = 1 };
            fake.ObservePropertyChanged(x => x.Value, false)
                .Subscribe(changes.Add);
            Assert.AreEqual(0, changes.Count);
            fake.Value++;
            Assert.AreEqual(1, changes.Count);
            AssertEventPattern(fake, "Value", changes.Last());

            fake.IsTrue = !fake.IsTrue;

            Assert.AreEqual(1, changes.Count); // No notification when changing other property
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase("Value")]
        public void ReactsOnStringEmptyOrNull(string propertyName)
        {
            var changes = new List<EventPattern<PropertyChangedEventArgs>>();
            var fake = new Fake { Value = 1 };
            fake.ObservePropertyChanged(x => x.Value, false)
                .Subscribe(changes.Add);

            Assert.AreEqual(0, changes.Count);

            fake.OnPropertyChanged(propertyName); // This means all properties changed according to wpf convention

            Assert.AreEqual(1, changes.Count);
            AssertEventPattern(fake, propertyName, changes.Last());
        }

        [TestCase(true, 1)]
        [TestCase(false, 0)]
        public void SignalsInitial(bool signalInitial, int expected)
        {
            var changes = new List<EventPattern<PropertyChangedEventArgs>>();
            var fake = new Fake { Value = 1 };

            fake.ObservePropertyChanged(x => x.Value, signalInitial)
                .Subscribe(changes.Add);

            Assert.AreEqual(expected, changes.Count);
            if (signalInitial)
            {
                Assert.AreSame(fake, changes.Single().Sender);
                Assert.AreEqual("Value", changes.Single().EventArgs.PropertyName);
            }

            fake.Value++;
            Assert.AreEqual(expected + 1, changes.Count); // Double check that we are subscribing
        }

        [Test]
        public void MemoryLeakDisposeTest()
        {
            var fake = new Fake();
            var wr = new WeakReference(fake);
            using (fake.ObservePropertyChanged(x => x.IsTrueOrNull).Subscribe())
            {
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
            // ReSharper disable once UnusedVariable
            var subscribe = observable.Subscribe();
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