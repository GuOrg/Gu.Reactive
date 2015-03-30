namespace Gu.Reactive.Tests
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reactive;

    using NUnit.Framework;

    /// <summary>
    /// Dunno if it is nice to test internals like this, was used for development.
    /// </summary>
    public class PathObservableTests
    {
        [Test]
        public void ThrowsIfNotNotifyingTest()
        {
            var fake = new FakeInpc();
            var exception = Assert.Throws<ArgumentException>(() => new PathObservable<FakeInpc, int>(fake, x => x.Name.Length));
            Console.WriteLine(exception.Message);
        }

        [Test]
        public void TwoLevelsValueType()
        {
            var args = new List<EventPattern<PropertyChangedEventArgs>>();
            var fake = new FakeInpc();
            var observable = new PathObservable<FakeInpc, bool>(fake, x => x.Next.IsTrue);
            observable.Subscribe(args.Add);
            fake.Next = new Level();
            Assert.AreEqual(1, args.Count);
            fake.Next.IsTrue = !fake.Next.IsTrue;
            Assert.AreEqual(2, args.Count);
            fake.Next = null;
            Assert.AreEqual(3, args.Count);
        }

        [Test]
        public void TwoLevelsExisting()
        {
            var args = new List<EventPattern<PropertyChangedEventArgs>>();
            var fake = new FakeInpc { Next = new Level { Next = new Level() } };
            var observable = new PathObservable<FakeInpc, Level>(fake, x => x.Next.Next);
            observable.Subscribe(args.Add);
            fake.Next.Next = new Level();
            Assert.AreEqual(1, args.Count);
            fake.Next.Next = null;
            Assert.AreEqual(2, args.Count);
            fake.Next = null;
            Assert.AreEqual(2, args.Count);
        }

        [Test]
        public void TwoLevelsReferenceType()
        {
            var args = new List<EventPattern<PropertyChangedEventArgs>>();
            var fake = new FakeInpc();
            var observable = new PathObservable<FakeInpc, Level>(fake, x => x.Next.Next);
            observable.Subscribe(args.Add);
            fake.Next = new Level();
            Assert.AreEqual(0, args.Count);
            fake.Next.Next = new Level();
            Assert.AreEqual(1, args.Count);
            fake.Next.Next = null;
            Assert.AreEqual(2, args.Count);
            fake.Next = null;
            Assert.AreEqual(2, args.Count);
        }

        [Test]
        public void Unsubscribes()
        {
            var args = new List<EventPattern<PropertyChangedEventArgs>>();
            var fake = new FakeInpc();
            var observable = new PathObservable<FakeInpc, bool>(fake, x => x.Next.IsTrue);
            observable.Subscribe(args.Add);
            fake.Next = new Level
                        {
                            Next = new Level()
                        };

            Assert.AreEqual(1, args.Count);
            var temp = fake.Next;
            fake.Next = null;
            Assert.AreEqual(2, args.Count);
            temp.IsTrue = !temp.IsTrue;
            Assert.AreEqual(2, args.Count);
        }

        [Test]
        public void ThreeLevels()
        {
            var args = new List<EventPattern<PropertyChangedEventArgs>>();
            var fake = new FakeInpc();
            var observable = new PathObservable<FakeInpc, bool>(fake, x => x.Next.Next.IsTrue);
            observable.Subscribe(args.Add);
            fake.Next = new Level();
            Assert.AreEqual(0, args.Count);
            fake.Next.Next = new Level
                             {
                                 Next = new Level()
                             };
            Assert.AreEqual(1, args.Count);
            fake.Next.Next = null;
            Assert.AreEqual(2, args.Count);
            fake.Next = null;
            Assert.AreEqual(2, args.Count);
            fake.Next = new Level
                        {
                            Next = new Level
                                   {
                                       IsTrue = true
                                   }
                        };
            Assert.AreEqual(3, args.Count);
            fake.Next.Next.IsTrue = false;
            Assert.AreEqual(4, args.Count);
            fake.Next.Next = null;
            Assert.AreEqual(5, args.Count);
            fake.Next = null;
            Assert.AreEqual(5, args.Count);
        }

        [Test]
        public void MemoryLeakTest()
        {
            var fake = new FakeInpc();
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