namespace Gu.Reactive.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;

    using NUnit.Framework;

    public class NotifyCollectionChangedExt
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
            var subscription = ints.ObservePropertyChanged().Subscribe();
            ints = null;
            subscription.Dispose();
            GC.Collect();
            Assert.IsFalse(wr.IsAlive);
            var s = subscription.ToString(); // touching it after GC.Collect for no optimizations
        }

        [Test]
        public void MemoryLeakNoDisposeTest()
        {
            var ints = new ObservableCollection<int>();
            var wr = new WeakReference(ints);
            var subscription = ints.ObservePropertyChanged().Subscribe();
            ints = null;
            GC.Collect();
            Assert.IsFalse(wr.IsAlive);
            var s = subscription.ToString(); // touching it after GC.Collect for no optimizations
        }
    }
}
