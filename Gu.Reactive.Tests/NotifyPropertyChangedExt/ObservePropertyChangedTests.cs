#pragma warning disable WPF1014 // Don't raise PropertyChanged for missing property.
namespace Gu.Reactive.Tests.NotifyPropertyChangedExt
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reactive;

    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    public class ObservePropertyChangedTests
    {
        private List<EventPattern<PropertyChangedEventArgs>> changes;

        [SetUp]
        public void SetUp()
        {
            this.changes = new List<EventPattern<PropertyChangedEventArgs>>();
        }

        [Test]
        public void DoesNotSignalSubscribe()
        {
            var fake = new Fake { Value = 1 };
            var observable = fake.ObservePropertyChanged();
            using (observable.Subscribe(this.changes.Add))
            {
                Assert.AreEqual(0, this.changes.Count);
            }
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase("Value")]
        public void ReactsOnStringEmptyOrNull(string prop)
        {
            var fake = new Fake { Value = 1 };
            fake.ObservePropertyChanged()
                .Subscribe(this.changes.Add);
            Assert.AreEqual(0, this.changes.Count);
            fake.OnPropertyChanged(prop); // This means all properties changed according to wpf convention
            Assert.AreEqual(1, this.changes.Count);
        }

        [Test]
        public void ReactsOnEvent()
        {
            var fake = new Fake { Value = 1 };
            fake.ObservePropertyChanged()
                .Subscribe(this.changes.Add);
            Assert.AreEqual(0, this.changes.Count);
            fake.OnPropertyChanged("SomeProp");
            Assert.AreEqual(1, this.changes.Count);
            AssertRx.AreEqual(fake, "SomeProp", this.changes.Last());
        }

        [Test]
        public void ReactsOnEventDerived()
        {
            var fake = new DerivedFake { Value = 1 };
            fake.ObservePropertyChanged()
                .Subscribe(this.changes.Add);
            Assert.AreEqual(0, this.changes.Count);
            fake.OnPropertyChanged("SomeProp");
            Assert.AreEqual(1, this.changes.Count);
            AssertRx.AreEqual(fake, "SomeProp", this.changes.Last());
        }

        [Test]
        public void ReactsValue()
        {
            var fake = new Fake { Value = 1 };
            fake.ObservePropertyChanged()
                .Subscribe(this.changes.Add);
            Assert.AreEqual(0, this.changes.Count);
            fake.Value++;
            Assert.AreEqual(1, this.changes.Count);
            AssertRx.AreEqual(fake, "Value", this.changes.Last());
        }

        [Test]
        public void ReactsTwoInstancesValue()
        {
            var fake1 = new Fake { Value = 1 };
            fake1.ObservePropertyChanged()
                .Subscribe(this.changes.Add);
            var fake2 = new Fake { Value = 1 };
            fake2.ObservePropertyChanged()
                .Subscribe(this.changes.Add);
            Assert.AreEqual(0, this.changes.Count);

            fake1.Value++;
            Assert.AreEqual(1, this.changes.Count);
            AssertRx.AreEqual(fake1, "Value", this.changes.Last());

            fake2.Value++;
            Assert.AreEqual(2, this.changes.Count);
            AssertRx.AreEqual(fake2, "Value", this.changes.Last());
        }

        [Test]
        public void ReactsNullable()
        {
            var fake = new Fake { IsTrueOrNull = null };
            var observable = fake.ObservePropertyChanged();
            using (observable.Subscribe(this.changes.Add))
            {
                Assert.AreEqual(0, this.changes.Count);

                fake.IsTrueOrNull = true;
                Assert.AreEqual(1, this.changes.Count);
                AssertRx.AreEqual(fake, "IsTrueOrNull", this.changes.Last());

                fake.IsTrueOrNull = null;
                Assert.AreEqual(2, this.changes.Count);
                AssertRx.AreEqual(fake, "IsTrueOrNull", this.changes.Last());
            }

            Assert.AreEqual(2, this.changes.Count);
            AssertRx.AreEqual(fake, "IsTrueOrNull", this.changes.Last());
        }

        [Test]
        public void StopsListeningOnDispose()
        {
            var fake = new Fake { IsTrue = true };
            var observable = fake.ObservePropertyChanged();
            using (observable.Subscribe(this.changes.Add))
            {
                fake.IsTrue = !fake.IsTrue;
                Assert.AreEqual(1, this.changes.Count);
            }

            fake.IsTrue = !fake.IsTrue;
            Assert.AreEqual(1, this.changes.Count);
        }

        [Test]
        public void MemoryLeakDisposeTest()
        {
            var fake = new Fake();
            var wr = new WeakReference(fake);
            var observable = fake.ObservePropertyChanged();
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
            var observable = fake.ObservePropertyChanged();
#pragma warning disable GU0030 // Use using.
            var subscription = observable.Subscribe();
#pragma warning restore GU0030 // Use using.
            GC.KeepAlive(observable);
            GC.KeepAlive(subscription);

            fake = null;
            GC.Collect();

            Assert.IsFalse(wr.IsAlive);
        }
    }
}
