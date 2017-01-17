namespace Gu.Reactive.Tests.NotifyCollectionChangedExt
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reactive;

    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    // ReSharper disable once InconsistentNaming
    public class ObserveItemPropertyChangedTests
    {
        private List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>> changes;

        [SetUp]
        public void SetUp()
        {
            this.changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
        }

        [Test]
        public void SignalsInitial()
        {
            var item1 = new Fake { Name = "1" };
            var item2 = new Fake { Name = "2" };
            var collection = new ObservableCollection<Fake> { item1, item2 };
            using (collection.ObserveItemPropertyChanged(x => x.Name, true)
                             .Subscribe(this.changes.Add))
            {
                Assert.AreEqual(2, this.changes.Count);
                AssertRx.AreEqual(item1, "Name", item1, "1", this.changes[0]);
                AssertRx.AreEqual(item2, "Name", item2, "2", this.changes[1]);
            }

            Assert.AreEqual(2, this.changes.Count);
            AssertRx.AreEqual(item1, "Name", item1, "1", this.changes[0]);
            AssertRx.AreEqual(item2, "Name", item2, "2", this.changes[1]);
        }

        [Test]
        public void DoesNotSignalInitial()
        {
            var item1 = new Fake { Name = "1" };
            var item2 = new Fake { Name = "2" };
            var collection = new ObservableCollection<Fake> { item1, item2 };
            using (collection.ObserveItemPropertyChanged(x => x.Name, false)
                             .Subscribe(this.changes.Add))
            {
                CollectionAssert.IsEmpty(this.changes);
            }

            CollectionAssert.IsEmpty(this.changes);
        }

        [Test]
        public void ReactsWhenPropertyChanges()
        {
            var item1 = new Fake { Name = "1" };
            var item2 = new Fake { Name = "2" };
            var collection = new ObservableCollection<Fake> { item1, item2 };
            using (collection.ObserveItemPropertyChanged(x => x.Name, false)
                             .Subscribe(this.changes.Add))
            {
                CollectionAssert.IsEmpty(this.changes);
                item1.Name = "new1";
                Assert.AreEqual(1, this.changes.Count);
                AssertRx.AreEqual(item1, "Name", item1, "new1", this.changes.Last());

                item2.Name = "new2";
                Assert.AreEqual(2, this.changes.Count);
                AssertRx.AreEqual(item2, "Name", item2, "new2", this.changes.Last());
            }

            Assert.AreEqual(2, this.changes.Count);
            AssertRx.AreEqual(item2, "Name", item2, "new2", this.changes.Last());
        }

        [Test]
        public void ReactsWhenPropertyChangesView()
        {
            var item1 = new Fake { Name = "1" };
            var item2 = new Fake { Name = "2" };
            var collection = new ObservableCollection<Fake> { item1, item2 };
            using (var view = collection.AsReadOnlyFilteredView(x => true))
            {
                using (view.ObserveItemPropertyChanged(x => x.Name, false)
                           .Subscribe(this.changes.Add))
                {
                    CollectionAssert.IsEmpty(this.changes);
                    item1.Name = "new1";
                    Assert.AreEqual(1, this.changes.Count);
                    AssertRx.AreEqual(item1, "Name", item1, "new1", this.changes.Last());

                    item2.Name = "new2";
                    Assert.AreEqual(2, this.changes.Count);
                    AssertRx.AreEqual(item2, "Name", item2, "new2", this.changes.Last());
                }
            }

            Assert.AreEqual(2, this.changes.Count);
            AssertRx.AreEqual(item2, "Name", item2, "new2", this.changes.Last());
        }

        [Test]
        public void ReactsNested()
        {
            var item1 = new Fake { Next = new Level { Name = "1" } };
            var item2 = new Fake();
            var collection = new ObservableCollection<Fake> { item1, item2 };
            using (collection.ObserveItemPropertyChanged(x => x.Next.Name, false)
                             .Subscribe(this.changes.Add))
            {
                CollectionAssert.IsEmpty(this.changes);
                item1.Next.Name = "new1";
                Assert.AreEqual(1, this.changes.Count);
                AssertRx.AreEqual(item1.Next, "Name", item1, "new1", this.changes.Last());

                item2.Next = new Level { Name = "new2" };
                Assert.AreEqual(2, this.changes.Count);
                AssertRx.AreEqual(item2.Next, "Name", item2, "new2", this.changes.Last());
            }

            Assert.AreEqual(2, this.changes.Count);
            AssertRx.AreEqual(item2.Next, "Name", item2, "new2", this.changes.Last());
        }

        [Test]
        public void ReactsOnceWhenSameItemIsTwoElementsInCollection()
        {
            var item = new Fake { Name = "1" };
            var collection = new ObservableCollection<Fake> { item, item };
            using (collection.ObserveItemPropertyChanged(x => x.Name, false)
                             .Subscribe(this.changes.Add))
            {
                CollectionAssert.IsEmpty(this.changes);
                item.Name = "new";
                Assert.AreEqual(1, this.changes.Count);
                AssertRx.AreEqual(item, "Name", item, "new", this.changes.Last());
            }

            Assert.AreEqual(1, this.changes.Count);
            AssertRx.AreEqual(item, "Name", item, "new", this.changes.Last());
        }

        [Test]
        public void HandlesNullItem()
        {
            var item = new Fake { Name = "1" };
            var collection = new ObservableCollection<Fake> { item, null };
            using (collection.ObserveItemPropertyChanged(x => x.Name, false)
                             .Subscribe(this.changes.Add))
            {
                CollectionAssert.IsEmpty(this.changes);

                collection.Add(null);
                Assert.AreEqual(3, collection.Count);
                Assert.AreEqual(0, this.changes.Count);

                item.Name = "new";
                Assert.AreEqual(1, this.changes.Count);
                AssertRx.AreEqual(item, "Name", item, "new", this.changes.Last());

                var item2 = new Fake { Name = "2" };
                collection[1] = item2;
                Assert.AreEqual(2, this.changes.Count);
                AssertRx.AreEqual(item2, "Name", item2, "2", this.changes.Last());
            }

            Assert.AreEqual(2, this.changes.Count);
        }

        [Test]
        public void ReactsToAdd()
        {
            var item1 = new Fake { Name = "1" };
            var item2 = new Fake { Name = "2" };
            var collection = new ObservableCollection<Fake> { item1, item2 };
            using (collection.ObserveItemPropertyChanged(x => x.Name, false)
                             .Subscribe(this.changes.Add))
            {
                CollectionAssert.IsEmpty(this.changes);
                var item3 = new Fake { Name = "3" };
                collection.Add(item3);
                Assert.AreEqual(1, this.changes.Count);
                AssertRx.AreEqual(item3, "Name", item3, "3", this.changes.Last());

                item3.Name = "new";
                AssertRx.AreEqual(item3, "Name", item3, "new", this.changes.Last());
            }
        }

        [Test]
        public void ReactsToReplace()
        {
            var item1 = new Fake { Name = "1" };
            var item2 = new Fake { Name = "2" };
            var collection = new ObservableCollection<Fake> { item1, item2 };
            using (collection.ObserveItemPropertyChanged(x => x.Name, false)
                             .Subscribe(this.changes.Add))
            {
                CollectionAssert.IsEmpty(this.changes);
                var item3 = new Fake { Name = "3" };
                collection[0] = item3;
                Assert.AreEqual(1, this.changes.Count);
                AssertRx.AreEqual(item3, "Name", item3, "3", this.changes.Last());

                item1.Name = "new";
                Assert.AreEqual(1, this.changes.Count); // Stopped subscribing
            }

            Assert.AreEqual(1, this.changes.Count);
        }

        [Test]
        public void RemoveRemovesSubscription()
        {
            var item1 = new Fake { Name = "1" };
            var item2 = new Fake { Name = "2" };
            var collection = new ObservableCollection<Fake> { item1, item2 };
            using (collection.ObserveItemPropertyChanged(x => x.Name, false)
                             .Subscribe(this.changes.Add))
            {
                CollectionAssert.IsEmpty(this.changes);
                collection.Remove(item2);
                item2.Name = "new";
                CollectionAssert.IsEmpty(this.changes);
            }

            CollectionAssert.IsEmpty(this.changes);
        }

        [Test]
        public void DisposeStopsSubscribing()
        {
            var item1 = new Fake { Name = "1" };
            var item2 = new Fake { Name = "2" };
            var collection = new ObservableCollection<Fake> { item1, item2 };
            using (collection.ObserveItemPropertyChanged(x => x.Name, false)
                                                .Subscribe(this.changes.Add))
            {
            }

            collection.Add(new Fake { Name = "3" });
            CollectionAssert.IsEmpty(this.changes);
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
            //// http://stackoverflow.com/a/579001/1069200
            using (var subscription = observable.Subscribe())
            {
                GC.KeepAlive(observable);
                GC.KeepAlive(subscription);
                CollectionAssert.IsEmpty(this.changes);
            }

            GC.Collect();
            Assert.IsFalse(collectionRef.IsAlive);
            Assert.IsFalse(item1Ref.IsAlive);
        }

        [Test(Description = "Works in release build. Debug extends scope of variables.")]
        public void MemoryLeakNoDisposeTest()
        {
#if DEBUG
            Assert.Inconclusive();
#endif
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
            //// http://stackoverflow.com/a/579001/1069200
#pragma warning disable GU0030 // Use using.
            var subscription = observable.Subscribe();
#pragma warning restore GU0030 // Use using.
            GC.KeepAlive(observable);
            GC.KeepAlive(subscription);
            CollectionAssert.IsEmpty(this.changes);

            GC.Collect();

            Assert.IsFalse(collectionRef.IsAlive);
            Assert.IsFalse(item1Ref.IsAlive);
        }
    }
}