namespace Gu.Reactive.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Reactive;

    using NUnit.Framework;

    public class NotifyCollectionChangedExt_ObserveItemPropertyChanged
    {
        private List<EventPattern<ChildPropertyChangedEventArgs<FakeInpc, string>>> changes;

        [SetUp]
        public void SetUp()
        {
            changes = new List<EventPattern<ChildPropertyChangedEventArgs<FakeInpc, string>>>();
        }

        [Test]
        public void SignalsInitial()
        {
            var item1 = new FakeInpc { Name = "1" };
            var item2 = new FakeInpc { Name = "2" };
            var collection = new ObservableCollection<FakeInpc> { item1, item2 };
            var subscription = collection.ObserveItemPropertyChanges(x => x.Name)
                                   .Subscribe(changes.Add);

            Assert.AreEqual(2, changes.Count);
            Assert.AreSame(collection, changes[0].Sender);
            Assert.AreSame(item1, changes[0].EventArgs.OriginalSender);
            Assert.AreSame("1", changes[0].EventArgs.CurrentValue);
            Assert.AreSame("Name", changes[0].EventArgs.PropertyName);

            Assert.AreSame(collection, changes[1].Sender);
            Assert.AreSame(item1, changes[1].EventArgs.OriginalSender);
            Assert.AreSame("2", changes[1].EventArgs.CurrentValue);
            Assert.AreSame("Name", changes[1].EventArgs.PropertyName);
        }

        [Test]
        public void Reacts()
        {
            Assert.Fail();
            //var ints = new ObservableCollection<int>();
            //var subscription = ints.ObserveCollectionChanged(false)
            //                       .Subscribe(x => changes.Add(x.EventArgs));
            //ints.Add(1);
            //Assert.AreEqual(1, changes.Count);
        }

        [Test]
        public void Disposes()
        {
            Assert.Fail();
            //var ints = new ObservableCollection<int>();
            //var subscription = ints.ObserveCollectionChanged(false)
            //                       .Subscribe(x => changes.Add(x.EventArgs));
            //ints.Add(1);
            //Assert.AreEqual(1, changes.Count);
            //subscription.Dispose();
            //ints.Add(2);
            //Assert.AreEqual(1, changes.Count);
        }

        [Test]
        public void MemoryLeakDisposeTest()
        {
            Assert.Fail();
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
            Assert.Fail();
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