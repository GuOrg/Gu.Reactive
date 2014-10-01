namespace Gu.Reactive.Tests.Conditions
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;

    using Moq;

    using NUnit.Framework;
    using NUnit.Framework.Constraints;

    public class NegatedConditionTests
    {
        [TestCase(false, true)]
        [TestCase(true, false)]
        [TestCase(null, null)]
        public void NegateCondition(bool? value, bool? expected)
        {
            var fake = new FakeInpc { Prop1 = value };
            var condition = new Condition(fake.ToObservable(x => x.Prop1), () => fake.Prop1);
            var negatedCondition = condition.Negate();
            Assert.AreEqual(expected, negatedCondition.IsSatisfied);
        }

        [TestCase(true, true, true, false)]
        [TestCase(true, false, false, true)]
        [TestCase(true, false, null, true)]
        [TestCase(true, null, null, null)]
        [TestCase(false, null, null, true)]
        [TestCase(false, false, false, true)]
        [TestCase(null, null, null, null)]
        public void NegateAndCondition(bool? first, bool? other, bool? third, bool? expected)
        {
            var fake1 = new FakeInpc { Prop1 = first };
            var condition1 = new Condition(fake1.ToObservable(x => x.Prop1), () => fake1.Prop1);

            var fake2 = new FakeInpc { Prop1 = other };
            var condition2 = new Condition(fake2.ToObservable(x => x.Prop1), () => fake2.Prop1);

            var fake3 = new FakeInpc { Prop1 = third };
            var condition3 = new Condition(fake3.ToObservable(x => x.Prop1), () => fake3.Prop1);

            var andCondition = new AndCondition(condition1, condition2, condition3);
            var negated = andCondition.Negate();

            Assert.AreEqual(expected, negated.IsSatisfied);
        }

        [TestCase(true, true, true, false)]
        [TestCase(true, false, false, false)]
        [TestCase(true, false, null, false)]
        [TestCase(true, null, null, false)]
        [TestCase(false, null, null, null)]
        [TestCase(false, false, false, true)]
        [TestCase(null, null, null, null)]
        public void NegateOrCondition(bool? first, bool? other, bool? third, bool? expected)
        {
            var fake1 = new FakeInpc { Prop1 = first };
            var condition1 = new Condition(fake1.ToObservable(x => x.Prop1), () => fake1.Prop1);

            var fake2 = new FakeInpc { Prop1 = other };
            var condition2 = new Condition(fake2.ToObservable(x => x.Prop1), () => fake2.Prop1);

            var fake3 = new FakeInpc { Prop1 = third };
            var condition3 = new Condition(fake3.ToObservable(x => x.Prop1), () => fake3.Prop1);

            var orCondition = new OrCondition(condition1, condition2, condition3);
            var negated = orCondition.Negate();

            Assert.AreEqual(expected, negated.IsSatisfied);
        }

        [Test]
        public void Notifies()
        {
            var argses = new List<PropertyChangedEventArgs>();
            var negArgses = new List<PropertyChangedEventArgs>();
            var fake = new FakeInpc { Prop1 = false };
            var condition = new Condition(fake.ToObservable(x => x.Prop1), () => fake.Prop1 == true);
            var negatedCondition = condition.Negate();

            condition.PropertyChanged += (_, e) => argses.Add(e);
            negatedCondition.PropertyChanged += (_, e) => negArgses.Add(e);

            Assert.AreEqual(0, argses.Count);
            Assert.AreEqual(0, negArgses.Count);

            fake.Prop1 = true;
            Assert.AreEqual(1, argses.Count);
            Assert.AreEqual(1, negArgses.Count);

            fake.Prop1 = null;
            Assert.AreEqual(2, argses.Count);
            Assert.AreEqual(2, negArgses.Count);
        }

        [Test]
        public void Name()
        {
            var fake = new FakeInpc { Prop1 = false };
            var condition = new Condition(fake.ToObservable(x => x.Prop1), () => fake.Prop1);
            var negatedCondition = condition.Negate();
            Assert.AreEqual("Not_" + condition.Name, negatedCondition.Name);
        }

        [Test]
        public void NegateTwiceReturnsOriginal()
        {
            var fake = new FakeInpc { Prop1 = false };
            var condition = new Condition(fake.ToObservable(x => x.Prop1), () => fake.Prop1);
            var negatedCondition = condition.Negate();
            var negatedTwice = negatedCondition.Negate();
            Assert.AreSame(condition, negatedTwice);
        }
    }
}