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
            var condition = new ConditionExpandsAbstract(fake.ObservePropertyChanged(x => x.IsTrue)) { Name = "Name" };
            var negated = condition.Negate();
            Assert.AreEqual("Name", condition.Name);
            Assert.AreEqual("Not_Name", negated.Name);
        }

        private class ConditionExpandsAbstract : AbstractCondition
        {
            public ConditionExpandsAbstract(IObservable<object> observable)
                : base(observable)
            {
            }

            protected override bool? Criteria()
            {
                return false;
            }
        }
    }
}