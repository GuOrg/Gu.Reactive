﻿namespace Gu.Reactive.Tests.Conditions
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using Gu.Reactive.Tests.Helpers;
    using NUnit.Framework;

    public static class NegatedTests
    {
        [TestCase(false, true)]
        [TestCase(true, false)]
        [TestCase(null, null)]
        public static void NegateCondition(bool? value, bool? expected)
        {
            var fake = new Fake { IsTrueOrNull = value };
            using var condition = new Condition(fake.ObservePropertyChanged(x => x.IsTrueOrNull), () => fake.IsTrueOrNull);
            using var negatedCondition = new Negated<Condition>(condition);
            Assert.AreEqual(expected, negatedCondition.IsSatisfied);
        }

        [TestCase(true, true, true, false)]
        [TestCase(true, false, false, true)]
        [TestCase(true, false, null, true)]
        [TestCase(true, null, null, null)]
        [TestCase(false, null, null, true)]
        [TestCase(false, false, false, true)]
        [TestCase(null, null, null, null)]
        public static void NegateAndCondition(bool? first, bool? other, bool? third, bool? expected)
        {
            var fake1 = new Fake { IsTrueOrNull = first };
            using var condition1 = new Condition(fake1.ObservePropertyChanged(x => x.IsTrueOrNull), () => fake1.IsTrueOrNull);
            var fake2 = new Fake { IsTrueOrNull = other };
            using var condition2 = new Condition(fake2.ObservePropertyChanged(x => x.IsTrueOrNull), () => fake2.IsTrueOrNull);
            var fake3 = new Fake { IsTrueOrNull = third };
            using var condition3 = new Condition(fake3.ObservePropertyChanged(x => x.IsTrueOrNull), () => fake3.IsTrueOrNull);
            using var andCondition = new AndCondition(condition1, condition2, condition3);
            using var negated = new Negated<Condition>(andCondition);
            Assert.AreEqual(expected, negated.IsSatisfied);
        }

        [TestCase(true, true, true, false)]
        [TestCase(true, false, false, false)]
        [TestCase(true, false, null, false)]
        [TestCase(true, null, null, false)]
        [TestCase(false, null, null, null)]
        [TestCase(false, false, false, true)]
        [TestCase(null, null, null, null)]
        public static void NegateOrCondition(bool? first, bool? other, bool? third, bool? expected)
        {
            var fake1 = new Fake { IsTrueOrNull = first };
            using var condition1 = new Condition(fake1.ObservePropertyChanged(x => x.IsTrueOrNull), () => fake1.IsTrueOrNull);
            var fake2 = new Fake { IsTrueOrNull = other };
            using var condition2 = new Condition(fake2.ObservePropertyChanged(x => x.IsTrueOrNull), () => fake2.IsTrueOrNull);
            var fake3 = new Fake { IsTrueOrNull = third };
            using var condition3 = new Condition(fake3.ObservePropertyChanged(x => x.IsTrueOrNull), () => fake3.IsTrueOrNull);
            using var orCondition = new OrCondition(condition1, condition2, condition3);
            using var negated = new Negated<Condition>(orCondition);
            Assert.AreEqual(expected, negated.IsSatisfied);
        }

        [Test]
        public static void Notifies()
        {
            var argses = new List<PropertyChangedEventArgs>();
            var negArgses = new List<PropertyChangedEventArgs>();
            var fake = new Fake { IsTrueOrNull = false };
            using var condition = new Condition(fake.ObservePropertyChanged(x => x.IsTrueOrNull), () => fake.IsTrueOrNull == true);
            using var negatedCondition = new Negated<Condition>(condition);
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
        public static void Name()
        {
            var fake = new Fake { IsTrueOrNull = false };
            using var condition = new Condition(fake.ObservePropertyChanged(x => x.IsTrueOrNull), () => fake.IsTrueOrNull) { Name = "IsTrueOrNull" };
            using var negatedCondition = new Negated<Condition>(condition);
            Assert.AreEqual("Not_" + condition.Name, negatedCondition.Name);
        }

        [Test]
        public static void History()
        {
            var fake = new Fake { IsTrueOrNull = false };
            using var condition = new Condition(fake.ObservePropertyChanged(x => x.IsTrueOrNull), () => fake.IsTrueOrNull) { Name = "IsTrueOrNull" };
            using var negatedCondition = new Negated<Condition>(condition);
            var expected = new List<bool?> { true };
            CollectionAssert.AreEqual(expected, negatedCondition.History.Select(x => x.State));

            fake.IsTrueOrNull = true;
            expected.Add(false);
            CollectionAssert.AreEqual(expected, negatedCondition.History.Select(x => x.State));

            fake.IsTrueOrNull = null;
            expected.Add(null);
            CollectionAssert.AreEqual(expected, negatedCondition.History.Select(x => x.State));
        }

        [Test]
        public static void NegateTwiceReturnsOriginal()
        {
            var fake = new Fake { IsTrueOrNull = false };
            using var condition = new Condition(fake.ObservePropertyChanged(x => x.IsTrueOrNull), () => fake.IsTrueOrNull);
            using var negatedCondition = new Negated<Condition>(condition);
#pragma warning disable CS0618 // Type or member is obsolete
            using var negatedTwice = negatedCondition.Negate();
#pragma warning restore CS0618 // Type or member is obsolete
            Assert.AreSame(condition, negatedTwice);
        }
    }
}
