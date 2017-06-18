namespace Gu.Reactive.Tests.Conditions
{
    using System;
    using System.Collections.Generic;
    using Gu.Reactive.Tests.Helpers;
    using NUnit.Framework;

    public class NullIsFalseTests
    {
        [TestCase(false, false)]
        [TestCase(true, true)]
        [TestCase(null, false)]
        public void Ctor(bool? value, bool? expected)
        {
            var fake = new Fake { IsTrueOrNull = value };
            using (var condition = new Condition(
                fake.ObservePropertyChanged(x => x.IsTrueOrNull),
                () => fake.IsTrueOrNull))
            {
                using (var nullIsFalse = new NullIsFalse<Condition>(condition))
                {
                    Assert.AreEqual(expected, nullIsFalse.IsSatisfied);
                }

                using (var nullIsFalse = condition.NullIsFalse())
                {
                    Assert.AreEqual(expected, nullIsFalse.IsSatisfied);
                }
            }
        }

        [Test]
        public void Notifies()
        {
            var fake = new Fake { IsTrueOrNull = null };
            using (var condition = new Condition(
                fake.ObservePropertyChanged(x => x.IsTrueOrNull),
                () => fake.IsTrueOrNull))
            {
                var actual = new List<string>();
                var expected = new List<string>();
                using (var nullIsFalse = new NullIsFalse<Condition>(condition))
                {
                    using (nullIsFalse.ObservePropertyChangedSlim()
                                      .Subscribe(x => actual.Add(x.PropertyName)))
                    {
                        CollectionAssert.IsEmpty(actual);
                        Assert.AreEqual(false, nullIsFalse.IsSatisfied);

                        fake.IsTrueOrNull = false;
                        CollectionAssert.IsEmpty(actual);
                        Assert.AreEqual(false, nullIsFalse.IsSatisfied);

                        fake.IsTrueOrNull = true;
                        expected.Add("IsSatisfied");
                        CollectionAssert.AreEqual(expected, actual);
                        Assert.AreEqual(true, nullIsFalse.IsSatisfied);

                        fake.IsTrueOrNull = null;
                        expected.Add("IsSatisfied");
                        CollectionAssert.AreEqual(expected, actual);
                        Assert.AreEqual(false, nullIsFalse.IsSatisfied);

                        fake.IsTrueOrNull = true;
                        expected.Add("IsSatisfied");
                        CollectionAssert.AreEqual(expected, actual);
                        Assert.AreEqual(true, nullIsFalse.IsSatisfied);

                        fake.IsTrueOrNull = false;
                        expected.Add("IsSatisfied");
                        CollectionAssert.AreEqual(expected, actual);
                        Assert.AreEqual(false, nullIsFalse.IsSatisfied);
                    }
                }
            }
        }
    }
}