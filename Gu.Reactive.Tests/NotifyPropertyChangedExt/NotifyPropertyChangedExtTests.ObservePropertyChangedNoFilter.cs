// ReSharper disable NotResolvedInText
// ReSharper disable UnusedVariable
// ReSharper disable HeuristicUnreachableCode
#pragma warning disable WPF1014 // Don't raise PropertyChanged for missing property.
namespace Gu.Reactive.Tests.NotifyPropertyChangedExt
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reactive;

    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    public partial class NotifyPropertyChangedExtTests
    {
        public class ObservePropertyChangedNoFilter
        {
            [Test]
            public void DoesNotSignalSubscribe()
            {
                var changes = new List<EventPattern<PropertyChangedEventArgs>>();
                var fake = new Fake { Value = 1 };
                var observable = fake.ObservePropertyChanged();
                using (observable.Subscribe(changes.Add))
                {
                    Assert.AreEqual(0, changes.Count);
                }
            }

            [TestCase("")]
            [TestCase(null)]
            [TestCase("Value")]
            public void ReactsOnStringEmptyOrNullWhenHasValue(string prop)
            {
                var changes = new List<EventPattern<PropertyChangedEventArgs>>();
                var fake = new Fake { Value = 1 };
                using (fake.ObservePropertyChanged()
                           .Subscribe(changes.Add))
                {
                    Assert.AreEqual(0, changes.Count);
                    fake.OnPropertyChanged(prop); // This means all properties changed according to wpf convention
                    Assert.AreEqual(1, changes.Count);
                }
            }

            [TestCase("")]
            [TestCase(null)]
            [TestCase("Name")]
            public void ReactsOnStringEmptyOrNullWhenNull(string prop)
            {
                var changes = new List<EventPattern<PropertyChangedEventArgs>>();
                var fake = new Fake { Name = null };
                using (fake.ObservePropertyChanged()
                           .Subscribe(changes.Add))
                {
                    Assert.AreEqual(0, changes.Count);
                    fake.OnPropertyChanged(prop); // This means all properties changed according to wpf convention
                    Assert.AreEqual(1, changes.Count);
                }
            }

            [Test]
            public void ReactsOnEvent()
            {
                var changes = new List<EventPattern<PropertyChangedEventArgs>>();
                var fake = new Fake { Value = 1 };
                using (fake.ObservePropertyChanged()
                           .Subscribe(changes.Add))
                {
                    Assert.AreEqual(0, changes.Count);
                    fake.OnPropertyChanged("SomeProp");
                    Assert.AreEqual(1, changes.Count);
                    EventPatternAssert.AreEqual(fake, "SomeProp", changes.Last());
                }
            }

            [Test]
            public void ReactsOnEventDerived()
            {
                var changes = new List<EventPattern<PropertyChangedEventArgs>>();
                var fake = new DerivedFake { Value = 1 };
                using (fake.ObservePropertyChanged()
                           .Subscribe(changes.Add))
                {
                    Assert.AreEqual(0, changes.Count);
                    fake.OnPropertyChanged("SomeProp");
                    Assert.AreEqual(1, changes.Count);
                    EventPatternAssert.AreEqual(fake, "SomeProp", changes.Last());
                }
            }

            [Test]
            public void ReactsValue()
            {
                var changes = new List<EventPattern<PropertyChangedEventArgs>>();
                var fake = new Fake { Value = 1 };
                using (fake.ObservePropertyChanged()
                           .Subscribe(changes.Add))
                {
                    Assert.AreEqual(0, changes.Count);
                    fake.Value++;
                    Assert.AreEqual(1, changes.Count);
                    EventPatternAssert.AreEqual(fake, "Value", changes.Last());
                }
            }

            [Test]
            public void ReactsTwoInstancesValue()
            {
                var changes = new List<EventPattern<PropertyChangedEventArgs>>();
                var fake1 = new Fake { Value = 1 };
                using (fake1.ObservePropertyChanged()
                            .Subscribe(changes.Add))
                {
                    var fake2 = new Fake { Value = 1 };
                    using (fake2.ObservePropertyChanged()
                                .Subscribe(changes.Add))
                    {
                        Assert.AreEqual(0, changes.Count);

                        fake1.Value++;
                        Assert.AreEqual(1, changes.Count);
                        EventPatternAssert.AreEqual(fake1, "Value", changes.Last());

                        fake2.Value++;
                        Assert.AreEqual(2, changes.Count);
                        EventPatternAssert.AreEqual(fake2, "Value", changes.Last());
                    }
                }
            }

            [Test]
            public void ReactsNullable()
            {
                var changes = new List<EventPattern<PropertyChangedEventArgs>>();
                var fake = new Fake { IsTrueOrNull = null };
                var observable = fake.ObservePropertyChanged();
                using (observable.Subscribe(changes.Add))
                {
                    Assert.AreEqual(0, changes.Count);

                    fake.IsTrueOrNull = true;
                    Assert.AreEqual(1, changes.Count);
                    EventPatternAssert.AreEqual(fake, "IsTrueOrNull", changes.Last());

                    fake.IsTrueOrNull = null;
                    Assert.AreEqual(2, changes.Count);
                    EventPatternAssert.AreEqual(fake, "IsTrueOrNull", changes.Last());
                }

                Assert.AreEqual(2, changes.Count);
                EventPatternAssert.AreEqual(fake, "IsTrueOrNull", changes.Last());
            }

            [Test]
            public void StopsListeningOnDispose()
            {
                var changes = new List<EventPattern<PropertyChangedEventArgs>>();
                var fake = new Fake { IsTrue = true };
                var observable = fake.ObservePropertyChanged();
                using (observable.Subscribe(changes.Add))
                {
                    fake.IsTrue = !fake.IsTrue;
                    Assert.AreEqual(1, changes.Count);
                }

                fake.IsTrue = !fake.IsTrue;
                Assert.AreEqual(1, changes.Count);
            }

            [Test]
            public void MemoryLeakDisposeTest()
            {
#if DEBUG
                Assert.Inconclusive("Debugger keeps things alive for the scope of the method.");
#endif
                var fake = new Fake();
                var wr = new WeakReference(fake);
                var observable = fake.ObservePropertyChanged();
                using (var subscription = observable.Subscribe())
                {
                    GC.KeepAlive(observable);
                    GC.KeepAlive(subscription);
                }

                GC.Collect();
                Assert.IsFalse(wr.IsAlive);
            }

            [Test]
            public void MemoryLeakNoDisposeTest()
            {
#if DEBUG
                Assert.Inconclusive("Debugger keeps things alive for the scope of the method.");
#endif
                var fake = new Fake();
                var wr = new WeakReference(fake);
                var observable = fake.ObservePropertyChanged();
#pragma warning disable IDISP001  // Dispose created.
                var subscription = observable.Subscribe();
#pragma warning restore IDISP001  // Dispose created.

                // ReSharper disable once RedundantAssignment
                fake = null;
                GC.Collect();

                Assert.IsFalse(wr.IsAlive);
            }
        }
    }
}
