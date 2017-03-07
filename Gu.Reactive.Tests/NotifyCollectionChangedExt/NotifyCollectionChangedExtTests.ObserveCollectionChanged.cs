// ReSharper disable HeuristicUnreachableCode
namespace Gu.Reactive.Tests.NotifyCollectionChangedExt
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Reactive;

    using NUnit.Framework;

    // ReSharper disable once InconsistentNaming
    public partial class NotifyCollectionChangedExtTests
    {
        public class ObserveCollectionChanged
        {
            [Test]
            public void SignalsInitial()
            {
                var changes = new List<NotifyCollectionChangedEventArgs>();
                var ints = new ObservableCollection<int> { 1, 2 };
                var observable = ints.ObserveCollectionChanged();
                using (observable.Subscribe(x => changes.Add(x.EventArgs)))
                {
                    CollectionAssert.AreEqual(new[] { CachedEventArgs.NotifyCollectionReset }, changes);
                }

                CollectionAssert.AreEqual(new[] { CachedEventArgs.NotifyCollectionReset }, changes);

                using (observable.Subscribe(x => changes.Add(x.EventArgs)))
                {
                    var expected = new[] { CachedEventArgs.NotifyCollectionReset, CachedEventArgs.NotifyCollectionReset };
                    CollectionAssert.AreEqual(expected, changes);

                    ints.Add(1);
                    Assert.AreEqual(3, changes.Count);
                    Assert.AreEqual(NotifyCollectionChangedAction.Add, changes.Last().Action);
                }
            }

            [Test]
            public void Add()
            {
                var changes = new List<NotifyCollectionChangedEventArgs>();
                var ints = new ObservableCollection<int>();
                using (ints.ObserveCollectionChanged(false)
                           .Subscribe(x => changes.Add(x.EventArgs)))
                {
                    ints.Add(1);
                    Assert.AreEqual(1, changes.Count);
                }
            }

            [Test]
            public void OneObservableTwoSubscriptions()
            {
                var changes1 = new List<NotifyCollectionChangedEventArgs>();
                var changes2 = new List<NotifyCollectionChangedEventArgs>();
                var ints = new ObservableCollection<int>();
                var observable = ints.ObserveCollectionChanged(false);
                using (observable.Subscribe(x => changes1.Add(x.EventArgs)))
                {
                    using (observable.Subscribe(x => changes2.Add(x.EventArgs)))
                    {
                        ints.Add(1);
                        Assert.AreEqual(1, changes1.Count);
                        Assert.AreEqual(1, changes2.Count);

                        ints.Add(2);
                        Assert.AreEqual(2, changes1.Count);
                        Assert.AreEqual(2, changes2.Count);
                    }
                }
            }

            [Test]
            public void ReactsOnView()
            {
                var changes = new List<EventPattern<NotifyCollectionChangedEventArgs>>();
                var ints = new ObservableCollection<int>();
                using (var view = ints.AsReadOnlyFilteredView(x => true))
                {
                    using (view.ObserveCollectionChanged(false)
                               .Subscribe(x => changes.Add(x)))
                    {
                        ints.Add(1);
                        Assert.AreEqual(1, changes.Count);
                        Assert.AreEqual(ints, changes[0].Sender);
                        Assert.AreEqual(NotifyCollectionChangedAction.Add, changes[0].EventArgs.Action);
                    }
                }
            }

            [Test]
            public void Disposes()
            {
                var changes = new List<NotifyCollectionChangedEventArgs>();
                var ints = new ObservableCollection<int>();
                using (ints.ObserveCollectionChanged(false)
                           .Subscribe(x => changes.Add(x.EventArgs)))
                {
                    ints.Add(1);
                    Assert.AreEqual(1, changes.Count);
                }

                ints.Add(2);
                Assert.AreEqual(1, changes.Count);
            }

            [Test]
            public void MemoryLeakDisposeTest()
            {
#if DEBUG
            Assert.Inconclusive("Debugger keeps things alive for the scope of the method.");
#endif
                var ints = new ObservableCollection<int>();
                var wr = new WeakReference(ints);
                var observable = ints.ObserveCollectionChanged();
                using (var subscription = observable.Subscribe())
                {
                    GC.KeepAlive(observable);
                    GC.KeepAlive(subscription);
                }

                // ReSharper disable once RedundantAssignment
                ints = null;
                GC.Collect();
                Assert.IsFalse(wr.IsAlive);
            }

            [Test]
            public void MemoryLeakNoDisposeTest()
            {
#if DEBUG
            Assert.Inconclusive("Debugger keeps things alive for the scope of the method.");
#endif

                var ints = new ObservableCollection<int>();
                var wr = new WeakReference(ints);
                var observable = ints.ObserveCollectionChanged();
#pragma warning disable GU0030 // Use using.
                var subscription = observable.Subscribe();
#pragma warning restore GU0030 // Use using.
                GC.KeepAlive(observable);
                GC.KeepAlive(subscription);

                // ReSharper disable once RedundantAssignment
                ints = null;
                GC.Collect();

                Assert.IsFalse(wr.IsAlive);
            }
        }
    }
}
