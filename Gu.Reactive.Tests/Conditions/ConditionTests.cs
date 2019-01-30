namespace Gu.Reactive.Tests.Conditions
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    using Gu.Reactive.Tests.Helpers;

    using Moq;

    using NUnit.Framework;

    public class ConditionTests
    {
        [Test]
        public void IsSatisfied()
        {
            var fake = new Fake { IsTrueOrNull = false };
            using (var condition = new Condition(
                fake.ObservePropertyChanged(x => x.IsTrueOrNull),
                () => fake.IsTrueOrNull))
            {
                Assert.AreEqual(false, condition.IsSatisfied);

                fake.IsTrueOrNull = true;
                Assert.AreEqual(true, condition.IsSatisfied);

                fake.IsTrueOrNull = false;
                Assert.AreEqual(false, condition.IsSatisfied);
            }
        }

        [Test]
        public void ConditionImplSimpleIsSatisfied()
        {
            var fake = new Fake { IsTrueOrNull = false };
            using (var condition = new ConditionImplSimple(fake))
            {
                Assert.AreEqual(false, condition.IsSatisfied);

                fake.IsTrueOrNull = true;
                Assert.AreEqual(true, condition.IsSatisfied);

                fake.IsTrueOrNull = false;
                Assert.AreEqual(false, condition.IsSatisfied);
            }
        }

        [Test]
        public void ConditionImplEqualityComparerIsSatisfied()
        {
            var fake = new Fake { IsTrueOrNull = false };
            using (var condition = new ConditionImplEqualityComparer(fake))
            {
                Assert.AreEqual(false, condition.IsSatisfied);

                fake.IsTrueOrNull = true;
                Assert.AreEqual(true, condition.IsSatisfied);

                fake.IsTrueOrNull = false;
                Assert.AreEqual(false, condition.IsSatisfied);
            }
        }

        [Test]
        public void ConditionImplFuncIsSatisfied()
        {
            var fake = new Fake { IsTrueOrNull = false };
            using (var condition = new ConditionImplFunc(fake))
            {
                Assert.AreEqual(false, condition.IsSatisfied);

                fake.IsTrueOrNull = true;
                Assert.AreEqual(true, condition.IsSatisfied);

                fake.IsTrueOrNull = false;
                Assert.AreEqual(false, condition.IsSatisfied);
            }
        }

        [Test]
        public void ConditionImplMaybeFuncIsSatisfied()
        {
            var fake = new Fake { IsTrueOrNull = false };
            using (var condition = new ConditionImplMaybeFunc(fake))
            {
                Assert.AreEqual(false, condition.IsSatisfied);

                fake.IsTrueOrNull = true;
                Assert.AreEqual(true, condition.IsSatisfied);

                fake.IsTrueOrNull = false;
                Assert.AreEqual(false, condition.IsSatisfied);
            }
        }

        [Test]
        public void Notifies()
        {
            var fake = new Fake { IsTrueOrNull = false };
            using (var condition = new Condition(
                fake.ObservePropertyChanged(x => x.IsTrueOrNull, signalInitial: false),
                () => fake.IsTrueOrNull))
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
            using (var condition = new Condition(
                fake.ObservePropertyChanged(x => x.IsTrueOrNull, signalInitial: false),
                () => fake.IsTrueOrNull))
            {
                CollectionAssert.AreEqual(new[] { false }, condition.History.Select(x => x.State));

                fake.IsTrueOrNull = true;
                CollectionAssert.AreEqual(new[] { false, true }, condition.History.Select(x => x.State));

                fake.IsTrueOrNull = null;
                CollectionAssert.AreEqual(new bool?[] { false, true, null }, condition.History.Select(x => x.State));
            }
        }

        [Test]
        public void DisposeTwice()
        {
            var observableMock = new Mock<IObservable<object>>(MockBehavior.Strict);
            var disposableMock = new Mock<IDisposable>(MockBehavior.Strict);
            observableMock.Setup(x => x.Subscribe(It.IsAny<IObserver<object>>()))
                          .Returns(disposableMock.Object);
            using (var condition = new Condition(observableMock.Object, () => true))
            {
                disposableMock.Setup(x => x.Dispose());
                condition.Dispose();
                disposableMock.Verify(x => x.Dispose(), Times.Once);
                condition.Dispose();
                disposableMock.Verify(x => x.Dispose(), Times.Once);
            }

            disposableMock.Verify(x => x.Dispose(), Times.Once);
        }

        [Test]
        public void MemoryLeakTest()
        {
#if DEBUG
            return; // debugger keeps things alive.
#endif
            var dummy = new Fake();
            var wr = new WeakReference(dummy);
            Assert.IsTrue(wr.IsAlive);
            //// ReSharper disable once AccessToModifiedClosure
            using (new Condition(dummy.ObservePropertyChanged(x => x.IsTrueOrNull, signalInitial: false), () => dummy.IsTrueOrNull))
            {
            }

            dummy = null;
            GC.Collect();
            Assert.IsFalse(wr.IsAlive);
        }

        private class ConditionImplSimple : Condition
        {
            public ConditionImplSimple(Fake fake)
                : base(For(fake, x => x.IsTrueOrNull, true))
            {
            }
        }

        private class ConditionImplEqualityComparer : Condition
        {
            public ConditionImplEqualityComparer(Fake fake)
                : base(For(fake, x => x.IsTrueOrNull, true, EqualityComparer<bool?>.Default))
            {
            }
        }

        private class ConditionImplFunc : Condition
        {
            public ConditionImplFunc(Fake fake)
                : base(For(fake, x => x.IsTrueOrNull, true, Nullable.Equals))
            {
            }
        }

        private class ConditionImplMaybeFunc : Condition
        {
            public ConditionImplMaybeFunc(Fake fake)
                : base(For(fake, x => x.IsTrueOrNull, true, (x, y) => Maybe.Equals(x, y, Nullable.Equals)))
            {
            }
        }
    }
}
