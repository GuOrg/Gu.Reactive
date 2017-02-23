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

                fake.IsTrueOrNull = null;
                Assert.AreEqual(null, condition.IsSatisfied);

                fake.IsTrueOrNull = false;
                Assert.AreEqual(false, condition.IsSatisfied);
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

                fake.IsTrueOrNull = false;
                Assert.AreEqual(2, argses.Count);

                fake.IsTrueOrNull = null;
                Assert.AreEqual(3, argses.Count);
            }
        }

        [Test]
        public void History()
        {
            var fake = new Fake { IsTrueOrNull = false };
            using (var condition = new FakeCondition(fake))
            {
                CollectionAssert.AreEqual(new bool?[] { false }, condition.History.Select(x => x.State));

                fake.IsTrueOrNull = true;
                CollectionAssert.AreEqual(new bool?[] { false, true }, condition.History.Select(x => x.State));

                fake.IsTrueOrNull = null;
                CollectionAssert.AreEqual(new bool?[] { false, true, null }, condition.History.Select(x => x.State));
            }
        }

        [Test]
        public void HistoryNegated()
        {
            var fake = new Fake { IsTrueOrNull = false };
            using (var condition = new FakeCondition(fake))
            {
                using (var negated = condition.Negate())
                {
                    CollectionAssert.AreEqual(new bool?[] { false }, condition.History.Select(x => x.State));
                    CollectionAssert.AreEqual(new bool?[] { true }, negated.History.Select(x => x.State));

                    fake.IsTrueOrNull = true;
                    CollectionAssert.AreEqual(new bool?[] { false, true }, condition.History.Select(x => x.State));
                    CollectionAssert.AreEqual(new bool?[] { true, false }, negated.History.Select(x => x.State));

                    fake.IsTrueOrNull = null;
                    CollectionAssert.AreEqual(new bool?[] { false, true, null }, condition.History.Select(x => x.State));
                    CollectionAssert.AreEqual(new bool?[] { true, false, null }, negated.History.Select(x => x.State));
                }
            }
        }

        [Test]
        public void DefaultName()
        {
            using (var condition = new AbstractConditionImpl(Observable.Empty<object>()))
            {
                Assert.AreEqual("AbstractConditionImpl", condition.Name);
                using (var negated = condition.Negate())
                {
                    Assert.AreEqual("Not_AbstractConditionImpl", negated.Name);
                }
            }
        }

        [Test]
        public void ExplicitName()
        {
            using (var condition = new AbstractConditionImpl(Observable.Empty<object>()) { Name = "Explicit name" })
            {
                Assert.AreEqual("Explicit name", condition.Name);
                using (var negated = condition.Negate())
                {
                    Assert.AreEqual("Not_Explicit name", negated.Name);
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