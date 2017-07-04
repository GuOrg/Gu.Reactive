namespace Gu.Reactive.Tests.Conditions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;

    using Gu.Reactive.Tests.Helpers;
    using Moq;
    using NUnit.Framework;

    public class AndConditionTests
    {
        [TestCase(true, true, true, true)]
        [TestCase(true, true, null, null)]
        [TestCase(null, true, null, null)]
        [TestCase(null, null, true, null)]
        [TestCase(true, true, false, false)]
        [TestCase(true, false, null, false)]
        [TestCase(null, false, true, false)]
        [TestCase(false, null, null, false)]
        [TestCase(null, false, null, false)]
        [TestCase(null, null, false, false)]
        [TestCase(null, null, null, null)]
        public void IsSatisfied(bool? first, bool? second, bool? third, bool? expected)
        {
            using (var condition = new AndCondition(
#pragma warning disable GU0033 // Don't ignore returnvalue of type IDisposable.
                Mock.Of<ICondition>(x => x.IsSatisfied == first),
                Mock.Of<ICondition>(x => x.IsSatisfied == second),
                Mock.Of<ICondition>(x => x.IsSatisfied == third)))
#pragma warning restore GU0033 // Don't ignore returnvalue of type IDisposable.
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
            using (var condition = new AndCondition(condition1, condition2, condition3))
            {
                using (condition.ObserveIsSatisfiedChanged()
                                 .Subscribe(_ => count++))
                {
                    Assert.AreEqual(false, condition.IsSatisfied);
                    fake1.IsTrue = !fake1.IsTrue;
                    Assert.AreEqual(false, condition.IsSatisfied);
                    Assert.AreEqual(0, count);

                    fake2.IsTrue = !fake2.IsTrue;
                    Assert.AreEqual(false, condition.IsSatisfied);
                    Assert.AreEqual(0, count);

                    fake3.IsTrue = !fake3.IsTrue;
                    Assert.AreEqual(true, condition.IsSatisfied);
                    Assert.AreEqual(1, count);

                    fake1.IsTrue = !fake1.IsTrue;
                    Assert.AreEqual(false, condition.IsSatisfied);
                    Assert.AreEqual(2, count);

                    fake2.IsTrue = !fake2.IsTrue;
                    Assert.AreEqual(false, condition.IsSatisfied);
                    Assert.AreEqual(2, count);

                    fake3.IsTrue = !fake3.IsTrue;
                    Assert.AreEqual(false, condition.IsSatisfied);
                    Assert.AreEqual(2, count);
                }
            }
        }

        [Test]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
        public void ThrowsIfPrerequisiteIsNull()
        {
#pragma warning disable GU0030 // Use using.
            var mock = Mock.Of<ICondition>();
            var exception = Assert.Throws<ArgumentNullException>(() => new AndCondition(mock, null));
            Assert.AreEqual("Value cannot be null.\r\nParameter name: condition2", exception.Message);

            exception = Assert.Throws<ArgumentNullException>(() => new AndCondition(null, mock));
            Assert.AreEqual("Value cannot be null.\r\nParameter name: condition1", exception.Message);
#pragma warning restore GU0030 // Use using.
        }

        [Test]
        public void Prerequisites2()
        {
#pragma warning disable GU0030 // Use using.
            var mock1 = Mock.Of<ICondition>();
            var mock2 = Mock.Of<ICondition>();
            using (var condition = new AndCondition(mock1, mock2))
            {
                CollectionAssert.AreEqual(new[] { mock1, mock2 }, condition.Prerequisites);
            }
#pragma warning restore GU0030 // Use using.
        }

        [Test]
        public void Prerequisites3()
        {
#pragma warning disable GU0030 // Use using.
            var mock1 = Mock.Of<ICondition>();
            var mock2 = Mock.Of<ICondition>();
            var mock3 = Mock.Of<ICondition>();
            using (var condition = new AndCondition(mock1, mock2, mock3))
            {
                CollectionAssert.AreEqual(new[] { mock1, mock2, mock3 }, condition.Prerequisites);
            }
#pragma warning restore GU0030 // Use using.
        }

        [Test]
        public void Prerequisites4()
        {
#pragma warning disable GU0030 // Use using.
            var mock1 = Mock.Of<ICondition>();
            var mock2 = Mock.Of<ICondition>();
            var mock3 = Mock.Of<ICondition>();
            var mock4 = Mock.Of<ICondition>();
            using (var condition = new AndCondition(mock1, mock2, mock3, mock4))
            {
                CollectionAssert.AreEqual(new[] { mock1, mock2, mock3, mock4 }, condition.Prerequisites);
            }
#pragma warning restore GU0030 // Use using.
        }

        [Test]
        public void DisposeDoesNotDisposeInjected()
        {
            var mock1 = new Mock<ICondition>(MockBehavior.Strict);
            mock1.SetupGet(x => x.IsSatisfied)
                .Returns(false);
            var mock2 = new Mock<ICondition>(MockBehavior.Strict);
            mock2.SetupGet(x => x.IsSatisfied)
                .Returns(false);
            using (new AndCondition(mock1.Object, mock2.Object))
            {
            }

            mock1.Verify(x => x.Dispose(), Times.Never);
        }

        [Test]
        public void DynamicList()
        {
            var conditions = new ObservableCollection<ICondition>();
            using (var condition = new AndCondition(conditions, leaveOpen: true))
            {
                var actuals = new List<bool?>();
                using (condition.ObserveIsSatisfiedChanged()
                                .Subscribe(c => actuals.Add(c.IsSatisfied)))
                {
                    var preRequisiteChanges = 0;
                    using (condition.ObservePropertyChangedSlim(x => x.Prerequisites, signalInitial: false)
                                    .Subscribe(x => preRequisiteChanges++))
                    {
                        Assert.AreEqual(0, preRequisiteChanges);
                        CollectionAssert.IsEmpty(condition.Prerequisites);
                        Assert.AreSame(condition.Prerequisites, condition.Prerequisites);
                        Assert.AreEqual(null, condition.IsSatisfied);

                        conditions.Add(Mock.Of<ICondition>(x => x.IsSatisfied == true));
                        Assert.AreEqual(true, condition.IsSatisfied);
                        CollectionAssert.AreEqual(new[] { true }, actuals);
                        Assert.AreEqual(1, preRequisiteChanges);
                        CollectionAssert.AreEqual(conditions, condition.Prerequisites);
                        Assert.AreSame(condition.Prerequisites, condition.Prerequisites);

                        Mock.Get(conditions[0]).SetupGet(x => x.IsSatisfied).Returns(false);
                        Mock.Get(conditions[0]).Raise(x => x.PropertyChanged += null, new PropertyChangedEventArgs("IsSatisfied"));
                        Assert.AreEqual(false, condition.IsSatisfied);
                        CollectionAssert.AreEqual(new[] { true, false }, actuals);
                        Assert.AreEqual(1, preRequisiteChanges);
                        CollectionAssert.AreEqual(conditions, condition.Prerequisites);
                        Assert.AreSame(condition.Prerequisites, condition.Prerequisites);
                    }
                }
            }
        }

        [Test]
        public void DisposeTwice()
        {
            var mock1 = new Mock<ICondition>(MockBehavior.Strict);
            mock1.SetupGet(x => x.IsSatisfied)
                .Returns(false);
            var mock2 = new Mock<ICondition>(MockBehavior.Strict);
            mock2.SetupGet(x => x.IsSatisfied)
                .Returns(false);
            using (var condition = new AndCondition(mock1.Object, mock2.Object))
            {
                condition.Dispose();
                condition.Dispose();
            }

            mock1.Verify(x => x.Dispose(), Times.Never);
        }
    }
}