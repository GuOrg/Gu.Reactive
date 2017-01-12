namespace Gu.Reactive.Tests.Conditions
{
    using System;

    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    public class AbstractConditionTests
    {
        [Test]
        public void Naming()
        {
            var fake = new Fake { IsTrue = false };
            var condition = new ConditionExpandsAbstract(
                fake.ObservePropertyChanged(x => x.IsTrue));
            var notCondition = new ConditionExpandsAbstract(
                fake.ObservePropertyChanged(x => x.IsTrue)).Negate();

            Assert.AreEqual("TestNameCondition", condition.Name);
            Assert.AreEqual("Not_TestNameCondition", notCondition.Name);
        }

        private class ConditionExpandsAbstract : AbstractCondition
        {
            public ConditionExpandsAbstract(IObservable<object> observable)
                : base(observable)
            {
                Name = "TestNameCondition";
            }

            protected override bool? Criteria()
            {
                return false;
            }
        }
    }
}