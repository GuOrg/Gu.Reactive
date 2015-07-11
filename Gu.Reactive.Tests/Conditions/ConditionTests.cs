namespace Gu.Reactive.Tests.Conditions
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    public class ConditionTests
    {
        [Test]
        public void ConditionTest()
        {
            var fake = new Fake { IsTrueOrNull = false };
            var observable = fake.ObservePropertyChanged(x => x.IsTrueOrNull);
            var condition = new Condition(observable, () => fake.IsTrueOrNull);
            Assert.AreEqual(false, condition.IsSatisfied);
            fake.IsTrueOrNull = true;
            Assert.AreEqual(true, condition.IsSatisfied);
        }

        [Test]
        public void Notifies()
        {
            var fake = new Fake { IsTrueOrNull = false };
            var observable = fake.ObservePropertyChanged(x => x.IsTrueOrNull, false);
            var condition = new Condition(observable, () => fake.IsTrueOrNull);
            var argses = new List<PropertyChangedEventArgs>();
            condition.PropertyChanged += (sender, args) => argses.Add(args);
            fake.IsTrueOrNull = true;
            Assert.AreEqual(1, argses.Count);
        }

        [Test]
        public void History()
        {
            var fake = new Fake { IsTrueOrNull = false };
            var observable = fake.ObservePropertyChanged(x => x.IsTrueOrNull, false);
            var condition = new Condition(observable, () => fake.IsTrueOrNull);
            fake.IsTrueOrNull = true;
            CollectionAssert.AreEqual(new[] { false, true }, condition.History.Select(x => x.State));
        }

        [Test]
        public void MemoryLeakTest()
        {
            var dummy = new Fake();
            var wr = new WeakReference(dummy);
            Assert.IsTrue(wr.IsAlive);
            var condition = new Condition(dummy.ObservePropertyChanged(x => x.IsTrueOrNull, false), () => dummy.IsTrueOrNull);
            dummy = null;
            condition.Dispose();
            GC.Collect();
            Assert.IsFalse(wr.IsAlive);
        }
    }
}