// ReSharper disable RedundantArgumentDefaultValue
// ReSharper disable ClassNeverInstantiated.Global
namespace Gu.Reactive.Tests.NotifyCollectionChangedExt
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reactive;

    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    public partial class NotifyCollectionChangedExtTests
    {
        public class ObserveItemPropertyChanged
        {
            [Test]
            public void SignalsInitial()
            {
                var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
                var item1 = new Fake { Name = "1" };
                var item2 = new Fake { Name = "2" };
                var collection = new ObservableCollection<Fake> { item1, item2 };
                using (collection.ObserveItemPropertyChanged(x => x.Name, true)
                                 .Subscribe(changes.Add))
                {
                    Assert.AreEqual(2, changes.Count);
                    EventPatternAssert.AreEqual(item1, string.Empty, item1, "1", changes[0]);
                    EventPatternAssert.AreEqual(item2, string.Empty, item2, "2", changes[1]);
                }

                Assert.AreEqual(2, changes.Count);
                EventPatternAssert.AreEqual(item1, string.Empty, item1, "1", changes[0]);
                EventPatternAssert.AreEqual(item2, string.Empty, item2, "2", changes[1]);
            }

            [Test]
            public void ReadOnlyObservableCollectionCount()
            {
                var ints = new ObservableCollection<int> { 1 };
                var item1 = new ReadOnlyObservableCollection<int>(ints);

                var source = new ObservableCollection<ReadOnlyObservableCollection<int>>();
                var changes =
                    new List<EventPattern<ItemPropertyChangedEventArgs<ReadOnlyObservableCollection<int>, int>>>();
                using (source.ObserveItemPropertyChanged(x => x.Count, false)
                             .Subscribe(x => changes.Add(x)))
                {
                    CollectionAssert.IsEmpty(changes);

                    source.Add(item1);
                    EventPatternAssert.AreEqual(item1, string.Empty, item1, 1, changes[0]);
                }
            }

            [Test]
            public void DoesNotSignalInitial()
            {
                var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
                var item1 = new Fake { Name = "1" };
                var item2 = new Fake { Name = "2" };
                var collection = new ObservableCollection<Fake> { item1, item2 };
                using (collection.ObserveItemPropertyChanged(x => x.Name, false)
                                 .Subscribe(changes.Add))
                {
                    CollectionAssert.IsEmpty(changes);
                }

                CollectionAssert.IsEmpty(changes);
            }

            [Test]
            public void ReactsWhenPropertyChanges()
            {
                var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
                var item1 = new Fake { Name = "1" };
                var item2 = new Fake { Name = "2" };
                var collection = new ObservableCollection<Fake> { item1, item2 };
                using (collection.ObserveItemPropertyChanged(x => x.Name, false)
                                 .Subscribe(changes.Add))
                {
                    CollectionAssert.IsEmpty(changes);
                    item1.Name = "new1";
                    Assert.AreEqual(1, changes.Count);
                    EventPatternAssert.AreEqual(item1, "Name", item1, "new1", changes.Last());

                    item2.Name = "new2";
                    Assert.AreEqual(2, changes.Count);
                    EventPatternAssert.AreEqual(item2, "Name", item2, "new2", changes.Last());
                }

                Assert.AreEqual(2, changes.Count);
                EventPatternAssert.AreEqual(item2, "Name", item2, "new2", changes.Last());
            }

            [Test]
            public void ReactsWhenPropertyChangesSameInstanceTwice()
            {
                var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
                var item = new Fake { Name = "1" };
                var collection = new ObservableCollection<Fake> { item };
                using (collection.ObserveItemPropertyChanged(x => x.Name, false)
                                 .Subscribe(changes.Add))
                {
                    CollectionAssert.IsEmpty(changes);
                    item.Name = "new1";
                    Assert.AreEqual(1, changes.Count);
                    EventPatternAssert.AreEqual(item, "Name", item, "new1", changes[0]);

                    collection.Add(item);
                    Assert.AreEqual(1, changes.Count);
                    EventPatternAssert.AreEqual(item, "Name", item, "new1", changes[0]);

                    item.Name = "new2";
                    Assert.AreEqual(2, changes.Count);
                    EventPatternAssert.AreEqual(item, "Name", item, "new2", changes.Last());

                    collection.RemoveAt(1);
                    Assert.AreEqual(2, changes.Count);
                    EventPatternAssert.AreEqual(item, "Name", item, "new2", changes.Last());

                    item.Name = "new3";
                    Assert.AreEqual(3, changes.Count);
                    EventPatternAssert.AreEqual(item, "Name", item, "new3", changes.Last());
                }

                Assert.AreEqual(3, changes.Count);
                EventPatternAssert.AreEqual(item, "Name", item, "new3", changes.Last());
            }

            [Test]
            public void OneObservableTwoSubscriptions()
            {
                var changes1 = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
                var changes2 = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
                var item1 = new Fake { Name = "1" };
                var item2 = new Fake { Name = "2" };
                var collection = new ObservableCollection<Fake> { item1, item2 };
                var observable = collection.ObserveItemPropertyChanged(x => x.Name, false);
                using (observable.Subscribe(changes1.Add))
                {
                    using (observable.Subscribe(changes2.Add))
                    {
                        CollectionAssert.IsEmpty(changes1);
                        CollectionAssert.IsEmpty(changes2);

                        item1.Name = "new1";
                        Assert.AreEqual(1, changes1.Count);
                        Assert.AreEqual(1, changes2.Count);
                        EventPatternAssert.AreEqual(item1, "Name", item1, "new1", changes1.Last());
                        EventPatternAssert.AreEqual(item1, "Name", item1, "new1", changes2.Last());

                        item2.Name = "new2";
                        Assert.AreEqual(2, changes1.Count);
                        Assert.AreEqual(2, changes2.Count);
                        EventPatternAssert.AreEqual(item2, "Name", item2, "new2", changes1.Last());
                        EventPatternAssert.AreEqual(item2, "Name", item2, "new2", changes2.Last());
                    }
                }

                Assert.AreEqual(2, changes1.Count);
                Assert.AreEqual(2, changes2.Count);
                EventPatternAssert.AreEqual(item2, "Name", item2, "new2", changes1.Last());
                EventPatternAssert.AreEqual(item2, "Name", item2, "new2", changes2.Last());
            }

            [Test]
            public void ReactsWhenPropertyChangesView()
            {
                var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
                var item1 = new Fake { Name = "1" };
                var item2 = new Fake { Name = "2" };
                var collection = new ObservableCollection<Fake> { item1, item2 };
                using (var view = collection.AsReadOnlyFilteredView(x => true))
                {
                    using (view.ObserveItemPropertyChanged(x => x.Name, false)
                               .Subscribe(changes.Add))
                    {
                        CollectionAssert.IsEmpty(changes);
                        item1.Name = "new1";
                        Assert.AreEqual(1, changes.Count);
                        EventPatternAssert.AreEqual(item1, "Name", item1, "new1", changes.Last());

                        item2.Name = "new2";
                        Assert.AreEqual(2, changes.Count);
                        EventPatternAssert.AreEqual(item2, "Name", item2, "new2", changes.Last());
                    }
                }

                Assert.AreEqual(2, changes.Count);
                EventPatternAssert.AreEqual(item2, "Name", item2, "new2", changes.Last());
            }

            [Test]
            public void ReactsNested()
            {
                var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
                var item1 = new Fake { Next = new Level { Name = "1" } };
                var item2 = new Fake();
                var collection = new ObservableCollection<Fake> { item1, item2 };
                using (collection.ObserveItemPropertyChanged(x => x.Next.Name, false)
                                 .Subscribe(changes.Add))
                {
                    CollectionAssert.IsEmpty(changes);
                    item1.Next.Name = "new1";
                    Assert.AreEqual(1, changes.Count);
                    EventPatternAssert.AreEqual(item1.Next, "Name", item1, "new1", changes.Last());

                    item2.Next = new Level { Name = "new2" };
                    Assert.AreEqual(2, changes.Count);
                    EventPatternAssert.AreEqual(item2, "Next", item2, "new2", changes.Last());
                }

                Assert.AreEqual(2, changes.Count);
                EventPatternAssert.AreEqual(item2, "Next", item2, "new2", changes.Last());
            }

            [Test]
            public void ReactsOnceWhenSameItemIsTwoElementsInCollection()
            {
                var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
                var item = new Fake { Name = "1" };
                var collection = new ObservableCollection<Fake> { item, item };
                using (collection.ObserveItemPropertyChanged(x => x.Name, false)
                                 .Subscribe(changes.Add))
                {
                    CollectionAssert.IsEmpty(changes);
                    item.Name = "new";
                    Assert.AreEqual(1, changes.Count);
                    EventPatternAssert.AreEqual(item, "Name", item, "new", changes.Last());
                }

                Assert.AreEqual(1, changes.Count);
                EventPatternAssert.AreEqual(item, "Name", item, "new", changes.Last());
            }

            [Test]
            public void HandlesNullItem()
            {
                var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
                var item = new Fake { Name = "1" };
                var collection = new ObservableCollection<Fake> { item, null };
                using (collection.ObserveItemPropertyChanged(x => x.Name, false)
                                 .Subscribe(changes.Add))
                {
                    CollectionAssert.IsEmpty(changes);

                    collection.Add(null);
                    Assert.AreEqual(3, collection.Count);
                    Assert.AreEqual(0, changes.Count);

                    item.Name = "new";
                    Assert.AreEqual(1, changes.Count);
                    EventPatternAssert.AreEqual(item, "Name", item, "new", changes.Last());

                    var item2 = new Fake { Name = "2" };
                    collection[1] = item2;
                    Assert.AreEqual(2, changes.Count);
                    EventPatternAssert.AreEqual(item2, string.Empty, item2, "2", changes.Last());
                }

                Assert.AreEqual(2, changes.Count);
            }

            [Test]
            public void HandlesMove()
            {
                var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
                var item1 = new Fake { Name = "1" };
                var item2 = new Fake { Name = "2" };
                var collection = new ObservableCollection<Fake> { item1, item2 };
                using (collection.ObserveItemPropertyChanged(x => x.Name, false)
                                 .Subscribe(changes.Add))
                {
                    CollectionAssert.IsEmpty(changes);

                    collection.Move(0, 1);
                    CollectionAssert.IsEmpty(changes);

                    item1.Name = "new 1";
                    EventPatternAssert.AreEqual(item1, "Name", item1, "new 1", changes.Last());

                    item2.Name = "new 2";
                    Assert.AreEqual(2, changes.Count);
                    EventPatternAssert.AreEqual(item2, "Name", item2, "new 2", changes.Last());
                }

                Assert.AreEqual(2, changes.Count);
            }

            [Test]
            public void HandlesAdd()
            {
                var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
                var item1 = new Fake { Name = "1" };
                var item2 = new Fake { Name = "2" };
                var collection = new ObservableCollection<Fake> { item1, item2 };
                using (collection.ObserveItemPropertyChanged(x => x.Name, false)
                                 .Subscribe(changes.Add))
                {
                    CollectionAssert.IsEmpty(changes);
                    var item3 = new Fake { Name = "3" };
                    collection.Add(item3);
                    Assert.AreEqual(1, changes.Count);
                    EventPatternAssert.AreEqual(item3, string.Empty, item3, "3", changes.Last());

                    item3.Name = "new";
                    Assert.AreEqual(2, changes.Count);
                    EventPatternAssert.AreEqual(item3, "Name", item3, "new", changes.Last());
                }
            }

            [Test]
            public void HandlesReplace()
            {
                var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
                var item1 = new Fake { Name = "1" };
                var item2 = new Fake { Name = "2" };
                var collection = new ObservableCollection<Fake> { item1, item2 };
                using (collection.ObserveItemPropertyChanged(x => x.Name, false)
                                 .Subscribe(changes.Add))
                {
                    CollectionAssert.IsEmpty(changes);
                    var item3 = new Fake { Name = "3" };
                    collection[0] = item3;
                    Assert.AreEqual(1, changes.Count);
                    EventPatternAssert.AreEqual(item3, string.Empty, item3, "3", changes.Single());

                    item3.Name = "new";
                    Assert.AreEqual(2, changes.Count);
                    EventPatternAssert.AreEqual(item3, "Name", item3, "new", changes.Last());

                    item1.Name = "new1";
                    Assert.AreEqual(2, changes.Count); // Stopped subscribing
                }

                Assert.AreEqual(2, changes.Count);
            }

            [Test]
            public void HandlesRemove()
            {
                var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
                var item1 = new Fake { Name = "1" };
                var item2 = new Fake { Name = "2" };
                var collection = new ObservableCollection<Fake> { item1, item2 };
                using (collection.ObserveItemPropertyChanged(x => x.Name, false)
                                 .Subscribe(changes.Add))
                {
                    CollectionAssert.IsEmpty(changes);
                    collection.Remove(item2);
                    item2.Name = "new";
                    CollectionAssert.IsEmpty(changes);
                }

                CollectionAssert.IsEmpty(changes);
            }

            [Test]
            public void DisposeStopsSubscribing()
            {
                var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
                var item1 = new Fake { Name = "1" };
                var item2 = new Fake { Name = "2" };
                var collection = new ObservableCollection<Fake> { item1, item2 };
                using (collection.ObserveItemPropertyChanged(x => x.Name, false)
                                 .Subscribe(changes.Add))
                {
                }

                collection.Add(new Fake { Name = "3" });
                CollectionAssert.IsEmpty(changes);
            }

            [Test]
            [SuppressMessage("ReSharper", "HeuristicUnreachableCode")]
            public void MemoryLeakDisposeTest()
            {
#if DEBUG
            Assert.Inconclusive("Debugger keeps things alive for the scope of the method.");
#endif
                var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
                var collection = new ObservableCollection<Fake> { new Fake { Name = "1" }, new Fake { Name = "2" } };

                var collectionRef = new WeakReference(collection);
                var item1Ref = new WeakReference(collection[0]);
                IObservable<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>> observable =
                    collection.ObserveItemPropertyChanged(x => x.Name, false);
                Assert.IsTrue(collectionRef.IsAlive);
                //// http://stackoverflow.com/a/579001/1069200
                using (observable.Subscribe(changes.Add))
                {
                    CollectionAssert.IsEmpty(changes);
                }

                GC.Collect();
                Assert.IsFalse(collectionRef.IsAlive);
                Assert.IsFalse(item1Ref.IsAlive);
            }

            [Test]
            [SuppressMessage("ReSharper", "HeuristicUnreachableCode")]
            public void MemoryLeakNoDisposeTest()
            {
#if DEBUG
            Assert.Inconclusive("Debugger keeps things alive for the scope of the method.");
#endif
                var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
                var collection = new ObservableCollection<Fake> { new Fake { Name = "1" }, new Fake { Name = "2" } };

                var collectionRef = new WeakReference(collection);
                var item1Ref = new WeakReference(collection[0]);
                IObservable<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>> observable =
                    collection.ObserveItemPropertyChanged(x => x.Name, false);
                Assert.IsTrue(collectionRef.IsAlive);
                //// http://stackoverflow.com/a/579001/1069200
#pragma warning disable GU0030 // Use using.
                var subscription = observable.Subscribe(changes.Add);
#pragma warning restore GU0030 // Use using.
                GC.KeepAlive(observable);
                GC.KeepAlive(subscription);
                CollectionAssert.IsEmpty(changes);

                GC.Collect();

                Assert.IsFalse(collectionRef.IsAlive);
                Assert.IsFalse(item1Ref.IsAlive);
            }
        }
    }
}