namespace Gu.Reactive.Tests.NotifyCollectionChangedExt
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;

    using NUnit.Framework;

    // ReSharper disable once InconsistentNaming
    public class ObserveCollectionChangedTests
    {
        [Test]
        public void SignalsInitial()
        {
            var changes = new List<NotifyCollectionChangedEventArgs>();
            var ints = new ObservableCollection<int> { 1, 2 };
            var subscription = ints.ObserveCollectionChanged()
                                   .Subscribe(x => changes.Add(x.EventArgs));

            Assert.AreEqual(1, changes.Count);
            Assert.AreEqual(NotifyCollectionChangedAction.Reset, changes[0].Action);
            Assert.IsNull(changes[0].NewItems);
            Assert.IsNull(changes[0].OldItems);

            ints.Add(1);
            Assert.AreEqual(2, changes.Count);
            Assert.AreEqual(NotifyCollectionChangedAction.Add, changes[1].Action);
        }

        [Test]
        public void Reacts()
        {
            var changes = new List<NotifyCollectionChangedEventArgs>();
            var ints = new ObservableCollection<int>();
            var subscription = ints.ObserveCollectionChanged(false)
                                   .Subscribe(x => changes.Add(x.EventArgs));
            ints.Add(1);
            Assert.AreEqual(1, changes.Count);
        }

        [Test]
        public void Disposes()
        {
            var changes = new List<NotifyCollectionChangedEventArgs>();
            var ints = new ObservableCollection<int>();
            var subscription = ints.ObserveCollectionChanged(false)
                                   .Subscribe(x => changes.Add(x.EventArgs));
            ints.Add(1);
            Assert.AreEqual(1, changes.Count);
            subscription.Dispose();
            ints.Add(2);
            Assert.AreEqual(1, changes.Count);
        }

        [Test]
        public void MemoryLeakDisposeTest()
        {
            var ints = new ObservableCollection<int>();
            var wr = new WeakReference(ints);
            var observable = ints.ObservePropertyChanged();
            var subscription = observable.Subscribe();
            GC.KeepAlive(observable);
            GC.KeepAlive(subscription);

            ints = null;
            subscription.Dispose();
            
            GC.Collect();
            Assert.IsFalse(wr.IsAlive);
        }

        [Test]
        public void MemoryLeakNoDisposeTest()
        {
            var ints = new ObservableCollection<int>();
            var wr = new WeakReference(ints);
            var observable = ints.ObservePropertyChanged();
            var subscription = observable.Subscribe();
            GC.KeepAlive(observable);
            GC.KeepAlive(subscription);

            ints = null;
            GC.Collect();

            Assert.IsFalse(wr.IsAlive);
        }
    }
}
