// ReSharper disable HeuristicUnreachableCode
namespace Gu.Reactive.Tests.NotifyPropertyChangedExt
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    public static class ObserveFullPropertyPathSlim
    {
        [Test]
        public static void ReactsTwoPropertiesSameInstance()
        {
            var actual = new List<PropertyChangedEventArgs>();
            var source = new Fake { Level1 = new Level1 { Value = 1 } };
            using (source.ObserveFullPropertyPathSlim(x => x.Level1.Value, signalInitial: false)
                         .Subscribe(actual.Add))
            {
                using (source.ObserveFullPropertyPathSlim(x => x.Level1.IsTrue, signalInitial: false)
                           .Subscribe(actual.Add))
                {
                    Assert.AreEqual(0, actual.Count);

                    source.Level1.Value++;
                    var expected = new List<string> { "Value" };
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    source.Level1.IsTrue = !source.IsTrue;
                    expected.Add("IsTrue");
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));
                }
            }
        }

        [Test]
        public static void ReactsTwoInstancesSameProperty()
        {
            var actual = new List<PropertyChangedEventArgs>();
            var source1 = new Fake { Level1 = new Level1() };
            using (source1.ObserveFullPropertyPathSlim(x => x.Level1.Value, signalInitial: false)
                          .Subscribe(actual.Add))
            {
                var source2 = new Fake { Level1 = new Level1() };
                using (source2.ObserveFullPropertyPathSlim(x => x.Level1.Value, signalInitial: false)
                            .Subscribe(actual.Add))
                {
                    Assert.AreEqual(0, actual.Count);

                    source1.Level1.Value++;
                    var expected = new List<string> { "Value" };
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    source2.Level1.Value++;
                    expected.Add("Value");
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));
                }
            }
        }

        [Test]
        public static void TwoSubscriptionsOneObservable()
        {
            var actual1 = new List<PropertyChangedEventArgs>();
            var actual2 = new List<PropertyChangedEventArgs>();
            var source = new Fake { Level1 = new Level1() };
            var observable = source.ObserveFullPropertyPathSlim(x => x.Level1.IsTrue, signalInitial: false);
            using (observable.Subscribe(actual1.Add))
            {
                using (observable.Subscribe(actual2.Add))
                {
                    Assert.AreEqual(0, actual1.Count);
                    Assert.AreEqual(0, actual2.Count);

                    source.Level1.IsTrue = !source.Level1.IsTrue;
                    var expected = new List<string> { "IsTrue" };
                    CollectionAssert.AreEqual(expected, actual1.Select(x => x.PropertyName));
                    CollectionAssert.AreEqual(expected, actual2.Select(x => x.PropertyName));

                    source.Level1.IsTrue = !source.Level1.IsTrue;
                    expected.Add("IsTrue");
                    CollectionAssert.AreEqual(expected, actual1.Select(x => x.PropertyName));
                    CollectionAssert.AreEqual(expected, actual2.Select(x => x.PropertyName));

                    source.Level1 = null;
                    expected.Add("Level1");
                    CollectionAssert.AreEqual(expected, actual1.Select(x => x.PropertyName));
                    CollectionAssert.AreEqual(expected, actual2.Select(x => x.PropertyName));
                }
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public static void ThrowsOnStructInPath(bool signalIntital)
        {
            var source = new Fake();
            var exception =
                Assert.Throws<ArgumentException>(
                    () => source.ObserveFullPropertyPathSlim(x => x.StructLevel.Name, signalIntital));
            var expected = "Error found in x => x.StructLevel.Name\r\n" +
                           "Property path cannot have structs in it. Copy by value will make subscribing error prone. Also mutable struct much?\r\n" +
                           "The type StructLevel is a value type not so StructLevel.Name will not notify when it changes.\r\n" +
                           "The path is: x => x.StructLevel.Name\r\n\r\n" +
                           "Parameter name: property";
            Assert.AreEqual(expected, exception.Message);
        }

        [TestCase(true)]
        [TestCase(false)]
        public static void ThrowsOnNotNotifyingnPath(bool signalIntital)
        {
            var source = new Fake();
            var exception = Assert.Throws<ArgumentException>(
                    () => source.ObserveFullPropertyPathSlim(x => x.Name.Length, signalIntital));
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
            var source = new Fake();
            var exception = Assert.Throws<ArgumentException>(
                () => source.ObserveFullPropertyPathSlim(
                    x => x.Method()
                          .Name));
            Assert.AreEqual("Expected path to be properties only. Was x.Method().Name", exception.Message);
        }

        [Test]
        public static void ThrowsOnSingleItemPath()
        {
            var source = new Fake();
            var exception = Assert.Throws<ArgumentException>(() => source.ObserveFullPropertyPathSlim(x => x.IsTrue));
            var expected = "Expected path to have more than one item.\r\n" +
                           "The path was x => x.IsTrue\r\n" +
                           "Did you mean to call ObservePropertyChangedSlim?\r\n" +
                           "Parameter name: property";
            Assert.AreEqual(expected, exception.Message);
        }

        [Test]
        public static void Unsubscribes()
        {
            var actual = new List<PropertyChangedEventArgs>();
            var source = new Fake();
            using (source.ObserveFullPropertyPathSlim(x => x.Level1.IsTrue, signalInitial: false)
                         .Subscribe(actual.Add))
            {
                CollectionAssert.IsEmpty(actual);
                source.Level1 = new Level1
                {
                    Level2 = new Level2(),
                };

                var expected = new List<string> { "Level1" };
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                var old = source.Level1;
                source.Level1 = null;
                expected.Add("Level1");
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                old.IsTrue = !old.IsTrue;
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public static void SignalsInitialWhenHasValue(bool signalInitial)
        {
            var actual = new List<PropertyChangedEventArgs>();
            var source = new Fake { Level1 = new Level1() };
            using (source.ObserveFullPropertyPathSlim(x => x.Level1.Value, signalInitial)
                         .Subscribe(actual.Add))
            {
                var expected = signalInitial
                                   ? new List<string> { string.Empty }
                                   : new List<string>();
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                // Double check that we are subscribing
                source.Level1.Value++;
                expected.Add("Value");
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public static void SignalsInitialWhenHasValueGeneric(bool signalInitial)
        {
            var actual = new List<PropertyChangedEventArgs>();
            var source = new Fake<int> { Next = new Level<int>() };
            using (source.ObserveFullPropertyPathSlim(x => x.Next.Value, signalInitial)
                         .Subscribe(actual.Add))
            {
                var expected = signalInitial
                                   ? new List<string> { string.Empty }
                                   : new List<string>();
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                // Double check that we are subscribing
                source.Next.Value++;
                expected.Add("Value");
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public static void SignalsInitialWhenNoValue(bool signalInitial)
        {
            var actual = new List<PropertyChangedEventArgs>();
            var source = new Fake();
            using (source.ObserveFullPropertyPathSlim(x => x.Level1.Value, signalInitial)
                         .Subscribe(actual.Add))
            {
                var expected = signalInitial
                                   ? new List<string> { string.Empty }
                                   : new List<string>();
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                // Double check that we are subscribing
                source.Level1 = new Level1();
                expected.Add("Level1");
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public static void SignalsInitialWhenNoValueGeneric(bool signalInitial)
        {
            var actual = new List<PropertyChangedEventArgs>();
            var source = new Fake<int>();
            using (source.ObserveFullPropertyPathSlim(x => x.Next.Value, signalInitial)
                         .Subscribe(actual.Add))
            {
                var expected = signalInitial
                                   ? new List<string> { string.Empty }
                                   : new List<string>();
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                // Double check that we are subscribing
                source.Next = new Level<int>();
                expected.Add("Next");
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));
            }
        }

        [TestCase("")]
        [TestCase(null)]
        public static void ReactToStringEmptyOrNullFromRootWhenNull(string propertyName)
        {
            var actual = new List<PropertyChangedEventArgs>();
            var source = new Fake();
            using (source.ObserveFullPropertyPathSlim(x => x.Level1.IsTrue, signalInitial: false)
                         .Subscribe(actual.Add))
            {
                Assert.AreEqual(0, actual.Count);
                source.OnPropertyChanged(propertyName);

                //// This means all properties changed according to wpf convention
                CollectionAssert.AreEqual(new[] { propertyName }, actual.Select(x => x.PropertyName));
            }
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("Level1")]
        public static void DoesReactToStringEmptyOrNullFromRootWhenNotNull(string propertyName)
        {
            var actual = new List<PropertyChangedEventArgs>();
            var source = new Fake { Level1 = new Level1() };
            using (source.ObserveFullPropertyPathSlim(x => x.Level1.IsTrue, signalInitial: false)
                         .Subscribe(actual.Add))
            {
                Assert.AreEqual(0, actual.Count);

                source.OnPropertyChanged(propertyName);
                //// This means all properties changed according to wpf convention
                CollectionAssert.AreEqual(new[] { propertyName }, actual.Select(x => x.PropertyName));
            }
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase("IsTrue")]
        public static void ReactsOnStringEmptyOrNullFromSourceWhenHasValue(string propertyName)
        {
            var actual = new List<PropertyChangedEventArgs>();
            var source = new Fake { Level1 = new Level1() };
            using (source.ObserveFullPropertyPathSlim(x => x.Level1.IsTrue, signalInitial: false)
                         .Subscribe(actual.Add))
            {
                Assert.AreEqual(0, actual.Count);

                source.Level1.OnPropertyChanged(propertyName);
                //// This means all properties changed according to wpf convention
                CollectionAssert.AreEqual(new[] { propertyName }, actual.Select(x => x.PropertyName));
            }
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase("Name")]
        public static void ReactsOnStringEmptyOrNullFromSourceWhenNull(string propertyName)
        {
            var actual = new List<PropertyChangedEventArgs>();
            var source = new Fake { Level1 = new Level1 { Name = null } };
            using (source.ObserveFullPropertyPathSlim(x => x.Level1.Name, signalInitial: false)
                         .Subscribe(actual.Add))
            {
                Assert.AreEqual(0, actual.Count);

                source.Level1.OnPropertyChanged(propertyName);
                //// This means all properties changed according to wpf convention
                CollectionAssert.AreEqual(new[] { propertyName }, actual.Select(x => x.PropertyName));
            }
        }

        [Test]
        public static void TwoLevelsReacts()
        {
            var actual = new List<PropertyChangedEventArgs>();
            var source = new Fake();
            using (source.ObserveFullPropertyPathSlim(x => x.Level1.IsTrue, signalInitial: true)
                         .Subscribe(actual.Add))
            {
                var expected = new List<string> { string.Empty };
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                source.Level1 = new Level1();
                expected.Add("Level1");
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                source.Level1.IsTrue = !source.Level1.IsTrue;
                expected.Add("IsTrue");
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));
            }
        }

        [Test]
        public static void TwoSubscriptionsTwoLevelsReacts()
        {
            var actual = new List<PropertyChangedEventArgs>();
            var intActual = new List<PropertyChangedEventArgs>();
            var source = new Fake();
            var intsource = new Fake<int>();
            using (source.ObserveFullPropertyPathSlim(x => x.Next.Value, signalInitial: true)
                         .Subscribe(actual.Add))
            {
                using (intsource.ObserveFullPropertyPathSlim(x => x.Next.Value, signalInitial: true)
                              .Subscribe(intActual.Add))
                {
                    CollectionAssert.AreEqual(new[] { string.Empty }, actual.Select(x => x.PropertyName));
                    CollectionAssert.AreEqual(new[] { string.Empty }, intActual.Select(x => x.PropertyName));

                    source.Next = new Level();
                    CollectionAssert.AreEqual(new[] { string.Empty, "Next" }, actual.Select(x => x.PropertyName));
                    CollectionAssert.AreEqual(new[] { string.Empty }, intActual.Select(x => x.PropertyName));

                    source.Next.Value++;
                    CollectionAssert.AreEqual(
                        new[] { string.Empty, "Next", "Value" },
                        actual.Select(x => x.PropertyName));
                    CollectionAssert.AreEqual(new[] { string.Empty }, intActual.Select(x => x.PropertyName));

                    intsource.Next = new Level<int>();
                    CollectionAssert.AreEqual(
                        new[] { string.Empty, "Next", "Value" },
                        actual.Select(x => x.PropertyName));
                    CollectionAssert.AreEqual(new[] { string.Empty, "Next" }, intActual.Select(x => x.PropertyName));

                    intsource.Next.Value++;
                    CollectionAssert.AreEqual(
                        new[] { string.Empty, "Next", "Value" },
                        actual.Select(x => x.PropertyName));
                    CollectionAssert.AreEqual(
                        new[] { string.Empty, "Next", "Value", },
                        intActual.Select(x => x.PropertyName));
                }
            }
        }

        [Test]
        public static void TwoLevelsExisting()
        {
            var actual = new List<PropertyChangedEventArgs>();
            var source = new Fake { Level1 = new Level1 { Level2 = new Level2() } };
            using (source.ObserveFullPropertyPathSlim(x => x.Level1.Level2, signalInitial: false)
                         .Subscribe(actual.Add))
            {
                CollectionAssert.IsEmpty(actual);

                source.Level1.Level2 = new Level2();
                var expected = new List<string> { "Level2" };
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                source.Level1.Level2 = null;
                expected.Add("Level2");
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                source.Level1 = null;
                expected.Add("Level1");
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                source.OnPropertyChanged(nameof(source.Level1));
                expected.Add("Level1");
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));
            }
        }

        [Test]
        public static void TwoLevelsExistsingChangeLastValueInPath()
        {
            var actual = new List<PropertyChangedEventArgs>();
            var source = new Fake { Next = new Level() };
            using (source.ObserveFullPropertyPathSlim(x => x.Next.IsTrue, signalInitial: false)
                         .Subscribe(actual.Add))
            {
                source.Next.IsTrue = !source.Next.IsTrue;
                CollectionAssert.AreEqual(new[] { "IsTrue" }, actual.Select(x => x.PropertyName));
            }
        }

        [Test]
        public static void TwoLevelsReferenceType()
        {
            var actual = new List<PropertyChangedEventArgs>();
            var source = new Fake();
            using (source.ObserveFullPropertyPathSlim(x => x.Next.Next, signalInitial: false)
                         .Subscribe(actual.Add))
            {
                CollectionAssert.IsEmpty(actual);

                source.Next = new Level();
                CollectionAssert.AreEqual(new[] { "Next" }, actual.Select(x => x.PropertyName));

                source.Next.Next = new Level();
                CollectionAssert.AreEqual(new[] { "Next", "Next" }, actual.Select(x => x.PropertyName));

                source.Next.Next = null;
                CollectionAssert.AreEqual(new[] { "Next", "Next", "Next" }, actual.Select(x => x.PropertyName));

                source.Next = null;
                CollectionAssert.AreEqual(
                    new[] { "Next", "Next", "Next", "Next" },
                    actual.Select(x => x.PropertyName));
            }
        }

        [Test]
        public static void ThreeLevelsReferenceType1()
        {
            var actual = new List<PropertyChangedEventArgs>();
            var source = new Fake();
            using (source.ObserveFullPropertyPathSlim(x => x.Next.Next.Next, signalInitial: false)
                         .Subscribe(actual.Add))
            {
                var expected = new List<string>();
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                source.Next = new Level();
                expected.Add("Next");
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                source.Next.Next = new Level();
                expected.Add("Next");
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                source.Next.Next.Next = new Level();
                expected.Add("Next");
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                source.Next.Next.Next = null;
                expected.Add("Next");
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                source.Next.Next = null;
                expected.Add("Next");
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                source.Next = null;
                expected.Add("Next");
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));
            }
        }

        [Test]
        public static void ThreeLevelsReferenceType2()
        {
            var actual = new List<PropertyChangedEventArgs>();
            var source = new Fake();
            using (source.ObserveFullPropertyPathSlim(x => x.Level1.Level2.Level3, signalInitial: false)
                         .Subscribe(actual.Add))
            {
                var expected = new List<string>();
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                source.Level1 = new Level1();
                expected.Add("Level1");
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                source.Level1.Level2 = new Level2();
                expected.Add("Level2");
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                source.Level1.Level2.Level3 = new Level3();
                expected.Add("Level3");
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                source.Level1.Level2.Level3 = null;
                expected.Add("Level3");
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                source.Level1.Level2 = null;
                expected.Add("Level2");
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                source.Level1 = null;
                expected.Add("Level1");
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));
            }
        }

        [Test]
        public static void TwoLevelsReactsGeneric()
        {
            var actual = new List<PropertyChangedEventArgs>();
            var source = new Fake<int>();
            using (source.ObserveFullPropertyPathSlim(x => x.Next.Value, signalInitial: true)
                         .Subscribe(actual.Add))
            {
                var expected = new List<string> { string.Empty };
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                source.Next = new Level<int>();
                expected.Add("Next");
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                source.Next.Value++;
                expected.Add("Value");
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));
            }
        }

        [Test]
        public static void TwoLevelsReactsGenerics()
        {
            var intActual = new List<PropertyChangedEventArgs>();
            var doubleActual = new List<PropertyChangedEventArgs>();
            var intFake = new Fake<int>();
            var doubleFake = new Fake<double>();
            using (intFake.ObserveFullPropertyPathSlim(x => x.Next.Value, signalInitial: true)
                          .Subscribe(intActual.Add))
            {
                using (doubleFake.ObserveFullPropertyPathSlim(x => x.Next.Value, signalInitial: true)
                                 .Subscribe(doubleActual.Add))
                {
                    var intExpected = new List<string> { string.Empty };
                    var doubleExpected = new List<string> { string.Empty };
                    CollectionAssert.AreEqual(intExpected, intActual.Select(x => x.PropertyName));
                    CollectionAssert.AreEqual(doubleExpected, doubleActual.Select(x => x.PropertyName));

                    intFake.Next = new Level<int>();
                    intExpected.Add("Next");
                    CollectionAssert.AreEqual(intExpected, intActual.Select(x => x.PropertyName));
                    CollectionAssert.AreEqual(doubleExpected, doubleActual.Select(x => x.PropertyName));

                    intFake.Next.Value++;
                    intExpected.Add("Value");
                    CollectionAssert.AreEqual(intExpected, intActual.Select(x => x.PropertyName));
                    CollectionAssert.AreEqual(doubleExpected, doubleActual.Select(x => x.PropertyName));

                    doubleFake.Next = new Level<double>();
                    doubleExpected.Add("Next");
                    CollectionAssert.AreEqual(intExpected, intActual.Select(x => x.PropertyName));
                    CollectionAssert.AreEqual(doubleExpected, doubleActual.Select(x => x.PropertyName));

                    doubleFake.Next.Value++;
                    doubleExpected.Add("Value");
                    CollectionAssert.AreEqual(intExpected, intActual.Select(x => x.PropertyName));
                    CollectionAssert.AreEqual(doubleExpected, doubleActual.Select(x => x.PropertyName));
                }
            }
        }

        [Test]
        public static void TwoLevelsRootChangesFromValueToNull()
        {
            var actual = new List<PropertyChangedEventArgs>();
            var source = new Fake { Next = new Level() };
            using (source.ObserveFullPropertyPathSlim(x => x.Next.IsTrue, signalInitial: true)
                         .Subscribe(actual.Add))
            {
                var expected = new List<string> { string.Empty };
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                source.Next = null;
                expected.Add("Next");
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));
            }
        }

        [Test]
        public static void TwoLevelsRootChangesFromNullToValue()
        {
            var actual = new List<PropertyChangedEventArgs>();
            var source = new Fake();
            using (source.ObserveFullPropertyPathSlim(x => x.Next.IsTrue, signalInitial: true)
                         .Subscribe(actual.Add))
            {
                var expected = new List<string> { string.Empty };
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                source.Next = new Level();
                expected.Add("Next");
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));
            }
        }

        [Test]
        public static void TwoLevelsStartingWithValue()
        {
            var actual = new List<PropertyChangedEventArgs>();
            var source = new Fake { Level1 = new Level1() };
            using (source.ObserveFullPropertyPathSlim(x => x.Level1.IsTrue, signalInitial: true)
                         .Subscribe(actual.Add))
            {
                var expected = new List<string> { string.Empty };
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                source.Level1.IsTrue = !source.Level1.IsTrue;
                expected.Add("IsTrue");
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));
            }
        }

        [Test]
        public static void TwoLevelsStartingWithoutValue()
        {
            var actual = new List<PropertyChangedEventArgs>();
            var source = new Fake();
            using (source.ObserveFullPropertyPathSlim(x => x.Next.IsTrue, signalInitial: true)
                         .Subscribe(actual.Add))
            {
                var expected = new List<string> { string.Empty };
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                source.Next = new Level();
                expected.Add("Next");
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                source.Next.IsTrue = !source.Next.IsTrue;
                expected.Add("IsTrue");
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));
            }
        }

        [TestCase(true, null)]
        [TestCase(null, false)]
        [TestCase(null, true)]
        public static void TwoLevelsNullableStartingWithValue(bool? first, bool? other)
        {
            var actual = new List<PropertyChangedEventArgs>();
            var source = new Fake { Next = new Level { IsTrueOrNull = first } };
            using (source.ObserveFullPropertyPathSlim(x => x.Next.IsTrueOrNull, signalInitial: true)
                         .Subscribe(actual.Add))
            {
                var expected = new List<string> { string.Empty };
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                source.Next.IsTrueOrNull = other;
                expected.Add("IsTrueOrNull");
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));
            }
        }

        [Test]
        public static void TwoLevelsValueType()
        {
            var actual = new List<PropertyChangedEventArgs>();
            var source = new Fake();
            using (source.ObserveFullPropertyPathSlim(x => x.Level1.IsTrue, signalInitial: false)
                         .Subscribe(actual.Add))
            {
                CollectionAssert.IsEmpty(actual);

                source.Level1 = new Level1();
                var expected = new List<string> { "Level1" };
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                source.Level1.IsTrue = !source.Level1.IsTrue;
                expected.Add("IsTrue");
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                source.Level1 = null;
                expected.Add("Level1");
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));
            }
        }

        [Test]
        public static void ThreeLevelsStartingWithFirstNullThenAddingLevelsOneByOne()
        {
            var actual = new List<PropertyChangedEventArgs>();
            var source = new Fake();
            using (source.ObserveFullPropertyPathSlim(x => x.Level1.Level2.IsTrue)
                         .Subscribe(actual.Add))
            {
                var expected = new List<string> { string.Empty };
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                source.Level1 = new Level1();
                expected.Add("Level1");
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                source.Level1.Level2 = new Level2();
                expected.Add("Level2");
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                source.Level1.Level2.IsTrue = !source.Level1.Level2.IsTrue;
                expected.Add("IsTrue");
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                source.Level1 = null;
                expected.Add("Level1");
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));
            }
        }

        [Test]
        public static void ThreeLevelsStartingWithFirstNullThenAddingTwoLevels()
        {
            var actual = new List<PropertyChangedEventArgs>();
            var source = new Fake();
            using (source.ObserveFullPropertyPathSlim(x => x.Level1.Level2.IsTrue)
                         .Subscribe(actual.Add))
            {
                var expected = new List<string> { string.Empty };
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                source.Level1 = new Level1 { Level2 = new Level2() };
                expected.Add("Level1");
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                source.Level1.Level2.IsTrue = !source.Level1.Level2.IsTrue;
                expected.Add("IsTrue");
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                source.Level1 = null;
                expected.Add("Level1");
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));
            }
        }

        [Test]
        public static void ThreeLevelsStartingWithNullGeneric()
        {
            var actual = new List<PropertyChangedEventArgs>();
            var source = new Fake();
            using (source.ObserveFullPropertyPathSlim(x => x.Next.NextInt.Value)
                         .Subscribe(actual.Add))
            {
                var expected = new List<string> { string.Empty };
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                source.Next = new Level { NextInt = new Level<int>() };
                expected.Add("Next");
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                source.Next.NextInt.Value++;
                expected.Add("Value");
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                source.Next = null;
                expected.Add("Next");
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));
            }
        }

        [Test]
        public static void ThreeLevelsStartingWithNullGenericGeneric()
        {
            var actual = new List<PropertyChangedEventArgs>();
            var source = new Fake();
            using (source.ObserveFullPropertyPathSlim(x => x.NextInt.Next.Value)
                         .Subscribe(actual.Add))
            {
                var expected = new List<string> { string.Empty };
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                source.NextInt = new Level<int> { Next = new Level<int>() };
                expected.Add("NextInt");
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                source.NextInt.Next.Value++;
                expected.Add("Value");
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                source.NextInt = null;
                expected.Add("NextInt");
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));
            }
        }

        [Test]
        public static void FourLevelsStartingWithNullAfterFirstThenAddingLevelsOneByOne()
        {
            var actual = new List<PropertyChangedEventArgs>();
            var source = new Fake();
            using (source.ObserveFullPropertyPathSlim(x => x.Level1.Level2.Level3.Value)
                         .Subscribe(actual.Add))
            {
                var expected = new List<string> { string.Empty };
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                source.Level1 = new Level1();
                expected.Add("Level1");
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                source.Level1.Level2 = new Level2();
                expected.Add("Level2");
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                source.Level1.Level2.Level3 = new Level3();
                expected.Add("Level3");
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                source.Level1.Level2.Level3.Value++;
                expected.Add("Value");
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                source.Level1 = null;
                expected.Add("Level1");
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));
            }
        }

        [Test]
        public static void FourLevelsStartingWithNullAfterFirstThenAddingTwoLevels()
        {
            var actual = new List<PropertyChangedEventArgs>();
            var source = new Fake();
            using (source.ObserveFullPropertyPathSlim(x => x.Level1.Level2.Level3.Value)
                         .Subscribe(actual.Add))
            {
                var expected = new List<string> { string.Empty };
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                source.Level1 = new Level1 { Level2 = new Level2 { Level3 = new Level3() } };
                expected.Add("Level1");
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                source.Level1.Level2.Level3.Value++;
                expected.Add("Value");
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                source.Level1 = null;
                expected.Add("Level1");
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));
            }
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase("Level1")]
        public static void FirstInPathSignalsWhenHasValue(string propertyName)
        {
            var actual = new List<PropertyChangedEventArgs>();
            var source = new Fake { Level1 = new Level1() };
            using (source.ObserveFullPropertyPathSlim(x => x.Level1.IsTrue, signalInitial: false)
                         .Subscribe(actual.Add))
            {
                source.OnPropertyChanged(propertyName);
                CollectionAssert.AreEqual(new[] { propertyName }, actual.Select(x => x.PropertyName));
            }
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase("IsTrue")]
        public static void LastInPathInPathSignalsWhenHasValue(string propertyName)
        {
            var actual = new List<PropertyChangedEventArgs>();
            var source = new Fake { Level1 = new Level1() };
            using (source.ObserveFullPropertyPathSlim(x => x.Level1.IsTrue, signalInitial: false)
                         .Subscribe(actual.Add))
            {
                source.Level1.OnPropertyChanged(propertyName);
                CollectionAssert.AreEqual(new[] { propertyName }, actual.Select(x => x.PropertyName));
            }
        }

        [Test]
        public static void ThreeLevels()
        {
            var actual = new List<PropertyChangedEventArgs>();
            var source = new Fake();
            using (source.ObserveFullPropertyPathSlim(x => x.Level1.Level2.Value, signalInitial: false)
                         .Subscribe(actual.Add))
            {
                source.Level1 = new Level1();
                var expected = new List<string> { "Level1" };
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                source.Level1.Level2 = new Level2();
                expected.Add("Level2");
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                source.Level1.Level2 = null;
                expected.Add("Level2");
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                source.Level1 = null;
                expected.Add("Level1");
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                source.Level1 = new Level1 { Level2 = new Level2 { Value = 1 } };
                expected.Add("Level1");
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                source.Level1.Level2.Value++;
                expected.Add("Value");
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                source.Level1.Level2 = null;
                expected.Add("Level2");
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                source.Level1 = null;
                expected.Add("Level1");
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));
            }
        }

        [Test]
        public static void ThreeLevelsExisting()
        {
            var actual = new List<PropertyChangedEventArgs>();
            var source = new Fake { Level1 = new Level1 { Level2 = new Level2() } };
            using (source.ObserveFullPropertyPathSlim(x => x.Level1.Level2.Value)
                         .Subscribe(actual.Add))
            {
                var expected = new List<string> { string.Empty };
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                source.Level1.Level2.Value++;
                expected.Add("Value");
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));
            }
        }

        [Test]
        public static void ThreeLevelsExistingLevelBecomesNull()
        {
            var actual = new List<PropertyChangedEventArgs>();
            var source = new Fake { Level1 = new Level1 { Level2 = new Level2() } };
            using (source.ObserveFullPropertyPathSlim(x => x.Level1.Level2.IsTrue)
                         .Subscribe(actual.Add))
            {
                var expected = new List<string> { string.Empty };
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                source.Level1 = null;
                expected.Add("Level1");
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));
            }
        }

        [Test]
        public static void ThreeLevelsExistingLevelBecomesNew()
        {
            var actual = new List<PropertyChangedEventArgs>();
            var source = new Fake { Level1 = new Level1 { Level2 = new Level2() } };
            using (source.ObserveFullPropertyPathSlim(x => x.Level1.Level2.IsTrue)
                         .Subscribe(actual.Add))
            {
                var expected = new List<string> { string.Empty };
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                source.Level1 = new Level1 { Level2 = new Level2() };
                expected.Add("Level1");
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));
            }
        }

        [Test]
        public static void Reacts()
        {
            var actual = new List<PropertyChangedEventArgs>();
            var source = new Fake();
            using (source.ObserveFullPropertyPathSlim(x => x.Next.IsTrue)
                         .Subscribe(actual.Add))
            {
                var expected = new List<string> { string.Empty };
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                source.Next = new Level { IsTrue = false };
                Assert.AreEqual(2, actual.Count);
                expected.Add("Next");
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                source.Next.IsTrue = true;
                Assert.AreEqual(3, actual.Count);
                expected.Add("IsTrue");
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                var level1 = source.Next;
                source.Next = null;
                Assert.AreEqual(4, actual.Count);
                expected.Add("Next");
                CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                level1.IsTrue = !level1.IsTrue;
                Assert.AreEqual(4, actual.Count);
            }
        }

        [Test]
        [SuppressMessage("ReSharper", "UnusedVariable")]
        public static void MemoryLeakRootDisposeTest()
        {
#if DEBUG
            Assert.Inconclusive("Debugger keeps things alive for the scope of the method.");
#endif
            var root = new Fake { Next = new Level() };
            var rootRef = new WeakReference(root);
            var levelRef = new WeakReference(root.Next);
            Assert.IsTrue(rootRef.IsAlive);
            var observable = root.ObserveFullPropertyPathSlim(x => x.Next.Name, signalInitial: false);
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
            var observable = root.ObserveFullPropertyPathSlim(x => x.Next.Name, signalInitial: false);
#pragma warning disable IDISP001  // Dispose created.
            //// ReSharper disable once UnusedVariable
            var subscription = observable.Subscribe();
#pragma warning restore IDISP001  // Dispose created.

            GC.Collect();
            Assert.IsFalse(rootRef.IsAlive);
            Assert.IsFalse(levelRef.IsAlive);
        }

        [Test]
        [SuppressMessage("ReSharper", "UnusedVariable")]
        public static void MemoryLeakLevelNoDisposeTest()
        {
#if DEBUG
            Assert.Inconclusive("Debugger keeps things alive for the scope of the method.");
#endif
            var root = new Fake { Next = new Level() };
            var rootRef = new WeakReference(root);
            var levelRef = new WeakReference(root.Next);
            Assert.IsTrue(rootRef.IsAlive);
            var observable = root.ObserveFullPropertyPathSlim(x => x.Next.Name, signalInitial: false);
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
            var observable = root.ObserveFullPropertyPathSlim(x => x.Next.Name, signalInitial: false);
            //// ReSharper disable once UnusedVariable
            using (var subscription = observable.Subscribe())
            {
            }

            GC.Collect();
            Assert.IsFalse(rootRef.IsAlive);
            Assert.IsFalse(levelRef.IsAlive);
        }
    }
}
