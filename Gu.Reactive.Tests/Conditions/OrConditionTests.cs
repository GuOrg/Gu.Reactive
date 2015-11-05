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
            var count = 0;
            var fake1 = new Fake { IsTrue = false };
            var fake2 = new Fake { IsTrue = false };
            var fake3 = new Fake { IsTrue = false };
            var condition1 = new Condition(fake1.ObservePropertyChanged(x => x.IsTrue), () => fake1.IsTrue);
            var condition2 = new Condition(fake2.ObservePropertyChanged(x => x.IsTrue), () => fake2.IsTrue);
            var condition3 = new Condition(fake3.ObservePropertyChanged(x => x.IsTrue), () => fake3.IsTrue);
            var collection = new OrCondition(condition1, condition2, condition3);
            collection.ObserveIsSatisfiedChanged()
                      .Subscribe(_ => count++);
            Assert.AreEqual(false, collection.IsSatisfied);
            fake1.IsTrue = !fake1.IsTrue;
            Assert.AreEqual(true, collection.IsSatisfied);
            Assert.AreEqual(1, count);

            fake2.IsTrue = !fake2.IsTrue;
            Assert.AreEqual(true, collection.IsSatisfied);
            Assert.AreEqual(1, count);

            fake3.IsTrue = !fake3.IsTrue;
            Assert.AreEqual(true, collection.IsSatisfied);
            Assert.AreEqual(1, count);

            fake1.IsTrue = !fake1.IsTrue;
            Assert.AreEqual(true, collection.IsSatisfied);
            Assert.AreEqual(1, count);

            fake2.IsTrue = !fake2.IsTrue;
            Assert.AreEqual(true, collection.IsSatisfied);
            Assert.AreEqual(1, count);

            fake3.IsTrue = !fake3.IsTrue;
            Assert.AreEqual(false, collection.IsSatisfied);
            Assert.AreEqual(2, count);
        }

        [Test]
        public void ThrowsIfEmpty()
        {
            Assert.Throws<ArgumentNullException>(() => new OrCondition());
        }

        [Test]
        public void Prerequisites()
        {
            var collection = new OrCondition(_mock1.Object, _mock2.Object, _mock3.Object);
            CollectionAssert.AreEqual(new[] { _mock1.Object, _mock2.Object, _mock3.Object }, collection.Prerequisites);
        }
    }
}