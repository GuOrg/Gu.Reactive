namespace Gu.Reactive.Tests.Conditions
{
    using System.Collections.Generic;
    using System.ComponentModel;

    using NUnit.Framework;

    public class NegatedConditionTests
    {
        [Test]
        public void Negate()
        {
            var fake = new FakeInpc { Prop1 = false };
            var condition = new Condition(fake.ToObservable(x => x.Prop1, false), () => fake.Prop1);
            var negatedCondition = condition.Negate();
            Assert.AreEqual("Not_" + condition.Name, negatedCondition.Name);

            Assert.AreEqual(false, condition.IsSatisfied);
            Assert.AreEqual(true, negatedCondition.IsSatisfied);
            var argses = new List<PropertyChangedEventArgs>();
            var negArgses = new List<PropertyChangedEventArgs>();

            condition.PropertyChanged += (_, e) => argses.Add(e);
            negatedCondition.PropertyChanged += (_, e) => negArgses.Add(e);
            fake.Prop1 = true;
            Assert.AreEqual(true, condition.IsSatisfied);
            Assert.IsTrue(argses.Count == 1);

            Assert.AreEqual(false, negatedCondition.IsSatisfied);
            Assert.IsTrue(negArgses.Count == 1);
        }
    }
}