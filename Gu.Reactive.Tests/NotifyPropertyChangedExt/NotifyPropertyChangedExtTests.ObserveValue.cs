// ReSharper disable RedundantArgumentDefaultValue
// ReSharper disable NotResolvedInText
// ReSharper disable HeuristicUnreachableCode
namespace Gu.Reactive.Tests.NotifyPropertyChangedExt
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;

    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    public partial class NotifyPropertyChangedExtTests
    {
        public class ObserveValue
        {
            [Test]
            public void Simple()
            {
                var fake = new Fake();
                var actuals = new List<Maybe<int>>();
                using (fake.ObserveValue(x => x.Value, false)
                           .Subscribe(actuals.Add))
                {
                    var expecteds = new List<Maybe<int>>();
                    CollectionAssert.AreEqual(expecteds, actuals);

                    fake.Value++;
                    expecteds.Add(Maybe<int>.Some(1));
                    CollectionAssert.AreEqual(expecteds, actuals);

                    fake.Value++;
                    expecteds.Add(Maybe<int>.Some(2));
                    CollectionAssert.AreEqual(expecteds, actuals);

                    fake.OnPropertyChanged("Value");
                    CollectionAssert.AreEqual(expecteds, actuals);

                    fake.OnPropertyChanged(string.Empty);
                    CollectionAssert.AreEqual(expecteds, actuals);

                    fake.OnPropertyChanged(null);
                    CollectionAssert.AreEqual(expecteds, actuals);

                    fake.OnPropertyChanged("Next");
                    CollectionAssert.AreEqual(expecteds, actuals);
                }
            }

            [Test]
            public void FakeOfIntValueDefault()
            {
                var fake = new Fake<int>();
                var actuals = new List<Maybe<int>>();
                using (fake.ObserveValue(x => x.Value)
                           .Subscribe(actuals.Add))
                {
                    var expecteds = new List<Maybe<int>> { Maybe.Some(0) };
                    CollectionAssert.AreEqual(expecteds, actuals);

                    fake.Value++;
                    expecteds.Add(Maybe.Some(1));
                    CollectionAssert.AreEqual(expecteds, actuals);

                    fake.Value++;
                    expecteds.Add(Maybe.Some(2));
                    CollectionAssert.AreEqual(expecteds, actuals);

                    fake.OnPropertyChanged("Value");
                    CollectionAssert.AreEqual(expecteds, actuals);
                }
            }

            [Test]
            public void FakeOfIntValueSignalInitial()
            {
                var fake = new Fake<int>();
                var actuals = new List<Maybe<int>>();
                using (fake.ObserveValue(x => x.Value, true)
                           .Subscribe(actuals.Add))
                {
                    var expecteds = new List<Maybe<int>> { Maybe.Some(0) };
                    CollectionAssert.AreEqual(expecteds, actuals);

                    fake.Value++;
                    expecteds.Add(Maybe.Some(1));
                    CollectionAssert.AreEqual(expecteds, actuals);

                    fake.Value++;
                    expecteds.Add(Maybe.Some(2));
                    CollectionAssert.AreEqual(expecteds, actuals);

                    fake.OnPropertyChanged("Value");
                    CollectionAssert.AreEqual(expecteds, actuals);
                }
            }

            [Test]
            public void FakeOfIntValueNoInitial()
            {
                var fake = new Fake<int>();
                var actuals = new List<Maybe<int>>();
                using (fake.ObserveValue(x => x.Value, false)
                           .Subscribe(actuals.Add))
                {
                    var expecteds = new List<Maybe<int>>();
                    CollectionAssert.AreEqual(expecteds, actuals);

                    fake.Value++;
                    expecteds.Add(Maybe.Some(1));
                    CollectionAssert.AreEqual(expecteds, actuals);

                    fake.Value++;
                    expecteds.Add(Maybe.Some(2));
                    CollectionAssert.AreEqual(expecteds, actuals);

                    fake.OnPropertyChanged("Value");
                    CollectionAssert.AreEqual(expecteds, actuals);
                }
            }

            [Test]
            public void FakeNextValueDefault()
            {
                var fake = new Fake();
                var actuals = new List<Maybe<string>>();
                using (fake.ObserveValue(x => x.Next.Name)
                           .Subscribe(actuals.Add))
                {
                    var expecteds = new List<Maybe<string>> { Maybe<string>.None };
                    CollectionAssert.AreEqual(expecteds, actuals);

                    fake.Next = new Level();
                    expecteds.Add(Maybe<string>.Some(null));
                    CollectionAssert.AreEqual(expecteds, actuals);

                    fake.Next.Name = "Johan";
                    expecteds.Add(Maybe<string>.Some("Johan"));
                    CollectionAssert.AreEqual(expecteds, actuals);

                    fake.Next.OnPropertyChanged("Name");
                    CollectionAssert.AreEqual(expecteds, actuals);

                    fake.Next = null;
                    expecteds.Add(Maybe<string>.None);
                    CollectionAssert.AreEqual(expecteds, actuals);
                }
            }

            [Test]
            public void FakeNextValueSignalInitial()
            {
                var fake = new Fake();
                var actuals = new List<Maybe<string>>();
                using (fake.ObserveValue(x => x.Next.Name, true)
                           .Subscribe(actuals.Add))
                {
                    var expecteds = new List<Maybe<string>> { Maybe<string>.None };
                    CollectionAssert.AreEqual(expecteds, actuals);

                    fake.Next = new Level();
                    expecteds.Add(Maybe<string>.Some(null));
                    CollectionAssert.AreEqual(expecteds, actuals);

                    fake.Next.Name = "Johan";
                    expecteds.Add(Maybe<string>.Some("Johan"));
                    CollectionAssert.AreEqual(expecteds, actuals);

                    fake.Next.OnPropertyChanged("Name");
                    CollectionAssert.AreEqual(expecteds, actuals);

                    fake.Next = null;
                    expecteds.Add(Maybe<string>.None);
                    CollectionAssert.AreEqual(expecteds, actuals);
                }
            }

            [Test]
            public void FakenextValueValueNoInitial()
            {
                var fake = new Fake();
                var actuals = new List<Maybe<string>>();
                using (fake.ObserveValue(x => x.Next.Name, false)
                           .Subscribe(actuals.Add))
                {
                    var expecteds = new List<Maybe<string>>();
                    CollectionAssert.AreEqual(expecteds, actuals);

                    fake.Next = new Level();
                    expecteds.Add(Maybe<string>.Some(null));
                    CollectionAssert.AreEqual(expecteds, actuals);

                    fake.Next.Name = "Johan";
                    expecteds.Add(Maybe<string>.Some("Johan"));
                    CollectionAssert.AreEqual(expecteds, actuals);

                    fake.Next.OnPropertyChanged("Name");
                    CollectionAssert.AreEqual(expecteds, actuals);

                    fake.Next = null;
                    expecteds.Add(Maybe<string>.None);
                    CollectionAssert.AreEqual(expecteds, actuals);
                }
            }

            [Test]
            public void FakeOfIFakeValueDefault()
            {
                var fake = new Fake<IFake>();
                var actuals = new List<Maybe<string>>();
                using (fake.ObserveValue(x => x.Next.Name)
                           .Subscribe(actuals.Add))
                {
                    var expecteds = new List<Maybe<string>> { Maybe<string>.None };
                    CollectionAssert.AreEqual(expecteds, actuals);

                    fake.Next = new Level<IFake>();
                    expecteds.Add(Maybe<string>.Some(null));
                    CollectionAssert.AreEqual(expecteds, actuals);

                    fake.Next.Name = "Johan";
                    expecteds.Add(Maybe<string>.Some("Johan"));
                    CollectionAssert.AreEqual(expecteds, actuals);

                    fake.Next.OnPropertyChanged("Name");
                    CollectionAssert.AreEqual(expecteds, actuals);

                    fake.Next = null;
                    expecteds.Add(Maybe<string>.None);
                    CollectionAssert.AreEqual(expecteds, actuals);
                }
            }

            [Test]
            public void FakeOfIFakeValueSignalInitial()
            {
                var fake = new Fake<IFake>();
                var actuals = new List<Maybe<string>>();
                using (fake.ObserveValue(x => x.Next.Name, true)
                           .Subscribe(actuals.Add))
                {
                    var expecteds = new List<Maybe<string>> { Maybe<string>.None };
                    CollectionAssert.AreEqual(expecteds, actuals);

                    fake.Next = new Level<IFake>();
                    expecteds.Add(Maybe<string>.Some(null));
                    CollectionAssert.AreEqual(expecteds, actuals);

                    fake.Next.Name = "Johan";
                    expecteds.Add(Maybe<string>.Some("Johan"));
                    CollectionAssert.AreEqual(expecteds, actuals);

                    fake.Next.OnPropertyChanged("Name");
                    CollectionAssert.AreEqual(expecteds, actuals);

                    fake.Next = null;
                    expecteds.Add(Maybe<string>.None);
                    CollectionAssert.AreEqual(expecteds, actuals);
                }
            }

            [Test]
            public void FakeOfIFakeValueNoInitial()
            {
                var fake = new Fake<IFake>();
                var actuals = new List<Maybe<string>>();
                using (fake.ObserveValue(x => x.Next.Name, false)
                           .Subscribe(actuals.Add))
                {
                    var expecteds = new List<Maybe<string>>();
                    CollectionAssert.AreEqual(expecteds, actuals);

                    fake.Next = new Level<IFake>();
                    expecteds.Add(Maybe<string>.Some(null));
                    CollectionAssert.AreEqual(expecteds, actuals);

                    fake.Next.Name = "Johan";
                    expecteds.Add(Maybe<string>.Some("Johan"));
                    CollectionAssert.AreEqual(expecteds, actuals);

                    fake.Next.OnPropertyChanged("Name");
                    CollectionAssert.AreEqual(expecteds, actuals);

                    fake.Next = null;
                    expecteds.Add(Maybe<string>.None);
                    CollectionAssert.AreEqual(expecteds, actuals);
                }
            }

            [Test]
            public void VirtualProperty()
            {
                var fake = new Fake<A>();
                var actuals = new List<Maybe<string>>();
                using (fake.ObserveValue(x => x.Value.Value, false)
                           .Subscribe(actuals.Add))
                {
                    var expecteds = new List<Maybe<string>>();
                    CollectionAssert.AreEqual(expecteds, actuals);

                    fake.Value = new A();
                    expecteds.Add(Maybe<string>.Some("A"));
                    CollectionAssert.AreEqual(expecteds, actuals);

                    fake.Value = new B();
                    expecteds.Add(Maybe<string>.Some("B"));
                    CollectionAssert.AreEqual(expecteds, actuals);
                }
            }

            [TestCase(true, new[] { 1 })]
            [TestCase(false, new int[0])]
            public void SimpleSignalInitial(bool signalInitial, int[] start)
            {
                var expected = start.Select(Maybe.Some)
                                    .ToList();
                var values = new List<Maybe<int>>();
                var fake = new Fake { Value = 1 };
                using (fake.ObserveValue(x => x.Value, signalInitial)
                           .Subscribe(values.Add))
                {
                    CollectionAssert.AreEqual(expected, values);

                    fake.Value++;
                    expected.Add(Maybe.Some(fake.Value));
                    CollectionAssert.AreEqual(expected, values);

                    fake.Value++;
                    expected.Add(Maybe.Some(fake.Value));
                    CollectionAssert.AreEqual(expected, values);
                }
            }

            [TestCase(true, new[] { 1 })]
            [TestCase(false, new int[0])]
            public void SimpleSignalInitialDifferent(bool signalInitial, int[] start)
            {
                var expected = start.Select(Maybe.Some)
                                    .ToList();
                var actuals1 = new List<Maybe<int>>();
                var actuals2 = new List<Maybe<int>>();
                var fake = new Fake { Value = 1 };
                var observable = fake.ObserveValue(x => x.Value, signalInitial);
                using (observable.Subscribe(actuals1.Add))
                {
                    CollectionAssert.AreEqual(expected, actuals1);

                    fake.Value++;
                    expected.Add(Maybe.Some(fake.Value));
                    CollectionAssert.AreEqual(expected, actuals1);

                    using (observable.Subscribe(actuals2.Add))
                    {
                        CollectionAssert.AreEqual(expected, actuals1);
                        CollectionAssert.AreEqual(expected.Skip(1), actuals2);

                        fake.Value++;
                        expected.Add(Maybe.Some(fake.Value));
                        CollectionAssert.AreEqual(expected, actuals1);
                        CollectionAssert.AreEqual(expected.Skip(1), actuals2);
                    }
                }
            }

            [TestCase(true, new[] { 1 })]
            [TestCase(false, new int[0])]
            public void TwoLevelsSignalInitial(bool signalInitial, int[] start)
            {
                var actuals = new List<Maybe<int>>();
                var fake = new Fake { Level1 = new Level1 { Value = 1 } };
                using (fake.ObserveValue(x => x.Level1.Value, signalInitial)
                           .Subscribe(actuals.Add))
                {
                    var expected = start.Select(Maybe.Some)
                                        .ToList();
                    CollectionAssert.AreEqual(expected, actuals);

                    fake.Level1.Value++;
                    expected.Add(Maybe.Some(fake.Level1.Value));
                    CollectionAssert.AreEqual(expected, actuals);

                    fake.Level1.OnPropertyChanged("Value");
                    CollectionAssert.AreEqual(expected, actuals);

                    fake.Level1.OnPropertyChanged("Next");
                    CollectionAssert.AreEqual(expected, actuals);

                    fake.Level1 = null;
                    expected.Add(Maybe<int>.None);
                    CollectionAssert.AreEqual(expected, actuals);
                }
            }

            [Test]
            public void TwoLevelsSignalInitialDifferentWhenSubscribingSignalInitial()
            {
                var actuals1 = new List<Maybe<int>>();
                var actuals2 = new List<Maybe<int>>();
                var fake = new Fake { Level1 = new Level1 { Value = 1 } };
                var observable = fake.ObserveValue(x => x.Level1.Value, true);
                using (observable.Subscribe(actuals1.Add))
                {
                    var expected = new List<Maybe<int>> { Maybe<int>.Some(1) };
                    CollectionAssert.AreEqual(expected, actuals1);

                    fake.Level1.Value++;
                    expected.Add(Maybe.Some(fake.Level1.Value));
                    CollectionAssert.AreEqual(expected, actuals1);
                    using (observable.Subscribe(actuals2.Add))
                    {
                        CollectionAssert.AreEqual(expected, actuals1);
                        CollectionAssert.AreEqual(expected.Skip(1), actuals2);

                        fake.Level1.OnPropertyChanged("Value");
                        CollectionAssert.AreEqual(expected, actuals1);
                        CollectionAssert.AreEqual(expected.Skip(1), actuals2);

                        fake.Level1.OnPropertyChanged("Next");
                        CollectionAssert.AreEqual(expected, actuals1);
                        CollectionAssert.AreEqual(expected.Skip(1), actuals2);

                        fake.Level1 = null;
                        expected.Add(Maybe<int>.None);
                        CollectionAssert.AreEqual(expected, actuals1);
                        CollectionAssert.AreEqual(expected.Skip(1), actuals2);
                    }
                }
            }

            [Test]
            public void TwoLevelsSignalInitialDifferentWhenSubscribingNoInitial()
            {
                var actuals1 = new List<Maybe<int>>();
                var actuals2 = new List<Maybe<int>>();
                var fake = new Fake { Next = new Level { Value = 1 } };
                var observable = fake.ObserveValue(x => x.Next.Value, false);
                using (observable.Subscribe(actuals1.Add))
                {
                    var expected = new List<Maybe<int>>();
                    CollectionAssert.AreEqual(expected, actuals1);

                    fake.Next.Value++;
                    expected.Add(Maybe.Some(fake.Next.Value));
                    CollectionAssert.AreEqual(expected, actuals1);
                    using (observable.Subscribe(actuals2.Add))
                    {
                        CollectionAssert.AreEqual(expected, actuals1);
                        CollectionAssert.AreEqual(expected.Skip(1), actuals2);

                        fake.Next.OnPropertyChanged("Value");
                        CollectionAssert.AreEqual(expected, actuals1);
                        CollectionAssert.AreEqual(expected, actuals2);

                        fake.Next.OnPropertyChanged("Next");
                        CollectionAssert.AreEqual(expected, actuals1);
                        CollectionAssert.AreEqual(expected, actuals2);

                        fake.Next = null;
                        expected.Add(Maybe<int>.None);
                        CollectionAssert.AreEqual(expected, actuals1);
                        CollectionAssert.AreEqual(expected, actuals2);
                    }
                }
            }

            [Test]
            public void ThreeLevelsStartingWithFirstNullThenAddingLevelsOneByOne()
            {
                var actuals = new List<Maybe<bool>>();
                var fake = new Fake();
                using (fake.ObserveValue(x => x.Level1.Level2.IsTrue)
                           .Subscribe(actuals.Add))
                {
                    var expecteds = new List<Maybe<bool>> { Maybe<bool>.None };
                    CollectionAssert.AreEqual(expecteds, actuals);

                    fake.Level1 = new Level1();
                    CollectionAssert.AreEqual(expecteds, actuals);

                    fake.Level1.Level2 = new Level2();
                    expecteds.Add( Maybe<bool>.Some(fake.Level1.Level2.IsTrue));
                    CollectionAssert.AreEqual(expecteds, actuals);

                    fake.Level1.Level2.IsTrue = !fake.Level1.Level2.IsTrue;
                    expecteds.Add(Maybe<bool>.Some(fake.Level1.Level2.IsTrue));
                    CollectionAssert.AreEqual(expecteds, actuals);

                    fake.Level1 = null;
                    expecteds.Add(Maybe<bool>.None);
                    CollectionAssert.AreEqual(expecteds, actuals);
                }
            }

            [Test]
            public void ReadOnlyObservableCollectionCount()
            {
                var ints = new ObservableCollection<int>();
                var source = new ReadOnlyObservableCollection<int>(ints);
                var values = new List<Maybe<int>>();
                using (source.ObserveValue(x => x.Count, false)
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
            public void MemoryLeakSimpleDisposeTest()
            {
#if DEBUG
                Assert.Inconclusive("Debugger keeps things alive for the scope of the method.");
#endif
                var fake = new Fake();
                var wr = new WeakReference(fake);
                using (fake.ObserveValue(x => x.IsTrueOrNull)
                           .Subscribe())
                {
                }

                GC.Collect();
                Assert.IsFalse(wr.IsAlive);
            }

            [Test]
            public void MemoryLeakNestedDisposeTest()
            {
#if DEBUG
                Assert.Inconclusive("Debugger keeps things alive for the scope of the method.");
#endif
                var fake = new Fake();
                var wr = new WeakReference(fake);
                using (fake.ObserveValue(x => x.Next.Next.Value)
                           .Subscribe())
                {
                }

                GC.Collect();
                Assert.IsFalse(wr.IsAlive);
            }

            [Test]
            public void MemoryLeakSimpleNoDisposeTest()
            {
#if DEBUG
                Assert.Inconclusive("Debugger keeps things alive for the scope of the method.");
#endif
                var fake = new Fake();
                var wr = new WeakReference(fake);
                var observable = fake.ObserveValue(x => x.IsTrueOrNull);
#pragma warning disable GU0030 // Use using.
                //// ReSharper disable once UnusedVariable
                var subscribe = observable.Subscribe();
#pragma warning restore GU0030 // Use using.
                GC.Collect();
                Assert.IsFalse(wr.IsAlive);
            }

            [Test]
            public void MemoryLeakNestedNoDisposeTest()
            {
#if DEBUG
                Assert.Inconclusive("Debugger keeps things alive for the scope of the method.");
#endif
                var fake = new Fake();
                var wr = new WeakReference(fake);
                var observable = fake.ObserveValue(x => x.Next.Next.Value);
#pragma warning disable GU0030 // Use using.
                //// ReSharper disable once UnusedVariable
                var subscribe = observable.Subscribe();
#pragma warning restore GU0030 // Use using.
                GC.Collect();
                Assert.IsFalse(wr.IsAlive);
            }

            public class A : INotifyPropertyChanged
            {
                public event PropertyChangedEventHandler PropertyChanged;

                public virtual string Value => "A";
            }

            public class B : A
            {
                public override string Value => "B";
            }
        }
    }
}