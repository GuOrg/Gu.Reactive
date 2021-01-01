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
    using System.Linq.Expressions;

    using Gu.Reactive.Tests.Helpers;
    using NUnit.Framework;

    public static class ObservePropertyChangedSlimLambda
    {
        [Test]
        public static void ReactsTwoPropertiesSameInstance()
        {
            var changes = new List<PropertyChangedEventArgs>();
            var fake = new Fake { Next = new Level { Value = 1 } };
            using (fake.ObservePropertyChangedSlim(x => x.Next.Value, signalInitial: false)
                       .Subscribe(changes.Add))
            {
                using (fake.ObservePropertyChangedSlim(x => x.Next.IsTrue, signalInitial: false)
                           .Subscribe(changes.Add))
                {
                    Assert.AreEqual(0, changes.Count);

                    fake.Next.Value++;
                    Assert.AreEqual(1, changes.Count);
                    Assert.AreEqual("Value", changes.Last().PropertyName);

                    fake.Next.IsTrue = !fake.IsTrue;
                    Assert.AreEqual(2, changes.Count);
                    Assert.AreEqual("IsTrue", changes.Last().PropertyName);
                }
            }
        }

        [Test]
        public static void ReactsTwoInstancesSameProperty()
        {
            var changes = new List<PropertyChangedEventArgs>();
            var fake1 = new Fake { Next = new Level() };
            using (fake1.ObservePropertyChangedSlim(x => x.Next.Value, signalInitial: false)
                        .Subscribe(changes.Add))
            {
                var fake2 = new Fake { Next = new Level() };
                using (fake2.ObservePropertyChangedSlim(x => x.Next.Value, signalInitial: false)
                            .Subscribe(changes.Add))
                {
                    Assert.AreEqual(0, changes.Count);

                    fake1.Next.Value++;
                    Assert.AreEqual(1, changes.Count);
                    Assert.AreEqual("Value", changes.Last().PropertyName);

                    fake2.Next.Value++;
                    Assert.AreEqual(2, changes.Count);
                    Assert.AreEqual("Value", changes.Last().PropertyName);
                }
            }
        }

        [Test]
        public static void ShadowingProperty()
        {
            var changes = new List<PropertyChangedEventArgs>();
            var withFake = new WithShadowing<int>();
            using (withFake.ObservePropertyChangedSlim(x => x.Value, signalInitial: false)
                           .Subscribe(changes.Add))
            {
                withFake.Value = 1;
                CollectionAssert.AreEqual(new[] { "Value" }, changes.Select(x => x.PropertyName));
            }
        }

        [Test]
        public static void ShadowingPropertyCreateObservableOnBase()
        {
            static IObservable<PropertyChangedEventArgs> CreateObservable(With<int> source)
            {
                return source.ObservePropertyChangedSlim(x => x.Value, signalInitial: false);
            }

            var changes = new List<PropertyChangedEventArgs>();
            var withFake = new WithShadowing<int>();
            using (CreateObservable(withFake)
                           .Subscribe(changes.Add))
            {
                withFake.Value = 1;
                CollectionAssert.AreEqual(new[] { "Value" }, changes.Select(x => x.PropertyName));
            }
        }

        [Test]
        public static void GenericMessOneLevel()
        {
            var changes = new List<PropertyChangedEventArgs>();
            var withFake = new With<AbstractFake>();
            using (withFake.ObservePropertyChangedSlim(x => x.Value, signalInitial: false)
                        .Subscribe(changes.Add))
            {
                withFake.Value = new ConcreteFake1();
                CollectionAssert.AreEqual(new[] { "Value" }, changes.Select(x => x.PropertyName));
            }
        }

        [Test]
        public static void GenericHelperMethodConstrainedToClassAndInterface()
        {
            static IObservable<PropertyChangedEventArgs> CreateObservable<T>(T source, Expression<Func<T, int>> path)
                where T : class, INotifyPropertyChanged
            {
                return source.ObservePropertyChangedSlim(path, signalInitial: false);
            }

            var changes = new List<PropertyChangedEventArgs>();
            var withFake = new With<int>();
            using (CreateObservable(withFake, x => x.Value)
                           .Subscribe(changes.Add))
            {
                withFake.Value++;
                CollectionAssert.AreEqual(new[] { "Value" }, changes.Select(x => x.PropertyName));
            }
        }

        [Test]
        public static void GenericHelperMethodConstrainedToAbstractType1()
        {
            static IObservable<PropertyChangedEventArgs> CreateObservable<T>(T source, Expression<Func<T, int>> path)
                where T : AbstractFake
            {
                return source.ObservePropertyChangedSlim(path, signalInitial: false);
            }

            var changes = new List<PropertyChangedEventArgs>();
            var fake = new ConcreteFake1();
            using (CreateObservable(fake, x => x.BaseValue).Subscribe(changes.Add))
            {
                fake.BaseValue++;
                CollectionAssert.AreEqual(new[] { "BaseValue" }, changes.Select(x => x.PropertyName));
            }
        }

        [Test]
        public static void GenericHelperMethodConstrainedToAbstractType2()
        {
            static IObservable<PropertyChangedEventArgs> CreateObservable<T>(T source)
                where T : AbstractFake
            {
                return source.ObservePropertyChangedSlim(x => x.BaseValue, signalInitial: false);
            }

            var changes = new List<PropertyChangedEventArgs>();
            var fake = new ConcreteFake1();
            using (CreateObservable(fake).Subscribe(changes.Add))
            {
                fake.BaseValue++;
                CollectionAssert.AreEqual(new[] { "BaseValue" }, changes.Select(x => x.PropertyName));
            }
        }

        [Test]
        public static void GenericHelperMethodConstrainedToClass1()
        {
            static IObservable<PropertyChangedEventArgs> CreateObservable<T>(T source, Expression<Func<T, int>> path)
                where T : With<AbstractFake>
            {
                return source.ObservePropertyChangedSlim(path, signalInitial: false);
            }

            var changes = new List<PropertyChangedEventArgs>();
            var withFake = new WithAbstractFake { Value = new ConcreteFake2() };
            using (CreateObservable(withFake, x => x.Value.BaseValue)
                .Subscribe(changes.Add))
            {
                withFake.Value.BaseValue++;
                CollectionAssert.AreEqual(new[] { "BaseValue" }, changes.Select(x => x.PropertyName));
            }
        }

        [Test]
        public static void GenericHelperMethodConstrainedToClass2()
        {
            static IObservable<PropertyChangedEventArgs> CreateObservable<T>(T source)
                where T : With<AbstractFake>
            {
                return source.ObservePropertyChangedSlim(x => x.Value.BaseValue, signalInitial: false);
            }

            var changes = new List<PropertyChangedEventArgs>();
            var withFake = new WithAbstractFake { Value = new ConcreteFake2() };
            using (CreateObservable(withFake)
                .Subscribe(changes.Add))
            {
                withFake.Value.BaseValue++;
                CollectionAssert.AreEqual(new[] { "BaseValue" }, changes.Select(x => x.PropertyName));

                withFake.Value = new ConcreteFake1();
                CollectionAssert.AreEqual(new[] { "BaseValue", "Value" }, changes.Select(x => x.PropertyName));

                withFake.Value.BaseValue++;
                CollectionAssert.AreEqual(new[] { "BaseValue", "Value", "BaseValue" }, changes.Select(x => x.PropertyName));
            }
        }

        [Test]
        public static void TwoSubscriptionsOneObservable()
        {
            var changes1 = new List<PropertyChangedEventArgs>();
            var changes2 = new List<PropertyChangedEventArgs>();
            var fake = new Fake { Next = new Level() };
            var observable = fake.ObservePropertyChangedSlim(x => x.Next.IsTrue, signalInitial: false);
            using (observable.Subscribe(changes1.Add))
            {
                using (observable.Subscribe(changes2.Add))
                {
                    Assert.AreEqual(0, changes1.Count);
                    Assert.AreEqual(0, changes2.Count);

                    fake.Next.IsTrue = !fake.Next.IsTrue;
                    Assert.AreEqual(1, changes1.Count);
                    Assert.AreEqual(1, changes2.Count);
                    Assert.AreEqual("IsTrue", changes1.Last().PropertyName);
                    Assert.AreEqual("IsTrue", changes2.Last().PropertyName);

                    fake.Next.IsTrue = !fake.Next.IsTrue;
                    Assert.AreEqual(2, changes1.Count);
                    Assert.AreEqual(2, changes2.Count);
                    Assert.AreEqual("IsTrue", changes1.Last().PropertyName);
                    Assert.AreEqual("IsTrue", changes2.Last().PropertyName);

                    fake.Next = null;
                    Assert.AreEqual(3, changes1.Count);
                    Assert.AreEqual(3, changes2.Count);
                    Assert.AreEqual("Next", changes1.Last().PropertyName);
                    Assert.AreEqual("Next", changes2.Last().PropertyName);
                }
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public static void ThrowsOnStructInPath(bool signalInitial)
        {
            var fake = new Fake();
            var exception =
                Assert.Throws<ArgumentException>(
#pragma warning disable GUREA03 // Path must notify.
                    () => fake.ObservePropertyChangedSlim(x => x.StructLevel.Name, signalInitial));
#pragma warning restore GUREA03 // Path must notify.
            var expected = "Error found in x => x.StructLevel.Name\r\n" +
                           "Property path cannot have structs in it. Copy by value will make subscribing error prone. Also mutable struct much?\r\n" +
                           "The type StructLevel is a value type not so StructLevel.Name will not notify when it changes.\r\n" +
                           "The path is: x => x.StructLevel.Name\r\n\r\n" +
                           "Parameter name: property";
            Assert.AreEqual(expected, exception.Message);
        }

        [TestCase(true)]
        [TestCase(false)]
        public static void ThrowsOnNotNotifyingPath(bool signalInitial)
        {
            var fake = new Fake();
            var exception =
                Assert.Throws<ArgumentException>(
#pragma warning disable GUREA03 // Path must notify.
                    () => fake.ObservePropertyChangedSlim(x => x.Name.Length, signalInitial));
#pragma warning restore GUREA03 // Path must notify.
            var expected = "Error found in x => x.Name.Length\r\n" +
                           "All levels in the path must implement INotifyPropertyChanged.\r\n" +
                           "The type string does not so Name.Length will not notify when it changes.\r\n" +
                           "The path is: x => x.Name.Length\r\n\r\n" +
                           "Parameter name: property";
            Assert.AreEqual(expected, exception.Message);
        }

        [Test]
        public static void ThrowsOnMethodInPath()
        {
            var fake = new Fake();
            var exception = Assert.Throws<ArgumentException>(
                () => fake.ObservePropertyChangedSlim(
                    x => x.Method()
                          .Name));
            Assert.AreEqual("Expected path to be properties only. Was x.Method().Name", exception.Message);
        }

        [Test]
        public static void Unsubscribes()
        {
            var changes = new List<PropertyChangedEventArgs>();
            var fake = new Fake();
            using (fake.ObservePropertyChangedSlim(x => x.Next.IsTrue, signalInitial: false)
                       .Subscribe(changes.Add))
            {
                fake.Next = new Level
                {
                    Next = new Level(),
                };

                Assert.AreEqual(1, changes.Count);
                var old = fake.Next;
                fake.Next = null;
                Assert.AreEqual(2, changes.Count);
                old.IsTrue = !old.IsTrue;
                Assert.AreEqual(2, changes.Count);
            }
        }

        [Test]
        public static void ReadOnlyObservableCollectionCount()
        {
            var ints = new ObservableCollection<int>();
            var source = new ReadOnlyObservableCollection<int>(ints);
            var values = new List<string>();
            using (source.ObservePropertyChangedSlim(x => x.Count, signalInitial: false)
                         .Subscribe(x => values.Add(x.PropertyName)))
            {
                CollectionAssert.IsEmpty(values);

                ints.Add(1);
                CollectionAssert.AreEqual(new[] { "Count" }, values);

                ints.Add(2);
                CollectionAssert.AreEqual(new[] { "Count", "Count" }, values);
            }
        }

        [TestCase(true, 1)]
        [TestCase(false, 0)]
        public static void SignalsInitialWhenHasValue(bool signalInitial, int expected)
        {
            var changes = new List<PropertyChangedEventArgs>();
            var fake = new Fake { Next = new Level() };
            using (fake.ObservePropertyChangedSlim(x => x.Next.Value, signalInitial)
                       .Subscribe(changes.Add))
            {
                Assert.AreEqual(expected, changes.Count);
                if (signalInitial)
                {
                    Assert.AreEqual(string.Empty, changes.Last().PropertyName);
                }

                fake.Next.Value++;
                Assert.AreEqual(expected + 1, changes.Count); // Double check that we are subscribing
            }
        }

        [TestCase(true, 1)]
        [TestCase(false, 0)]
        public static void SignalsInitialWhenHasValueGeneric(bool signalInitial, int expected)
        {
            var changes = new List<PropertyChangedEventArgs>();
            var fake = new Fake<int> { Next = new Level<int>() };
            using (fake.ObservePropertyChangedSlim(x => x.Next.Value, signalInitial)
                       .Subscribe(changes.Add))
            {
                Assert.AreEqual(expected, changes.Count);
                if (signalInitial)
                {
                    Assert.AreEqual(string.Empty, changes.Last().PropertyName);
                }

                fake.Next.Value++;
                Assert.AreEqual(expected + 1, changes.Count); // Double check that we are subscribing
            }
        }

        [TestCase(true, 1)]
        [TestCase(false, 0)]
        public static void SignalsInitialWhenNoValue(bool signalInitial, int expected)
        {
            var changes = new List<PropertyChangedEventArgs>();
            var fake = new Fake();
            using (fake.ObservePropertyChangedSlim(x => x.Next.Value, signalInitial)
                       .Subscribe(changes.Add))
            {
                Assert.AreEqual(expected, changes.Count);
                if (signalInitial)
                {
                    Assert.AreEqual(string.Empty, changes.Last().PropertyName);
                }

                fake.Next = new Level();
                Assert.AreEqual(expected + 1, changes.Count); // Double check that we are subscribing
            }
        }

        [TestCase(true, 1)]
        [TestCase(false, 0)]
        public static void SignalsInitialWhenNoValueGeneric(bool signalInitial, int expected)
        {
            var changes = new List<PropertyChangedEventArgs>();
            var fake = new Fake<int>();
            using (fake.ObservePropertyChangedSlim(x => x.Next.Value, signalInitial)
                       .Subscribe(changes.Add))
            {
                Assert.AreEqual(expected, changes.Count);
                if (signalInitial)
                {
                    Assert.AreEqual(string.Empty, changes.Last().PropertyName);
                }

                fake.Next = new Level<int>();
                Assert.AreEqual(expected + 1, changes.Count); // Double check that we are subscribing
            }
        }

        [TestCase("")]
        [TestCase(null)]
        public static void DoesNotReactToStringEmptyOrNullFromRootWhenNull(string propertyName)
        {
            var changes = new List<PropertyChangedEventArgs>();
            var fake = new Fake();
            using (fake.ObservePropertyChangedSlim(x => x.Next.IsTrue, signalInitial: false)
                       .Subscribe(changes.Add))
            {
                Assert.AreEqual(0, changes.Count);
                fake.OnPropertyChanged(propertyName);
                //// This means all properties changed according to wpf convention
                Assert.AreEqual(0, changes.Count);
            }
        }

        [TestCase("")]
        [TestCase(null)]
        public static void DoesReactToStringEmptyOrNullFromRootWhenNotNull(string propertyName)
        {
            var changes = new List<PropertyChangedEventArgs>();
            var fake = new Fake { Next = new Level() };
            using (fake.ObservePropertyChangedSlim(x => x.Next.IsTrue, signalInitial: false)
                       .Subscribe(changes.Add))
            {
                Assert.AreEqual(0, changes.Count);

                fake.OnPropertyChanged(propertyName);
                //// This means all properties changed according to wpf convention
                Assert.AreEqual(1, changes.Count);
                Assert.AreEqual(propertyName, changes.Last().PropertyName);
            }
        }

        [TestCase("")]
        [TestCase(null)]
        public static void ReactsOnStringEmptyOrNullFromSource(string propertyName)
        {
            var changes = new List<PropertyChangedEventArgs>();
            var fake = new Fake { Next = new Level() };
            using (fake.ObservePropertyChangedSlim(x => x.Next.IsTrue, signalInitial: false)
                       .Subscribe(changes.Add))
            {
                Assert.AreEqual(0, changes.Count);

                fake.Next.OnPropertyChanged(propertyName);

                // This means all properties changed according to wpf convention
                Assert.AreEqual(1, changes.Count);
                Assert.AreEqual(propertyName, changes.Last().PropertyName);
            }
        }

        [Test]
        public static void TwoLevelsReacts()
        {
            var changes = new List<PropertyChangedEventArgs>();
            var fake = new Fake();
            using (fake.ObservePropertyChangedSlim(x => x.Next.IsTrue, signalInitial: true)
                       .Subscribe(changes.Add))
            {
                Assert.AreEqual(1, changes.Count);
                Assert.AreEqual(string.Empty, changes.Last().PropertyName);

                fake.Next = new Level();
                Assert.AreEqual(2, changes.Count);
                Assert.AreEqual("Next", changes.Last().PropertyName);

                fake.Next.IsTrue = !fake.Next.IsTrue;
                Assert.AreEqual(3, changes.Count);
                Assert.AreEqual("IsTrue", changes.Last().PropertyName);

                fake.Next.IsTrue = !fake.Next.IsTrue;
                Assert.AreEqual(4, changes.Count);
                Assert.AreEqual("IsTrue", changes.Last().PropertyName);
            }

            fake.Next.IsTrue = !fake.Next.IsTrue;
            Assert.AreEqual(4, changes.Count);
            Assert.AreEqual("IsTrue", changes.Last().PropertyName);
        }

        [Test]
        public static void TwoSubscriptionsTwoLevelsReacts()
        {
            var changes = new List<PropertyChangedEventArgs>();
            var intChanges = new List<PropertyChangedEventArgs>();
            var fake = new Fake();
            var intfake = new Fake<int>();
            using (fake.ObservePropertyChangedSlim(x => x.Next.Value, signalInitial: true)
                       .Subscribe(changes.Add))
            {
                using (intfake.ObservePropertyChangedSlim(x => x.Next.Value, signalInitial: true)
                              .Subscribe(intChanges.Add))
                {
                    Assert.AreEqual(1, changes.Count);
                    Assert.AreEqual(1, intChanges.Count);
                    Assert.AreEqual(string.Empty, changes.Last().PropertyName);
                    Assert.AreEqual(string.Empty, intChanges.Last().PropertyName);

                    fake.Next = new Level();
                    Assert.AreEqual(2, changes.Count);
                    Assert.AreEqual(1, intChanges.Count);
                    Assert.AreEqual("Next", changes.Last().PropertyName);
                    Assert.AreEqual(string.Empty, intChanges.Last().PropertyName);

                    fake.Next.Value++;
                    Assert.AreEqual(3, changes.Count);
                    Assert.AreEqual(1, intChanges.Count);
                    Assert.AreEqual("Value", changes.Last().PropertyName);
                    Assert.AreEqual(string.Empty, intChanges.Last().PropertyName);

                    intfake.Next = new Level<int>();
                    Assert.AreEqual(3, changes.Count);
                    Assert.AreEqual(2, intChanges.Count);
                    Assert.AreEqual("Value", changes.Last().PropertyName);
                    Assert.AreEqual("Next", intChanges.Last().PropertyName);

                    intfake.Next.Value++;
                    Assert.AreEqual(3, changes.Count);
                    Assert.AreEqual(3, intChanges.Count);
                    Assert.AreEqual("Value", changes.Last().PropertyName);
                    Assert.AreEqual("Value", intChanges.Last().PropertyName);
                }
            }
        }

        [Test]
        public static void TwoLevelsExisting()
        {
            var changes = new List<PropertyChangedEventArgs>();
            var fake = new Fake { Next = new Level { Next = new Level() } };
            using (fake.ObservePropertyChangedSlim(x => x.Next.Next, signalInitial: false)
                       .Subscribe(changes.Add))
            {
                fake.Next.Next = new Level();
                Assert.AreEqual(1, changes.Count);

                fake.Next.Next = null;
                Assert.AreEqual(2, changes.Count);

                fake.Next = null;
                Assert.AreEqual(3, changes.Count);

                fake.OnPropertyChanged("Next");
                Assert.AreEqual(3, changes.Count);
            }
        }

        [Test]
        public static void TwoLevelsExistingChangeLastValueInPath()
        {
            var changes = new List<PropertyChangedEventArgs>();
            var fake = new Fake { Next = new Level() };
            using (fake.ObservePropertyChangedSlim(x => x.Next.IsTrue, signalInitial: false)
                       .Subscribe(changes.Add))
            {
                fake.Next.IsTrue = !fake.Next.IsTrue;
                Assert.AreEqual(1, changes.Count);
            }
        }

        [Test]
        public static void TwoLevelsReferenceType()
        {
            var changes = new List<PropertyChangedEventArgs>();
            var fake = new Fake();
            using (fake.ObservePropertyChangedSlim(x => x.Next.Next, signalInitial: false)
                       .Subscribe(changes.Add))
            {
                CollectionAssert.IsEmpty(changes);

                fake.Next = new Level();
                CollectionAssert.AreEqual(new[] { "Next" }, changes.Select(x => x.PropertyName));

                fake.Next.Next = new Level();
                CollectionAssert.AreEqual(new[] { "Next", "Next" }, changes.Select(x => x.PropertyName));

                fake.Next.Next = null;
                CollectionAssert.AreEqual(new[] { "Next", "Next", "Next" }, changes.Select(x => x.PropertyName));

                fake.Next = null;
                CollectionAssert.AreEqual(new[] { "Next", "Next", "Next", "Next" }, changes.Select(x => x.PropertyName));
            }
        }

        [Test]
        public static void ThreeLevelsReferenceType()
        {
            var changes = new List<PropertyChangedEventArgs>();
            var fake = new Fake();
            using (fake.ObservePropertyChangedSlim(x => x.Next.Next.Next, signalInitial: false)
                       .Subscribe(changes.Add))
            {
                fake.Next = new Level();
                Assert.AreEqual(0, changes.Count);

                fake.Next.Next = new Level();
                Assert.AreEqual(1, changes.Count);

                fake.Next.Next.Next = new Level();
                Assert.AreEqual(2, changes.Count);

                fake.Next.Next.Next = null;
                Assert.AreEqual(3, changes.Count);

                fake.Next.Next = null;
                Assert.AreEqual(4, changes.Count);

                fake.Next = null;
                Assert.AreEqual(4, changes.Count);
            }
        }

        [Test]
        public static void TwoLevelsReactsGeneric()
        {
            var changes = new List<PropertyChangedEventArgs>();
            var fake = new Fake<int>();
            using (fake.ObservePropertyChangedSlim(x => x.Next.Value, signalInitial: true)
                       .Subscribe(changes.Add))
            {
                Assert.AreEqual(1, changes.Count);
                Assert.AreEqual(string.Empty, changes.Last().PropertyName);

                fake.Next = new Level<int>();
                Assert.AreEqual(2, changes.Count);
                Assert.AreEqual("Next", changes.Last().PropertyName);

                fake.Next.Value++;
                Assert.AreEqual(3, changes.Count);
                Assert.AreEqual("Value", changes.Last().PropertyName);

                fake.Next.Value++;
                Assert.AreEqual(4, changes.Count);
                Assert.AreEqual("Value", changes.Last().PropertyName);
            }

            fake.Next.Value++;
            Assert.AreEqual(4, changes.Count);
            Assert.AreEqual("Value", changes.Last().PropertyName);
        }

        [Test]
        public static void TwoLevelsReactsGenerics()
        {
            var intChanges = new List<PropertyChangedEventArgs>();
            var doubleChanges = new List<PropertyChangedEventArgs>();
            var intFake = new Fake<int>();
            var doubleFake = new Fake<double>();
            using (intFake.ObservePropertyChangedSlim(x => x.Next.Value, signalInitial: true)
                          .Subscribe(intChanges.Add))
            {
                using (doubleFake.ObservePropertyChangedSlim(x => x.Next.Value, signalInitial: true)
                                 .Subscribe(doubleChanges.Add))
                {
                    Assert.AreEqual(1, intChanges.Count);
                    Assert.AreEqual(1, doubleChanges.Count);
                    Assert.AreEqual(string.Empty, intChanges.Last().PropertyName);

                    intFake.Next = new Level<int>();
                    Assert.AreEqual(2, intChanges.Count);
                    Assert.AreEqual("Next", intChanges.Last().PropertyName);

                    intFake.Next.Value++;
                    Assert.AreEqual(3, intChanges.Count);
                    Assert.AreEqual("Value", intChanges.Last().PropertyName);

                    Assert.AreEqual(1, doubleChanges.Count);
                    Assert.AreEqual(string.Empty, doubleChanges.Last().PropertyName);

                    doubleFake.Next = new Level<double>();
                    Assert.AreEqual(2, doubleChanges.Count);
                    Assert.AreEqual("Next", doubleChanges.Last().PropertyName);

                    doubleFake.Next.Value++;
                    Assert.AreEqual(3, doubleChanges.Count);
                    Assert.AreEqual(3, intChanges.Count);
                    Assert.AreEqual("Value", doubleChanges.Last().PropertyName);
                }
            }
        }

        [Test]
        public static void TwoLevelsRootChangesFromValueToNull()
        {
            var changes = new List<PropertyChangedEventArgs>();
            var fake = new Fake { Next = new Level() };
            using (fake.ObservePropertyChangedSlim(x => x.Next.IsTrue, signalInitial: true)
                       .Subscribe(changes.Add))
            {
                Assert.AreEqual(1, changes.Count);
                Assert.AreEqual(string.Empty, changes.Last().PropertyName);

                fake.Next = null;
                Assert.AreEqual(2, changes.Count);
                Assert.AreEqual("Next", changes.Last().PropertyName);
            }
        }

        [Test]
        public static void TwoLevelsRootChangesFromNullToValue()
        {
            var changes = new List<PropertyChangedEventArgs>();
            var fake = new Fake();
            using (fake.ObservePropertyChangedSlim(x => x.Next.IsTrue, signalInitial: true)
                       .Subscribe(changes.Add))
            {
                Assert.AreEqual(1, changes.Count);
                Assert.AreEqual(string.Empty, changes.Last().PropertyName);

                fake.Next = new Level();
                Assert.AreEqual(2, changes.Count);
                Assert.AreEqual("Next", changes.Last().PropertyName);
            }
        }

        [Test]
        public static void TwoLevelsStartingWithValue()
        {
            var changes = new List<PropertyChangedEventArgs>();
            var fake = new Fake { Next = new Level() };
            using (fake.ObservePropertyChangedSlim(x => x.Next.IsTrue, signalInitial: true)
                       .Subscribe(changes.Add))
            {
                Assert.AreEqual(1, changes.Count);
                Assert.AreEqual(string.Empty, changes.Last().PropertyName);

                fake.Next.IsTrue = !fake.Next.IsTrue;
                Assert.AreEqual(2, changes.Count);
                Assert.AreEqual("IsTrue", changes.Last().PropertyName);
            }
        }

        [Test]
        public static void TwoLevelsStartingWithoutValue()
        {
            var changes = new List<PropertyChangedEventArgs>();
            var fake = new Fake();
            using (fake.ObservePropertyChangedSlim(x => x.Next.IsTrue, signalInitial: true)
                       .Subscribe(changes.Add))
            {
                Assert.AreEqual(1, changes.Count);
                Assert.AreEqual(string.Empty, changes.Last().PropertyName);

                fake.Next = new Level();
                Assert.AreEqual(2, changes.Count);
                Assert.AreEqual("Next", changes.Last().PropertyName);

                fake.Next.IsTrue = !fake.Next.IsTrue;
                Assert.AreEqual(3, changes.Count);
                Assert.AreEqual("IsTrue", changes.Last().PropertyName);
            }
        }

        [TestCase(true, null)]
        [TestCase(null, false)]
        [TestCase(null, true)]
        public static void TwoLevelsNullableStartingWithValue(bool? first, bool? other)
        {
            var changes = new List<PropertyChangedEventArgs>();
            var fake = new Fake { Next = new Level { IsTrueOrNull = first } };
            using (fake.ObservePropertyChangedSlim(x => x.Next.IsTrueOrNull, signalInitial: true)
                       .Subscribe(changes.Add))
            {
                Assert.AreEqual(1, changes.Count);
                Assert.AreEqual(string.Empty, changes.Last().PropertyName);

                fake.Next.IsTrueOrNull = other;
                Assert.AreEqual(2, changes.Count);
                Assert.AreEqual("IsTrueOrNull", changes.Last().PropertyName);
            }
        }

        [Test]
        public static void TwoLevelsValueType()
        {
            var changes = new List<PropertyChangedEventArgs>();
            var fake = new Fake();
            using (fake.ObservePropertyChangedSlim(x => x.Next.IsTrue, signalInitial: false)
                       .Subscribe(changes.Add))
            {
                fake.Next = new Level();
                Assert.AreEqual(1, changes.Count);
                fake.Next.IsTrue = !fake.Next.IsTrue;
                Assert.AreEqual(2, changes.Count);
                fake.Next = null;
                Assert.AreEqual(3, changes.Count);
            }
        }

        [Test]
        public static void ThreeLevelsStartingWithNullAddingOneByOne()
        {
            var changes = new List<PropertyChangedEventArgs>();
            var fake = new Fake();
            using (fake.ObservePropertyChangedSlim(x => x.Next.Next.IsTrue)
                       .Subscribe(changes.Add))
            {
                Assert.AreEqual(1, changes.Count);

                fake.Next = new Level { Name = "1" };
                Assert.AreEqual(1, changes.Count);

                fake.Next.Next = new Level { Name = "2" };
                Assert.AreEqual(2, changes.Count);
                Assert.AreEqual("Next", changes.Last().PropertyName);

                fake.Next.Next.IsTrue = !fake.Next.Next.IsTrue;
                Assert.AreEqual(3, changes.Count);
                Assert.AreEqual("IsTrue", changes.Last().PropertyName);

                fake.Next = null;
                Assert.AreEqual(4, changes.Count);
                Assert.AreEqual("Next", changes.Last().PropertyName);
            }
        }

        [Test]
        public static void ThreeLevelsStartingWithNullAddingTwo()
        {
            var changes = new List<PropertyChangedEventArgs>();
            var fake = new Fake();
            using (fake.ObservePropertyChangedSlim(x => x.Level1.Level2.IsTrue)
                       .Subscribe(changes.Add))
            {
                Assert.AreEqual(1, changes.Count);

                fake.Level1 = new Level1 { Name = "1", Level2 = new Level2 { Name = "2" } };
                Assert.AreEqual(2, changes.Count);
                Assert.AreEqual("Level1", changes.Last().PropertyName);

                fake.Level1.Level2.IsTrue = !fake.Level1.Level2.IsTrue;
                Assert.AreEqual(3, changes.Count);
                Assert.AreEqual("IsTrue", changes.Last().PropertyName);

                fake.Level1 = null;
                Assert.AreEqual(4, changes.Count);
                Assert.AreEqual("Level1", changes.Last().PropertyName);
            }
        }

        [Test]
        public static void ThreeLevelsStartingWithNullGeneric()
        {
            var changes = new List<PropertyChangedEventArgs>();
            var fake = new Fake();
            using (fake.ObservePropertyChangedSlim(x => x.Next.NextInt.Value)
                       .Subscribe(changes.Add))
            {
                Assert.AreEqual(1, changes.Count);
                fake.Next = new Level { NextInt = new Level<int>() };
                Assert.AreEqual(2, changes.Count);
                Assert.AreEqual("Next", changes.Last().PropertyName);

                fake.Next.NextInt.Value++;
                Assert.AreEqual(3, changes.Count);
                Assert.AreEqual("Value", changes.Last().PropertyName);

                fake.Next = null;
                Assert.AreEqual(4, changes.Count);
                Assert.AreEqual("Next", changes.Last().PropertyName);
            }
        }

        [Test]
        public static void ThreeLevelsStartingWithNullGenericGeneric()
        {
            var changes = new List<PropertyChangedEventArgs>();
            var fake = new Fake();
            using (fake.ObservePropertyChangedSlim(x => x.NextInt.Next.Value)
                       .Subscribe(changes.Add))
            {
                Assert.AreEqual(1, changes.Count);
                fake.NextInt = new Level<int> { Next = new Level<int>() };
                Assert.AreEqual(2, changes.Count);
                Assert.AreEqual("NextInt", changes.Last().PropertyName);

                fake.NextInt.Next.Value++;
                Assert.AreEqual(3, changes.Count);
                Assert.AreEqual("Value", changes.Last().PropertyName);

                fake.NextInt = null;
                Assert.AreEqual(4, changes.Count);
                Assert.AreEqual("NextInt", changes.Last().PropertyName);
            }
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase("Next")]
        public static void FirstInPathSignalsWhenHasValue(string propertyName)
        {
            var changes = new List<PropertyChangedEventArgs>();
            var next = new Level();
            var fake = new Fake { Next = next };
            using (fake.ObservePropertyChangedSlim(x => x.Next.IsTrue, signalInitial: false)
                       .Subscribe(changes.Add))
            {
                fake.OnPropertyChanged(propertyName);
                Assert.AreEqual(1, changes.Count);
            }
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase("IsTrue")]
        public static void LastInPathSignalsEvent(string propertyName)
        {
            var changes = new List<PropertyChangedEventArgs>();
            var next = new Level();
            var fake = new Fake { Next = next };
            using (fake.ObservePropertyChangedSlim(x => x.Next.IsTrue, signalInitial: false)
                       .Subscribe(changes.Add))
            {
                fake.Next.OnPropertyChanged(propertyName);
                Assert.AreEqual(1, changes.Count);
                Assert.AreEqual(propertyName, changes.Last().PropertyName);
            }
        }

        [Test]
        public static void ThreeLevels()
        {
            var changes = new List<PropertyChangedEventArgs>();
            var fake = new Fake();
            using (fake.ObservePropertyChangedSlim(x => x.Next.Next.IsTrue, signalInitial: false)
                       .Subscribe(changes.Add))
            {
                fake.Next = new Level();
                Assert.AreEqual(0, changes.Count);
                fake.Next.Next = new Level
                {
                    Next = new Level(),
                };
                Assert.AreEqual(1, changes.Count);
                fake.Next.Next = null;
                Assert.AreEqual(2, changes.Count);
                fake.Next = null;
                Assert.AreEqual(2, changes.Count);
                fake.Next = new Level
                {
                    Next = new Level
                    {
                        IsTrue = true,
                    },
                };
                Assert.AreEqual(3, changes.Count);
                fake.Next.Next.IsTrue = false;
                Assert.AreEqual(4, changes.Count);
                fake.Next.Next = null;
                Assert.AreEqual(5, changes.Count);
                fake.Next = null;
                Assert.AreEqual(5, changes.Count);
            }
        }

        [Test]
        public static void ThreeLevelsExisting()
        {
            var changes = new List<PropertyChangedEventArgs>();
            var fake = new Fake { Next = new Level { Next = new Level() } };
            using (fake.ObservePropertyChangedSlim(x => x.Next.Next.IsTrue)
                       .Subscribe(changes.Add))
            {
                Assert.AreEqual(1, changes.Count);
                Assert.AreEqual(string.Empty, changes.Last().PropertyName);

                fake.Next.Next.IsTrue = !fake.Next.Next.IsTrue;
                Assert.AreEqual(2, changes.Count);
                Assert.AreEqual("IsTrue", changes.Last().PropertyName);
            }
        }

        [Test]
        public static void ThreeLevelsExistingLevelBecomesNull()
        {
            var changes = new List<PropertyChangedEventArgs>();
            var fake = new Fake { Next = new Level { Next = new Level() } };
            using (fake.ObservePropertyChangedSlim(x => x.Next.Next.IsTrue)
                       .Subscribe(changes.Add))
            {
                Assert.AreEqual(1, changes.Count);
                Assert.AreEqual(string.Empty, changes.Last().PropertyName);

                fake.Next = null;
                Assert.AreEqual(2, changes.Count);
                Assert.AreEqual("Next", changes.Last().PropertyName);
            }
        }

        [Test]
        public static void ThreeLevelsExistingLevelBecomesNew()
        {
            var changes = new List<PropertyChangedEventArgs>();
            var fake = new Fake { Next = new Level { Next = new Level() } };
            using (fake.ObservePropertyChangedSlim(x => x.Next.Next.IsTrue)
                       .Subscribe(changes.Add))
            {
                Assert.AreEqual(1, changes.Count);
                Assert.AreEqual(string.Empty, changes.Last().PropertyName);

                fake.Next = new Level { Next = new Level() };
                Assert.AreEqual(2, changes.Count);
                Assert.AreEqual("Next", changes.Last().PropertyName);
            }
        }

        [Test]
        public static void Reacts()
        {
            var changes = new List<PropertyChangedEventArgs>();
            var fake = new Fake();
            using (fake.ObservePropertyChangedSlim(x => x.Next.IsTrue)
                       .Subscribe(changes.Add))
            {
                Assert.AreEqual(1, changes.Count);
                Assert.AreEqual(string.Empty, changes.Last().PropertyName);

                fake.Next = new Level { IsTrue = false };
                Assert.AreEqual(2, changes.Count);
                Assert.AreEqual("Next", changes.Last().PropertyName);

                fake.Next.IsTrue = true;
                Assert.AreEqual(3, changes.Count);
                Assert.AreEqual("IsTrue", changes.Last().PropertyName);

                var level1 = fake.Next;
                fake.Next = null;
                Assert.AreEqual(4, changes.Count);
                Assert.AreEqual("Next", changes.Last().PropertyName);

                level1.IsTrue = !level1.IsTrue;
                Assert.AreEqual(4, changes.Count);
            }
        }

        [Test]
        public static void MemoryLeakRootDisposeTest()
        {
#if DEBUG
            Assert.Inconclusive("Debugger keeps things alive for the scope of the method.");
#endif
            var root = new Fake { Next = new Level() };
            var rootRef = new WeakReference(root);
            var levelRef = new WeakReference(root.Next);
            Assert.IsTrue(rootRef.IsAlive);
            var observable = root.ObservePropertyChangedSlim(x => x.Next.Name, signalInitial: false);
            using (var subscription = observable.Subscribe())
            {
            }

            GC.Collect();
            Assert.IsFalse(rootRef.IsAlive);
            Assert.IsFalse(levelRef.IsAlive);
        }

        [Test]
        public static void MemoryLeakRootNoDisposeTest()
        {
#if DEBUG
            Assert.Inconclusive("Debugger keeps things alive for the scope of the method.");
#endif
            var root = new Fake { Next = new Level() };
            var rootRef = new WeakReference(root);
            var levelRef = new WeakReference(root.Next);
            Assert.IsTrue(rootRef.IsAlive);
            var observable = root.ObservePropertyChangedSlim(x => x.Next.Name, signalInitial: false);
#pragma warning disable IDISP001  // Dispose created.
            var subscription = observable.Subscribe();
#pragma warning restore IDISP001  // Dispose created.

            GC.Collect();
            Assert.IsFalse(rootRef.IsAlive);
            Assert.IsFalse(levelRef.IsAlive);
        }

        [Test]
        public static void MemoryLeakLevelNoDisposeTest()
        {
#if DEBUG
            Assert.Inconclusive("Debugger keeps things alive for the scope of the method.");
#endif
            var root = new Fake { Next = new Level() };
            var rootRef = new WeakReference(root);
            var levelRef = new WeakReference(root.Next);
            Assert.IsTrue(rootRef.IsAlive);
            var observable = root.ObservePropertyChangedSlim(x => x.Next.Name, signalInitial: false);
#pragma warning disable IDISP001  // Dispose created.
            var subscription = observable.Subscribe();
#pragma warning restore IDISP001  // Dispose created.

            GC.Collect();
            Assert.IsFalse(rootRef.IsAlive);
            Assert.IsFalse(levelRef.IsAlive);
        }

        [Test]
        public static void MemoryLeakLevelDisposeTest()
        {
#if DEBUG
            Assert.Inconclusive("Debugger keeps things alive for the scope of the method.");
#endif
            var root = new Fake { Next = new Level() };
            var rootRef = new WeakReference(root);
            var levelRef = new WeakReference(root.Next);
            Assert.IsTrue(rootRef.IsAlive);
            var observable = root.ObservePropertyChangedSlim(x => x.Next.Name, signalInitial: false);
            using (var subscription = observable.Subscribe())
            {
            }

            GC.Collect();
            Assert.IsFalse(rootRef.IsAlive);
            Assert.IsFalse(levelRef.IsAlive);
        }
    }
}
