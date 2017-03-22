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
        public void ConditionTest()
        {
            var fake = new Fake { IsTrueOrNull = false };
            var observable = fake.ObservePropertyChanged(x => x.IsTrueOrNull);
            using (var condition = new Condition(observable, () => fake.IsTrueOrNull))
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
            var observable = fake.ObservePropertyChanged(x => x.IsTrueOrNull, false);
            using (var condition = new Condition(observable, () => fake.IsTrueOrNull))
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
            var observable = fake.ObservePropertyChanged(x => x.IsTrueOrNull, false);
            using (var condition = new Condition(observable, () => fake.IsTrueOrNull))
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
            var dummy = new Fake();
            var wr = new WeakReference(dummy);
            Assert.IsTrue(wr.IsAlive);
            //// ReSharper disable once AccessToModifiedClosure
            using (new Condition(dummy.ObservePropertyChanged(x => x.IsTrueOrNull, false), () => dummy.IsTrueOrNull))
            {
            }

            dummy = null;
            GC.Collect();
            Assert.IsFalse(wr.IsAlive);
        }
    }
}