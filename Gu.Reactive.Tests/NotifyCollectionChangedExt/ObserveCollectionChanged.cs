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

    public static class ObserveCollectionChanged
    {
        [Test]
        public static void SignalsInitial()
        {
            var changes = new List<NotifyCollectionChangedEventArgs>();
            var source = new ObservableCollection<int> { 1, 2 };
            var observable = source.ObserveCollectionChanged();
            using (observable.Subscribe(x => changes.Add(x.EventArgs)))
            {
                CollectionAssert.AreEqual(new[] { CachedEventArgs.NotifyCollectionReset }, changes);
            }

            CollectionAssert.AreEqual(new[] { CachedEventArgs.NotifyCollectionReset }, changes);

            using (observable.Subscribe(x => changes.Add(x.EventArgs)))
            {
                var expected = new[] { CachedEventArgs.NotifyCollectionReset, CachedEventArgs.NotifyCollectionReset };
                CollectionAssert.AreEqual(expected, changes);

                source.Add(1);
                Assert.AreEqual(3, changes.Count);
                Assert.AreEqual(NotifyCollectionChangedAction.Add, changes.Last().Action);
            }
        }

        [Test]
        public static void Add()
        {
            var changes = new List<NotifyCollectionChangedEventArgs>();
            var source = new ObservableCollection<int>();
            using (source.ObserveCollectionChanged(signalInitial: false)
                       .Subscribe(x => changes.Add(x.EventArgs)))
            {
                source.Add(1);
                Assert.AreEqual(1, changes.Count);
            }
        }

        [Test]
        public static void OneObservableTwoSubscriptions()
        {
            var changes1 = new List<NotifyCollectionChangedEventArgs>();
            var changes2 = new List<NotifyCollectionChangedEventArgs>();
            var source = new ObservableCollection<int>();
            var observable = source.ObserveCollectionChanged(signalInitial: false);
            using (observable.Subscribe(x => changes1.Add(x.EventArgs)))
            {
                using (observable.Subscribe(x => changes2.Add(x.EventArgs)))
                {
                    source.Add(1);
                    Assert.AreEqual(1, changes1.Count);
                    Assert.AreEqual(1, changes2.Count);

                    source.Add(2);
                    Assert.AreEqual(2, changes1.Count);
                    Assert.AreEqual(2, changes2.Count);
                }
            }
        }

        [Test]
        public static void ReactsOnView()
        {
            var changes = new List<EventPattern<NotifyCollectionChangedEventArgs>>();
            var source = new ObservableCollection<int>();
            using var view = source.AsReadOnlyFilteredView(x => true);
            using (view.ObserveCollectionChanged(signalInitial: false)
                       .Subscribe(x => changes.Add(x)))
            {
                source.Add(1);
                Assert.AreEqual(1, changes.Count);
                Assert.AreEqual(source, changes[0].Sender);
                Assert.AreEqual(NotifyCollectionChangedAction.Add, changes[0].EventArgs.Action);
            }
        }

        [Test]
        public static void Disposes()
        {
            var changes = new List<NotifyCollectionChangedEventArgs>();
            var source = new ObservableCollection<int>();
            using (source.ObserveCollectionChanged(signalInitial: false)
                       .Subscribe(x => changes.Add(x.EventArgs)))
            {
                source.Add(1);
                Assert.AreEqual(1, changes.Count);
            }

            source.Add(2);
            Assert.AreEqual(1, changes.Count);
        }

        [Test]
        public static void MemoryLeakDisposeTest()
        {
#if DEBUG
            Assert.Inconclusive("Debugger keeps things alive for the scope of the method.");
#endif
            var source = new ObservableCollection<int>();
            var wr = new WeakReference(source);
            var observable = source.ObserveCollectionChanged();
            using (var subscription = observable.Subscribe())
            {
                GC.KeepAlive(observable);
                GC.KeepAlive(subscription);
            }

            // ReSharper disable once RedundantAssignment
            source = null!;
            GC.Collect();
            Assert.IsFalse(wr.IsAlive);
        }

        [Test]
        public static void MemoryLeakNoDisposeTest()
        {
#if DEBUG
            Assert.Inconclusive("Debugger keeps things alive for the scope of the method.");
#endif

            var source = new ObservableCollection<int>();
            var wr = new WeakReference(source);
            var observable = source.ObserveCollectionChanged();
#pragma warning disable IDISP001  // Dispose created.
            var subscription = observable.Subscribe();
#pragma warning restore IDISP001  // Dispose created.
            GC.KeepAlive(observable);
            GC.KeepAlive(subscription);

            // ReSharper disable once RedundantAssignment
            source = null!;
            GC.Collect();

            Assert.IsFalse(wr.IsAlive);
        }
    }
}
