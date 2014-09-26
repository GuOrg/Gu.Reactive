namespace Gu.Reactive.Tests.Conditions
{
    using System.Collections.Generic;

    using Moq;

    using NUnit.Framework;

    public class OrConditionCollectionTests
    {
        private Mock<ICondition> _mock1;
        private Mock<ICondition> _mock2;
        private Mock<ICondition> _mock3;

        [SetUp]
        public void SetUp()
        {
            this._mock1 = new Mock<ICondition>();
            this._mock2 = new Mock<ICondition>();
            this._mock3 = new Mock<ICondition>();
        }

        [TestCase(true, true, true, true)]
        [TestCase(true, true, null, true)]
        [TestCase(true, true, false, true)]
        [TestCase(true, false, null, true)]
        [TestCase(false, null, null, false)]
        [TestCase(null, null, null, null)]
        public void IsSatisfiedIsTrueIfAnyChildIsTrue(bool? first, bool? second, bool? third, bool? expected)
        {
            this._mock1.SetupGet(x => x.IsSatisfied).Returns(first);
            this._mock2.SetupGet(x => x.IsSatisfied).Returns(second);
            this._mock3.SetupGet(x => x.IsSatisfied).Returns(third);
            var collection = new OrConditionCollection(this._mock1.Object, this._mock2.Object, this._mock3.Object);
            Assert.AreEqual(expected, collection.IsSatisfied);
        }

        [Test]
        public void NotifiesTest()
        {
            var argses = new List<string>();
            this._mock1.SetupGet(x => x.IsSatisfied).Returns(false);
            this._mock2.SetupGet(x => x.IsSatisfied).Returns(false);

            var collection = new OrConditionCollection(this._mock1.Object, this._mock2.Object);
            collection.PropertyChanged += (sender, args) => argses.Add(args.PropertyName);
            Assert.AreEqual(false, collection.IsSatisfied);
            this._mock1.SetAndRaisePropertyChanged(x => x.IsSatisfied, true);
            Assert.AreEqual(true, collection.IsSatisfied);
            Assert.AreEqual(1, argses.Count);
        }

        [Test]
        public void EmptyIsSatisfied()
        {
            var conditionCollection = new OrConditionCollection();
            Assert.AreEqual(null, conditionCollection.IsSatisfied);
        }
    }
}