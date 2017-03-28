// ReSharper disable All
namespace Gu.Reactive.Tests.NotifyPropertyChangedExt
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Linq;

    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    public partial class NotifyPropertyChangedExtTests
    {
        public class OnPropertyChangedNestedLamda
        {
            [Test]
            public void ReactsTwoPropertiesSameInstance()
            {
                var changes = new List<EventPattern<PropertyChangedEventArgs>>();
                var fake = new Fake { Level1 = new Level1 { Value = 1 } };
                using (fake.ObservePropertyChanged(x => x.Level1.Value, false)
                           .Subscribe(changes.Add))
                {
                    using (fake.ObservePropertyChanged(x => x.Level1.IsTrue, false)
                               .Subscribe(changes.Add))
                    {
                        Assert.AreEqual(0, changes.Count);

                        fake.Level1.Value++;
                        Assert.AreEqual(1, changes.Count);
                        EventPatternAssert.AreEqual(fake.Level1, "Value", changes.Last());

                        fake.Level1.IsTrue = !fake.IsTrue;
                        Assert.AreEqual(2, changes.Count);
                        EventPatternAssert.AreEqual(fake.Level1, "IsTrue", changes.Last());
                    }
                }
            }

            [Test]
            public void ReactsTwoInstancesSameProperty()
            {
                var changes = new List<EventPattern<PropertyChangedEventArgs>>();
                var fake1 = new Fake { Level1 = new Level1() };
                using (fake1.ObservePropertyChanged(x => x.Level1.Value, false)
                            .Subscribe(changes.Add))
                {
                    var fake2 = new Fake { Level1 = new Level1() };
                    using (fake2.ObservePropertyChanged(x => x.Level1.Value, false)
                                .Subscribe(changes.Add))
                    {
                        Assert.AreEqual(0, changes.Count);

                        fake1.Level1.Value++;
                        Assert.AreEqual(1, changes.Count);
                        EventPatternAssert.AreEqual(fake1.Level1, "Value", changes.Last());

                        fake2.Level1.Value++;
                        Assert.AreEqual(2, changes.Count);
                        EventPatternAssert.AreEqual(fake2.Level1, "Value", changes.Last());
                    }
                }
            }

            [Test]
            public void TwoSubscriptionsOneObservable()
            {
                var changes1 = new List<EventPattern<PropertyChangedEventArgs>>();
                var changes2 = new List<EventPattern<PropertyChangedEventArgs>>();
                var fake = new Fake { Level1 = new Level1() };
                var observable = fake.ObservePropertyChanged(x => x.Level1.IsTrue, false);
                using (observable.Subscribe(changes1.Add))
                {
                    using (observable.Subscribe(changes2.Add))
                    {
                        Assert.AreEqual(0, changes1.Count);
                        Assert.AreEqual(0, changes2.Count);

                        fake.Level1.IsTrue = !fake.Level1.IsTrue;
                        Assert.AreEqual(1, changes1.Count);
                        Assert.AreEqual(1, changes2.Count);
                        EventPatternAssert.AreEqual(fake.Level1, "IsTrue", changes1.Last());
                        EventPatternAssert.AreEqual(fake.Level1, "IsTrue", changes2.Last());

                        fake.Level1.IsTrue = !fake.Level1.IsTrue;
                        Assert.AreEqual(2, changes1.Count);
                        Assert.AreEqual(2, changes2.Count);
                        EventPatternAssert.AreEqual(fake.Level1, "IsTrue", changes1.Last());
                        EventPatternAssert.AreEqual(fake.Level1, "IsTrue", changes2.Last());

                        fake.Level1 = null;
                        Assert.AreEqual(3, changes1.Count);
                        Assert.AreEqual(3, changes2.Count);
                        EventPatternAssert.AreEqual(fake, "Level1", changes1.Last());
                        EventPatternAssert.AreEqual(fake, "Level1", changes2.Last());
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
                        () => fake.ObservePropertyChanged(x => x.StructLevel.Name, signalIntital));
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
                        () => fake.ObservePropertyChanged(x => x.Name.Length, signalIntital));
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
                    () => fake.ObservePropertyChanged(
                        x => x.Method()
                              .Name));
                Assert.AreEqual("Expected path to be properties only. Was x.Method().Name", exception.Message);
            }

            [Test]
            public void Unsubscribes()
            {
                var changes = new List<EventPattern<PropertyChangedEventArgs>>();
                var fake = new Fake();
                using (fake.ObservePropertyChanged(x => x.Level1.IsTrue, false)
                           .Subscribe(changes.Add))
                {
                    fake.Level1 = new Level1
                    {
                        Level2 = new Level2()
                    };

                    Assert.AreEqual(1, changes.Count);
                    var old = fake.Level1;
                    fake.Level1 = null;
                    Assert.AreEqual(2, changes.Count);
                    old.IsTrue = !old.IsTrue;
                    Assert.AreEqual(2, changes.Count);
                }
            }

            [TestCase(true, 1)]
            [TestCase(false, 0)]
            public void SignalsInitialWhenHasValue(bool signalInitial, int expected)
            {
                var changes = new List<EventPattern<PropertyChangedEventArgs>>();
                var fake = new Fake { Level1 = new Level1() };
                using (fake.ObservePropertyChanged(x => x.Level1.Value, signalInitial)
                           .Subscribe(changes.Add))
                {
                    Assert.AreEqual(expected, changes.Count);
                    if (signalInitial)
                    {
                        EventPatternAssert.AreEqual(fake.Level1, string.Empty, changes.Last());
                    }

                    fake.Level1.Value++;
                    Assert.AreEqual(expected + 1, changes.Count); // Double check that we are subscribing
                }
            }

            [TestCase(true, 1)]
            [TestCase(false, 0)]
            public void SignalsInitialWhenHasValueGeneric(bool signalInitial, int expected)
            {
                var changes = new List<EventPattern<PropertyChangedEventArgs>>();
                var fake = new Fake<int> { Next = new Level<int>() };
                using (fake.ObservePropertyChanged(x => x.Next.Value, signalInitial)
                           .Subscribe(changes.Add))
                {
                    Assert.AreEqual(expected, changes.Count);
                    if (signalInitial)
                    {
                        EventPatternAssert.AreEqual(fake.Next, string.Empty, changes.Last());
                    }

                    fake.Next.Value++;
                    Assert.AreEqual(expected + 1, changes.Count); // Double check that we are subscribing
                }
            }

            [TestCase(true, 1)]
            [TestCase(false, 0)]
            public void SignalsInitialWhenNoValue(bool signalInitial, int expected)
            {
                var changes = new List<EventPattern<PropertyChangedEventArgs>>();
                var fake = new Fake();
                using (fake.ObservePropertyChanged(x => x.Level1.Value, signalInitial)
                           .Subscribe(changes.Add))
                {
                    Assert.AreEqual(expected, changes.Count);
                    if (signalInitial)
                    {
                        EventPatternAssert.AreEqual(fake, string.Empty, changes.Last());
                    }

                    fake.Level1 = new Level1();
                    Assert.AreEqual(expected + 1, changes.Count); // Double check that we are subscribing
                }
            }

            [TestCase(true, 1)]
            [TestCase(false, 0)]
            public void SignalsInitialWhenNoValueGeneric(bool signalInitial, int expected)
            {
                var changes = new List<EventPattern<PropertyChangedEventArgs>>();
                var fake = new Fake<int>();
                using (fake.ObservePropertyChanged(x => x.Next.Value, signalInitial)
                           .Subscribe(changes.Add))
                {
                    Assert.AreEqual(expected, changes.Count);
                    if (signalInitial)
                    {
                        EventPatternAssert.AreEqual(fake, string.Empty, changes.Last());
                    }

                    fake.Next = new Level<int>();
                    Assert.AreEqual(expected + 1, changes.Count); // Double check that we are subscribing
                }
            }

            [TestCase("")]
            [TestCase(null)]
            public void DoesNotReactToStringEmptyOrNullFromRootWhenNull(string propertyName)
            {
                var changes = new List<EventPattern<PropertyChangedEventArgs>>();
                var fake = new Fake();
                using (fake.ObservePropertyChanged(x => x.Level1.IsTrue, false)
                           .Subscribe(changes.Add))
                {
                    Assert.AreEqual(0, changes.Count);
                    fake.OnPropertyChanged(propertyName);
                    //// This means all properties changed according to wpf convention
                    Assert.AreEqual(0, changes.Count);
                }
            }

            [TestCase(null)]
            [TestCase("")]
            [TestCase("Level1")]
            public void DoesReactToStringEmptyOrNullFromRootWhenNotNull(string propertyName)
            {
                var changes = new List<EventPattern<PropertyChangedEventArgs>>();
                var fake = new Fake { Level1 = new Level1() };
                using (fake.ObservePropertyChanged(x => x.Level1.IsTrue, false)
                           .Subscribe(changes.Add))
                {
                    Assert.AreEqual(0, changes.Count);

                    fake.OnPropertyChanged(propertyName);
                    //// This means all properties changed according to wpf convention
                    EventPatternAssert.AreEqual(fake, propertyName, changes.Single());
                }
            }

            [TestCase("")]
            [TestCase(null)]
            [TestCase("IsTrue")]
            public void ReactsOnStringEmptyOrNullFromSourceWhenHasValue(string propertyName)
            {
                var changes = new List<EventPattern<PropertyChangedEventArgs>>();
                var fake = new Fake { Level1 = new Level1() };
                using (fake.ObservePropertyChanged(x => x.Level1.IsTrue, false)
                           .Subscribe(changes.Add))
                {
                    Assert.AreEqual(0, changes.Count);

                    fake.Level1.OnPropertyChanged(propertyName);
                    //// This means all properties changed according to wpf convention
                    EventPatternAssert.AreEqual(fake.Level1, propertyName, changes.Single());
                }
            }

            [TestCase("")]
            [TestCase(null)]
            [TestCase("Name")]
            public void ReactsOnStringEmptyOrNullFromSourceWhenNull(string propertyName)
            {
                var changes = new List<EventPattern<PropertyChangedEventArgs>>();
                var fake = new Fake { Level1 = new Level1 { Name = null } };
                using (fake.ObservePropertyChanged(x => x.Level1.Name, false)
                           .Subscribe(changes.Add))
                {
                    Assert.AreEqual(0, changes.Count);

                    fake.Level1.OnPropertyChanged(propertyName);
                    //// This means all properties changed according to wpf convention
                    EventPatternAssert.AreEqual(fake.Level1, propertyName, changes.Single());
                }
            }

            [Test]
            public void TwoLevelsReacts()
            {
                var changes = new List<EventPattern<PropertyChangedEventArgs>>();
                var fake = new Fake();
                using (fake.ObservePropertyChanged(x => x.Level1.IsTrue, true)
                           .Subscribe(changes.Add))
                {
                    EventPatternAssert.AreEqual(fake, string.Empty, changes.Single());

                    fake.Level1 = new Level1();
                    Assert.AreEqual(2, changes.Count);
                    EventPatternAssert.AreEqual(fake, "Level1", changes.Last());

                    fake.Level1.IsTrue = !fake.Level1.IsTrue;
                    Assert.AreEqual(3, changes.Count);
                    EventPatternAssert.AreEqual(fake.Level1, "IsTrue", changes.Last());
                }
            }

            [Test]
            public void TwoSubscriptionsTwoLevelsReacts()
            {
                var changes = new List<EventPattern<PropertyChangedEventArgs>>();
                var intChanges = new List<EventPattern<PropertyChangedEventArgs>>();
                var fake = new Fake();
                var intfake = new Fake<int>();
                using (fake.ObservePropertyChanged(x => x.Next.Value, true)
                           .Subscribe(changes.Add))
                {
                    using (intfake.ObservePropertyChanged(x => x.Next.Value, true)
                                  .Subscribe(intChanges.Add))
                    {
                        EventPatternAssert.AreEqual(fake, string.Empty, changes.Single());
                        EventPatternAssert.AreEqual(intfake, string.Empty, intChanges.Single());

                        fake.Next = new Level();
                        Assert.AreEqual(2, changes.Count);
                        EventPatternAssert.AreEqual(fake, "Next", changes.Last());
                        EventPatternAssert.AreEqual(intfake, string.Empty, intChanges.Single());

                        fake.Next.Value++;
                        Assert.AreEqual(3, changes.Count);
                        EventPatternAssert.AreEqual(fake.Next, "Value", changes.Last());
                        EventPatternAssert.AreEqual(intfake, string.Empty, intChanges.Single());

                        intfake.Next = new Level<int>();
                        Assert.AreEqual(3, changes.Count);
                        EventPatternAssert.AreEqual(fake.Next, "Value", changes.Last());
                        Assert.AreEqual(2, intChanges.Count);
                        EventPatternAssert.AreEqual(intfake, "Next", intChanges.Last());

                        intfake.Next.Value++;
                        Assert.AreEqual(3, changes.Count);
                        EventPatternAssert.AreEqual(fake.Next, "Value", changes.Last());
                        Assert.AreEqual(3, intChanges.Count);
                        EventPatternAssert.AreEqual(intfake.Next, "Value", intChanges.Last());
                    }
                }
            }

            [Test]
            public void TwoLevelsExisting()
            {
                var changes = new List<EventPattern<PropertyChangedEventArgs>>();
                var fake = new Fake { Level1 = new Level1 { Level2 = new Level2() } };
                using (fake.ObservePropertyChanged(x => x.Level1.Level2, false)
                           .Subscribe(changes.Add))
                {
                    CollectionAssert.IsEmpty(changes);

                    fake.Level1.Level2 = new Level2();
                    EventPatternAssert.AreEqual(fake.Level1, "Level2", changes.Single());

                    fake.Level1.Level2 = null;
                    Assert.AreEqual(2, changes.Count);
                    EventPatternAssert.AreEqual(fake.Level1, "Level2", changes.Last());

                    fake.Level1 = null;
                    Assert.AreEqual(3, changes.Count);
                    EventPatternAssert.AreEqual(fake, "Level1", changes.Last());

                    fake.OnPropertyChanged("Level1");
                    Assert.AreEqual(3, changes.Count);
                }
            }

            [Test]
            public void TwoLevelsExistsingChangeLastValueInPath()
            {
                var changes = new List<EventPattern<PropertyChangedEventArgs>>();
                var fake = new Fake { Next = new Level() };
                using (fake.ObservePropertyChanged(x => x.Next.IsTrue, false)
                           .Subscribe(changes.Add))
                {
                    fake.Next.IsTrue = !fake.Next.IsTrue;
                    Assert.AreEqual(1, changes.Count);
                }
            }

            [Test]
            public void TwoLevelsReferenceType()
            {
                var changes = new List<EventPattern<PropertyChangedEventArgs>>();
                var fake = new Fake();
                using (fake.ObservePropertyChanged(x => x.Next.Next, false)
                           .Subscribe(changes.Add))
                {
                    CollectionAssert.IsEmpty(changes);

                    fake.Next = new Level();
                    CollectionAssert.AreEqual(new[] { "Next" }, changes.Select(x => x.EventArgs.PropertyName));
                    Assert.AreEqual(fake, changes.Single().Sender);

                    fake.Next.Next = new Level();
                    CollectionAssert.AreEqual(new[] { "Next", "Next" }, changes.Select(x => x.EventArgs.PropertyName));
                    Assert.AreEqual(fake.Next, changes.Last().Sender);

                    fake.Next.Next = null;
                    CollectionAssert.AreEqual(
                        new[] { "Next", "Next", "Next" },
                        changes.Select(x => x.EventArgs.PropertyName));
                    Assert.AreEqual(fake.Next, changes.Last().Sender);

                    fake.Next = null;
                    CollectionAssert.AreEqual(
                        new[] { "Next", "Next", "Next", "Next" },
                        changes.Select(x => x.EventArgs.PropertyName));
                    Assert.AreEqual(fake, changes.Last().Sender);
                }
            }

            [Test]
            public void ThreeLevelsReferenceType()
            {
                var changes = new List<EventPattern<PropertyChangedEventArgs>>();
                var fake = new Fake();
                using (fake.ObservePropertyChanged(x => x.Next.Next.Next, false)
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
            public void TwoLevelsReactsGeneric()
            {
                var changes = new List<EventPattern<PropertyChangedEventArgs>>();
                var fake = new Fake<int>();
                using (fake.ObservePropertyChanged(x => x.Next.Value, true)
                           .Subscribe(changes.Add))
                {
                    EventPatternAssert.AreEqual(fake, string.Empty, changes.Single());

                    fake.Next = new Level<int>();
                    Assert.AreEqual(2, changes.Count);
                    EventPatternAssert.AreEqual(fake, "Next", changes.Last());

                    fake.Next.Value++;
                    Assert.AreEqual(3, changes.Count);
                    EventPatternAssert.AreEqual(fake.Next, "Value", changes.Last());
                }
            }

            [Test]
            public void TwoLevelsReactsGenerics()
            {
                var intChanges = new List<EventPattern<PropertyChangedEventArgs>>();
                var doubleChanges = new List<EventPattern<PropertyChangedEventArgs>>();
                var intFake = new Fake<int>();
                var doubleFake = new Fake<double>();
                using (intFake.ObservePropertyChanged(x => x.Next.Value, true)
                              .Subscribe(intChanges.Add))
                {
                    using (doubleFake.ObservePropertyChanged(x => x.Next.Value, true)
                                     .Subscribe(doubleChanges.Add))
                    {
                        EventPatternAssert.AreEqual(intFake, string.Empty, intChanges.Single());
                        EventPatternAssert.AreEqual(doubleFake, string.Empty, doubleChanges.Single());

                        intFake.Next = new Level<int>();
                        Assert.AreEqual(2, intChanges.Count);
                        EventPatternAssert.AreEqual(intFake, "Next", intChanges.Last());
                        EventPatternAssert.AreEqual(doubleFake, string.Empty, doubleChanges.Single());

                        intFake.Next.Value++;
                        Assert.AreEqual(3, intChanges.Count);
                        EventPatternAssert.AreEqual(intFake.Next, "Value", intChanges.Last());
                        EventPatternAssert.AreEqual(doubleFake, string.Empty, doubleChanges.Single());

                        doubleFake.Next = new Level<double>();
                        Assert.AreEqual(3, intChanges.Count);
                        EventPatternAssert.AreEqual(intFake.Next, "Value", intChanges.Last());
                        Assert.AreEqual(2, doubleChanges.Count);
                        EventPatternAssert.AreEqual(doubleFake, "Next", doubleChanges.Last());

                        doubleFake.Next.Value++;
                        Assert.AreEqual(3, intChanges.Count);
                        EventPatternAssert.AreEqual(intFake.Next, "Value", intChanges.Last());
                        Assert.AreEqual(3, doubleChanges.Count);
                        EventPatternAssert.AreEqual(doubleFake.Next, "Value", doubleChanges.Last());
                    }
                }
            }

            [Test]
            public void TwoLevelsRootChangesFromValueToNull()
            {
                var changes = new List<EventPattern<PropertyChangedEventArgs>>();
                var fake = new Fake { Next = new Level() };
                using (fake.ObservePropertyChanged(x => x.Next.IsTrue, true)
                           .Subscribe(changes.Add))
                {
                    EventPatternAssert.AreEqual(fake.Next, string.Empty, changes.Single());

                    fake.Next = null;
                    Assert.AreEqual(2, changes.Count);
                    EventPatternAssert.AreEqual(fake, "Next", changes.Last());
                }
            }

            [Test]
            public void TwoLevelsRootChangesFromNullToValue()
            {
                var changes = new List<EventPattern<PropertyChangedEventArgs>>();
                var fake = new Fake();
                using (fake.ObservePropertyChanged(x => x.Next.IsTrue, true)
                           .Subscribe(changes.Add))
                {
                    EventPatternAssert.AreEqual(fake, string.Empty, changes.Single());

                    fake.Next = new Level();
                    Assert.AreEqual(2, changes.Count);
                    EventPatternAssert.AreEqual(fake, "Next", changes.Last());
                }
            }

            [Test]
            public void TwoLevelsStartingWithValue()
            {
                var changes = new List<EventPattern<PropertyChangedEventArgs>>();
                var fake = new Fake { Level1 = new Level1() };
                using (fake.ObservePropertyChanged(x => x.Level1.IsTrue, true)
                           .Subscribe(changes.Add))
                {
                    EventPatternAssert.AreEqual(fake.Level1, string.Empty, changes.Single());

                    fake.Level1.IsTrue = !fake.Level1.IsTrue;
                    Assert.AreEqual(2, changes.Count);
                    EventPatternAssert.AreEqual(fake.Level1, "IsTrue", changes.Last());
                }
            }

            [Test]
            public void TwoLevelsStartingWithoutValue()
            {
                var changes = new List<EventPattern<PropertyChangedEventArgs>>();
                var fake = new Fake();
                using (fake.ObservePropertyChanged(x => x.Next.IsTrue, true)
                           .Subscribe(changes.Add))
                {
                    EventPatternAssert.AreEqual(fake, string.Empty, changes.Single());

                    fake.Next = new Level();
                    Assert.AreEqual(2, changes.Count);
                    EventPatternAssert.AreEqual(fake, "Next", changes.Last());

                    fake.Next.IsTrue = !fake.Next.IsTrue;
                    Assert.AreEqual(3, changes.Count);
                    EventPatternAssert.AreEqual(fake.Next, "IsTrue", changes.Last());
                }
            }

            [TestCase(true, null)]
            [TestCase(null, false)]
            [TestCase(null, true)]
            public void TwoLevelsNullableStartingWithValue(bool? first, bool? other)
            {
                var changes = new List<EventPattern<PropertyChangedEventArgs>>();
                var fake = new Fake { Next = new Level { IsTrueOrNull = first } };
                using (fake.ObservePropertyChanged(x => x.Next.IsTrueOrNull, true)
                           .Subscribe(changes.Add))
                {
                    EventPatternAssert.AreEqual(fake.Next, string.Empty, changes.Single());

                    fake.Next.IsTrueOrNull = other;
                    Assert.AreEqual(2, changes.Count);
                    EventPatternAssert.AreEqual(fake.Next, "IsTrueOrNull", changes.Last());
                }
            }

            [Test]
            public void TwoLevelsValueType()
            {
                var changes = new List<EventPattern<PropertyChangedEventArgs>>();
                var fake = new Fake();
                using (fake.ObservePropertyChanged(x => x.Level1.IsTrue, false)
                           .Subscribe(changes.Add))
                {
                    CollectionAssert.IsEmpty(changes);

                    fake.Level1 = new Level1();
                    EventPatternAssert.AreEqual(fake, "Level1", changes.Single());

                    fake.Level1.IsTrue = !fake.Level1.IsTrue;
                    Assert.AreEqual(2, changes.Count);
                    EventPatternAssert.AreEqual(fake.Level1, "IsTrue", changes.Last());

                    fake.Level1 = null;
                    Assert.AreEqual(3, changes.Count);
                    EventPatternAssert.AreEqual(fake, "Level1", changes.Last());
                }
            }

            [Test]
            public void ThreeLevelsStartingWithFirstNullThenAddingLevelsOneByOne()
            {
                var changes = new List<EventPattern<PropertyChangedEventArgs>>();
                var fake = new Fake();
                using (fake.ObservePropertyChanged(x => x.Level1.Level2.IsTrue)
                           .Subscribe(changes.Add))
                {
                    EventPatternAssert.AreEqual(fake, string.Empty, changes.Single());

                    fake.Level1 = new Level1();
                    Assert.AreEqual(1, changes.Count);

                    fake.Level1.Level2 = new Level2();
                    Assert.AreEqual(2, changes.Count);
                    EventPatternAssert.AreEqual(fake.Level1, "Level2", changes.Last());

                    fake.Level1.Level2.IsTrue = !fake.Level1.Level2.IsTrue;
                    Assert.AreEqual(3, changes.Count);
                    EventPatternAssert.AreEqual(fake.Level1.Level2, "IsTrue", changes.Last());

                    fake.Level1 = null;
                    Assert.AreEqual(4, changes.Count);
                    EventPatternAssert.AreEqual(fake, "Level1", changes.Last());
                }
            }

            [Test]
            public void ThreeLevelsStartingWithFirstNullThenAddingTwoLevels()
            {
                var changes = new List<EventPattern<PropertyChangedEventArgs>>();
                var fake = new Fake();
                using (fake.ObservePropertyChanged(x => x.Level1.Level2.IsTrue)
                           .Subscribe(changes.Add))
                {
                    EventPatternAssert.AreEqual(fake, string.Empty, changes.Single());

                    fake.Level1 = new Level1 { Level2 = new Level2() };
                    Assert.AreEqual(2, changes.Count);
                    EventPatternAssert.AreEqual(fake, "Level1", changes.Last());

                    fake.Level1.Level2.IsTrue = !fake.Level1.Level2.IsTrue;
                    Assert.AreEqual(3, changes.Count);
                    EventPatternAssert.AreEqual(fake.Level1.Level2, "IsTrue", changes.Last());

                    fake.Level1 = null;
                    Assert.AreEqual(4, changes.Count);
                    EventPatternAssert.AreEqual(fake, "Level1", changes.Last());
                }
            }

            [Test]
            public void ThreeLevelsStartingWithNullGeneric()
            {
                var changes = new List<EventPattern<PropertyChangedEventArgs>>();
                var fake = new Fake();
                using (fake.ObservePropertyChanged(x => x.Next.NextInt.Value)
                           .Subscribe(changes.Add))
                {
                    EventPatternAssert.AreEqual(fake, string.Empty, changes.Single());

                    fake.Next = new Level { NextInt = new Level<int>() };
                    Assert.AreEqual(2, changes.Count);
                    EventPatternAssert.AreEqual(fake, "Next", changes.Last());

                    fake.Next.NextInt.Value++;
                    Assert.AreEqual(3, changes.Count);
                    EventPatternAssert.AreEqual(fake.Next.NextInt, "Value", changes.Last());

                    fake.Next = null;
                    Assert.AreEqual(4, changes.Count);
                    EventPatternAssert.AreEqual(fake, "Next", changes.Last());
                }
            }

            [Test]
            public void ThreeLevelsStartingWithNullGenericGeneric()
            {
                var changes = new List<EventPattern<PropertyChangedEventArgs>>();
                var fake = new Fake();
                using (fake.ObservePropertyChanged(x => x.NextInt.Next.Value)
                           .Subscribe(changes.Add))
                {
                    EventPatternAssert.AreEqual(fake, string.Empty, changes.Single());

                    fake.NextInt = new Level<int> { Next = new Level<int>() };
                    Assert.AreEqual(2, changes.Count);
                    EventPatternAssert.AreEqual(fake, "NextInt", changes.Last());

                    fake.NextInt.Next.Value++;
                    Assert.AreEqual(3, changes.Count);
                    EventPatternAssert.AreEqual(fake.NextInt.Next, "Value", changes.Last());

                    fake.NextInt = null;
                    Assert.AreEqual(4, changes.Count);
                    EventPatternAssert.AreEqual(fake, "NextInt", changes.Last());
                }
            }

            [Test]
            public void FourLevelsStartingWithNullAfterFirstThenAddingLevelsOneByOne()
            {
                var changes = new List<EventPattern<PropertyChangedEventArgs>>();
                var fake = new Fake();
                using (fake.ObservePropertyChanged(x => x.Level1.Level2.Level3.Value)
                           .Subscribe(changes.Add))
                {
                    EventPatternAssert.AreEqual(fake, string.Empty, changes.Single());

                    fake.Level1 = new Level1();
                    EventPatternAssert.AreEqual(fake, string.Empty, changes.Single());

                    fake.Level1.Level2 = new Level2();
                    EventPatternAssert.AreEqual(fake, string.Empty, changes.Single());

                    fake.Level1.Level2.Level3 = new Level3();
                    Assert.AreEqual(2, changes.Count);
                    EventPatternAssert.AreEqual(fake.Level1.Level2, "Level3", changes.Last());

                    fake.Level1.Level2.Level3.Value++;
                    Assert.AreEqual(3, changes.Count);
                    EventPatternAssert.AreEqual(fake.Level1.Level2.Level3, "Value", changes.Last());

                    fake.Level1 = null;
                    Assert.AreEqual(4, changes.Count);
                    EventPatternAssert.AreEqual(fake, "Level1", changes.Last());
                }
            }

            [Test]
            public void FourLevelsStartingWithNullAfterFirstThenAddingTwoLevels()
            {
                var changes = new List<EventPattern<PropertyChangedEventArgs>>();
                var fake = new Fake();
                using (fake.ObservePropertyChanged(x => x.Level1.Level2.Level3.Value)
                           .Subscribe(changes.Add))
                {
                    EventPatternAssert.AreEqual(fake, string.Empty, changes.Single());

                    fake.Level1 = new Level1 { Level2 = new Level2 { Level3 = new Level3() } };
                    Assert.AreEqual(2, changes.Count);
                    EventPatternAssert.AreEqual(fake, "Level1", changes.Last());

                    fake.Level1.Level2.Level3.Value++;
                    Assert.AreEqual(3, changes.Count);
                    EventPatternAssert.AreEqual(fake.Level1.Level2.Level3, "Value", changes.Last());

                    fake.Level1 = null;
                    Assert.AreEqual(4, changes.Count);
                    EventPatternAssert.AreEqual(fake, "Level1", changes.Last());
                }
            }

            [TestCase("")]
            [TestCase(null)]
            [TestCase("Level1")]
            public void FirstInPathSignalsWhenHasValue(string propertyName)
            {
                var changes = new List<EventPattern<PropertyChangedEventArgs>>();
                var fake = new Fake { Level1 = new Level1() };
                using (fake.ObservePropertyChanged(x => x.Level1.IsTrue, false)
                           .Subscribe(changes.Add))
                {
                    fake.OnPropertyChanged(propertyName);
                    EventPatternAssert.AreEqual(fake, propertyName, changes.Single());
                }
            }

            [TestCase("")]
            [TestCase(null)]
            [TestCase("IsTrue")]
            public void LastInPathInPathSignalsWhenHasValue(string propertyName)
            {
                var changes = new List<EventPattern<PropertyChangedEventArgs>>();
                var fake = new Fake { Level1 = new Level1() };
                using (fake.ObservePropertyChanged(x => x.Level1.IsTrue, false)
                           .Subscribe(changes.Add))
                {
                    fake.Level1.OnPropertyChanged(propertyName);
                    EventPatternAssert.AreEqual(fake.Level1, propertyName, changes.Single());
                }
            }

            [Test]
            public void ThreeLevels()
            {
                var changes = new List<EventPattern<PropertyChangedEventArgs>>();
                var fake = new Fake();
                using (fake.ObservePropertyChanged(x => x.Level1.Level2.Value, false)
                           .Subscribe(changes.Add))
                {
                    fake.Level1 = new Level1();
                    CollectionAssert.IsEmpty(changes);

                    fake.Level1.Level2 = new Level2();
                    Assert.AreEqual(1, changes.Count);
                    EventPatternAssert.AreEqual(fake.Level1, "Level2", changes.Single());

                    fake.Level1.Level2 = null;
                    Assert.AreEqual(2, changes.Count);
                    EventPatternAssert.AreEqual(fake.Level1, "Level2", changes.Last());

                    fake.Level1 = null;
                    Assert.AreEqual(2, changes.Count);

                    fake.Level1 = new Level1 { Level2 = new Level2 { Value = 1 } };
                    Assert.AreEqual(3, changes.Count);
                    EventPatternAssert.AreEqual(fake, "Level1", changes.Last());

                    fake.Level1.Level2.Value++;
                    Assert.AreEqual(4, changes.Count);
                    EventPatternAssert.AreEqual(fake.Level1.Level2, "Value", changes.Last());

                    fake.Level1.Level2 = null;
                    Assert.AreEqual(5, changes.Count);
                    EventPatternAssert.AreEqual(fake.Level1, "Level2", changes.Last());

                    fake.Level1 = null;
                    Assert.AreEqual(5, changes.Count);
                }
            }

            [Test]
            public void ThreeLevelsExisting()
            {
                var changes = new List<EventPattern<PropertyChangedEventArgs>>();
                var fake = new Fake { Level1 = new Level1 { Level2 = new Level2() } };
                using (fake.ObservePropertyChanged(x => x.Level1.Level2.Value)
                           .Subscribe(changes.Add))
                {
                    EventPatternAssert.AreEqual(fake.Level1.Level2, string.Empty, changes.Single());

                    fake.Level1.Level2.Value++;
                    Assert.AreEqual(2, changes.Count);
                    EventPatternAssert.AreEqual(fake.Level1.Level2, "Value", changes.Last());
                }
            }

            [Test]
            public void ThreeLevelsExistingLevelBecomesNull()
            {
                var changes = new List<EventPattern<PropertyChangedEventArgs>>();
                var fake = new Fake { Level1 = new Level1 { Level2 = new Level2() } };
                using (fake.ObservePropertyChanged(x => x.Level1.Level2.IsTrue)
                           .Subscribe(changes.Add))
                {
                    EventPatternAssert.AreEqual(fake.Level1.Level2, string.Empty, changes.Single());

                    fake.Level1 = null;
                    Assert.AreEqual(2, changes.Count);
                    EventPatternAssert.AreEqual(fake, "Level1", changes.Last());
                }
            }

            [Test]
            public void ThreeLevelsExistingLevelBecomesNew()
            {
                var actuals = new List<EventPattern<PropertyChangedEventArgs>>();
                var fake = new Fake { Level1 = new Level1 { Level2 = new Level2() } };
                using (fake.ObservePropertyChanged(x => x.Level1.Level2.IsTrue)
                           .Subscribe(actuals.Add))
                {
                    EventPatternAssert.AreEqual(fake.Level1.Level2, string.Empty, actuals.Single());

                    fake.Level1 = new Level1() { Level2 = new Level2() };
                    Assert.AreEqual(2, actuals.Count);
                    EventPatternAssert.AreEqual(fake, "Level1", actuals.Last());
                }
            }

            [Test]
            public void Reacts()
            {
                var changes = new List<EventPattern<PropertyChangedEventArgs>>();
                var fake = new Fake();
                using (fake.ObservePropertyChanged(x => x.Next.IsTrue)
                           .Subscribe(changes.Add))
                {
                    EventPatternAssert.AreEqual(fake, string.Empty, changes.Single());

                    fake.Next = new Level { IsTrue = false };
                    Assert.AreEqual(2, changes.Count);
                    EventPatternAssert.AreEqual(fake, "Next", changes.Last());

                    fake.Next.IsTrue = true;
                    Assert.AreEqual(3, changes.Count);
                    EventPatternAssert.AreEqual(fake.Next, "IsTrue", changes.Last());

                    Level level1 = fake.Next;
                    fake.Next = null;
                    Assert.AreEqual(4, changes.Count);
                    EventPatternAssert.AreEqual(fake, "Next", changes.Last());

                    level1.IsTrue = !level1.IsTrue;
                    Assert.AreEqual(4, changes.Count);
                }
            }

            [Test]
            public void MemoryLeakRootDisposeTest()
            {
#if DEBUG
                Assert.Inconclusive("Debugger keeps things alive for the scope of the method.");
#endif
                var root = new Fake { Next = new Level() };
                var rootRef = new WeakReference(root);
                var levelRef = new WeakReference(root.Next);
                Assert.IsTrue(rootRef.IsAlive);
                var observable = root.ObservePropertyChanged(x => x.Next.Name, false);
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
                var observable = root.ObservePropertyChanged(x => x.Next.Name, false);
#pragma warning disable GU0030 // Use using.
                var subscription = observable.Subscribe();
#pragma warning restore GU0030 // Use using.

                GC.Collect();
                Assert.IsFalse(rootRef.IsAlive);
                Assert.IsFalse(levelRef.IsAlive);
            }

            [Test]
            public void MemoryLeakLevelNoDisposeTest()
            {
#if DEBUG
                Assert.Inconclusive("Debugger keeps things alive for the scope of the method.");
#endif
                var root = new Fake { Next = new Level() };
                var rootRef = new WeakReference(root);
                var levelRef = new WeakReference(root.Next);
                Assert.IsTrue(rootRef.IsAlive);
                var observable = root.ObservePropertyChanged(x => x.Next.Name, false);
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
                var observable = root.ObservePropertyChanged(x => x.Next.Name, false);
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