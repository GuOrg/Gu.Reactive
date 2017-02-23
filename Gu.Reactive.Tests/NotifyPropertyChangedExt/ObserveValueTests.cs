namespace Gu.Reactive.Tests.NotifyPropertyChangedExt
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Gu.Reactive.Tests.Helpers;
    using NUnit.Framework;

    public class ObserveValueTests
    {
        [Test]
        public void Simple()
        {
            var fake = new Fake();
            var values = new List<Maybe<int>>();
            fake.ObserveValue(x => x.Value, false)
                .Subscribe(values.Add);
            CollectionAssert.IsEmpty(values);

            fake.Value++;
            CollectionAssert.AreEqual(new[] { Maybe.Some(1) }, values);

            fake.Value++;
            CollectionAssert.AreEqual(new[] { Maybe.Some(1), Maybe.Some(2) }, values);

            fake.OnPropertyChanged("Value");
            CollectionAssert.AreEqual(new[] { Maybe.Some(1), Maybe.Some(2), Maybe.Some(2) }, values);
        }

        [TestCase(true, new[] { 1 })]
        [TestCase(false, new int[0])]
        public void SimpleSignalInitial(bool signalInitial, int[] start)
        {
            var expected = start.Select(Maybe.Some).ToList();
            var values = new List<Maybe<int>>();
            var fake = new Fake { Value = 1 };
            fake.ObserveValue(x => x.Value, signalInitial)
                .Subscribe(values.Add);
            CollectionAssert.AreEqual(expected, values);

            fake.Value++;
            expected.Add(Maybe.Some(fake.Value));
            CollectionAssert.AreEqual(expected, values);
        }

        [TestCase(true, new[] { 1 })]
        [TestCase(false, new int[0])]
        public void NestedSignalInitial(bool signalInitial, int[] start)
        {
            var expected = start.Select(Maybe.Some).ToList();
            var values = new List<Maybe<int>>();
            var fake = new Fake { Next = new Level { Value = 1 } };
            fake.ObserveValue(x => x.Next.Value, signalInitial)
                .Subscribe(values.Add);
            CollectionAssert.AreEqual(expected, values);

            fake.Next.Value++;
            expected.Add(Maybe.Some(fake.Next.Value));
            CollectionAssert.AreEqual(expected, values);

            fake.Next.OnPropertyChanged("Value");
            expected.Add(Maybe.Some(fake.Next.Value));
            CollectionAssert.AreEqual(expected, values);

            fake.Next.OnPropertyChanged("Next");
            CollectionAssert.AreEqual(expected, values);

            fake.Next = null;
            expected.Add(Maybe<int>.None);
            CollectionAssert.AreEqual(expected, values);
        }

        [Test]
        public void MemoryLeakSimpleDisposeTest()
        {
#if DEBUG
            Assert.Inconclusive("Debugger keeps things alive for the scope of the method.");
#endif
            var fake = new Fake();
            var wr = new WeakReference(fake);
            using (fake.ObserveValue(x => x.IsTrueOrNull).Subscribe())
            {
            }

            GC.Collect();
            Assert.IsFalse(wr.IsAlive);
        }

        [Test]
        public void MemoryLeakNestedDisposeTest()
        {
#if DEBUG
            Assert.Inconclusive("Debugger keeps things alive for the scope of the method.");
#endif
            var fake = new Fake();
            var wr = new WeakReference(fake);
            using (fake.ObserveValue(x => x.Next.Next.Value).Subscribe())
            {
            }

            GC.Collect();
            Assert.IsFalse(wr.IsAlive);
        }

        [Test]
        public void MemoryLeakSimpleNoDisposeTest()
        {
#if DEBUG
            Assert.Inconclusive("Debugger keeps things alive for the scope of the method.");
#endif
            var fake = new Fake();
            var wr = new WeakReference(fake);
            var observable = fake.ObserveValue(x => x.IsTrueOrNull);
#pragma warning disable GU0030 // Use using.
            //// ReSharper disable once UnusedVariable
            var subscribe = observable.Subscribe();
#pragma warning restore GU0030 // Use using.
            GC.Collect();
            Assert.IsFalse(wr.IsAlive);
        }

        [Test]
        public void MemoryLeakNestedNoDisposeTest()
        {
#if DEBUG
            Assert.Inconclusive("Debugger keeps things alive for the scope of the method.");
#endif
            var fake = new Fake();
            var wr = new WeakReference(fake);
            var observable = fake.ObserveValue(x => x.Next.Next.Value);
#pragma warning disable GU0030 // Use using.
            //// ReSharper disable once UnusedVariable
            var subscribe = observable.Subscribe();
#pragma warning restore GU0030 // Use using.
            GC.Collect();
            Assert.IsFalse(wr.IsAlive);
        }
    }
}