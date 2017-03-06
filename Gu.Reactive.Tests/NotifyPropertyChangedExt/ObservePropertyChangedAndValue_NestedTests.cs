// ReSharper disable All
namespace Gu.Reactive.Tests.NotifyPropertyChangedExt
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive;

    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    // ReSharper disable once InconsistentNaming
    public class ObservePropertyChangedAndValue_NestedTests
    {
        [TestCase(true)]
        [TestCase(false)]
        public void ThrowsOnStructInPath(bool signalIntital)
        {
            var fake = new Fake();
            var exception = Assert.Throws<ArgumentException>(() => fake.ObservePropertyChangedWithValue(x => x.StructLevel.Name, signalIntital));
            var expected = "Error found in x => x.StructLevel.Name\r\n" +
                           "Property path cannot have structs in it. Copy by value will make subscribing error prone. Also mutable struct much?\r\n" +
                           "The type StructLevel is a value type not so StructLevel.Name will not notify when it changes.\r\n" +
                           "The path is: x => x.StructLevel.Name\r\n\r\n" +
                           "Parameter name: property";
            Assert.AreEqual(expected, exception.Message);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ThrowsOnNotNotifyingnPath(bool signalIntital)
        {
            var fake = new Fake();
            var exception = Assert.Throws<ArgumentException>(() => fake.ObservePropertyChangedWithValue(x => x.Name.Length, signalIntital));
            var expected = "Error found in x => x.Name.Length\r\n" +
                           "All levels in the path must implement INotifyPropertyChanged.\r\n" +
                           "The type string does not so Name.Length will not notify when it changes.\r\n" +
                           "The path is: x => x.Name.Length\r\n\r\n" +
                           "Parameter name: property";
            Assert.AreEqual(expected, exception.Message);
        }

        [Test]
        [Explicit("Implement")]
        public void TypedEventArgsTest()
        {
        }

        [TestCase("")]
        [TestCase(null)]
        public void DoesNotReactOnStringEmptyOrNullFromRootWithoutValue(string prop)
        {
            var changes = new List<EventPattern<PropertyChangedAndValueEventArgs<string>>>();
            var fake = new Fake();
            using (fake.ObservePropertyChangedWithValue(x => x.Next.Name, false)
                       .Subscribe(changes.Add))
            {
                Assert.AreEqual(0, changes.Count);
                fake.OnPropertyChanged(prop); // This means all properties changed according to wpf convention
                Assert.AreEqual(0, changes.Count);
            }
        }

        [TestCase("")]
        [TestCase(null)]
        public void ReactsOnStringEmptyOrNullFromRootWithValue(string prop)
        {
            var changes = new List<EventPattern<PropertyChangedAndValueEventArgs<string>>>();
            var fake = new Fake { Next = new Level { Next = new Level { Name = "Johan" } } };
            using (fake.ObservePropertyChangedWithValue(x => x.Next.Next.Name, false)
                       .Subscribe(changes.Add))
            {
                Assert.AreEqual(0, changes.Count);
                fake.OnPropertyChanged(prop); // This means all properties changed according to wpf convention
                Assert.AreEqual(1, changes.Count);
                Assert.IsTrue(changes.Single().EventArgs.HasValue);
                Assert.AreEqual("Johan", changes.Single().EventArgs.Value);
                Assert.AreSame(fake.Next.Next, changes.Single().Sender);
                Assert.AreEqual(prop, changes.Last().EventArgs.PropertyName);
            }
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase("Name")]
        public void ReactsOnStringEmptyOrNullWithValue(string prop)
        {
            var changes = new List<EventPattern<PropertyChangedAndValueEventArgs<string>>>();
            var fake = new Fake { Next = new Level { Next = new Level { Name = "Johan" } } };
            using (fake.ObservePropertyChangedWithValue(x => x.Next.Next.Name, false)
                       .Subscribe(changes.Add))
            {
                Assert.AreEqual(0, changes.Count);
                fake.Next.Next.OnPropertyChanged(prop); // This means all properties changed according to wpf convention
                Assert.AreEqual(1, changes.Count);
                Assert.IsTrue(changes.Single().EventArgs.HasValue);
                Assert.AreEqual("Johan", changes.Single().EventArgs.Value);
                Assert.AreSame(fake.Next.Next, changes.Single().Sender);
                Assert.AreEqual(prop, changes.Last().EventArgs.PropertyName);
            }
        }

        [Test]
        public void TwoSubscriptionsOneObservable()
        {
            var changes1 = new List<EventPattern<PropertyChangedAndValueEventArgs<string>>>();
            var changes2 = new List<EventPattern<PropertyChangedAndValueEventArgs<string>>>();
            var fake = new Fake { Next = new Level { Name = string.Empty } };
            var observable = fake.ObservePropertyChangedWithValue(x => x.Next.Name, false);
            using (observable.Subscribe(changes1.Add))
            {
                using (observable.Subscribe(changes2.Add))
                {
                    Assert.AreEqual(0, changes1.Count);
                    Assert.AreEqual(0, changes2.Count);

                    fake.Next.Name += "a";
                    Assert.AreEqual(1, changes1.Count);
                    Assert.AreEqual(1, changes2.Count);

                    fake.Next.Name += "a";
                    Assert.AreEqual(2, changes1.Count);
                    Assert.AreEqual(2, changes2.Count);

                    fake.Next = null;
                    Assert.AreEqual(3, changes1.Count);
                    Assert.AreEqual(3, changes2.Count);
                }
            }
        }

        [TestCase(null)]
        [TestCase("abc")]
        public void SignalsInitialThreeLevelsWhenValueNull(string value)
        {
            var changes = new List<EventPattern<PropertyChangedAndValueEventArgs<string>>>();
            var fake = new Fake { Next = new Level { Name = value } };
            using (fake.ObservePropertyChangedWithValue(x => x.Next.Name, true)
                       .Subscribe(changes.Add))
            {
                Assert.AreEqual(1, changes.Count);
                Assert.AreSame(fake.Next, changes.Single().Sender);
                Assert.IsTrue(changes.Single().EventArgs.HasValue);
                Assert.AreEqual(value, changes.Single().EventArgs.Value);
                Assert.AreEqual(string.Empty, changes.Last().EventArgs.PropertyName);
            }
        }

        [Test]
        public void SignalsInitialThreeLevelsWhenNull()
        {
            var changes = new List<EventPattern<PropertyChangedAndValueEventArgs<string>>>();
            var fake = new Fake();
            using (fake.ObservePropertyChangedWithValue(x => x.Next.Name, true)
                       .Subscribe(changes.Add))
            {
                Assert.AreEqual(1, changes.Count);
                Assert.AreSame(null, changes.Single().Sender);
                Assert.IsFalse(changes.Single().EventArgs.HasValue);
                Assert.AreEqual(null, changes.Single().EventArgs.Value);
                Assert.AreEqual(string.Empty, changes.Last().EventArgs.PropertyName);
            }
        }

        [Test]
        public void CapturesInitialValue()
        {
            var changes = new List<EventPattern<PropertyChangedAndValueEventArgs<string>>>();
            var fake = new Fake { Next = new Level { Name = "Johan" } };
            using (fake.ObservePropertyChangedWithValue(x => x.Next.Name, true)
                       .Subscribe(changes.Add))
            {
                Assert.AreEqual(1, changes.Count);
                Assert.AreEqual("Johan", changes.Last().EventArgs.Value);
                Assert.AreEqual(string.Empty, changes.Last().EventArgs.PropertyName);
                Assert.AreSame(fake.Next, changes.Last().Sender);
                Assert.IsTrue(changes.Single().EventArgs.HasValue);
            }
        }

        [Test]
        public void DoesNotSignalOnSubscribe()
        {
            var changes = new List<EventPattern<PropertyChangedAndValueEventArgs<string>>>();
            var fake = new Fake { Next = new Level { Name = "Johan" } };
            using (fake.ObservePropertyChangedWithValue(x => x.Next.Name, false)
                       .Subscribe(changes.Add))
            {
                CollectionAssert.IsEmpty(changes);
            }
        }

        [Test]
        public void SignalsOnSourceChanges()
        {
            var changes = new List<EventPattern<PropertyChangedAndValueEventArgs<string>>>();
            var fake = new Fake();
            using (fake.ObservePropertyChangedWithValue(x => x.Next.Name, true)
                       .Subscribe(changes.Add))
            {
                Assert.AreEqual(string.Empty, changes.Single().EventArgs.PropertyName);
                Assert.AreEqual(null, changes.Single().EventArgs.Value);
                Assert.AreEqual(false, changes.Single().EventArgs.HasValue);
                Assert.AreEqual(null, changes.Single().Sender);

                fake.Next = new Level();
                Assert.AreEqual(2, changes.Count);
                Assert.AreEqual("Next", changes.Last().EventArgs.PropertyName);
                Assert.AreEqual(null, changes.Last().EventArgs.Value);
                Assert.AreEqual(true, changes.Last().EventArgs.HasValue);
                Assert.AreEqual(fake, changes.Last().Sender);

                fake.Next.Name = "Johan";
                Assert.AreEqual(2, changes.Count);
                Assert.AreEqual("Johan", changes.Last().EventArgs.Value);
                Assert.IsTrue(changes.Last().EventArgs.HasValue);
                Assert.AreEqual(fake.Next, changes.Last().Sender);
            }
        }

        [Test]
        public void MemoryLeakLevelNoDisposeTest()
        {
            var fake = new Fake { Next = new Level() };
            WeakReference wr = new WeakReference(fake.Next);
            Assert.IsTrue(wr.IsAlive);
            var observable = fake.ObservePropertyChangedWithValue(x => x.Next.Name, false);
#pragma warning disable GU0030 // Use using.
            var subscription = observable.Subscribe();
#pragma warning restore GU0030 // Use using.
            fake.Next = null;
            GC.Collect();
            Assert.IsFalse(wr.IsAlive);
            Assert.NotNull(fake);
            Assert.IsNotNull(observable); // touching it after GC.Collect
            Assert.IsNotNull(subscription); // touching it after GC.Collect
        }

        [Test]
        public void MemoryLeakLevelDisposeTest()
        {
            var fake = new Fake { Next = new Level() };
            WeakReference wr = new WeakReference(fake.Next);
            Assert.IsTrue(wr.IsAlive);
            var observable = fake.ObservePropertyChangedWithValue(x => x.Next.Name, false);
            using (var subscription = observable.Subscribe())
            {
            }

            fake.Next = null;
            GC.Collect();
            Assert.IsFalse(wr.IsAlive);
            Assert.NotNull(fake);
            Assert.IsNotNull(observable); // touching it after GC.Collect
        }
    }
}