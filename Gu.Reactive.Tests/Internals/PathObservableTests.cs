namespace Gu.Reactive.Tests.Internals
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reactive;

    using Gu.Reactive.PropertyPathStuff;
    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    /// <summary>
    /// Dunno if it is nice to test internals like this, was used for development.
    /// </summary>
    public class PathObservableTests
    {
        private List<EventPattern<PropertyChangedEventArgs>> _changes;

        [SetUp]
        public void SetUp()
        {
            _changes = new List<EventPattern<PropertyChangedEventArgs>>();
        }

        [Test(Description = "All parts of the property path must be INotifyPropertyChanged")]
        public void ThrowsIfNotNotifyingPath()
        {
            var exception = Assert.Throws<ArgumentException>(() => new PropertyPathObservable<Fake, int>(new Fake(), x => x.Name.Length));
            Console.WriteLine(exception.Message);
        }

        [Test(Description = "All parts of the property path must be class")]
        public void ThrowsIfStructInPath()
        {
            var exception = Assert.Throws<ArgumentException>(() => new PropertyPathObservable<Fake, string>(new Fake(), x => x.StructLevel.Name));
            Console.WriteLine(exception.Message);
        }

        [Test]
        public void SubscribeToExistsingChangeLastValueInPath()
        {
            var fake = new Fake { Next = new Level() };
            var observable = new PropertyPathObservable<Fake, bool>(fake, x => x.Next.IsTrue);
            observable.Subscribe(_changes.Add);
            fake.Next.IsTrue = !fake.Next.IsTrue;
            Assert.AreEqual(1, _changes.Count);
        }

        [Test]
        public void TwoLevelsValueType()
        {
            var fake = new Fake();
            var observable = new PropertyPathObservable<Fake, bool>(fake, x => x.Next.IsTrue);
            observable.Subscribe(_changes.Add);
            fake.Next = new Level();
            Assert.AreEqual(1, _changes.Count);
            fake.Next.IsTrue = !fake.Next.IsTrue;
            Assert.AreEqual(2, _changes.Count);
            fake.Next = null;
            Assert.AreEqual(3, _changes.Count);
        }

        [Test]
        public void TwoLevelsExisting()
        {
            var fake = new Fake { Next = new Level { Next = new Level() } };
            var observable = new PropertyPathObservable<Fake, Level>(fake, x => x.Next.Next);
            observable.Subscribe(_changes.Add);
            fake.Next.Next = new Level();
            Assert.AreEqual(1, _changes.Count);
            fake.Next.Next = null;
            Assert.AreEqual(2, _changes.Count);
            fake.Next = null;
            Assert.AreEqual(2, _changes.Count);
        }

        [Test]
        public void TwoLevelsReferenceType()
        {
            var fake = new Fake();
            var observable = new PropertyPathObservable<Fake, Level>(fake, x => x.Next.Next);
            observable.Subscribe(_changes.Add);
            fake.Next = new Level();
            Assert.AreEqual(0, _changes.Count);
            fake.Next.Next = new Level();
            Assert.AreEqual(1, _changes.Count);
            fake.Next.Next = null;
            Assert.AreEqual(2, _changes.Count);
            fake.Next = null;
            Assert.AreEqual(2, _changes.Count);
        }

        [Test]
        public void Unsubscribes()
        {
            var fake = new Fake();
            var observable = new PropertyPathObservable<Fake, bool>(fake, x => x.Next.IsTrue);
            observable.Subscribe(_changes.Add);
            fake.Next = new Level
                        {
                            Next = new Level()
                        };

            Assert.AreEqual(1, _changes.Count);
            var temp = fake.Next;
            fake.Next = null;
            Assert.AreEqual(2, _changes.Count);
            temp.IsTrue = !temp.IsTrue;
            Assert.AreEqual(2, _changes.Count);
        }

        [Test]
        public void ThreeLevels()
        {
            var fake = new Fake();
            var observable = new PropertyPathObservable<Fake, bool>(fake, x => x.Next.Next.IsTrue);
            observable.Subscribe(_changes.Add);
            fake.Next = new Level();
            Assert.AreEqual(0, _changes.Count);
            fake.Next.Next = new Level
                             {
                                 Next = new Level()
                             };
            Assert.AreEqual(1, _changes.Count);
            fake.Next.Next = null;
            Assert.AreEqual(2, _changes.Count);
            fake.Next = null;
            Assert.AreEqual(2, _changes.Count);
            fake.Next = new Level
                        {
                            Next = new Level
                                   {
                                       IsTrue = true
                                   }
                        };
            Assert.AreEqual(3, _changes.Count);
            fake.Next.Next.IsTrue = false;
            Assert.AreEqual(4, _changes.Count);
            fake.Next.Next = null;
            Assert.AreEqual(5, _changes.Count);
            fake.Next = null;
            Assert.AreEqual(5, _changes.Count);
        }

        [Test]
        public void MemoryLeakTest()
        {
            var fake = new Fake();
            var level1 = new Level();
            var wr = new WeakReference(level1);
            Assert.IsTrue(wr.IsAlive);
            var subscription = fake.ObservePropertyChanged(x => x.Next.IsTrue).Subscribe();
            fake.Next = level1;
            fake.Next = null;
            level1 = null;
            subscription.Dispose();
            GC.Collect();
            Assert.IsFalse(wr.IsAlive);
        }
    }
}