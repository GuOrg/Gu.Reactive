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

    public partial class NotifyPropertyChangedExtTests
    {
        public class ObserveFullPropertyPathSlim
        {
            [Test]
            public void ReactsTwoPropertiesSameInstance()
            {
                var actual = new List<PropertyChangedEventArgs>();
                var fake = new Fake { Level1 = new Level1 { Value = 1 } };
                using (fake.ObserveFullPropertyPathSlim(x => x.Level1.Value, signalInitial: false)
                           .Subscribe(actual.Add))
                {
                    using (fake.ObserveFullPropertyPathSlim(x => x.Level1.IsTrue, signalInitial: false)
                               .Subscribe(actual.Add))
                    {
                        Assert.AreEqual(0, actual.Count);

                        fake.Level1.Value++;
                        var expected = new List<string> { "Value" };
                        CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                        fake.Level1.IsTrue = !fake.IsTrue;
                        expected.Add("IsTrue");
                        CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));
                    }
                }
            }

            [Test]
            public void ReactsTwoInstancesSameProperty()
            {
                var actual = new List<PropertyChangedEventArgs>();
                var fake1 = new Fake { Level1 = new Level1() };
                using (fake1.ObserveFullPropertyPathSlim(x => x.Level1.Value, signalInitial: false)
                            .Subscribe(actual.Add))
                {
                    var fake2 = new Fake { Level1 = new Level1() };
                    using (fake2.ObserveFullPropertyPathSlim(x => x.Level1.Value, signalInitial: false)
                                .Subscribe(actual.Add))
                    {
                        Assert.AreEqual(0, actual.Count);

                        fake1.Level1.Value++;
                        var expected = new List<string> { "Value" };
                        CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                        fake2.Level1.Value++;
                        expected.Add("Value");
                        CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));
                    }
                }
            }

            [Test]
            public void TwoSubscriptionsOneObservable()
            {
                var actual1 = new List<PropertyChangedEventArgs>();
                var actual2 = new List<PropertyChangedEventArgs>();
                var fake = new Fake { Level1 = new Level1() };
                var observable = fake.ObserveFullPropertyPathSlim(x => x.Level1.IsTrue, signalInitial: false);
                using (observable.Subscribe(actual1.Add))
                {
                    using (observable.Subscribe(actual2.Add))
                    {
                        Assert.AreEqual(0, actual1.Count);
                        Assert.AreEqual(0, actual2.Count);

                        fake.Level1.IsTrue = !fake.Level1.IsTrue;
                        var expected = new List<string> { "IsTrue" };
                        CollectionAssert.AreEqual(expected, actual1.Select(x => x.PropertyName));
                        CollectionAssert.AreEqual(expected, actual2.Select(x => x.PropertyName));

                        fake.Level1.IsTrue = !fake.Level1.IsTrue;
                        expected.Add("IsTrue");
                        CollectionAssert.AreEqual(expected, actual1.Select(x => x.PropertyName));
                        CollectionAssert.AreEqual(expected, actual2.Select(x => x.PropertyName));

                        fake.Level1 = null;
                        expected.Add("Level1");
                        CollectionAssert.AreEqual(expected, actual1.Select(x => x.PropertyName));
                        CollectionAssert.AreEqual(expected, actual2.Select(x => x.PropertyName));
                    }
                }
            }

            [TestCase(true)]
            [TestCase(false)]
            public void ThrowsOnStructInPath(bool signalIntital)
            {
                var fake = new Fake();
                var exception =
                    Assert.Throws<ArgumentException>(
                        () => fake.ObserveFullPropertyPathSlim(x => x.StructLevel.Name, signalIntital));
                var expected = "Error found in x => x.StructLevel.Name\r\n" +
                               "Property path cannot have structs in it. Copy by value will make subscribing error prone. Also mutable struct much?\r\n" +
                               "The type StructLevel is a value type not so StructLevel.Name will not notify when it changes.\r\n" +
                               "The path is: x => x.StructLevel.Name\r\n\r\n" +
                               "Parameter name: property";
                Assert.AreEqual(expected, exception.Message);
            }

            [TestCase(true)]
            [TestCase(false)]
            public void ThrowsOnNotNotifyingnPath(bool signalIntital)
            {
                var fake = new Fake();
                var exception =
                    Assert.Throws<ArgumentException>(
                        () => fake.ObserveFullPropertyPathSlim(x => x.Name.Length, signalIntital));
                var expected = "Error found in x => x.Name.Length\r\n" +
                               "All levels in the path must implement INotifyPropertyChanged.\r\n" +
                               "The type string does not so Name.Length will not notify when it changes.\r\n" +
                               "The path is: x => x.Name.Length\r\n\r\n" +
                               "Parameter name: property";
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public void ThrowsOnMethodInPath()
            {
                var fake = new Fake();
                var exception = Assert.Throws<ArgumentException>(
                    () => fake.ObserveFullPropertyPathSlim(
                        x => x.Method()
                              .Name));
                Assert.AreEqual("Expected path to be properties only. Was x.Method().Name", exception.Message);
            }

            [Test]
            public void Unsubscribes()
            {
                var actual = new List<PropertyChangedEventArgs>();
                var fake = new Fake();
                using (fake.ObserveFullPropertyPathSlim(x => x.Level1.IsTrue, signalInitial: false)
                           .Subscribe(actual.Add))
                {
                    CollectionAssert.IsEmpty(actual);
                    fake.Level1 = new Level1
                    {
                        Level2 = new Level2()
                    };

                    var expected = new List<string> { "Level1" };
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    var old = fake.Level1;
                    fake.Level1 = null;
                    expected.Add("Level1");
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    old.IsTrue = !old.IsTrue;
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));
                }
            }

            [TestCase(true)]
            [TestCase(false)]
            public void SignalsInitialWhenHasValue(bool signalInitial)
            {
                var actual = new List<PropertyChangedEventArgs>();
                var fake = new Fake { Level1 = new Level1() };
                using (fake.ObserveFullPropertyPathSlim(x => x.Level1.Value, signalInitial)
                           .Subscribe(actual.Add))
                {
                    var expected = signalInitial
                                       ? new List<string> { string.Empty }
                                       : new List<string>();
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    // Double check that we are subscribing
                    fake.Level1.Value++;
                    expected.Add("Value");
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));
                }
            }

            [TestCase(true)]
            [TestCase(false)]
            public void SignalsInitialWhenHasValueGeneric(bool signalInitial)
            {
                var actual = new List<PropertyChangedEventArgs>();
                var fake = new Fake<int> { Next = new Level<int>() };
                using (fake.ObserveFullPropertyPathSlim(x => x.Next.Value, signalInitial)
                           .Subscribe(actual.Add))
                {
                    var expected = signalInitial
                                       ? new List<string> { string.Empty }
                                       : new List<string>();
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    // Double check that we are subscribing
                    fake.Next.Value++;
                    expected.Add("Value");
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));
                }
            }

            [TestCase(true)]
            [TestCase(false)]
            public void SignalsInitialWhenNoValue(bool signalInitial)
            {
                var actual = new List<PropertyChangedEventArgs>();
                var fake = new Fake();
                using (fake.ObserveFullPropertyPathSlim(x => x.Level1.Value, signalInitial)
                           .Subscribe(actual.Add))
                {
                    var expected = signalInitial
                                       ? new List<string> { string.Empty }
                                       : new List<string>();
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    // Double check that we are subscribing
                    fake.Level1 = new Level1();
                    expected.Add("Level1");
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));
                }
            }

            [TestCase(true)]
            [TestCase(false)]
            public void SignalsInitialWhenNoValueGeneric(bool signalInitial)
            {
                var actual = new List<PropertyChangedEventArgs>();
                var fake = new Fake<int>();
                using (fake.ObserveFullPropertyPathSlim(x => x.Next.Value, signalInitial)
                           .Subscribe(actual.Add))
                {
                    var expected = signalInitial
                                       ? new List<string> { string.Empty }
                                       : new List<string>();
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    // Double check that we are subscribing
                    fake.Next = new Level<int>();
                    expected.Add("Next");
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));
                }
            }

            [TestCase("")]
            [TestCase(null)]
            public void ReactToStringEmptyOrNullFromRootWhenNull(string propertyName)
            {
                var actual = new List<PropertyChangedEventArgs>();
                var fake = new Fake();
                using (fake.ObserveFullPropertyPathSlim(x => x.Level1.IsTrue, signalInitial: false)
                           .Subscribe(actual.Add))
                {
                    Assert.AreEqual(0, actual.Count);
                    fake.OnPropertyChanged(propertyName);

                    //// This means all properties changed according to wpf convention
                    CollectionAssert.AreEqual(new[] { propertyName }, actual.Select(x => x.PropertyName));
                }
            }

            [TestCase(null)]
            [TestCase("")]
            [TestCase("Level1")]
            public void DoesReactToStringEmptyOrNullFromRootWhenNotNull(string propertyName)
            {
                var actual = new List<PropertyChangedEventArgs>();
                var fake = new Fake { Level1 = new Level1() };
                using (fake.ObserveFullPropertyPathSlim(x => x.Level1.IsTrue, signalInitial: false)
                           .Subscribe(actual.Add))
                {
                    Assert.AreEqual(0, actual.Count);

                    fake.OnPropertyChanged(propertyName);
                    //// This means all properties changed according to wpf convention
                    CollectionAssert.AreEqual(new[] { propertyName }, actual.Select(x => x.PropertyName));
                }
            }

            [TestCase("")]
            [TestCase(null)]
            [TestCase("IsTrue")]
            public void ReactsOnStringEmptyOrNullFromSourceWhenHasValue(string propertyName)
            {
                var actual = new List<PropertyChangedEventArgs>();
                var fake = new Fake { Level1 = new Level1() };
                using (fake.ObserveFullPropertyPathSlim(x => x.Level1.IsTrue, signalInitial: false)
                           .Subscribe(actual.Add))
                {
                    Assert.AreEqual(0, actual.Count);

                    fake.Level1.OnPropertyChanged(propertyName);
                    //// This means all properties changed according to wpf convention
                    CollectionAssert.AreEqual(new[] { propertyName }, actual.Select(x => x.PropertyName));
                }
            }

            [TestCase("")]
            [TestCase(null)]
            [TestCase("Name")]
            public void ReactsOnStringEmptyOrNullFromSourceWhenNull(string propertyName)
            {
                var actual = new List<PropertyChangedEventArgs>();
                var fake = new Fake { Level1 = new Level1 { Name = null } };
                using (fake.ObserveFullPropertyPathSlim(x => x.Level1.Name, signalInitial: false)
                           .Subscribe(actual.Add))
                {
                    Assert.AreEqual(0, actual.Count);

                    fake.Level1.OnPropertyChanged(propertyName);
                    //// This means all properties changed according to wpf convention
                    CollectionAssert.AreEqual(new[] { propertyName }, actual.Select(x => x.PropertyName));
                }
            }

            [Test]
            public void TwoLevelsReacts()
            {
                var actual = new List<PropertyChangedEventArgs>();
                var fake = new Fake();
                using (fake.ObserveFullPropertyPathSlim(x => x.Level1.IsTrue, signalInitial: true)
                           .Subscribe(actual.Add))
                {
                    var expected = new List<string> { string.Empty };
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    fake.Level1 = new Level1();
                    expected.Add("Level1");
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    fake.Level1.IsTrue = !fake.Level1.IsTrue;
                    expected.Add("IsTrue");
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));
                }
            }

            [Test]
            public void TwoSubscriptionsTwoLevelsReacts()
            {
                var actual = new List<PropertyChangedEventArgs>();
                var intActual = new List<PropertyChangedEventArgs>();
                var fake = new Fake();
                var intfake = new Fake<int>();
                using (fake.ObserveFullPropertyPathSlim(x => x.Next.Value, signalInitial: true)
                           .Subscribe(actual.Add))
                {
                    using (intfake.ObserveFullPropertyPathSlim(x => x.Next.Value, signalInitial: true)
                                  .Subscribe(intActual.Add))
                    {
                        CollectionAssert.AreEqual(new[] { string.Empty }, actual.Select(x => x.PropertyName));
                        CollectionAssert.AreEqual(new[] { string.Empty }, intActual.Select(x => x.PropertyName));

                        fake.Next = new Level();
                        CollectionAssert.AreEqual(new[] { string.Empty, "Next" }, actual.Select(x => x.PropertyName));
                        CollectionAssert.AreEqual(new[] { string.Empty }, intActual.Select(x => x.PropertyName));

                        fake.Next.Value++;
                        CollectionAssert.AreEqual(
                            new[] { string.Empty, "Next", "Value" },
                            actual.Select(x => x.PropertyName));
                        CollectionAssert.AreEqual(new[] { string.Empty }, intActual.Select(x => x.PropertyName));

                        intfake.Next = new Level<int>();
                        CollectionAssert.AreEqual(
                            new[] { string.Empty, "Next", "Value" },
                            actual.Select(x => x.PropertyName));
                        CollectionAssert.AreEqual(new[] { string.Empty, "Next" }, intActual.Select(x => x.PropertyName));

                        intfake.Next.Value++;
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
            public void TwoLevelsExisting()
            {
                var actual = new List<PropertyChangedEventArgs>();
                var fake = new Fake { Level1 = new Level1 { Level2 = new Level2() } };
                using (fake.ObserveFullPropertyPathSlim(x => x.Level1.Level2, signalInitial: false)
                           .Subscribe(actual.Add))
                {
                    CollectionAssert.IsEmpty(actual);

                    fake.Level1.Level2 = new Level2();
                    var expected = new List<string> { "Level2" };
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    fake.Level1.Level2 = null;
                    expected.Add("Level2");
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    fake.Level1 = null;
                    expected.Add("Level1");
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    fake.OnPropertyChanged(nameof(fake.Level1));
                    expected.Add("Level1");
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));
                }
            }

            [Test]
            public void TwoLevelsExistsingChangeLastValueInPath()
            {
                var actual = new List<PropertyChangedEventArgs>();
                var fake = new Fake { Next = new Level() };
                using (fake.ObserveFullPropertyPathSlim(x => x.Next.IsTrue, signalInitial: false)
                           .Subscribe(actual.Add))
                {
                    fake.Next.IsTrue = !fake.Next.IsTrue;
                    CollectionAssert.AreEqual(new[] { "IsTrue" }, actual.Select(x => x.PropertyName));
                }
            }

            [Test]
            public void TwoLevelsReferenceType()
            {
                var actual = new List<PropertyChangedEventArgs>();
                var fake = new Fake();
                using (fake.ObserveFullPropertyPathSlim(x => x.Next.Next, signalInitial: false)
                           .Subscribe(actual.Add))
                {
                    CollectionAssert.IsEmpty(actual);

                    fake.Next = new Level();
                    CollectionAssert.AreEqual(new[] { "Next" }, actual.Select(x => x.PropertyName));

                    fake.Next.Next = new Level();
                    CollectionAssert.AreEqual(new[] { "Next", "Next" }, actual.Select(x => x.PropertyName));

                    fake.Next.Next = null;
                    CollectionAssert.AreEqual(new[] { "Next", "Next", "Next" }, actual.Select(x => x.PropertyName));

                    fake.Next = null;
                    CollectionAssert.AreEqual(
                        new[] { "Next", "Next", "Next", "Next" },
                        actual.Select(x => x.PropertyName));
                }
            }

            [Test]
            public void ThreeLevelsReferenceType1()
            {
                var actual = new List<PropertyChangedEventArgs>();
                var fake = new Fake();
                using (fake.ObserveFullPropertyPathSlim(x => x.Next.Next.Next, signalInitial: false)
                           .Subscribe(actual.Add))
                {
                    var expected = new List<string>();
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    fake.Next = new Level();
                    expected.Add("Next");
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    fake.Next.Next = new Level();
                    expected.Add("Next");
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    fake.Next.Next.Next = new Level();
                    expected.Add("Next");
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    fake.Next.Next.Next = null;
                    expected.Add("Next");
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    fake.Next.Next = null;
                    expected.Add("Next");
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    fake.Next = null;
                    expected.Add("Next");
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));
                }
            }

            [Test]
            public void ThreeLevelsReferenceType2()
            {
                var actual = new List<PropertyChangedEventArgs>();
                var fake = new Fake();
                using (fake.ObserveFullPropertyPathSlim(x => x.Level1.Level2.Level3, signalInitial: false)
                           .Subscribe(actual.Add))
                {
                    var expected = new List<string>();
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    fake.Level1 = new Level1();
                    expected.Add("Level1");
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    fake.Level1.Level2 = new Level2();
                    expected.Add("Level2");
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    fake.Level1.Level2.Level3 = new Level3();
                    expected.Add("Level3");
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    fake.Level1.Level2.Level3 = null;
                    expected.Add("Level3");
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    fake.Level1.Level2 = null;
                    expected.Add("Level2");
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    fake.Level1 = null;
                    expected.Add("Level1");
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));
                }
            }

            [Test]
            public void TwoLevelsReactsGeneric()
            {
                var actual = new List<PropertyChangedEventArgs>();
                var fake = new Fake<int>();
                using (fake.ObserveFullPropertyPathSlim(x => x.Next.Value, signalInitial: true)
                           .Subscribe(actual.Add))
                {
                    var expected = new List<string> { string.Empty };
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    fake.Next = new Level<int>();
                    expected.Add("Next");
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    fake.Next.Value++;
                    expected.Add("Value");
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));
                }
            }

            [Test]
            public void TwoLevelsReactsGenerics()
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
            public void TwoLevelsRootChangesFromValueToNull()
            {
                var actual = new List<PropertyChangedEventArgs>();
                var fake = new Fake { Next = new Level() };
                using (fake.ObserveFullPropertyPathSlim(x => x.Next.IsTrue, signalInitial: true)
                           .Subscribe(actual.Add))
                {
                    var expected = new List<string> { string.Empty };
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    fake.Next = null;
                    expected.Add("Next");
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));
                }
            }

            [Test]
            public void TwoLevelsRootChangesFromNullToValue()
            {
                var actual = new List<PropertyChangedEventArgs>();
                var fake = new Fake();
                using (fake.ObserveFullPropertyPathSlim(x => x.Next.IsTrue, signalInitial: true)
                           .Subscribe(actual.Add))
                {
                    var expected = new List<string> { string.Empty };
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    fake.Next = new Level();
                    expected.Add("Next");
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));
                }
            }

            [Test]
            public void TwoLevelsStartingWithValue()
            {
                var actual = new List<PropertyChangedEventArgs>();
                var fake = new Fake { Level1 = new Level1() };
                using (fake.ObserveFullPropertyPathSlim(x => x.Level1.IsTrue, signalInitial: true)
                           .Subscribe(actual.Add))
                {
                    var expected = new List<string> { string.Empty };
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    fake.Level1.IsTrue = !fake.Level1.IsTrue;
                    expected.Add("IsTrue");
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));
                }
            }

            [Test]
            public void TwoLevelsStartingWithoutValue()
            {
                var actual = new List<PropertyChangedEventArgs>();
                var fake = new Fake();
                using (fake.ObserveFullPropertyPathSlim(x => x.Next.IsTrue, signalInitial: true)
                           .Subscribe(actual.Add))
                {
                    var expected = new List<string> { string.Empty };
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    fake.Next = new Level();
                    expected.Add("Next");
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    fake.Next.IsTrue = !fake.Next.IsTrue;
                    expected.Add("IsTrue");
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));
                }
            }

            [TestCase(true, null)]
            [TestCase(null, false)]
            [TestCase(null, true)]
            public void TwoLevelsNullableStartingWithValue(bool? first, bool? other)
            {
                var actual = new List<PropertyChangedEventArgs>();
                var fake = new Fake { Next = new Level { IsTrueOrNull = first } };
                using (fake.ObserveFullPropertyPathSlim(x => x.Next.IsTrueOrNull, signalInitial: true)
                           .Subscribe(actual.Add))
                {
                    var expected = new List<string> { string.Empty };
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    fake.Next.IsTrueOrNull = other;
                    expected.Add("IsTrueOrNull");
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));
                }
            }

            [Test]
            public void TwoLevelsValueType()
            {
                var actual = new List<PropertyChangedEventArgs>();
                var fake = new Fake();
                using (fake.ObserveFullPropertyPathSlim(x => x.Level1.IsTrue, signalInitial: false)
                           .Subscribe(actual.Add))
                {
                    CollectionAssert.IsEmpty(actual);

                    fake.Level1 = new Level1();
                    var expected = new List<string> { "Level1" };
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    fake.Level1.IsTrue = !fake.Level1.IsTrue;
                    expected.Add("IsTrue");
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    fake.Level1 = null;
                    expected.Add("Level1");
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));
                }
            }

            [Test]
            public void ThreeLevelsStartingWithFirstNullThenAddingLevelsOneByOne()
            {
                var actual = new List<PropertyChangedEventArgs>();
                var fake = new Fake();
                using (fake.ObserveFullPropertyPathSlim(x => x.Level1.Level2.IsTrue)
                           .Subscribe(actual.Add))
                {
                    var expected = new List<string> { string.Empty };
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    fake.Level1 = new Level1();
                    expected.Add("Level1");
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    fake.Level1.Level2 = new Level2();
                    expected.Add("Level2");
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    fake.Level1.Level2.IsTrue = !fake.Level1.Level2.IsTrue;
                    expected.Add("IsTrue");
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    fake.Level1 = null;
                    expected.Add("Level1");
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));
                }
            }

            [Test]
            public void ThreeLevelsStartingWithFirstNullThenAddingTwoLevels()
            {
                var actual = new List<PropertyChangedEventArgs>();
                var fake = new Fake();
                using (fake.ObserveFullPropertyPathSlim(x => x.Level1.Level2.IsTrue)
                           .Subscribe(actual.Add))
                {
                    var expected = new List<string> { string.Empty };
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    fake.Level1 = new Level1 { Level2 = new Level2() };
                    expected.Add("Level1");
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    fake.Level1.Level2.IsTrue = !fake.Level1.Level2.IsTrue;
                    expected.Add("IsTrue");
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    fake.Level1 = null;
                    expected.Add("Level1");
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));
                }
            }

            [Test]
            public void ThreeLevelsStartingWithNullGeneric()
            {
                var actual = new List<PropertyChangedEventArgs>();
                var fake = new Fake();
                using (fake.ObserveFullPropertyPathSlim(x => x.Next.NextInt.Value)
                           .Subscribe(actual.Add))
                {
                    var expected = new List<string> { string.Empty };
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    fake.Next = new Level { NextInt = new Level<int>() };
                    expected.Add("Next");
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    fake.Next.NextInt.Value++;
                    expected.Add("Value");
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    fake.Next = null;
                    expected.Add("Next");
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));
                }
            }

            [Test]
            public void ThreeLevelsStartingWithNullGenericGeneric()
            {
                var actual = new List<PropertyChangedEventArgs>();
                var fake = new Fake();
                using (fake.ObserveFullPropertyPathSlim(x => x.NextInt.Next.Value)
                           .Subscribe(actual.Add))
                {
                    var expected = new List<string> { string.Empty };
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    fake.NextInt = new Level<int> { Next = new Level<int>() };
                    expected.Add("NextInt");
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    fake.NextInt.Next.Value++;
                    expected.Add("Value");
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    fake.NextInt = null;
                    expected.Add("NextInt");
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));
                }
            }

            [Test]
            public void FourLevelsStartingWithNullAfterFirstThenAddingLevelsOneByOne()
            {
                var actual = new List<PropertyChangedEventArgs>();
                var fake = new Fake();
                using (fake.ObserveFullPropertyPathSlim(x => x.Level1.Level2.Level3.Value)
                           .Subscribe(actual.Add))
                {
                    var expected = new List<string> { string.Empty };
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    fake.Level1 = new Level1();
                    expected.Add("Level1");
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    fake.Level1.Level2 = new Level2();
                    expected.Add("Level2");
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    fake.Level1.Level2.Level3 = new Level3();
                    expected.Add("Level3");
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    fake.Level1.Level2.Level3.Value++;
                    expected.Add("Value");
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    fake.Level1 = null;
                    expected.Add("Level1");
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));
                }
            }

            [Test]
            public void FourLevelsStartingWithNullAfterFirstThenAddingTwoLevels()
            {
                var actual = new List<PropertyChangedEventArgs>();
                var fake = new Fake();
                using (fake.ObserveFullPropertyPathSlim(x => x.Level1.Level2.Level3.Value)
                           .Subscribe(actual.Add))
                {
                    var expected = new List<string> { string.Empty };
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    fake.Level1 = new Level1 { Level2 = new Level2 { Level3 = new Level3() } };
                    expected.Add("Level1");
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    fake.Level1.Level2.Level3.Value++;
                    expected.Add("Value");
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    fake.Level1 = null;
                    expected.Add("Level1");
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));
                }
            }

            [TestCase("")]
            [TestCase(null)]
            [TestCase("Level1")]
            public void FirstInPathSignalsWhenHasValue(string propertyName)
            {
                var actual = new List<PropertyChangedEventArgs>();
                var fake = new Fake { Level1 = new Level1() };
                using (fake.ObserveFullPropertyPathSlim(x => x.Level1.IsTrue, signalInitial: false)
                           .Subscribe(actual.Add))
                {
                    fake.OnPropertyChanged(propertyName);
                    CollectionAssert.AreEqual(new[] { propertyName }, actual.Select(x => x.PropertyName));
                }
            }

            [TestCase("")]
            [TestCase(null)]
            [TestCase("IsTrue")]
            public void LastInPathInPathSignalsWhenHasValue(string propertyName)
            {
                var actual = new List<PropertyChangedEventArgs>();
                var fake = new Fake { Level1 = new Level1() };
                using (fake.ObserveFullPropertyPathSlim(x => x.Level1.IsTrue, signalInitial: false)
                           .Subscribe(actual.Add))
                {
                    fake.Level1.OnPropertyChanged(propertyName);
                    CollectionAssert.AreEqual(new[] { propertyName }, actual.Select(x => x.PropertyName));
                }
            }

            [Test]
            public void ThreeLevels()
            {
                var actual = new List<PropertyChangedEventArgs>();
                var fake = new Fake();
                using (fake.ObserveFullPropertyPathSlim(x => x.Level1.Level2.Value, signalInitial: false)
                           .Subscribe(actual.Add))
                {
                    fake.Level1 = new Level1();
                    var expected = new List<string> { "Level1" };
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    fake.Level1.Level2 = new Level2();
                    expected.Add("Level2");
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    fake.Level1.Level2 = null;
                    expected.Add("Level2");
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    fake.Level1 = null;
                    expected.Add("Level1");
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    fake.Level1 = new Level1 { Level2 = new Level2 { Value = 1 } };
                    expected.Add("Level1");
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    fake.Level1.Level2.Value++;
                    expected.Add("Value");
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    fake.Level1.Level2 = null;
                    expected.Add("Level2");
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    fake.Level1 = null;
                    expected.Add("Level1");
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));
                }
            }

            [Test]
            public void ThreeLevelsExisting()
            {
                var actual = new List<PropertyChangedEventArgs>();
                var fake = new Fake { Level1 = new Level1 { Level2 = new Level2() } };
                using (fake.ObserveFullPropertyPathSlim(x => x.Level1.Level2.Value)
                           .Subscribe(actual.Add))
                {
                    var expected = new List<string> { string.Empty };
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    fake.Level1.Level2.Value++;
                    expected.Add("Value");
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));
                }
            }

            [Test]
            public void ThreeLevelsExistingLevelBecomesNull()
            {
                var actual = new List<PropertyChangedEventArgs>();
                var fake = new Fake { Level1 = new Level1 { Level2 = new Level2() } };
                using (fake.ObserveFullPropertyPathSlim(x => x.Level1.Level2.IsTrue)
                           .Subscribe(actual.Add))
                {
                    var expected = new List<string> { string.Empty };
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    fake.Level1 = null;
                    expected.Add("Level1");
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));
                }
            }

            [Test]
            public void ThreeLevelsExistingLevelBecomesNew()
            {
                var actual = new List<PropertyChangedEventArgs>();
                var fake = new Fake { Level1 = new Level1 { Level2 = new Level2() } };
                using (fake.ObserveFullPropertyPathSlim(x => x.Level1.Level2.IsTrue)
                           .Subscribe(actual.Add))
                {
                    var expected = new List<string> { string.Empty };
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    fake.Level1 = new Level1 { Level2 = new Level2() };
                    expected.Add("Level1");
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));
                }
            }

            [Test]
            public void Reacts()
            {
                var actual = new List<PropertyChangedEventArgs>();
                var fake = new Fake();
                using (fake.ObserveFullPropertyPathSlim(x => x.Next.IsTrue)
                           .Subscribe(actual.Add))
                {
                    var expected = new List<string> { string.Empty };
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    fake.Next = new Level { IsTrue = false };
                    Assert.AreEqual(2, actual.Count);
                    expected.Add("Next");
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    fake.Next.IsTrue = true;
                    Assert.AreEqual(3, actual.Count);
                    expected.Add("IsTrue");
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    Level level1 = fake.Next;
                    fake.Next = null;
                    Assert.AreEqual(4, actual.Count);
                    expected.Add("Next");
                    CollectionAssert.AreEqual(expected, actual.Select(x => x.PropertyName));

                    level1.IsTrue = !level1.IsTrue;
                    Assert.AreEqual(4, actual.Count);
                }
            }

            [Test]
            [SuppressMessage("ReSharper", "UnusedVariable")]
            public void MemoryLeakRootDisposeTest()
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
            public void MemoryLeakRootNoDisposeTest()
            {
#if DEBUG
                Assert.Inconclusive("Debugger keeps things alive for the scope of the method.");
#endif
                var root = new Fake { Next = new Level() };
                var rootRef = new WeakReference(root);
                var levelRef = new WeakReference(root.Next);
                Assert.IsTrue(rootRef.IsAlive);
                var observable = root.ObserveFullPropertyPathSlim(x => x.Next.Name, signalInitial: false);
#pragma warning disable GU0030 // Use using.
                //// ReSharper disable once UnusedVariable
                var subscription = observable.Subscribe();
#pragma warning restore GU0030 // Use using.

                GC.Collect();
                Assert.IsFalse(rootRef.IsAlive);
                Assert.IsFalse(levelRef.IsAlive);
            }

            [Test]
            [SuppressMessage("ReSharper", "UnusedVariable")]
            public void MemoryLeakLevelNoDisposeTest()
            {
#if DEBUG
                Assert.Inconclusive("Debugger keeps things alive for the scope of the method.");
#endif
                var root = new Fake { Next = new Level() };
                var rootRef = new WeakReference(root);
                var levelRef = new WeakReference(root.Next);
                Assert.IsTrue(rootRef.IsAlive);
                var observable = root.ObserveFullPropertyPathSlim(x => x.Next.Name, signalInitial: false);
#pragma warning disable GU0030 // Use using.
                var subscription = observable.Subscribe();
#pragma warning restore GU0030 // Use using.

                GC.Collect();
                Assert.IsFalse(rootRef.IsAlive);
                Assert.IsFalse(levelRef.IsAlive);
            }

            [Test]
            public void MemoryLeakLevelDisposeTest()
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
}