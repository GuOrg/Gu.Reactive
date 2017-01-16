namespace Gu.Reactive.Tests.Conditions
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reactive.Linq;

    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    public class AbstractConditionTests
    {
        [Test]
        public void IsSatisfied()
        {
            var fake = new Fake { IsTrueOrNull = false };
            using (var condition = new FakeCondition(fake))
            {
                Assert.AreEqual(false, condition.IsSatisfied);
                fake.IsTrueOrNull = true;
                Assert.AreEqual(true, condition.IsSatisfied);
            }
        }

        [Test]
        public void Notifies()
        {
            var fake = new Fake { IsTrueOrNull = false };
            using (var condition = new FakeCondition(fake))
            {
                var argses = new List<PropertyChangedEventArgs>();
                condition.PropertyChanged += (sender, args) => argses.Add(args);
                fake.IsTrueOrNull = true;
                Assert.AreEqual(1, argses.Count);
            }
        }

        [Test]
        public void History()
        {
            var fake = new Fake { IsTrueOrNull = false };
            using (var condition = new FakeCondition(fake))
            {
                fake.IsTrueOrNull = true;
                CollectionAssert.AreEqual(new bool?[] { null, true }, condition.History.Select(x => x.State));
            }
        }

        [Test]
        public void Name()
        {
            using (var condition = new AbstractConditionImpl(Observable.Empty<object>()) { Name = "Name" })
            {
                using (var negated = condition.Negate())
                {
                    Assert.AreEqual("Name", condition.Name);
                    Assert.AreEqual("Not_Name", negated.Name);
                }
            }
        }

        private class FakeCondition : AbstractCondition
        {
            private readonly Fake fake;

            public FakeCondition(Fake fake)
                : base(fake.ObservePropertyChanged(x => x.IsTrueOrNull))
            {
                this.fake = fake;
            }

            protected override bool? Criteria()
            {
                return this.fake?.IsTrueOrNull;
            }
        }

        private class AbstractConditionImpl : AbstractCondition
        {
            public AbstractConditionImpl(IObservable<object> observable)
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