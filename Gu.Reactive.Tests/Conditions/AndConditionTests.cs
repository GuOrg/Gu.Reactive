namespace Gu.Reactive.Tests.Conditions
{
    using System;
    using Gu.Reactive.Tests.Helpers;
    using Moq;
    using NUnit.Framework;

    public class AndConditionTests
    {
        [TestCase(true, true, true, true)]
        [TestCase(true, true, null, null)]
        [TestCase(true, true, false, false)]
        [TestCase(true, false, null, false)]
        [TestCase(false, null, null, false)]
        [TestCase(null, null, null, null)]
        public void IsSatisfied(bool? first, bool? second, bool? third, bool? expected)
        {
            using (var condition = new AndCondition(
                Mock.Of<ICondition>(x => x.IsSatisfied == first),
                Mock.Of<ICondition>(x => x.IsSatisfied == second),
                Mock.Of<ICondition>(x => x.IsSatisfied == third)))
            {
                Assert.AreEqual(expected, condition.IsSatisfied);
            }
        }

        [Test]
        public void Notifies()
        {
            var count = 0;
            var fake1 = new Fake { IsTrue = false };
            var fake2 = new Fake { IsTrue = false };
            var fake3 = new Fake { IsTrue = false };
            using (var condition1 = new Condition(fake1.ObservePropertyChanged(x => x.IsTrue), () => fake1.IsTrue))
            using (var condition2 = new Condition(fake2.ObservePropertyChanged(x => x.IsTrue), () => fake2.IsTrue))
            using (var condition3 = new Condition(fake3.ObservePropertyChanged(x => x.IsTrue), () => fake3.IsTrue))
            using (var collection = new AndCondition(condition1, condition2, condition3))
            {
                using (collection.ObserveIsSatisfiedChanged()
                                 .Subscribe(_ => count++))
                {
                    Assert.AreEqual(false, collection.IsSatisfied);
                    fake1.IsTrue = !fake1.IsTrue;
                    Assert.AreEqual(false, collection.IsSatisfied);
                    Assert.AreEqual(0, count);

                    fake2.IsTrue = !fake2.IsTrue;
                    Assert.AreEqual(false, collection.IsSatisfied);
                    Assert.AreEqual(0, count);

                    fake3.IsTrue = !fake3.IsTrue;
                    Assert.AreEqual(true, collection.IsSatisfied);
                    Assert.AreEqual(1, count);

                    fake1.IsTrue = !fake1.IsTrue;
                    Assert.AreEqual(false, collection.IsSatisfied);
                    Assert.AreEqual(2, count);

                    fake2.IsTrue = !fake2.IsTrue;
                    Assert.AreEqual(false, collection.IsSatisfied);
                    Assert.AreEqual(2, count);

                    fake3.IsTrue = !fake3.IsTrue;
                    Assert.AreEqual(false, collection.IsSatisfied);
                    Assert.AreEqual(2, count);
                }
            }
        }

        [Test]
        public void ThrowsIfEmpty()
        {
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            // ReSharper disable once ObjectCreationAsStatement
            var exception = Assert.Throws<ArgumentNullException>(() => new AndCondition());
            Assert.AreEqual("Value cannot be null.\r\nParameter name: prerequisites", exception.Message);
        }

        [Test]
        public void Prerequisites()
        {
#pragma warning disable GU0030 // Use using.
            var mock1 = Mock.Of<ICondition>();
            var mock2 = Mock.Of<ICondition>();
            var mock3 = Mock.Of<ICondition>();
            using (var collection = new AndCondition(mock1, mock2, mock3))
            {
                CollectionAssert.AreEqual(new[] { mock1, mock2, mock3 }, collection.Prerequisites);
            }
#pragma warning restore GU0030 // Use using.
        }
    }
}