// ReSharper disable RedundantArgumentDefaultValue
// ReSharper disable NotResolvedInText
// ReSharper disable HeuristicUnreachableCode
// ReSharper disable ExplicitCallerInfoArgument
// ReSharper disable UnusedMember.Global
namespace Gu.Reactive.Tests.NotifyPropertyChangedExt
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;

    using Gu.Reactive.Tests.Helpers;
    using Moq;
    using NUnit.Framework;

    public static class ObserveValue
    {
        [Test]
        public static void Simple()
        {
            var source = new Fake();
            var actuals = new List<Maybe<int>>();
            using (source.ObserveValue(x => x.Value, signalInitial: false)
                         .Subscribe(actuals.Add))
            {
                var expecteds = new List<Maybe<int>>();
                CollectionAssert.AreEqual(expecteds, actuals);

                source.Value++;
                expecteds.Add(Maybe<int>.Some(1));
                CollectionAssert.AreEqual(expecteds, actuals);

                source.Value++;
                expecteds.Add(Maybe<int>.Some(2));
                CollectionAssert.AreEqual(expecteds, actuals);

                source.OnPropertyChanged("Value");
                CollectionAssert.AreEqual(expecteds, actuals);

                source.OnPropertyChanged(string.Empty);
                CollectionAssert.AreEqual(expecteds, actuals);

                source.OnPropertyChanged(null);
                CollectionAssert.AreEqual(expecteds, actuals);

                source.OnPropertyChanged("Next");
                CollectionAssert.AreEqual(expecteds, actuals);
            }
        }

        [Test]
        public static void FakeOfIntValueDefault()
        {
            var source = new Fake<int>();
            var actuals = new List<Maybe<int>>();
            using (source.ObserveValue(x => x.Value)
                         .Subscribe(actuals.Add))
            {
                var expecteds = new List<Maybe<int>> { Maybe.Some(0) };
                CollectionAssert.AreEqual(expecteds, actuals);

                source.Value++;
                expecteds.Add(Maybe.Some(1));
                CollectionAssert.AreEqual(expecteds, actuals);

                source.Value++;
                expecteds.Add(Maybe.Some(2));
                CollectionAssert.AreEqual(expecteds, actuals);

                source.OnPropertyChanged("Value");
                CollectionAssert.AreEqual(expecteds, actuals);
            }
        }

        [Test]
        public static void FakeOfIntValueSignalInitial()
        {
            var source = new Fake<int>();
            var actuals = new List<Maybe<int>>();
            using (source.ObserveValue(x => x.Value, signalInitial: true)
                         .Subscribe(actuals.Add))
            {
                var expecteds = new List<Maybe<int>> { Maybe.Some(0) };
                CollectionAssert.AreEqual(expecteds, actuals);

                source.Value++;
                expecteds.Add(Maybe.Some(1));
                CollectionAssert.AreEqual(expecteds, actuals);

                source.Value++;
                expecteds.Add(Maybe.Some(2));
                CollectionAssert.AreEqual(expecteds, actuals);

                source.OnPropertyChanged("Value");
                CollectionAssert.AreEqual(expecteds, actuals);
            }
        }

        [Test]
        public static void FakeOfIntValueNoInitial()
        {
            var source = new Fake<int>();
            var actuals = new List<Maybe<int>>();
            using (source.ObserveValue(x => x.Value, signalInitial: false)
                         .Subscribe(actuals.Add))
            {
                var expecteds = new List<Maybe<int>>();
                CollectionAssert.AreEqual(expecteds, actuals);

                source.Value++;
                expecteds.Add(Maybe.Some(1));
                CollectionAssert.AreEqual(expecteds, actuals);

                source.Value++;
                expecteds.Add(Maybe.Some(2));
                CollectionAssert.AreEqual(expecteds, actuals);

                source.OnPropertyChanged("Value");
                CollectionAssert.AreEqual(expecteds, actuals);
            }
        }

        [Test]
        public static void FakeNextValueDefault()
        {
            var source = new Fake();
            var actuals = new List<Maybe<string?>>();
            using (source.ObserveValue(x => x.Next.Name)
                         .Subscribe(actuals.Add))
            {
                var expecteds = new List<Maybe<string?>> { Maybe<string?>.None };
                CollectionAssert.AreEqual(expecteds, actuals);

                source.Next = new Level();
                expecteds.Add(Maybe<string?>.Some(null));
                CollectionAssert.AreEqual(expecteds, actuals);

                source.Next.Name = "Johan";
                expecteds.Add(Maybe<string?>.Some("Johan"));
                CollectionAssert.AreEqual(expecteds, actuals);

                source.Next.OnPropertyChanged("Name");
                CollectionAssert.AreEqual(expecteds, actuals);

                source.Next = null;
                expecteds.Add(Maybe<string?>.None);
                CollectionAssert.AreEqual(expecteds, actuals);
            }
        }

        [Test]
        public static void FakeNextValueSignalInitial()
        {
            var source = new Fake();
            var actuals = new List<Maybe<string?>>();
            using (source.ObserveValue(x => x.Next.Name, signalInitial: true)
                         .Subscribe(actuals.Add))
            {
                var expecteds = new List<Maybe<string?>> { Maybe<string?>.None };
                CollectionAssert.AreEqual(expecteds, actuals);

                source.Next = new Level();
                expecteds.Add(Maybe<string?>.Some(null));
                CollectionAssert.AreEqual(expecteds, actuals);

                source.Next.Name = "Johan";
                expecteds.Add(Maybe<string?>.Some("Johan"));
                CollectionAssert.AreEqual(expecteds, actuals);

                source.Next.OnPropertyChanged("Name");
                CollectionAssert.AreEqual(expecteds, actuals);

                source.Next = null;
                expecteds.Add(Maybe<string?>.None);
                CollectionAssert.AreEqual(expecteds, actuals);
            }
        }

        [Test]
        public static void FakenextValueValueNoInitial()
        {
            var source = new Fake();
            var actuals = new List<Maybe<string?>>();
            using (source.ObserveValue(x => x.Next.Name, signalInitial: false)
                         .Subscribe(actuals.Add))
            {
                var expecteds = new List<Maybe<string?>>();
                CollectionAssert.AreEqual(expecteds, actuals);

                source.Next = new Level();
                expecteds.Add(Maybe<string?>.Some(null));
                CollectionAssert.AreEqual(expecteds, actuals);

                source.Next.Name = "Johan";
                expecteds.Add(Maybe<string?>.Some("Johan"));
                CollectionAssert.AreEqual(expecteds, actuals);

                source.Next.OnPropertyChanged("Name");
                CollectionAssert.AreEqual(expecteds, actuals);

                source.Next = null;
                expecteds.Add(Maybe<string?>.None);
                CollectionAssert.AreEqual(expecteds, actuals);
            }
        }

        [Test]
        public static void FakeOfIFakeValueDefault()
        {
            var source = new Fake<IFake>();
            var actuals = new List<Maybe<string?>>();
            using (source.ObserveValue(x => x.Next.Name)
                         .Subscribe(actuals.Add))
            {
                var expecteds = new List<Maybe<string?>> { Maybe<string?>.None };
                CollectionAssert.AreEqual(expecteds, actuals);

                source.Next = new Level<IFake>();
                expecteds.Add(Maybe<string?>.Some(null));
                CollectionAssert.AreEqual(expecteds, actuals);

                source.Next.Name = "Johan";
                expecteds.Add(Maybe<string?>.Some("Johan"));
                CollectionAssert.AreEqual(expecteds, actuals);

                source.Next.OnPropertyChanged("Name");
                CollectionAssert.AreEqual(expecteds, actuals);

                source.Next = null;
                expecteds.Add(Maybe<string?>.None);
                CollectionAssert.AreEqual(expecteds, actuals);
            }
        }

        [Test]
        public static void FakeOfIFakeValueSignalInitial()
        {
            var source = new Fake<IFake>();
            var actuals = new List<Maybe<string?>>();
            using (source.ObserveValue(x => x.Next.Name, signalInitial: true)
                         .Subscribe(actuals.Add))
            {
                var expecteds = new List<Maybe<string?>> { Maybe<string?>.None };
                CollectionAssert.AreEqual(expecteds, actuals);

                source.Next = new Level<IFake>();
                expecteds.Add(Maybe<string?>.Some(null));
                CollectionAssert.AreEqual(expecteds, actuals);

                source.Next.Name = "Johan";
                expecteds.Add(Maybe<string?>.Some("Johan"));
                CollectionAssert.AreEqual(expecteds, actuals);

                source.Next.OnPropertyChanged("Name");
                CollectionAssert.AreEqual(expecteds, actuals);

                source.Next = null;
                expecteds.Add(Maybe<string?>.None);
                CollectionAssert.AreEqual(expecteds, actuals);
            }
        }

        [Test]
        public static void FakeOfIFakeValueNoInitial()
        {
            var source = new Fake<IFake>();
            var actuals = new List<Maybe<string?>>();
            using (source.ObserveValue(x => x.Next.Name, signalInitial: false)
                         .Subscribe(actuals.Add))
            {
                var expecteds = new List<Maybe<string?>>();
                CollectionAssert.AreEqual(expecteds, actuals);

                source.Next = new Level<IFake>();
                expecteds.Add(Maybe<string?>.Some(null));
                CollectionAssert.AreEqual(expecteds, actuals);

                source.Next.Name = "Johan";
                expecteds.Add(Maybe<string?>.Some("Johan"));
                CollectionAssert.AreEqual(expecteds, actuals);

                source.Next.OnPropertyChanged("Name");
                CollectionAssert.AreEqual(expecteds, actuals);

                source.Next = null;
                expecteds.Add(Maybe<string?>.None);
                CollectionAssert.AreEqual(expecteds, actuals);
            }
        }

        [Test]
        public static void VirtualProperty()
        {
            var source = new Fake<A>();
            var actuals = new List<Maybe<string>>();
            using (source.ObserveValue(x => x.Value.Value, signalInitial: false)
                         .Subscribe(actuals.Add))
            {
                var expecteds = new List<Maybe<string>>();
                CollectionAssert.AreEqual(expecteds, actuals);

                source.Value = new A();
                expecteds.Add(Maybe<string>.Some("A"));
                CollectionAssert.AreEqual(expecteds, actuals);

                source.Value = new B();
                expecteds.Add(Maybe<string>.Some("B"));
                CollectionAssert.AreEqual(expecteds, actuals);
            }
        }

        [Test]
        public static void Mock()
        {
            var source = new Mock<IFake>(MockBehavior.Strict);
            source.SetupGet(x => x.Value)
                .Returns(1);
            var actuals = new List<Maybe<int>>();
            using (source.Object.ObserveValue(x => x.Value, signalInitial: false)
                         .Subscribe(actuals.Add))
            {
                var expecteds = new List<Maybe<int>>();
                CollectionAssert.AreEqual(expecteds, actuals);

                source.SetupGet(x => x.Value)
                    .Returns(2);
                source.Raise(x => x.PropertyChanged += null, new PropertyChangedEventArgs("Value"));
                expecteds.Add(Maybe.Some(2));
                CollectionAssert.AreEqual(expecteds, actuals);

                source.SetupGet(x => x.Value)
                    .Returns(3);
                source.Raise(x => x.PropertyChanged += null, new PropertyChangedEventArgs("Value"));
                expecteds.Add(Maybe.Some(3));
                CollectionAssert.AreEqual(expecteds, actuals);
            }
        }

        [TestCase(true, new[] { 1 })]
        [TestCase(false, new int[0])]
        public static void SimpleSignalInitial(bool signalInitial, int[] start)
        {
            var expected = start.Select(Maybe.Some)
                                .ToList();
            var values = new List<Maybe<int>>();
            var source = new Fake { Value = 1 };
            using (source.ObserveValue(x => x.Value, signalInitial)
                         .Subscribe(values.Add))
            {
                CollectionAssert.AreEqual(expected, values);

                source.Value++;
                expected.Add(Maybe.Some(source.Value));
                CollectionAssert.AreEqual(expected, values);

                source.Value++;
                expected.Add(Maybe.Some(source.Value));
                CollectionAssert.AreEqual(expected, values);
            }
        }

        [TestCase(true, new[] { 1 })]
        [TestCase(false, new int[0])]
        public static void SimpleSignalInitialDifferent(bool signalInitial, int[] start)
        {
            var expected = start.Select(Maybe.Some)
                                .ToList();
            var actuals1 = new List<Maybe<int>>();
            var actuals2 = new List<Maybe<int>>();
            var source = new Fake { Value = 1 };
            var observable = source.ObserveValue(x => x.Value, signalInitial);
            using (observable.Subscribe(actuals1.Add))
            {
                CollectionAssert.AreEqual(expected, actuals1);

                source.Value++;
                expected.Add(Maybe.Some(source.Value));
                CollectionAssert.AreEqual(expected, actuals1);

                using (observable.Subscribe(actuals2.Add))
                {
                    CollectionAssert.AreEqual(expected, actuals1);
                    CollectionAssert.AreEqual(expected.Skip(1), actuals2);

                    source.Value++;
                    expected.Add(Maybe.Some(source.Value));
                    CollectionAssert.AreEqual(expected, actuals1);
                    CollectionAssert.AreEqual(expected.Skip(1), actuals2);
                }
            }
        }

        [TestCase(true, new[] { 1 })]
        [TestCase(false, new int[0])]
        public static void TwoLevelsSignalInitial(bool signalInitial, int[] start)
        {
            var actuals = new List<Maybe<int>>();
            var source = new Fake { Level1 = new Level1 { Value = 1 } };
            using (source.ObserveValue(x => x.Level1.Value, signalInitial)
                         .Subscribe(actuals.Add))
            {
                var expected = start.Select(Maybe.Some)
                                    .ToList();
                CollectionAssert.AreEqual(expected, actuals);

                source.Level1.Value++;
                expected.Add(Maybe.Some(source.Level1.Value));
                CollectionAssert.AreEqual(expected, actuals);

                source.Level1.OnPropertyChanged("Value");
                CollectionAssert.AreEqual(expected, actuals);

                source.Level1.OnPropertyChanged("IsTrue");
                CollectionAssert.AreEqual(expected, actuals);

                source.Level1 = null;
                expected.Add(Maybe<int>.None);
                CollectionAssert.AreEqual(expected, actuals);
            }
        }

        [Test]
        public static void TwoLevelsSignalInitialDifferentWhenSubscribingSignalInitial()
        {
            var actuals1 = new List<Maybe<int>>();
            var actuals2 = new List<Maybe<int>>();
            var source = new Fake { Level1 = new Level1 { Value = 1 } };
            var observable = source.ObserveValue(x => x.Level1.Value, signalInitial: true);
            using (observable.Subscribe(actuals1.Add))
            {
                var expected = new List<Maybe<int>> { Maybe<int>.Some(1) };
                CollectionAssert.AreEqual(expected, actuals1);

                source.Level1.Value++;
                expected.Add(Maybe.Some(source.Level1.Value));
                CollectionAssert.AreEqual(expected, actuals1);
                using (observable.Subscribe(actuals2.Add))
                {
                    CollectionAssert.AreEqual(expected, actuals1);
                    CollectionAssert.AreEqual(expected.Skip(1), actuals2);

                    source.Level1.OnPropertyChanged("Value");
                    CollectionAssert.AreEqual(expected, actuals1);
                    CollectionAssert.AreEqual(expected.Skip(1), actuals2);

                    source.Level1.OnPropertyChanged("IsTrue");
                    CollectionAssert.AreEqual(expected, actuals1);
                    CollectionAssert.AreEqual(expected.Skip(1), actuals2);

                    source.Level1 = null;
                    expected.Add(Maybe<int>.None);
                    CollectionAssert.AreEqual(expected, actuals1);
                    CollectionAssert.AreEqual(expected.Skip(1), actuals2);
                }
            }
        }

        [Test]
        public static void TwoLevelsSignalInitialDifferentWhenSubscribingNoInitial()
        {
            var actuals1 = new List<Maybe<int>>();
            var actuals2 = new List<Maybe<int>>();
            var source = new Fake { Next = new Level { Value = 1 } };
            var observable = source.ObserveValue(x => x.Next.Value, signalInitial: false);
            using (observable.Subscribe(actuals1.Add))
            {
                var expected = new List<Maybe<int>>();
                CollectionAssert.AreEqual(expected, actuals1);

                source.Next.Value++;
                expected.Add(Maybe.Some(source.Next.Value));
                CollectionAssert.AreEqual(expected, actuals1);
                using (observable.Subscribe(actuals2.Add))
                {
                    CollectionAssert.AreEqual(expected, actuals1);
                    CollectionAssert.AreEqual(expected.Skip(1), actuals2);

                    source.Next.OnPropertyChanged("Value");
                    CollectionAssert.AreEqual(expected, actuals1);
                    CollectionAssert.AreEqual(expected, actuals2);

                    source.Next.OnPropertyChanged("Next");
                    CollectionAssert.AreEqual(expected, actuals1);
                    CollectionAssert.AreEqual(expected, actuals2);

                    source.Next = null;
                    expected.Add(Maybe<int>.None);
                    CollectionAssert.AreEqual(expected, actuals1);
                    CollectionAssert.AreEqual(expected, actuals2);
                }
            }
        }

        [Test]
        public static void ThreeLevelsStartingWithFirstNullThenAddingLevelsOneByOne()
        {
            var actuals = new List<Maybe<bool>>();
            var source = new Fake();
            using (source.ObserveValue(x => x.Level1.Level2.IsTrue)
                         .Subscribe(actuals.Add))
            {
                var expecteds = new List<Maybe<bool>> { Maybe<bool>.None };
                CollectionAssert.AreEqual(expecteds, actuals);

                source.Level1 = new Level1();
                CollectionAssert.AreEqual(expecteds, actuals);

                source.Level1.Level2 = new Level2();
                expecteds.Add(Maybe<bool>.Some(source.Level1.Level2.IsTrue));
                CollectionAssert.AreEqual(expecteds, actuals);

                source.Level1.Level2.IsTrue = !source.Level1.Level2.IsTrue;
                expecteds.Add(Maybe<bool>.Some(source.Level1.Level2.IsTrue));
                CollectionAssert.AreEqual(expecteds, actuals);

                source.Level1 = null;
                expecteds.Add(Maybe<bool>.None);
                CollectionAssert.AreEqual(expecteds, actuals);
            }
        }

        [Test]
        public static void ReadOnlyObservableCollectionCount()
        {
            var ints = new ObservableCollection<int>();
            var source = new ReadOnlyObservableCollection<int>(ints);
            var values = new List<Maybe<int>>();
            using (source.ObserveValue(x => x.Count, signalInitial: false)
                         .Subscribe(values.Add))
            {
                CollectionAssert.IsEmpty(values);

                ints.Add(1);
                CollectionAssert.AreEqual(new[] { Maybe.Some(1) }, values);

                ints.Add(2);
                CollectionAssert.AreEqual(new[] { Maybe.Some(1), Maybe.Some(2) }, values);
            }
        }

        [Test]
        public static void MemoryLeakSimpleDisposeTest()
        {
#if DEBUG
            Assert.Inconclusive("Debugger keeps things alive for the scope of the method.");
#endif
            var source = new Fake();
            var wr = new WeakReference(source);
            using (source.ObserveValue(x => x.IsTrueOrNull)
                       .Subscribe())
            {
            }

            GC.Collect();
            Assert.IsFalse(wr.IsAlive);
        }

        [Test]
        public static void MemoryLeakNestedDisposeTest()
        {
#if DEBUG
            Assert.Inconclusive("Debugger keeps things alive for the scope of the method.");
#endif
            var source = new Fake();
            var wr = new WeakReference(source);
            using (source.ObserveValue(x => x.Next.Next.Value)
                       .Subscribe())
            {
            }

            GC.Collect();
            Assert.IsFalse(wr.IsAlive);
        }

        [Test]
        public static void MemoryLeakSimpleNoDisposeTest()
        {
#if DEBUG
            Assert.Inconclusive("Debugger keeps things alive for the scope of the method.");
#endif
            var source = new Fake();
            var wr = new WeakReference(source);
            var observable = source.ObserveValue(x => x.IsTrueOrNull);
#pragma warning disable IDISP001  // Dispose created.
            //// ReSharper disable once UnusedVariable
            var subscribe = observable.Subscribe();
#pragma warning restore IDISP001  // Dispose created.
            GC.Collect();
            Assert.IsFalse(wr.IsAlive);
        }

        [Test]
        public static void MemoryLeakNestedNoDisposeTest()
        {
#if DEBUG
            Assert.Inconclusive("Debugger keeps things alive for the scope of the method.");
#endif
            var source = new Fake();
            var wr = new WeakReference(source);
            var observable = source.ObserveValue(x => x.Next.Next.Value);
#pragma warning disable IDISP001  // Dispose created.
            //// ReSharper disable once UnusedVariable
            var subscribe = observable.Subscribe();
#pragma warning restore IDISP001  // Dispose created.
            GC.Collect();
            Assert.IsFalse(wr.IsAlive);
        }

        public class A : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler? PropertyChanged;

            public virtual string Value => "A";

            protected void OnPropertyChanged(PropertyChangedEventArgs e)
            {
                this.PropertyChanged?.Invoke(this, e);
            }
        }

        public class B : A
        {
            public override string Value => "B";
        }
    }
}
