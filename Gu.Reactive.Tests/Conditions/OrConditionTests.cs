namespace Gu.Reactive.Tests.Conditions
{
    using System;
    using System.Collections.Generic;

    using Gu.Reactive.Tests.Helpers;

    using Moq;
    using NUnit.Framework;

    public class OrConditionTests
    {
        private Mock<ICondition> _mock1;
        private Mock<ICondition> _mock2;
        private Mock<ICondition> _mock3;

        [SetUp]
        public void SetUp()
        {
            _mock1 = new Mock<ICondition>();
            _mock2 = new Mock<ICondition>();
            _mock3 = new Mock<ICondition>();
        }

        [TestCase(true, true, true, true)]
        [TestCase(true, true, null, true)]
        [TestCase(true, true, false, true)]
        [TestCase(true, false, null, true)]
        [TestCase(false, null, null, null)]
        [TestCase(null, null, null, null)]
        public void IsSatisfied(bool? first, bool? second, bool? third, bool? expected)
        {
            _mock1.SetupGet(x => x.IsSatisfied).Returns(first);
            _mock2.SetupGet(x => x.IsSatisfied).Returns(second);
            _mock3.SetupGet(x => x.IsSatisfied).Returns(third);
            var collection = new OrCondition(_mock1.Object, _mock2.Object, _mock3.Object);
            Assert.AreEqual(expected, collection.IsSatisfied);
        }

        [Test]
        public void Notifies()
        {
            var argses = new List<string>();
            var fake1 = new Fake { IsTrueOrNull = false };
            var fake2 = new Fake { IsTrueOrNull = false };
            var fake3 = new Fake { IsTrueOrNull = false };
            var condition1 = new Condition(fake1.ObservePropertyChanged(x => x.IsTrueOrNull), () => fake1.IsTrueOrNull);
            var condition2 = new Condition(fake2.ObservePropertyChanged(x => x.IsTrueOrNull), () => fake2.IsTrueOrNull);
            var condition3 = new Condition(fake3.ObservePropertyChanged(x => x.IsTrueOrNull), () => fake3.IsTrueOrNull);
            var collection = new OrCondition(condition1, condition2, condition3);
            collection.PropertyChanged += (sender, args) => argses.Add(args.PropertyName);
            Assert.AreEqual(false, collection.IsSatisfied);
            fake1.IsTrueOrNull = true;
            Assert.AreEqual(true, collection.IsSatisfied);
            Assert.AreEqual(1, argses.Count);

            fake2.IsTrueOrNull = true;
            Assert.AreEqual(true, collection.IsSatisfied);
            Assert.AreEqual(1, argses.Count);

            fake3.IsTrueOrNull = true;
            Assert.AreEqual(true, collection.IsSatisfied);
            Assert.AreEqual(1, argses.Count);

            fake1.IsTrueOrNull = false;
            Assert.AreEqual(true, collection.IsSatisfied);
            Assert.AreEqual(1, argses.Count);

            fake2.IsTrueOrNull = false;
            Assert.AreEqual(true, collection.IsSatisfied);
            Assert.AreEqual(1, argses.Count);

            fake3.IsTrueOrNull = false;
            Assert.AreEqual(false, collection.IsSatisfied);
            Assert.AreEqual(2, argses.Count);
        }

        [Test]
        public void ThrowsIfEmpty()
        {
            Assert.Throws<ArgumentException>(() => new OrCondition());
        }

        [Test]
        public void Prerequisites()
        {
            var collection = new OrCondition(_mock1.Object, _mock2.Object, _mock3.Object);
            CollectionAssert.AreEqual(new[] { _mock1.Object, _mock2.Object, _mock3.Object }, collection.Prerequisites);
        }
    }
}