namespace Gu.Reactive.Tests
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reactive;

    using NUnit.Framework;

    public class NestedObservableTests
    {
        [Test]
        public void ThrowsIfNotNotifyingTest()
        {
            var fake = new FakeInpc();
            var exception = Assert.Throws<ArgumentException>(() => new NestedObservable<FakeInpc, int>(fake, x => x.Name.Length));
            Console.WriteLine(exception.Message);
        }

        [Test]
        public void TwoLevelsValueType()
        {
            var args = new List<EventPattern<PropertyChangedEventArgs>>();
            var fake = new FakeInpc();
            var observable = new NestedObservable<FakeInpc, bool>(fake, x => x.Next.Value);
            observable.Subscribe(args.Add);
            fake.Next = new Level();
            Assert.AreEqual(1, args.Count);
            fake.Next.Value = !fake.Next.Value;
            Assert.AreEqual(2, args.Count);
            fake.Next = null;
            Assert.AreEqual(3, args.Count);
        }

        [Test]
        public void TwoLevelsExisting()
        {
            var args = new List<EventPattern<PropertyChangedEventArgs>>();
            var fake = new FakeInpc { Next = new Level { Next = new Level() } };
            var observable = new NestedObservable<FakeInpc, Level>(fake, x => x.Next.Next);
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
            var observable = new NestedObservable<FakeInpc, Level>(fake, x => x.Next.Next);
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
            var observable = new NestedObservable<FakeInpc, bool>(fake, x => x.Next.Value);
            observable.Subscribe(args.Add);
            fake.Next = new Level
                        {
                            Next = new Level()
                        };

            Assert.AreEqual(1, args.Count);
            var temp = fake.Next;
            fake.Next = null;
            Assert.AreEqual(2, args.Count);
            temp.Value = !temp.Value;
            Assert.AreEqual(2, args.Count);
        }

        [Test]
        public void ThreeLevels()
        {
            var args = new List<EventPattern<PropertyChangedEventArgs>>();
            var fake = new FakeInpc();
            var observable = new NestedObservable<FakeInpc, bool>(fake, x => x.Next.Next.Value);
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
                                       Value = true
                                   }
                        };
            Assert.AreEqual(3, args.Count);
            fake.Next.Next.Value = false;
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
            var subscription = fake.ToObservable(x => x.Next.Value).Subscribe();
            fake.Next = level1;
            fake.Next = null;
            level1 = null;
            subscription.Dispose();
            GC.Collect();
            Assert.IsFalse(wr.IsAlive);
        }
    }
}