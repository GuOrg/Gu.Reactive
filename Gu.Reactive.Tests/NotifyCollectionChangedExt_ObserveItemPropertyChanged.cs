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
        private List<EventPattern<ChildPropertyChangedEventArgs<FakeInpc, string>>> _changes;

        [SetUp]
        public void SetUp()
        {
            _changes = new List<EventPattern<ChildPropertyChangedEventArgs<FakeInpc, string>>>();
        }

        [Test]
        public void SignalsInitial()
        {
            var item1 = new FakeInpc { Name = "1" };
            var item2 = new FakeInpc { Name = "2" };
            var collection = new ObservableCollection<FakeInpc> { item1, item2 };
            var subscription = collection.ObserveItemPropertyChanges(x => x.Name, true)
                                         .Subscribe(_changes.Add);

            Assert.AreEqual(2, _changes.Count);
            Assert.AreSame(collection, _changes[0].Sender);
            Assert.AreSame(item1, _changes[0].EventArgs.OriginalSender);
            Assert.AreSame("1", _changes[0].EventArgs.Value);
            Assert.AreEqual("Name", _changes[0].EventArgs.PropertyName);

            Assert.AreSame(collection, _changes[1].Sender);
            Assert.AreSame(item2, _changes[1].EventArgs.OriginalSender);
            Assert.AreSame("2", _changes[1].EventArgs.Value);
            Assert.AreEqual("Name", _changes[1].EventArgs.PropertyName);
        }

        [Test]
        public void DoesNotSignalInitial()
        {
            var item1 = new FakeInpc { Name = "1" };
            var item2 = new FakeInpc { Name = "2" };
            var collection = new ObservableCollection<FakeInpc> { item1, item2 };
            var subscription = collection.ObserveItemPropertyChanges(x => x.Name, false)
                .Subscribe(_changes.Add);
            CollectionAssert.IsEmpty(_changes);
        }

        [Test]
        public void Reacts()
        {
            var item1 = new FakeInpc { Name = "1" };
            var item2 = new FakeInpc { Name = "2" };
            var collection = new ObservableCollection<FakeInpc> { item1, item2 };
            var subscription = collection.ObserveItemPropertyChanges(x => x.Name, false)
                                         .Subscribe(_changes.Add);
            CollectionAssert.IsEmpty(_changes);
            item1.Name = "new";
            Assert.AreEqual(1, _changes.Count);
            Assert.AreSame(collection, _changes[0].Sender);
            Assert.AreSame(item1, _changes[0].EventArgs.OriginalSender);
            Assert.AreSame("new", _changes[0].EventArgs.Value);
            Assert.AreEqual("Name", _changes[0].EventArgs.PropertyName);
        }

        [Test]
        public void ReactsNested()
        {
            var item1 = new FakeInpc { Next = new Level { Name = "1" } };
            var item2 = new FakeInpc();
            var collection = new ObservableCollection<FakeInpc> { item1, item2 };
            var subscription = collection.ObserveItemPropertyChanges(x => x.Next.Name, false)
                                         .Subscribe(_changes.Add);
            CollectionAssert.IsEmpty(_changes);
            item1.Next.Name = "new1";
            Assert.AreEqual(1, _changes.Count);
            Assert.AreSame(collection, _changes[0].Sender);
            Assert.AreSame(item1, _changes[0].EventArgs.OriginalSender);
            Assert.AreSame("new1", _changes[0].EventArgs.Value);
            Assert.AreEqual("Name", _changes[0].EventArgs.PropertyName);

            item2.Next = new Level { Name = "2" };
            Assert.AreEqual(2, _changes.Count);
            Assert.AreSame(collection, _changes[1].Sender);
            Assert.AreSame(item2, _changes[1].EventArgs.OriginalSender);
            Assert.AreSame("2", _changes[1].EventArgs.Value);
            Assert.AreEqual("Name", _changes[1].EventArgs.PropertyName);
        }

        [Test]
        public void ReactsOnceWhenSameItemIsTwoElements()
        {
            var item1 = new FakeInpc { Name = "1" };
            var collection = new ObservableCollection<FakeInpc> { item1, item1 };
            var subscription = collection.ObserveItemPropertyChanges(x => x.Name, false)
                                         .Subscribe(_changes.Add);
            CollectionAssert.IsEmpty(_changes);
            item1.Name = "new";
            Assert.AreEqual(1, _changes.Count);
            Assert.AreSame(collection, _changes[0].Sender);
            Assert.AreSame(item1, _changes[0].EventArgs.OriginalSender);
            Assert.AreSame("new", _changes[0].EventArgs.Value);
            Assert.AreEqual("Name", _changes[0].EventArgs.PropertyName);
        }

        [Test]
        public void HandlesNullItem()
        {
            var item1 = new FakeInpc { Name = "1" };
            var collection = new ObservableCollection<FakeInpc> { item1, null };
            var subscription = collection.ObserveItemPropertyChanges(x => x.Name, false)
                                         .Subscribe(_changes.Add);
            CollectionAssert.IsEmpty(_changes);
            collection.Add(null);
            item1.Name = "new";
            Assert.AreEqual(1, _changes.Count);
            Assert.AreSame(collection, _changes[0].Sender);
            Assert.AreSame(item1, _changes[0].EventArgs.OriginalSender);
            Assert.AreSame("new", _changes[0].EventArgs.Value);
            Assert.AreEqual("Name", _changes[0].EventArgs.PropertyName);
            collection.Remove(null);
            Assert.AreEqual(1, _changes.Count);
            var item2 = new FakeInpc { Name = "2" };
            collection[1] = item2;
            Assert.AreEqual(2, _changes.Count);
            Assert.AreSame(collection, _changes[1].Sender);
            Assert.AreSame(item2, _changes[1].EventArgs.OriginalSender);
            Assert.AreSame("2", _changes[1].EventArgs.Value);
            Assert.AreEqual("Name", _changes[1].EventArgs.PropertyName);
        }

        [Test]
        public void ReactsToAdd()
        {
            var item1 = new FakeInpc { Name = "1" };
            var item2 = new FakeInpc { Name = "2" };
            var collection = new ObservableCollection<FakeInpc> { item1, item2 };
            var subscription = collection.ObserveItemPropertyChanges(x => x.Name, false)
                                         .Subscribe(_changes.Add);
            CollectionAssert.IsEmpty(_changes);
            var item3 = new FakeInpc() { Name = "3" };
            collection.Add(item3);
            Assert.AreEqual(1, _changes.Count);
            Assert.AreSame(collection, _changes[0].Sender);
            Assert.AreSame(item3, _changes[0].EventArgs.OriginalSender);
            Assert.AreSame("3", _changes[0].EventArgs.Value);
            Assert.AreEqual("Name", _changes[0].EventArgs.PropertyName);
        }

        [Test]
        public void ReactsToReplace()
        {
            var item1 = new FakeInpc { Name = "1" };
            var item2 = new FakeInpc { Name = "2" };
            var collection = new ObservableCollection<FakeInpc> { item1, item2 };
            var subscription = collection.ObserveItemPropertyChanges(x => x.Name, false)
                                         .Subscribe(_changes.Add);
            CollectionAssert.IsEmpty(_changes);
            var item3 = new FakeInpc() { Name = "3" };
            collection[0] = item3;
            Assert.AreEqual(1, _changes.Count);
            Assert.AreSame(collection, _changes[0].Sender);
            Assert.AreSame(item3, _changes[0].EventArgs.OriginalSender);
            Assert.AreSame("3", _changes[0].EventArgs.Value);
            Assert.AreEqual("Name", _changes[0].EventArgs.PropertyName);
            item1.Name = "new";
            Assert.AreEqual(1, _changes.Count);

        }

        [Test]
        public void RemoveRemovesSubscription()
        {
            var item1 = new FakeInpc { Name = "1" };
            var item2 = new FakeInpc { Name = "2" };
            var collection = new ObservableCollection<FakeInpc> { item1, item2 };
            var subscription = collection.ObserveItemPropertyChanges(x => x.Name, false)
                                         .Subscribe(_changes.Add);
            CollectionAssert.IsEmpty(_changes);
            collection.Remove(item2);
            item2.Name = "new";
            CollectionAssert.IsEmpty(_changes);
        }

        [Test]
        public void DisposeStopsSubscribing()
        {
            var item1 = new FakeInpc { Name = "1" };
            var item2 = new FakeInpc { Name = "2" };
            var collection = new ObservableCollection<FakeInpc> { item1, item2 };
            var subscription = collection.ObserveItemPropertyChanges(x => x.Name, false)
                                         .Subscribe(_changes.Add);
            subscription.Dispose();
            collection.Add(new FakeInpc() { Name = "3" });
            CollectionAssert.IsEmpty(_changes);
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