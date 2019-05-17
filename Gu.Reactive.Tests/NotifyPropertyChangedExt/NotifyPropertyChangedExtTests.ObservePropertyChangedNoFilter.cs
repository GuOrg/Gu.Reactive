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
                var source = new Fake { Value = 1 };
                using (source.ObservePropertyChanged()
                             .Subscribe(changes.Add))
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
                var source = new Fake { Value = 1 };
                using (source.ObservePropertyChanged()
                             .Subscribe(changes.Add))
                {
                    Assert.AreEqual(0, changes.Count);
                    source.OnPropertyChanged(prop); // This means all properties changed according to wpf convention
                    Assert.AreEqual(1, changes.Count);
                }
            }

            [TestCase("")]
            [TestCase(null)]
            [TestCase("Name")]
            public void ReactsOnStringEmptyOrNullWhenNull(string prop)
            {
                var changes = new List<EventPattern<PropertyChangedEventArgs>>();
                var source = new Fake { Name = null };
                using (source.ObservePropertyChanged()
                             .Subscribe(changes.Add))
                {
                    Assert.AreEqual(0, changes.Count);
                    source.OnPropertyChanged(prop); // This means all properties changed according to wpf convention
                    Assert.AreEqual(1, changes.Count);
                }
            }

            [Test]
            public void ReactsOnEvent()
            {
                var changes = new List<EventPattern<PropertyChangedEventArgs>>();
                var source = new Fake { Value = 1 };
                using (source.ObservePropertyChanged()
                             .Subscribe(changes.Add))
                {
                    Assert.AreEqual(0, changes.Count);
                    source.OnPropertyChanged("SomeProp");
                    Assert.AreEqual(1, changes.Count);
                    EventPatternAssert.AreEqual(source, "SomeProp", changes.Last());
                }
            }

            [Test]
            public void ReactsOnEventDerived()
            {
                var changes = new List<EventPattern<PropertyChangedEventArgs>>();
                var source = new DerivedFake { Value = 1 };
                using (source.ObservePropertyChanged()
                             .Subscribe(changes.Add))
                {
                    Assert.AreEqual(0, changes.Count);
                    source.OnPropertyChanged("SomeProp");
                    Assert.AreEqual(1, changes.Count);
                    EventPatternAssert.AreEqual(source, "SomeProp", changes.Last());
                }
            }

            [Test]
            public void ReactsValue()
            {
                var changes = new List<EventPattern<PropertyChangedEventArgs>>();
                var source = new Fake { Value = 1 };
                using (source.ObservePropertyChanged()
                             .Subscribe(changes.Add))
                {
                    Assert.AreEqual(0, changes.Count);
                    source.Value++;
                    Assert.AreEqual(1, changes.Count);
                    EventPatternAssert.AreEqual(source, "Value", changes.Last());
                }
            }

            [Test]
            public void ReactsTwoInstancesValue()
            {
                var changes = new List<EventPattern<PropertyChangedEventArgs>>();
                var source1 = new Fake { Value = 1 };
                using (source1.ObservePropertyChanged()
                              .Subscribe(changes.Add))
                {
                    var source2 = new Fake { Value = 1 };
                    using (source2.ObservePropertyChanged()
                                  .Subscribe(changes.Add))
                    {
                        Assert.AreEqual(0, changes.Count);

                        source1.Value++;
                        Assert.AreEqual(1, changes.Count);
                        EventPatternAssert.AreEqual(source1, "Value", changes.Last());

                        source2.Value++;
                        Assert.AreEqual(2, changes.Count);
                        EventPatternAssert.AreEqual(source2, "Value", changes.Last());
                    }
                }
            }

            [Test]
            public void ReactsNullable()
            {
                var changes = new List<EventPattern<PropertyChangedEventArgs>>();
                var source = new Fake { IsTrueOrNull = null };
                using (source.ObservePropertyChanged()
                             .Subscribe(changes.Add))
                {
                    Assert.AreEqual(0, changes.Count);

                    source.IsTrueOrNull = true;
                    Assert.AreEqual(1, changes.Count);
                    EventPatternAssert.AreEqual(source, "IsTrueOrNull", changes.Last());

                    source.IsTrueOrNull = null;
                    Assert.AreEqual(2, changes.Count);
                    EventPatternAssert.AreEqual(source, "IsTrueOrNull", changes.Last());
                }

                Assert.AreEqual(2, changes.Count);
                EventPatternAssert.AreEqual(source, "IsTrueOrNull", changes.Last());
            }

            [Test]
            public void StopsListeningOnDispose()
            {
                var changes = new List<EventPattern<PropertyChangedEventArgs>>();
                var source = new Fake { IsTrue = true };
                using (source.ObservePropertyChanged()
                             .Subscribe(changes.Add))
                {
                    source.IsTrue = !source.IsTrue;
                    Assert.AreEqual(1, changes.Count);
                }

                source.IsTrue = !source.IsTrue;
                Assert.AreEqual(1, changes.Count);
            }

            [Test]
            public void MemoryLeakDisposeTest()
            {
#if DEBUG
                Assert.Inconclusive("Debugger keeps things alive for the scope of the method.");
#endif
                var source = new Fake();
                var wr = new WeakReference(source);
                var observable = source.ObservePropertyChanged();
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
                var source = new Fake();
                var wr = new WeakReference(source);
                var observable = source.ObservePropertyChanged();
#pragma warning disable IDISP001  // Dispose created.
                var subscription = observable.Subscribe();
#pragma warning restore IDISP001  // Dispose created.

                // ReSharper disable once RedundantAssignment
                source = null;
                GC.Collect();

                Assert.IsFalse(wr.IsAlive);
            }
        }
    }
}
