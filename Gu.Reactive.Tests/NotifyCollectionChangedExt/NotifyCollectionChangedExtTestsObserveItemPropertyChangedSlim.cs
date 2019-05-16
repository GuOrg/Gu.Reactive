namespace Gu.Reactive.Tests.NotifyCollectionChangedExt
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Gu.Reactive.Tests.Helpers;
    using NUnit.Framework;

    public partial class NotifyCollectionChangedExtTests
    {
        public class ObserveItemPropertyChangedSlim
        {
            [Test]
            public void SignalsInitial()
            {
                var changes = new List<PropertyChangedEventArgs>();
                var item1 = new Fake { Name = "1" };
                var item2 = new Fake { Name = "2" };
                var collection = new ObservableCollection<Fake> { item1, item2 };
                using (collection.ObserveItemPropertyChangedSlim(x => x.Name, signalInitial: true)
                                 .Subscribe(changes.Add))
                {
                    Assert.AreEqual(2, changes.Count);
                    Assert.AreEqual(string.Empty, changes[0].PropertyName);
                    Assert.AreEqual(string.Empty, changes[1].PropertyName);
                }

                Assert.AreEqual(2, changes.Count);
            }

            [Test]
            public void SignalsInitialNested()
            {
                var changes = new List<PropertyChangedEventArgs>();
                var item1 = new Fake { Level1 = new Level1 { Name = "1" } };
                var item2 = new Fake { Level1 = new Level1 { Name = "2" } };
                var collection = new ObservableCollection<Fake> { item1, item2 };
                using (collection.ObserveItemPropertyChangedSlim(x => x.Level1.Name, signalInitial: true)
                                 .Subscribe(changes.Add))
                {
                    Assert.AreEqual(2, changes.Count);
                    Assert.AreEqual(string.Empty, changes[0].PropertyName);
                    Assert.AreEqual(string.Empty, changes[1].PropertyName);
                }

                Assert.AreEqual(2, changes.Count);
            }

            [Test]
            public void DoesNotSignalInitial()
            {
                var changes = new List<PropertyChangedEventArgs>();
                var item1 = new Fake { Name = "1" };
                var item2 = new Fake { Name = "2" };
                var collection = new ObservableCollection<Fake> { item1, item2 };
                using (collection.ObserveItemPropertyChangedSlim(x => x.Name, signalInitial: false)
                                 .Subscribe(changes.Add))
                {
                    CollectionAssert.IsEmpty(changes);
                }

                CollectionAssert.IsEmpty(changes);
            }

            [Test]
            public void AddSimple()
            {
                var source = new ObservableCollection<Fake>();
                var changes = new List<PropertyChangedEventArgs>();
                using (source.ObserveItemPropertyChangedSlim(x => x.Name, signalInitial: false)
                             .Subscribe(x => changes.Add(x)))
                {
                    CollectionAssert.IsEmpty(changes);
                    source.Add(new Fake());
                    Assert.AreEqual(string.Empty, changes.Single().PropertyName);
                }
            }

            [Test]
            public void AddNullSimple()
            {
                var source = new ObservableCollection<Fake>();
                var changes = new List<PropertyChangedEventArgs>();
                using (source.ObserveItemPropertyChangedSlim(x => x.Name, signalInitial: false)
                             .Subscribe(x => changes.Add(x)))
                {
                    CollectionAssert.IsEmpty(changes);
                    source.Add(null);
                    CollectionAssert.IsEmpty(changes);
                }
            }

            [Test]
            public void AddThenUpdateSimple()
            {
                var changes = new List<PropertyChangedEventArgs>();
                var item1 = new Fake { Name = "1" };
                var item2 = new Fake { Name = "2" };
                var collection = new ObservableCollection<Fake> { item1, item2 };
                using (collection.ObserveItemPropertyChangedSlim(x => x.Name, signalInitial: false)
                                 .Subscribe(changes.Add))
                {
                    CollectionAssert.IsEmpty(changes);

                    var item3 = new Fake { Name = "3" };
                    collection.Add(item3);
                    Assert.AreEqual(string.Empty, changes.Single().PropertyName);

                    item3.Name = "new";
                    Assert.AreEqual(2, changes.Count);
                    Assert.AreEqual("Name", changes.Last().PropertyName);
                }
            }

            [Test]
            public void AddNested()
            {
                var changes = new List<PropertyChangedEventArgs>();
                var item1 = new Fake { Level1 = new Level1 { Name = "1" } };
                var item2 = new Fake { Level1 = new Level1 { Name = "2" } };
                var collection = new ObservableCollection<Fake> { item1, item2 };
                using (collection.ObserveItemPropertyChangedSlim(x => x.Level1.Name, signalInitial: false)
                                 .Subscribe(changes.Add))
                {
                    CollectionAssert.IsEmpty(changes);

                    var item3 = new Fake { Level1 = new Level1 { Name = "3" } };
                    collection.Add(item3);
                    Assert.AreEqual(string.Empty, changes.Single().PropertyName);

                    item3.Level1.Name = "new";
                    Assert.AreEqual(2, changes.Count);
                    Assert.AreEqual("Name", changes.Last().PropertyName);
                }
            }

            [Test]
            public void ReplaceSimple()
            {
                var changes = new List<PropertyChangedEventArgs>();
                var item1 = new Fake { Name = "1" };
                var item2 = new Fake { Name = "2" };
                var collection = new ObservableCollection<Fake> { item1, item2 };
                using (collection.ObserveItemPropertyChangedSlim(x => x.Name, signalInitial: false)
                                 .Subscribe(changes.Add))
                {
                    CollectionAssert.IsEmpty(changes);

                    var item3 = new Fake { Name = "3" };
                    collection[0] = item3;
                    Assert.AreEqual(string.Empty, changes.Single().PropertyName);

                    item3.Name = "new";
                    Assert.AreEqual(2, changes.Count);
                    Assert.AreEqual("Name", changes.Last().PropertyName);

                    item1.Name = "new1";
                    Assert.AreEqual(2, changes.Count);
                }

                Assert.AreEqual(2, changes.Count);
            }

            [Test]
            public void ReplaceNested()
            {
                var changes = new List<PropertyChangedEventArgs>();
                var item1 = new Fake { Level1 = new Level1 { Name = "1" } };
                var item2 = new Fake { Level1 = new Level1 { Name = "2" } };
                var collection = new ObservableCollection<Fake> { item1, item2 };
                using (collection.ObserveItemPropertyChangedSlim(x => x.Level1.Name, signalInitial: false)
                                 .Subscribe(changes.Add))
                {
                    CollectionAssert.IsEmpty(changes);

                    var item3 = new Fake { Level1 = new Level1 { Name = "3" } };
                    collection[0] = item3;
                    Assert.AreEqual(string.Empty, changes.Single().PropertyName);

                    item3.Level1.Name = "new";
                    Assert.AreEqual(2, changes.Count);
                    Assert.AreEqual("Name", changes.Last().PropertyName);

                    item1.Level1.Name = "new1";
                    Assert.AreEqual(2, changes.Count); // Stopped subscribing
                }

                Assert.AreEqual(2, changes.Count);
            }

            [Test]
            public void ReplaceWithSameSimple()
            {
                var changes = new List<PropertyChangedEventArgs>();
                var item1 = new Fake { Name = "1" };
                var item2 = new Fake { Name = "2" };
                var collection = new ObservableCollection<Fake> { item1, item2 };
                using (collection.ObserveItemPropertyChangedSlim(x => x.Value, signalInitial: false)
                                 .Subscribe(changes.Add))
                {
                    CollectionAssert.IsEmpty(changes);

                    collection[0] = item1;
                    CollectionAssert.IsEmpty(changes);

                    item1.Value++;
                    Assert.AreEqual("Value", changes.Single().PropertyName);
                }

                Assert.AreEqual(1, changes.Count);
            }

            [Test]
            public void ReplaceWithSameNested()
            {
                var changes = new List<PropertyChangedEventArgs>();
                var item1 = new Fake { Level1 = new Level1 { Name = "1" } };
                var item2 = new Fake { Level1 = new Level1 { Name = "2" } };
                var collection = new ObservableCollection<Fake> { item1, item2 };
                using (collection.ObserveItemPropertyChangedSlim(x => x.Level1.Value, signalInitial: false)
                                 .Subscribe(changes.Add))
                {
                    CollectionAssert.IsEmpty(changes);

                    collection[0] = item1;
                    CollectionAssert.IsEmpty(changes);

                    item1.Level1.Value++;
                    Assert.AreEqual("Value", changes.Single().PropertyName);
                }

                Assert.AreEqual(1, changes.Count);
            }

            [Test]
            public void RemoveSimple()
            {
                var changes = new List<PropertyChangedEventArgs>();
                var item1 = new Fake { Name = "1" };
                var item2 = new Fake { Name = "2" };
                var collection = new ObservableCollection<Fake> { item1, item2 };
                using (collection.ObserveItemPropertyChangedSlim(x => x.Name, signalInitial: false)
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
            public void RemoveNested()
            {
                var changes = new List<PropertyChangedEventArgs>();
                var item1 = new Fake { Level1 = new Level1 { Name = "1" } };
                var item2 = new Fake { Level1 = new Level1 { Name = "2" } };
                var collection = new ObservableCollection<Fake> { item1, item2 };
                using (collection.ObserveItemPropertyChangedSlim(x => x.Level1.Name, signalInitial: false)
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
            public void MoveSimple()
            {
                var changes = new List<PropertyChangedEventArgs>();
                var item1 = new Fake { Name = "1" };
                var item2 = new Fake { Name = "2" };
                var collection = new ObservableCollection<Fake> { item1, item2 };
                using (collection.ObserveItemPropertyChangedSlim(x => x.Name, signalInitial: false)
                                 .Subscribe(changes.Add))
                {
                    CollectionAssert.IsEmpty(changes);

                    collection.Move(0, 1);
                    CollectionAssert.IsEmpty(changes);

                    item1.Name = "new 1";
                    Assert.AreEqual("Name", changes.Single().PropertyName);

                    item2.Name = "new 2";
                    Assert.AreEqual(2, changes.Count);
                    Assert.AreEqual("Name", changes.Last().PropertyName);
                }

                Assert.AreEqual(2, changes.Count);
            }

            [Test]
            public void MoveNested()
            {
                var changes = new List<PropertyChangedEventArgs>();
                var item1 = new Fake { Level1 = new Level1 { Name = "1" } };
                var item2 = new Fake { Level1 = new Level1 { Name = "2" } };
                var collection = new ObservableCollection<Fake> { item1, item2 };
                using (collection.ObserveItemPropertyChangedSlim(x => x.Level1.Name, signalInitial: false)
                                 .Subscribe(changes.Add))
                {
                    CollectionAssert.IsEmpty(changes);

                    collection.Move(0, 1);
                    CollectionAssert.IsEmpty(changes);

                    item1.Level1.Name = "new 1";
                    Assert.AreEqual("Name", changes.Single().PropertyName);

                    item2.Level1.Name = "new 2";
                    Assert.AreEqual(2, changes.Count);
                    Assert.AreEqual("Name", changes.Last().PropertyName);
                }

                Assert.AreEqual(2, changes.Count);
            }

            [Test]
            public void ReadOnlyObservableCollectionCount()
            {
                var ints = new ObservableCollection<int> { 1 };
                var item1 = new ReadOnlyObservableCollection<int>(ints);

                var source = new ObservableCollection<ReadOnlyObservableCollection<int>>();
                var changes = new List<PropertyChangedEventArgs>();
                using (source.ObserveItemPropertyChangedSlim(x => x.Count, signalInitial: false)
                             .Subscribe(x => changes.Add(x)))
                {
                    CollectionAssert.IsEmpty(changes);

                    source.Add(item1);
                    Assert.AreEqual(string.Empty, changes[0].PropertyName);
                }
            }

            [Test]
            public void ReactsWhenPropertyChangesSimple()
            {
                var changes = new List<PropertyChangedEventArgs>();
                var item1 = new Fake { Name = "1" };
                var item2 = new Fake { Name = "2" };
                var collection = new ObservableCollection<Fake> { item1, item2 };
                using (collection.ObserveItemPropertyChangedSlim(x => x.Name, signalInitial: false)
                                 .Subscribe(changes.Add))
                {
                    CollectionAssert.IsEmpty(changes);

                    item1.Name = "new1";
                    Assert.AreEqual(1, changes.Count);
                    Assert.AreEqual("Name", changes.Single().PropertyName);

                    item2.Name = "new2";
                    Assert.AreEqual(2, changes.Count);
                    Assert.AreEqual("Name", changes.Last().PropertyName);
                }

                Assert.AreEqual(2, changes.Count);
            }

            [Test]
            public void DoesNotReactWhenOtherPropertyChangesSimple()
            {
                var changes = new List<PropertyChangedEventArgs>();
                var item1 = new Fake { Name = "1" };
                var item2 = new Fake { Name = "2" };
                var collection = new ObservableCollection<Fake> { item1, item2 };
                using (collection.ObserveItemPropertyChangedSlim(x => x.Name, signalInitial: false)
                                 .Subscribe(changes.Add))
                {
                    CollectionAssert.IsEmpty(changes);

                    item1.Name = "new1";
                    Assert.AreEqual(1, changes.Count);
                    Assert.AreEqual("Name", changes.Single().PropertyName);

                    item1.Value++;
                    Assert.AreEqual(1, changes.Count);

                    item2.Name = "new2";
                    Assert.AreEqual(2, changes.Count);
                    Assert.AreEqual("Name", changes.Last().PropertyName);

                    item2.Value++;
                    Assert.AreEqual(2, changes.Count);
                }

                Assert.AreEqual(2, changes.Count);
            }

            [Test]
            public void ReactsWhenPropertyChangesSameInstanceTwice()
            {
                var changes = new List<PropertyChangedEventArgs>();
                var item = new Fake { Name = "1" };
                var collection = new ObservableCollection<Fake> { item };
                using (collection.ObserveItemPropertyChangedSlim(x => x.Name, signalInitial: false)
                                 .Subscribe(changes.Add))
                {
                    CollectionAssert.IsEmpty(changes);

                    item.Name = "new1";
                    Assert.AreEqual("Name", changes.Single().PropertyName);

                    collection.Add(item);
                    Assert.AreEqual(1, changes.Count);

                    item.Name = "new2";
                    Assert.AreEqual(2, changes.Count);
                    Assert.AreEqual("Name", changes.Last().PropertyName);

                    collection.RemoveAt(1);
                    Assert.AreEqual(2, changes.Count);

                    item.Name = "new3";
                    Assert.AreEqual(3, changes.Count);
                    Assert.AreEqual("Name", changes.Last().PropertyName);
                }

                Assert.AreEqual(3, changes.Count);
            }

            [Test]
            public void ReactsWhenPropertyChangesSameInstanceTwiceTwoLevels()
            {
                var changes = new List<PropertyChangedEventArgs>();
                var item = new Fake { Level1 = new Level1 { Name = "1" } };
                var collection = new ObservableCollection<Fake> { item };
                using (collection.ObserveItemPropertyChangedSlim(x => x.Level1.Name, signalInitial: false)
                                 .Subscribe(changes.Add))
                {
                    CollectionAssert.IsEmpty(changes);
                    item.Level1.Name = "new1";
                    Assert.AreEqual("Name", changes.Single().PropertyName);

                    collection.Add(item);
                    Assert.AreEqual(1, changes.Count);

                    item.Level1.Name = "new2";
                    Assert.AreEqual(2, changes.Count);
                    Assert.AreEqual("Name", changes.Last().PropertyName);

                    collection.RemoveAt(1);
                    Assert.AreEqual(2, changes.Count);

                    item.Level1.Name = "new3";
                    Assert.AreEqual(3, changes.Count);
                    Assert.AreEqual("Name", changes.Last().PropertyName);
                }

                Assert.AreEqual(3, changes.Count);
            }

            [Test]
            public void OneObservableTwoSubscriptions()
            {
                var changes1 = new List<PropertyChangedEventArgs>();
                var changes2 = new List<PropertyChangedEventArgs>();
                var item1 = new Fake { Name = "1" };
                var item2 = new Fake { Name = "2" };
                var collection = new ObservableCollection<Fake> { item1, item2 };
                var observable = collection.ObserveItemPropertyChangedSlim(x => x.Name, signalInitial: false);
                using (observable.Subscribe(changes1.Add))
                {
                    using (observable.Subscribe(changes2.Add))
                    {
                        CollectionAssert.IsEmpty(changes1);
                        CollectionAssert.IsEmpty(changes2);

                        item1.Name = "new1";
                        Assert.AreEqual("Name", changes1.Single().PropertyName);
                        Assert.AreEqual("Name", changes2.Single().PropertyName);

                        item2.Name = "new2";
                        Assert.AreEqual(2, changes1.Count);
                        Assert.AreEqual(2, changes2.Count);
                        Assert.AreEqual("Name", changes1.Last().PropertyName);
                        Assert.AreEqual("Name", changes2.Last().PropertyName);
                    }
                }

                Assert.AreEqual(2, changes1.Count);
                Assert.AreEqual(2, changes2.Count);
            }

            [Test]
            public void ReactsWhenPropertyChangesView()
            {
                var changes = new List<PropertyChangedEventArgs>();
                var item1 = new Fake { Name = "1" };
                var item2 = new Fake { Name = "2" };
                var collection = new ObservableCollection<Fake> { item1, item2 };
                using (var view = collection.AsReadOnlyFilteredView(x => true))
                {
                    using (view.ObserveItemPropertyChangedSlim(x => x.Name, signalInitial: false)
                               .Subscribe(changes.Add))
                    {
                        CollectionAssert.IsEmpty(changes);

                        item1.Name = "new1";
                        Assert.AreEqual("Name", changes.Single().PropertyName);

                        item2.Name = "new2";
                        Assert.AreEqual(2, changes.Count);
                        Assert.AreEqual("Name", changes.Last().PropertyName);
                    }
                }

                Assert.AreEqual(2, changes.Count);
            }

            [Test]
            public void ReactsNested()
            {
                var changes = new List<PropertyChangedEventArgs>();
                var item1 = new Fake { Next = new Level { Name = "1" } };
                var item2 = new Fake();
                var collection = new ObservableCollection<Fake> { item1, item2 };
                using (collection.ObserveItemPropertyChangedSlim(x => x.Next.Name, signalInitial: false)
                                 .Subscribe(changes.Add))
                {
                    CollectionAssert.IsEmpty(changes);
                    item1.Next.Name = "new1";
                    Assert.AreEqual("Name", changes.Single().PropertyName);

                    item2.Next = new Level { Name = "new2" };
                    Assert.AreEqual(2, changes.Count);
                    Assert.AreEqual("Next", changes.Last().PropertyName);
                }

                Assert.AreEqual(2, changes.Count);
            }

            [Test]
            public void DoesNotReactToOtherPropertyNested()
            {
                var changes = new List<PropertyChangedEventArgs>();
                var item1 = new Fake { Next = new Level { Name = "1" } };
                var item2 = new Fake();
                var collection = new ObservableCollection<Fake> { item1, item2 };
                using (collection.ObserveItemPropertyChangedSlim(x => x.Next.Name, signalInitial: false)
                                 .Subscribe(changes.Add))
                {
                    CollectionAssert.IsEmpty(changes);
                    item1.Next.Name = "new1";
                    Assert.AreEqual("Name", changes.Single().PropertyName);

                    item1.Next.Value++;
                    Assert.AreEqual("Name", changes.Single().PropertyName);

                    item2.Next = new Level { Name = "new2" };
                    Assert.AreEqual(2, changes.Count);
                    Assert.AreEqual("Next", changes.Last().PropertyName);
                }

                Assert.AreEqual(2, changes.Count);
            }

            [Test]
            public void ReactsOnceWhenSameItemIsTwoElementsInCollection()
            {
                var changes = new List<PropertyChangedEventArgs>();
                var item = new Fake { Name = "1" };
                var collection = new ObservableCollection<Fake> { item, item };
                using (collection.ObserveItemPropertyChangedSlim(x => x.Name, signalInitial: false)
                                 .Subscribe(changes.Add))
                {
                    CollectionAssert.IsEmpty(changes);

                    item.Name = "new";
                    Assert.AreEqual("Name", changes.Single().PropertyName);
                }

                Assert.AreEqual(1, changes.Count);
            }

            [Test]
            public void NullItem()
            {
                var changes = new List<PropertyChangedEventArgs>();
                var item = new Fake { Name = "1" };
                var collection = new ObservableCollection<Fake> { item, null };
                using (collection.ObserveItemPropertyChangedSlim(x => x.Name, signalInitial: false)
                                 .Subscribe(changes.Add))
                {
                    CollectionAssert.IsEmpty(changes);

                    collection.Add(null);
                    Assert.AreEqual(3, collection.Count);
                    Assert.AreEqual(0, changes.Count);

                    item.Name = "new1";
                    Assert.AreEqual("Name", changes.Single().PropertyName);

                    collection[1] = new Fake { Name = "2" };
                    Assert.AreEqual(2, changes.Count);
                    Assert.AreEqual(string.Empty, changes.Last().PropertyName);
                }

                Assert.AreEqual(2, changes.Count);
            }

            [Test]
            public void DisposeStopsSubscribing()
            {
                var changes = new List<PropertyChangedEventArgs>();
                var item1 = new Fake { Name = "1" };
                var item2 = new Fake { Name = "2" };
                var collection = new ObservableCollection<Fake> { item1, item2 };
                using (collection.ObserveItemPropertyChangedSlim(x => x.Name, signalInitial: false)
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
                var changes = new List<PropertyChangedEventArgs>();
                var collection = new ObservableCollection<Fake> { new Fake { Name = "1" }, new Fake { Name = "2" } };

                var collectionRef = new WeakReference(collection);
                var item1Ref = new WeakReference(collection[0]);
                var observable = collection.ObserveItemPropertyChangedSlim(x => x.Name, signalInitial: false);
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
                var changes = new List<PropertyChangedEventArgs>();
                var collection = new ObservableCollection<Fake> { new Fake { Name = "1" }, new Fake { Name = "2" } };

                var collectionRef = new WeakReference(collection);
                var item1Ref = new WeakReference(collection[0]);
                var observable = collection.ObserveItemPropertyChangedSlim(x => x.Name, signalInitial: false);
                Assert.IsTrue(collectionRef.IsAlive);
                //// http://stackoverflow.com/a/579001/1069200
#pragma warning disable IDISP001  // Dispose created.
                var subscription = observable.Subscribe(changes.Add);
#pragma warning restore IDISP001  // Dispose created.
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
