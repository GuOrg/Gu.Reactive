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
            var observable = fake.ToObservable(x => x.Prop1, false);
            Func<bool?> criteria = () => fake.Prop1;
            var condition = new Condition(observable, criteria);

            Assert.AreEqual(false, condition.IsSatisfied);
            var argses = new List<PropertyChangedEventArgs>();
            condition.PropertyChanged += (sender, args) => argses.Add(args);
            fake.Prop1 = true;
            Assert.AreEqual(true, condition.IsSatisfied);
            Assert.IsTrue(argses.Count == 1);
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