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
                    EventPatternAssert.AreEqual(item1, item1, item1, Maybe.Some("1"), string.Empty, changes[0]);
                    EventPatternAssert.AreEqual(item2, item2, item2, Maybe.Some("2"), string.Empty, changes[1]);
                }

                Assert.AreEqual(2, changes.Count);
            }

            [Test]
            public void SignalsInitialNested()
            {
                var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
                var item1 = new Fake { Level1 = new Level1 { Name = "1" } };
                var item2 = new Fake { Level1 = new Level1 { Name = "2" } };
                var collection = new ObservableCollection<Fake> { item1, item2 };
                using (collection.ObserveItemPropertyChanged(x => x.Level1.Name, true)
                                 .Subscribe(changes.Add))
                {
                    Assert.AreEqual(2, changes.Count);
                    EventPatternAssert.AreEqual(item1, item1.Level1, item1.Level1, Maybe.Some("1"), string.Empty, changes[0]);
                    EventPatternAssert.AreEqual(item2, item2.Level1, item2.Level1, Maybe.Some("2"), string.Empty, changes[1]);
                }

                Assert.AreEqual(2, changes.Count);
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
                    EventPatternAssert.AreEqual(item1, item1, item1, Maybe.Some(1), string.Empty, changes.Single());
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
                    EventPatternAssert.AreEqual(item1, item1, item1, Maybe.Some("new1"), "Name", changes.Last());

                    item2.Name = "new2";
                    Assert.AreEqual(2, changes.Count);
                    EventPatternAssert.AreEqual(item2, item2, item2, Maybe.Some("new2"), "Name", changes.Last());
                }

                Assert.AreEqual(2, changes.Count);
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
                    EventPatternAssert.AreEqual(item, item, item, Maybe.Some("new1"), "Name", changes.Single());

                    collection.Add(item);
                    Assert.AreEqual(1, changes.Count);

                    item.Name = "new2";
                    Assert.AreEqual(2, changes.Count);
                    EventPatternAssert.AreEqual(item, item, item, Maybe.Some("new2"), "Name", changes.Last());

                    collection.RemoveAt(1);
                    Assert.AreEqual(2, changes.Count);

                    item.Name = "new3";
                    Assert.AreEqual(3, changes.Count);
                    EventPatternAssert.AreEqual(item, item, item, Maybe.Some("new3"), "Name", changes.Last());
                }

                Assert.AreEqual(3, changes.Count);
            }

            [Test]
            public void ReactsWhenPropertyChangesSameInstanceTwiceTwoLevels()
            {
                var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
                var item = new Fake { Level1 = new Level1 { Name = "1" } };
                var collection = new ObservableCollection<Fake> { item };
                using (collection.ObserveItemPropertyChanged(x => x.Level1.Name, false)
                                 .Subscribe(changes.Add))
                {
                    CollectionAssert.IsEmpty(changes);
                    item.Level1.Name = "new1";
                    EventPatternAssert.AreEqual(item, item.Level1, item.Level1, Maybe.Some("new1"), "Name", changes.Single());

                    collection.Add(item);
                    Assert.AreEqual(1, changes.Count);

                    item.Level1.Name = "new2";
                    Assert.AreEqual(2, changes.Count);
                    EventPatternAssert.AreEqual(item, item.Level1, item.Level1, Maybe.Some("new2"), "Name", changes.Last());

                    collection.RemoveAt(1);
                    Assert.AreEqual(2, changes.Count);

                    item.Level1.Name = "new3";
                    Assert.AreEqual(3, changes.Count);
                    EventPatternAssert.AreEqual(item, item.Level1, item.Level1, Maybe.Some("new3"), "Name", changes.Last());
                }

                Assert.AreEqual(3, changes.Count);
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
                        EventPatternAssert.AreEqual(item1, item1, item1, Maybe.Some("new1"), "Name", changes1.Single());
                        EventPatternAssert.AreEqual(item1, item1, item1, Maybe.Some("new1"), "Name", changes2.Single());

                        item2.Name = "new2";
                        Assert.AreEqual(2, changes1.Count);
                        Assert.AreEqual(2, changes2.Count);
                        EventPatternAssert.AreEqual(item2, item2, item2, Maybe.Some("new2"), "Name", changes1.Last());
                        EventPatternAssert.AreEqual(item2, item2, item2, Maybe.Some("new2"), "Name", changes2.Last());
                    }
                }

                Assert.AreEqual(2, changes1.Count);
                Assert.AreEqual(2, changes2.Count);
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
                        EventPatternAssert.AreEqual(item1, item1, item1, Maybe.Some("new1"), "Name", changes.Single());

                        item2.Name = "new2";
                        Assert.AreEqual(2, changes.Count);
                        EventPatternAssert.AreEqual(item2, item2, item2, Maybe.Some("new2"), "Name", changes.Last());
                    }
                }

                Assert.AreEqual(2, changes.Count);
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
                    EventPatternAssert.AreEqual(item1, item1.Next, item1.Next, Maybe.Some("new1"), "Name", changes.Single());

                    item2.Next = new Level { Name = "new2" };
                    Assert.AreEqual(2, changes.Count);
                    EventPatternAssert.AreEqual(item2, item2, item2.Next, Maybe.Some("new2"), "Next", changes.Last());
                }

                Assert.AreEqual(2, changes.Count);
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
                    EventPatternAssert.AreEqual(item, item, item, Maybe.Some("new"), "Name", changes.Single());
                }

                Assert.AreEqual(1, changes.Count);
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

                    item.Name = "new1";
                    EventPatternAssert.AreEqual(item, item, item, Maybe.Some("new1"), "Name", changes.Single());

                    var item2 = new Fake { Name = "2" };
                    collection[1] = item2;
                    Assert.AreEqual(2, changes.Count);
                    EventPatternAssert.AreEqual(item2, item2, item2, Maybe.Some("2"), string.Empty, changes.Last());
                }

                Assert.AreEqual(2, changes.Count);
            }

            [Test]
            public void HandlesMoveSimple()
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
                    EventPatternAssert.AreEqual(item1, item1, item1, Maybe.Some("new 1"), "Name", changes.Single());

                    item2.Name = "new 2";
                    Assert.AreEqual(2, changes.Count);
                    EventPatternAssert.AreEqual(item2, item2, item2, Maybe.Some("new 2"), "Name", changes.Last());
                }

                Assert.AreEqual(2, changes.Count);
            }

            [Test]
            public void HandlesMoveNested()
            {
                var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
                var item1 = new Fake { Level1 = new Level1 { Name = "1" } };
                var item2 = new Fake { Level1 = new Level1 { Name = "2" } };
                var collection = new ObservableCollection<Fake> { item1, item2 };
                using (collection.ObserveItemPropertyChanged(x => x.Level1.Name, false)
                                 .Subscribe(changes.Add))
                {
                    CollectionAssert.IsEmpty(changes);

                    collection.Move(0, 1);
                    CollectionAssert.IsEmpty(changes);

                    item1.Level1.Name = "new 1";
                    EventPatternAssert.AreEqual(item1, item1.Level1, item1.Level1, Maybe.Some("new 1"), "Name", changes.Single());

                    item2.Level1.Name = "new 2";
                    Assert.AreEqual(2, changes.Count);
                    EventPatternAssert.AreEqual(item2, item2.Level1, item2.Level1, Maybe.Some("new 2"), "Name", changes.Last());
                }

                Assert.AreEqual(2, changes.Count);
            }

            [Test]
            public void HandlesAddSimple()
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
                    EventPatternAssert.AreEqual(item3, item3, item3, Maybe.Some("3"), string.Empty, changes.Single());

                    item3.Name = "new";
                    Assert.AreEqual(2, changes.Count);
                    EventPatternAssert.AreEqual(item3, item3, item3, Maybe.Some("new"), "Name", changes.Last());
                }
            }

            [Test]
            public void HandlesAddNested()
            {
                var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
                var item1 = new Fake { Level1 = new Level1 { Name = "1" } };
                var item2 = new Fake { Level1 = new Level1 { Name = "2" } };
                var collection = new ObservableCollection<Fake> { item1, item2 };
                using (collection.ObserveItemPropertyChanged(x => x.Level1.Name, false)
                                 .Subscribe(changes.Add))
                {
                    CollectionAssert.IsEmpty(changes);

                    var item3 = new Fake { Level1 = new Level1 { Name = "3" } };
                    collection.Add(item3);
                    EventPatternAssert.AreEqual(item3, item3.Level1, item3.Level1, Maybe.Some("3"), string.Empty, changes.Single());

                    item3.Level1.Name = "new";
                    Assert.AreEqual(2, changes.Count);
                    EventPatternAssert.AreEqual(item3, item3.Level1, item3.Level1, Maybe.Some("new"), "Name", changes.Last());
                }
            }

            [Test]
            public void HandlesReplaceSimple()
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
                    EventPatternAssert.AreEqual(item3, item3, item3, Maybe.Some("3"), string.Empty, changes.Single());

                    item3.Name = "new";
                    Assert.AreEqual(2, changes.Count);
                    EventPatternAssert.AreEqual(item3, item3, item3, Maybe.Some("new"), "Name", changes.Last());

                    item1.Name = "new1";
                    Assert.AreEqual(2, changes.Count); // Stopped subscribing
                }

                Assert.AreEqual(2, changes.Count);
            }

            [Test]
            public void HandlesReplaceNested()
            {
                var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
                var item1 = new Fake { Level1 = new Level1 { Name = "1" } };
                var item2 = new Fake { Level1 = new Level1 { Name = "2" } };
                var collection = new ObservableCollection<Fake> { item1, item2 };
                using (collection.ObserveItemPropertyChanged(x => x.Level1.Name, false)
                                 .Subscribe(changes.Add))
                {
                    CollectionAssert.IsEmpty(changes);

                    var item3 = new Fake { Level1 = new Level1 { Name = "3" } };
                    collection[0] = item3;
                    EventPatternAssert.AreEqual(item3, item3.Level1, item3.Level1, Maybe.Some("3"), string.Empty, changes.Single());

                    item3.Level1.Name = "new";
                    Assert.AreEqual(2, changes.Count);
                    EventPatternAssert.AreEqual(item3, item3.Level1, item3.Level1, Maybe.Some("new"), "Name", changes.Last());

                    item1.Level1.Name = "new1";
                    Assert.AreEqual(2, changes.Count); // Stopped subscribing
                }

                Assert.AreEqual(2, changes.Count);
            }

            [Test]
            public void HandlesReplaceWithSameSimple()
            {
                var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, int>>>();
                var item1 = new Fake { Name = "1" };
                var item2 = new Fake { Name = "2" };
                var collection = new ObservableCollection<Fake> { item1, item2 };
                using (collection.ObserveItemPropertyChanged(x => x.Value, false)
                                 .Subscribe(changes.Add))
                {
                    CollectionAssert.IsEmpty(changes);

                    collection[0] = item1;
                    CollectionAssert.IsEmpty(changes);

                    item1.Value++;
                    EventPatternAssert.AreEqual(item1, item1, item1, Maybe.Some(item1.Value), "Value", changes.Single());
                }

                Assert.AreEqual(1, changes.Count);
            }

            [Test]
            public void HandlesReplaceWithSameNested()
            {
                var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, int>>>();
                var item1 = new Fake { Level1 = new Level1 { Name = "1" } };
                var item2 = new Fake { Level1 = new Level1 { Name = "2" } };
                var collection = new ObservableCollection<Fake> { item1, item2 };
                using (collection.ObserveItemPropertyChanged(x => x.Level1.Value, false)
                                 .Subscribe(changes.Add))
                {
                    CollectionAssert.IsEmpty(changes);

                    collection[0] = item1;
                    CollectionAssert.IsEmpty(changes);

                    item1.Level1.Value++;
                    EventPatternAssert.AreEqual(item1, item1.Level1, item1.Level1, Maybe.Some(item1.Level1.Value), "Value", changes.Single());
                }

                Assert.AreEqual(1, changes.Count);
            }

            [Test]
            public void HandlesRemoveSimple()
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
                    CollectionAssert.IsEmpty(changes);

                    item2.Name = "new";
                    CollectionAssert.IsEmpty(changes);
                }

                CollectionAssert.IsEmpty(changes);
            }

            [Test]
            public void HandlesRemoveNested()
            {
                var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
                var item1 = new Fake { Level1 = new Level1 { Name = "1" } };
                var item2 = new Fake { Level1 = new Level1 { Name = "2" } };
                var collection = new ObservableCollection<Fake> { item1, item2 };
                using (collection.ObserveItemPropertyChanged(x => x.Level1.Name, false)
                                 .Subscribe(changes.Add))
                {
                    CollectionAssert.IsEmpty(changes);

                    collection.Remove(item2);
                    CollectionAssert.IsEmpty(changes);

                    item2.Level1.Name = "new";
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
                IObservable<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>> observable = collection.ObserveItemPropertyChanged(x => x.Name, false);
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
                IObservable<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>> observable = collection.ObserveItemPropertyChanged(x => x.Name, false);
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