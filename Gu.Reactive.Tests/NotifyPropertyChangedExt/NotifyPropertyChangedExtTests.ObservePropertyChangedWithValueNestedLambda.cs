// ReSharper disable All
namespace Gu.Reactive.Tests.NotifyPropertyChangedExt
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive;

    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    public partial class NotifyPropertyChangedExtTests
    {
        public class ObservePropertyChangedWithValueNestedLambda
        {
            [TestCase(true)]
            [TestCase(false)]
            public void ThrowsOnStructInPath(bool signalIntital)
            {
                var fake = new Fake();
                var exception =
                    Assert.Throws<ArgumentException>(
                        () => fake.ObservePropertyChangedWithValue(x => x.StructLevel.Name, signalIntital));
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
                        () => fake.ObservePropertyChangedWithValue(x => x.Name.Length, signalIntital));
                var expected = "Error found in x => x.Name.Length\r\n" +
                               "All levels in the path must implement INotifyPropertyChanged.\r\n" +
                               "The type string does not so Name.Length will not notify when it changes.\r\n" +
                               "The path is: x => x.Name.Length\r\n\r\n" +
                               "Parameter name: property";
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            [Explicit("Implement")]
            public void TypedEventArgsTest()
            {
            }

            [TestCase("")]
            [TestCase(null)]
            public void DoesNotReactOnStringEmptyOrNullFromRootWithoutValue(string prop)
            {
                var changes = new List<EventPattern<PropertyChangedAndValueEventArgs<string>>>();
                var fake = new Fake();
                using (fake.ObservePropertyChangedWithValue(x => x.Level1.Name, signalInitial: false)
                           .Subscribe(changes.Add))
                {
                    Assert.AreEqual(0, changes.Count);
                    fake.OnPropertyChanged(prop); // This means all properties changed according to wpf convention
                    Assert.AreEqual(0, changes.Count);
                }
            }

            [TestCase("")]
            [TestCase(null)]
            public void ReactsOnStringEmptyOrNullFromRootWithValue(string prop)
            {
                var changes = new List<EventPattern<PropertyChangedAndValueEventArgs<string>>>();
                var fake = new Fake { Level1 = new Level1 { Level2 = new Level2 { Name = "Johan" } } };
                using (fake.ObservePropertyChangedWithValue(x => x.Level1.Level2.Name, signalInitial: false)
                           .Subscribe(changes.Add))
                {
                    Assert.AreEqual(0, changes.Count);
                    fake.OnPropertyChanged(prop);
                    //// This means all properties changed according to wpf convention
                    EventPatternAssert.AreEqual(fake, prop, Maybe.Some("Johan"), changes.Single());
                }
            }

            [TestCase("")]
            [TestCase(null)]
            [TestCase("Name")]
            public void ReactsOnStringEmptyOrNullWithValue(string prop)
            {
                var changes = new List<EventPattern<PropertyChangedAndValueEventArgs<string>>>();
                var fake = new Fake { Level1 = new Level1 { Level2 = new Level2 { Name = "Johan" } } };
                using (fake.ObservePropertyChangedWithValue(x => x.Level1.Level2.Name, signalInitial: false)
                           .Subscribe(changes.Add))
                {
                    Assert.AreEqual(0, changes.Count);
                    fake.Level1.Level2.OnPropertyChanged(prop);
                    //// This means all properties changed according to wpf convention
                    Assert.AreEqual(1, changes.Count);
                    Assert.IsTrue(changes.Single().EventArgs.HasValue);
                    Assert.AreEqual("Johan", changes.Single().EventArgs.Value);
                    Assert.AreSame(fake.Level1.Level2, changes.Single().Sender);
                    Assert.AreEqual(prop, changes.Last().EventArgs.PropertyName);
                }
            }

            [Test]
            public void TwoSubscriptionsOneObservable()
            {
                var changes1 = new List<EventPattern<PropertyChangedAndValueEventArgs<string>>>();
                var changes2 = new List<EventPattern<PropertyChangedAndValueEventArgs<string>>>();
                var fake = new Fake { Level1 = new Level1 { Name = string.Empty } };
                var observable = fake.ObservePropertyChangedWithValue(x => x.Level1.Name, signalInitial: false);
                using (observable.Subscribe(changes1.Add))
                {
                    using (observable.Subscribe(changes2.Add))
                    {
                        Assert.AreEqual(0, changes1.Count);
                        Assert.AreEqual(0, changes2.Count);

                        fake.Level1.Name += "a";
                        EventPatternAssert.AreEqual(fake.Level1, "Name", Maybe.Some("a"), changes1.Single());
                        EventPatternAssert.AreEqual(fake.Level1, "Name", Maybe.Some("a"), changes2.Single());

                        fake.Level1.Name += "a";
                        Assert.AreEqual(2, changes1.Count);
                        EventPatternAssert.AreEqual(fake.Level1, "Name", Maybe.Some("aa"), changes1.Last());
                        Assert.AreEqual(2, changes2.Count);
                        EventPatternAssert.AreEqual(fake.Level1, "Name", Maybe.Some("aa"), changes2.Last());

                        fake.Level1 = null;
                        Assert.AreEqual(3, changes1.Count);
                        EventPatternAssert.AreEqual(fake, "Level1", Maybe.None<string>(), changes1.Last());
                        Assert.AreEqual(3, changes2.Count);
                        EventPatternAssert.AreEqual(fake, "Level1", Maybe.None<string>(), changes2.Last());
                    }
                }
            }

            [TestCase(null)]
            [TestCase("abc")]
            public void SignalsInitialThreeLevelsWhenValueIs(string value)
            {
                var changes = new List<EventPattern<PropertyChangedAndValueEventArgs<string>>>();
                var fake = new Fake { Level1 = new Level1 { Name = value } };
                using (fake.ObservePropertyChangedWithValue(x => x.Level1.Name, signalInitial: true)
                           .Subscribe(changes.Add))
                {
                    EventPatternAssert.AreEqual(fake.Level1, string.Empty, Maybe.Some(value), changes.Single());
                }
            }

            [TestCase(true)]
            [TestCase(false)]
            public void SignalsInitialThreeLevelsWhenMissing(bool signalInitial)
            {
                var changes = new List<EventPattern<PropertyChangedAndValueEventArgs<string>>>();
                var fake = new Fake();
                using (fake.ObservePropertyChangedWithValue(x => x.Level1.Name, signalInitial)
                           .Subscribe(changes.Add))
                {
                    if (signalInitial)
                    {
                        EventPatternAssert.AreEqual(fake, string.Empty, Maybe<string>.None, changes.Single());
                    }
                    else
                    {
                        CollectionAssert.IsEmpty(changes);
                    }
                }
            }

            [TestCase(true)]
            [TestCase(false)]
            public void SignalsInitialThreeLevelsWhenNull(bool signalInitial)
            {
                var changes = new List<EventPattern<PropertyChangedAndValueEventArgs<string>>>();
                var fake = new Fake { Level1 = new Level1 { Name = null } };
                using (fake.ObservePropertyChangedWithValue(x => x.Level1.Name, signalInitial)
                           .Subscribe(changes.Add))
                {
                    if (signalInitial)
                    {
                        EventPatternAssert.AreEqual(fake.Level1, string.Empty, Maybe<string>.Some(null), changes.Single());

                        fake.Level1.Name = "Johan";
                        Assert.AreEqual(2, changes.Count);
                        EventPatternAssert.AreEqual(fake.Level1, "Name", Maybe<string>.Some("Johan"), changes.Last());

                        using (fake.ObservePropertyChangedWithValue(x => x.Level1.Name, signalInitial)
                                   .Subscribe(changes.Add))
                        {
                            Assert.AreEqual(3, changes.Count);
                            EventPatternAssert.AreEqual(
                                fake.Level1,
                                string.Empty,
                                Maybe<string>.Some("Johan"),
                                changes.Last());
                        }
                    }
                    else
                    {
                        CollectionAssert.IsEmpty(changes);
                    }
                }
            }

            [TestCase(true)]
            [TestCase(false)]
            public void SignalInitialWhenHasValue(bool signalInitial)
            {
                var changes = new List<EventPattern<PropertyChangedAndValueEventArgs<string>>>();
                var fake = new Fake { Level1 = new Level1 { Name = "Johan" } };
                using (fake.ObservePropertyChangedWithValue(x => x.Level1.Name, signalInitial)
                           .Subscribe(changes.Add))
                {
                    if (signalInitial)
                    {
                        EventPatternAssert.AreEqual(fake.Level1, string.Empty, Maybe.Some("Johan"), changes.Single());

                        fake.Level1.Name = "Erik";
                        Assert.AreEqual(2, changes.Count);
                        EventPatternAssert.AreEqual(fake.Level1, "Name", Maybe<string>.Some("Erik"), changes.Last());

                        using (fake.ObservePropertyChangedWithValue(x => x.Level1.Name, signalInitial)
                                   .Subscribe(changes.Add))
                        {
                            Assert.AreEqual(3, changes.Count);
                            EventPatternAssert.AreEqual(fake.Level1, string.Empty, Maybe<string>.Some("Erik"), changes.Last());
                        }
                    }
                    else
                    {
                        CollectionAssert.IsEmpty(changes);
                    }
                }
            }

            [Test]
            public void SignalsOnSourceChanges()
            {
                var changes = new List<EventPattern<PropertyChangedAndValueEventArgs<string>>>();
                var fake = new Fake();
                using (fake.ObservePropertyChangedWithValue(x => x.Level1.Name, signalInitial: true)
                           .Subscribe(changes.Add))
                {
                    EventPatternAssert.AreEqual(fake, string.Empty, Maybe<string>.None, changes.Single());

                    fake.Level1 = new Level1();
                    Assert.AreEqual(2, changes.Count);
                    EventPatternAssert.AreEqual(fake, "Level1", Maybe<string>.Some(null), changes.Last());

                    fake.Level1.Name = "Johan";
                    Assert.AreEqual(3, changes.Count);
                    EventPatternAssert.AreEqual(fake.Level1, "Name", Maybe<string>.Some("Johan"), changes.Last());
                }
            }

            [Test]
            public void MemoryLeakLevelNoDisposeTest()
            {
#if DEBUG
                return; // debugger keeps things alive.
#endif
#pragma warning disable CS0162 // Unreachable code detected
                var fake = new Fake { Level1 = new Level1() };
#pragma warning restore CS0162 // Unreachable code detected
                WeakReference wr = new WeakReference(fake.Level1);
                Assert.IsTrue(wr.IsAlive);
                var observable = fake.ObservePropertyChangedWithValue(x => x.Level1.Name, signalInitial: false);
#pragma warning disable IDISP001  // Dispose created.
                var subscription = observable.Subscribe();
#pragma warning restore IDISP001  // Dispose created.
                fake.Level1 = null;
                GC.Collect();
                Assert.IsFalse(wr.IsAlive);
                Assert.NotNull(fake);
                Assert.IsNotNull(observable); // touching it after GC.Collect
                Assert.IsNotNull(subscription); // touching it after GC.Collect
            }

            [Test]
            public void MemoryLeakLevelDisposeTest()
            {
#if DEBUG
                return; // debugger keeps things alive.
#endif
#pragma warning disable CS0162 // Unreachable code detected
                var fake = new Fake { Level1 = new Level1() };
#pragma warning restore CS0162 // Unreachable code detected
                WeakReference wr = new WeakReference(fake.Level1);
                Assert.IsTrue(wr.IsAlive);
                var observable = fake.ObservePropertyChangedWithValue(x => x.Level1.Name, signalInitial: false);
                using (var subscription = observable.Subscribe())
                {
                }

                fake.Level1 = null;
                GC.Collect();
                Assert.IsFalse(wr.IsAlive);
                Assert.NotNull(fake);
                Assert.IsNotNull(observable); // touching it after GC.Collect
            }
        }
    }
}
