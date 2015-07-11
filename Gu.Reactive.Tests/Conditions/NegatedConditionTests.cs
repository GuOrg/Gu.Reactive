namespace Gu.Reactive.Tests.Conditions
{
    using System.Collections.Generic;
    using System.ComponentModel;

    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    public class NegatedConditionTests
    {
        [TestCase(false, true)]
        [TestCase(true, false)]
        [TestCase(null, null)]
        public void NegateCondition(bool? value, bool? expected)
        {
            var fake = new Fake { IsTrueOrNull = value };
            var condition = new Condition(fake.ObservePropertyChanged(x => x.IsTrueOrNull), () => fake.IsTrueOrNull);
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
            var fake1 = new Fake { IsTrueOrNull = first };
            var condition1 = new Condition(fake1.ObservePropertyChanged(x => x.IsTrueOrNull), () => fake1.IsTrueOrNull);

            var fake2 = new Fake { IsTrueOrNull = other };
            var condition2 = new Condition(fake2.ObservePropertyChanged(x => x.IsTrueOrNull), () => fake2.IsTrueOrNull);

            var fake3 = new Fake { IsTrueOrNull = third };
            var condition3 = new Condition(fake3.ObservePropertyChanged(x => x.IsTrueOrNull), () => fake3.IsTrueOrNull);

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
            var fake1 = new Fake { IsTrueOrNull = first };
            var condition1 = new Condition(fake1.ObservePropertyChanged(x => x.IsTrueOrNull), () => fake1.IsTrueOrNull);

            var fake2 = new Fake { IsTrueOrNull = other };
            var condition2 = new Condition(fake2.ObservePropertyChanged(x => x.IsTrueOrNull), () => fake2.IsTrueOrNull);

            var fake3 = new Fake { IsTrueOrNull = third };
            var condition3 = new Condition(fake3.ObservePropertyChanged(x => x.IsTrueOrNull), () => fake3.IsTrueOrNull);

            var orCondition = new OrCondition(condition1, condition2, condition3);
            var negated = orCondition.Negate();

            Assert.AreEqual(expected, negated.IsSatisfied);
        }

        [Test]
        public void Notifies()
        {
            var argses = new List<PropertyChangedEventArgs>();
            var negArgses = new List<PropertyChangedEventArgs>();
            var fake = new Fake { IsTrueOrNull = false };
            var condition = new Condition(fake.ObservePropertyChanged(x => x.IsTrueOrNull), () => fake.IsTrueOrNull == true);
            var negatedCondition = condition.Negate();

            condition.PropertyChanged += (_, e) => argses.Add(e);
            negatedCondition.PropertyChanged += (_, e) => negArgses.Add(e);

            Assert.AreEqual(0, argses.Count);
            Assert.AreEqual(0, negArgses.Count);

            fake.IsTrueOrNull = true;
            Assert.AreEqual(1, argses.Count);
            Assert.AreEqual(1, negArgses.Count);

            fake.IsTrueOrNull = null;
            Assert.AreEqual(2, argses.Count);
            Assert.AreEqual(2, negArgses.Count);
        }

        [Test]
        public void Name()
        {
            var fake = new Fake { IsTrueOrNull = false };
            var condition = new Condition(fake.ObservePropertyChanged(x => x.IsTrueOrNull), () => fake.IsTrueOrNull);
            var negatedCondition = condition.Negate();
            Assert.AreEqual("Not_" + condition.Name, negatedCondition.Name);
        }

        [Test]
        public void NegateTwiceReturnsOriginal()
        {
            var fake = new Fake { IsTrueOrNull = false };
            var condition = new Condition(fake.ObservePropertyChanged(x => x.IsTrueOrNull), () => fake.IsTrueOrNull);
            var negatedCondition = condition.Negate();
            var negatedTwice = negatedCondition.Negate();
            Assert.AreSame(condition, negatedTwice);
        }
    }
}