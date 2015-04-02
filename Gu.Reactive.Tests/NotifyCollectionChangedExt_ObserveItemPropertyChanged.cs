namespace Gu.Reactive.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Reactive;

    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    public class NotifyCollectionChangedExt_ObserveItemPropertyChanged
    {
        private List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>> _changes;

        [SetUp]
        public void SetUp()
        {
            _changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
        }

        [Test]
        public void SignalsInitial()
        {
            var item1 = new Fake { Name = "1" };
            var item2 = new Fake { Name = "2" };
            var collection = new ObservableCollection<Fake> { item1, item2 };
            var subscription = collection.ObserveItemPropertyChanged(x => x.Name, true)
                                         .Subscribe(_changes.Add);

            Assert.AreEqual(2, _changes.Count);
            Assert.AreSame(collection, _changes[0].Sender);
            Assert.AreSame(item1, _changes[0].EventArgs.Item);
            Assert.AreSame("1", _changes[0].EventArgs.Value);
            Assert.AreEqual("Name", _changes[0].EventArgs.PropertyName);

            Assert.AreSame(collection, _changes[1].Sender);
            Assert.AreSame(item2, _changes[1].EventArgs.Item);
            Assert.AreSame("2", _changes[1].EventArgs.Value);
            Assert.AreEqual("Name", _changes[1].EventArgs.PropertyName);
        }

        [Test]
        public void DoesNotSignalInitial()
        {
            var item1 = new Fake { Name = "1" };
            var item2 = new Fake { Name = "2" };
            var collection = new ObservableCollection<Fake> { item1, item2 };
            var subscription = collection.ObserveItemPropertyChanged(x => x.Name, false)
                                         .Subscribe(_changes.Add);
            CollectionAssert.IsEmpty(_changes);
        }

        [Test]
        public void Reacts()
        {
            var item1 = new Fake { Name = "1" };
            var item2 = new Fake { Name = "2" };
            var collection = new ObservableCollection<Fake> { item1, item2 };
            var subscription = collection.ObserveItemPropertyChanged(x => x.Name, false)
                                         .Subscribe(_changes.Add);
            CollectionAssert.IsEmpty(_changes);
            item1.Name = "new";
            Assert.AreEqual(1, _changes.Count);
            Assert.AreSame(collection, _changes[0].Sender);
            Assert.AreSame(item1, _changes[0].EventArgs.Item);
            Assert.AreSame("new", _changes[0].EventArgs.Value);
            Assert.AreEqual("Name", _changes[0].EventArgs.PropertyName);
        }

        [Test]
        public void ReactsNested()
        {
            var item1 = new Fake { Next = new Level { Name = "1" } };
            var item2 = new Fake();
            var collection = new ObservableCollection<Fake> { item1, item2 };
            var subscription = collection.ObserveItemPropertyChanged(x => x.Next.Name, false)
                                         .Subscribe(_changes.Add);
            CollectionAssert.IsEmpty(_changes);
            item1.Next.Name = "new1";
            Assert.AreEqual(1, _changes.Count);
            Assert.AreSame(collection, _changes[0].Sender);
            Assert.AreSame(item1, _changes[0].EventArgs.Item);
            Assert.AreSame("new1", _changes[0].EventArgs.Value);
            Assert.AreEqual("Name", _changes[0].EventArgs.PropertyName);

            item2.Next = new Level { Name = "2" };
            Assert.AreEqual(2, _changes.Count);
            Assert.AreSame(collection, _changes[1].Sender);
            Assert.AreSame(item2, _changes[1].EventArgs.Item);
            Assert.AreSame("2", _changes[1].EventArgs.Value);
            Assert.AreEqual("Name", _changes[1].EventArgs.PropertyName);
        }

        [Test]
        public void ReactsOnceWhenSameItemIsTwoElements()
        {
            var item1 = new Fake { Name = "1" };
            var collection = new ObservableCollection<Fake> { item1, item1 };
            var subscription = collection.ObserveItemPropertyChanged(x => x.Name, false)
                                         .Subscribe(_changes.Add);
            CollectionAssert.IsEmpty(_changes);
            item1.Name = "new";
            Assert.AreEqual(1, _changes.Count);
            Assert.AreSame(collection, _changes[0].Sender);
            Assert.AreSame(item1, _changes[0].EventArgs.Item);
            Assert.AreSame("new", _changes[0].EventArgs.Value);
            Assert.AreEqual("Name", _changes[0].EventArgs.PropertyName);
        }

        [Test]
        public void HandlesNullItem()
        {
            var item1 = new Fake { Name = "1" };
            var collection = new ObservableCollection<Fake> { item1, null };
            var subscription = collection.ObserveItemPropertyChanged(x => x.Name, false)
                                         .Subscribe(_changes.Add);
            CollectionAssert.IsEmpty(_changes);
            collection.Add(null);
            item1.Name = "new";
            Assert.AreEqual(1, _changes.Count);
            Assert.AreSame(collection, _changes[0].Sender);
            Assert.AreSame(item1, _changes[0].EventArgs.Item);
            Assert.AreSame("new", _changes[0].EventArgs.Value);
            Assert.AreEqual("Name", _changes[0].EventArgs.PropertyName);
            collection.Remove(null);
            Assert.AreEqual(1, _changes.Count);
            var item2 = new Fake { Name = "2" };
            collection[1] = item2;
            Assert.AreEqual(2, _changes.Count);
            Assert.AreSame(collection, _changes[1].Sender);
            Assert.AreSame(item2, _changes[1].EventArgs.Item);
            Assert.AreSame("2", _changes[1].EventArgs.Value);
            Assert.AreEqual("Name", _changes[1].EventArgs.PropertyName);
        }

        [Test]
        public void ReactsToAdd()
        {
            var item1 = new Fake { Name = "1" };
            var item2 = new Fake { Name = "2" };
            var collection = new ObservableCollection<Fake> { item1, item2 };
            var subscription = collection.ObserveItemPropertyChanged(x => x.Name, false)
                                         .Subscribe(_changes.Add);
            CollectionAssert.IsEmpty(_changes);
            var item3 = new Fake() { Name = "3" };
            collection.Add(item3);
            Assert.AreEqual(1, _changes.Count);
            Assert.AreSame(collection, _changes[0].Sender);
            Assert.AreSame(item3, _changes[0].EventArgs.Item);
            Assert.AreSame("3", _changes[0].EventArgs.Value);
            Assert.AreEqual("Name", _changes[0].EventArgs.PropertyName);
        }

        [Test]
        public void ReactsToReplace()
        {
            var item1 = new Fake { Name = "1" };
            var item2 = new Fake { Name = "2" };
            var collection = new ObservableCollection<Fake> { item1, item2 };
            var subscription = collection.ObserveItemPropertyChanged(x => x.Name, false)
                                         .Subscribe(_changes.Add);
            CollectionAssert.IsEmpty(_changes);
            var item3 = new Fake() { Name = "3" };
            collection[0] = item3;
            Assert.AreEqual(1, _changes.Count);
            Assert.AreSame(collection, _changes[0].Sender);
            Assert.AreSame(item3, _changes[0].EventArgs.Item);
            Assert.AreSame("3", _changes[0].EventArgs.Value);
            Assert.AreEqual("Name", _changes[0].EventArgs.PropertyName);
            item1.Name = "new";
            Assert.AreEqual(1, _changes.Count);

        }

        [Test]
        public void RemoveRemovesSubscription()
        {
            var item1 = new Fake { Name = "1" };
            var item2 = new Fake { Name = "2" };
            var collection = new ObservableCollection<Fake> { item1, item2 };
            var subscription = collection.ObserveItemPropertyChanged(x => x.Name, false)
                                         .Subscribe(_changes.Add);
            CollectionAssert.IsEmpty(_changes);
            collection.Remove(item2);
            item2.Name = "new";
            CollectionAssert.IsEmpty(_changes);
        }

        [Test]
        public void DisposeStopsSubscribing()
        {
            var item1 = new Fake { Name = "1" };
            var item2 = new Fake { Name = "2" };
            var collection = new ObservableCollection<Fake> { item1, item2 };
            var subscription = collection.ObserveItemPropertyChanged(x => x.Name, false)
                                         .Subscribe(_changes.Add);
            subscription.Dispose();
            collection.Add(new Fake() { Name = "3" });
            CollectionAssert.IsEmpty(_changes);
        }

        [Test]
        public void MemoryLeakDisposeTest()
        {
            var collectionRef = new WeakReference(null);
            var item1Ref = new WeakReference(null);
            IObservable<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>> observable = null;
            new Action(
                () =>
                {
                    var collection = new ObservableCollection<Fake> { new Fake { Name = "1" }, new Fake { Name = "2" } };
                    collectionRef.Target = collection;
                    item1Ref.Target = collection[0];
                    Assert.IsTrue(collectionRef.IsAlive);
                    observable = collection.ObserveItemPropertyChanged(x => x.Name, false);
                })();
            // http://stackoverflow.com/a/579001/1069200
            var subscription = observable.Subscribe();
            GC.KeepAlive(observable);
            GC.KeepAlive(subscription);
            CollectionAssert.IsEmpty(_changes);
            
            subscription.Dispose();
            
            GC.Collect();
            Assert.IsFalse(collectionRef.IsAlive);
            Assert.IsFalse(item1Ref.IsAlive);
        }

        [Test, Explicit("Fix this")]
        public void MemoryLeakNoDisposeTest()
        {
            var collectionRef = new WeakReference(null);
            var item1Ref = new WeakReference(null);
            IObservable<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>> observable = null;
            new Action(
                () =>
                {
                    var collection = new ObservableCollection<Fake> { new Fake { Name = "1" }, new Fake { Name = "2" } };
                    collectionRef.Target = collection;
                    item1Ref.Target = collection[0];
                    Assert.IsTrue(collectionRef.IsAlive);
                    observable = collection.ObserveItemPropertyChanged(x => x.Name, false);
                })();
            // http://stackoverflow.com/a/579001/1069200
            var subscription = observable.Subscribe();
            GC.KeepAlive(observable);
            GC.KeepAlive(subscription);
            CollectionAssert.IsEmpty(_changes);
          
            GC.Collect();
            
            Assert.IsFalse(collectionRef.IsAlive);
            Assert.IsFalse(item1Ref.IsAlive);
        }
    }
}