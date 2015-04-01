namespace Gu.Reactive.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive;

    using NUnit.Framework;

    public class NotifyPropertyChangedExt_ObservePropertyChangedAndValue_Nested
    {
        private List<EventPattern<PropertyChangedAndValueEventArgs<string>>> _changes;

        [SetUp]
        public void SetUp()
        {
            _changes = new List<EventPattern<PropertyChangedAndValueEventArgs<string>>>();
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase("Name")]
        public void ReactsOnStringEmptyOrNullWithoutValue(string prop)
        {
            var fake = new FakeInpc();
            fake.ObservePropertyChangedWithValue(x => x.Next.Name, false)
                .Subscribe(_changes.Add);
            Assert.AreEqual(0, _changes.Count);
            fake.OnPropertyChanged(prop); // This means all properties changed according to wpf convention
            Assert.AreEqual(1, _changes.Count);
            Assert.AreEqual(null, _changes.Single().EventArgs.Value);
            Assert.IsTrue(_changes.Single().EventArgs.IsDefaultValue);
            Assert.AreEqual(prop,_changes.Last().EventArgs.PropertyName);
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase("Name")]
        public void ReactsOnStringEmptyOrNullWithValue(string prop)
        {
            var fake = new FakeInpc{Next = new Level{Name = "Johan"}};
            fake.ObservePropertyChangedWithValue(x => x.Next.Name, false)
                .Subscribe(_changes.Add);
            Assert.AreEqual(0, _changes.Count);
            fake.OnPropertyChanged(prop); // This means all properties changed according to wpf convention
            Assert.AreEqual(1, _changes.Count);
            Assert.AreEqual("Johan", _changes.Single().EventArgs.Value);
            Assert.IsFalse(_changes.Single().EventArgs.IsDefaultValue);
            Assert.AreEqual(prop, _changes.Last().EventArgs.PropertyName);
        }

        [Test]
        public void SignalsInitialNull()
        {
            var fake = new FakeInpc();
            fake.ObservePropertyChangedWithValue(x => x.Next.Name, true)
                .Subscribe(_changes.Add);
            Assert.AreEqual(1, _changes.Count);
            Assert.AreEqual(null, _changes.Single().EventArgs.Value);
            Assert.AreSame(null, _changes.Single().Sender);
            Assert.IsFalse(_changes.Single().EventArgs.IsDefaultValue);
            Assert.AreEqual("Name", _changes.Last().EventArgs.PropertyName);
        }

        [Test]
        public void CapturesInitialValue()
        {
            var fake = new FakeInpc { Next = new Level { Name = "Johan" } };
            fake.ObservePropertyChangedWithValue(x => x.Next.Name, true)
                .Subscribe(_changes.Add);
            Assert.AreEqual(1, _changes.Count);
            Assert.AreEqual("Johan", _changes.Last().EventArgs.Value);
            Assert.AreEqual("Name", _changes.Last().EventArgs.PropertyName);
            Assert.AreSame(fake.Next, _changes.Last().Sender);
            Assert.IsTrue(_changes.Single().EventArgs.IsDefaultValue);
        }

        [Test]
        public void DoesNotSignalOnSubscribe()
        {
            var fake = new FakeInpc { Next = new Level { Name = "Johan" } };
            fake.ObservePropertyChangedWithValue(x => x.Next.Name, false)
                .Subscribe(_changes.Add);
            CollectionAssert.IsEmpty(_changes);
        }

        [Test]
        public void SignalsOnSourceChanges()
        {
            var fake = new FakeInpc();
            fake.ObservePropertyChangedWithValue(x => x.Next.Name, false)
                .Subscribe(_changes.Add);
            CollectionAssert.IsEmpty(_changes);
            fake.Next = new Level();
            CollectionAssert.IsEmpty(_changes);
            fake.Next.Name = "El Kurro";
            Assert.AreEqual("El Kurro", _changes.Single().EventArgs.Value);
            Assert.IsTrue(_changes.Single().EventArgs.IsDefaultValue);
        }

        [Test]
        public void MemoryLeakNoDisposeTest()
        {
            var fake = new FakeInpc();
            var wr = new WeakReference(fake);
            Assert.IsTrue(wr.IsAlive);
            var subscription = fake.ObservePropertyChangedWithValue(x => x.Next.Name, false)
                                   .Subscribe();
            fake = null;
            GC.Collect();
            Assert.IsFalse(wr.IsAlive);
            var s = subscription.ToString(); // touching it after GC.Collect for no optimizations
        }

        [Test]
        public void MemoryLeakDisposeTest()
        {
            var fake = new FakeInpc();
            var wr = new WeakReference(fake);
            Assert.IsTrue(wr.IsAlive);
            var subscription = fake.ObservePropertyChangedWithValue(x => x.Next.Name, false)
                                   .Subscribe();
            fake = null;
            subscription.Dispose();
            GC.Collect();
            Assert.IsFalse(wr.IsAlive);
            var s = subscription.ToString(); // touching it after GC.Collect for no optimizations
        }
    }
}