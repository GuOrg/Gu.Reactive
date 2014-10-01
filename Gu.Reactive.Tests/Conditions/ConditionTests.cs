namespace Gu.Reactive.Tests.Conditions
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    using NUnit.Framework;

    public class ConditionTests
    {
        [Test]
        public void ConditionTest()
        {
            var fake = new FakeInpc { Prop1 = false };
            var observable = fake.ToObservable(x => x.Prop1);
            var condition = new Condition(observable, () => fake.Prop1);
            Assert.AreEqual(false, condition.IsSatisfied);
            fake.Prop1 = true;
            Assert.AreEqual(true, condition.IsSatisfied);
        }

        [Test]
        public void Notifies()
        {
            var fake = new FakeInpc { Prop1 = false };
            var observable = fake.ToObservable(x => x.Prop1, false);
            var condition = new Condition(observable, () => fake.Prop1);
            var argses = new List<PropertyChangedEventArgs>();
            condition.PropertyChanged += (sender, args) => argses.Add(args);
            fake.Prop1 = true;
            Assert.AreEqual(1, argses.Count);
        }

        [Test]
        public void History()
        {
            var fake = new FakeInpc { Prop1 = false };
            var observable = fake.ToObservable(x => x.Prop1, false);
            var condition = new Condition(observable, () => fake.Prop1);
            fake.Prop1 = true;
            CollectionAssert.AreEqual(new[] { false, true }, condition.History.Select(x => x.State));
        }

        [Test]
        public void MemoryLeakTest()
        {
            var dummy = new FakeInpc();
            var wr = new WeakReference(dummy);
            Assert.IsTrue(wr.IsAlive);
            var condition = new Condition(dummy.ToObservable(x => x.Prop1, false), () => dummy.Prop1);
            dummy = null;
            condition.Dispose();
            GC.Collect();
            Assert.IsFalse(wr.IsAlive);
        }
    }
}