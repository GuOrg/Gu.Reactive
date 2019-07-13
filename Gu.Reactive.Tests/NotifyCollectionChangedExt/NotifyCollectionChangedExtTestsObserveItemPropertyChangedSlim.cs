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

    public static partial class NotifyCollectionChangedExtTests
    {
        public static class ObserveItemPropertyChangedSlim
        {
            [Test]
            public static void SignalsInitial()
            {
                var changes = new List<PropertyChangedEventArgs>();
                var item1 = new Fake { Name = "1" };
                var item2 = new Fake { Name = "2" };
                var source = new ObservableCollection<Fake> { item1, item2 };
                using (source.ObserveItemPropertyChangedSlim(x => x.Name, signalInitial: true)
                             .Subscribe(changes.Add))
                {
                    Assert.AreEqual(string.Empty, changes.Single().PropertyName);
                }

                Assert.AreEqual(1, changes.Count);
            }

            [Test]
            public static void InitialWithNullSimple()
            {
                var changes = new List<PropertyChangedEventArgs>();
                var item = new Fake { Name = "1" };
                var source = new ObservableCollection<Fake> { item, null };
                using (source.ObserveItemPropertyChangedSlim(x => x.Name, signalInitial: false)
                             .Subscribe(changes.Add))
                {
                    CollectionAssert.IsEmpty(changes);

                    source.Add(null);
                    Assert.AreEqual(string.Empty, changes.Single().PropertyName);

                    item.Name = "new1";
                    Assert.AreEqual(2, changes.Count);
                    Assert.AreEqual("Name", changes.Last().PropertyName);

                    source[1] = new Fake { Name = "2" };
                    Assert.AreEqual(3, changes.Count);
                    Assert.AreEqual(string.Empty, changes.Last().PropertyName);
                }

                Assert.AreEqual(3, changes.Count);
            }

            [Test]
            public static void InitialWithNullNested()
            {
                var changes = new List<PropertyChangedEventArgs>();
                var item = new Fake { Next = new Level { Name = "1" } };
                var source = new ObservableCollection<Fake> { item, null };
                using (source.ObserveItemPropertyChangedSlim(x => x.Next.Name, signalInitial: false)
                             .Subscribe(changes.Add))
                {
                    CollectionAssert.IsEmpty(changes);

                    source.Add(null);
                    Assert.AreEqual(string.Empty, changes.Single().PropertyName);

                    item.Next.Name = "new1";
                    Assert.AreEqual(2, changes.Count);
                    Assert.AreEqual("Name", changes.Last().PropertyName);

                    source[1] = new Fake { Next = new Level { Name = "2" } };
                    Assert.AreEqual(3, changes.Count);
                    Assert.AreEqual(string.Empty, changes.Last().PropertyName);
                }

                Assert.AreEqual(3, changes.Count);
            }

            [Test]
            public static void SignalsInitialNested()
            {
                var changes = new List<PropertyChangedEventArgs>();
                var item1 = new Fake { Level1 = new Level1 { Name = "1" } };
                var item2 = new Fake { Level1 = new Level1 { Name = "2" } };
                var source = new ObservableCollection<Fake> { item1, item2 };
                using (source.ObserveItemPropertyChangedSlim(x => x.Level1.Name, signalInitial: true)
                             .Subscribe(changes.Add))
                {
                    Assert.AreEqual(string.Empty, changes.Last().PropertyName);
                }

                Assert.AreEqual(1, changes.Count);
            }

            [Test]
            public static void DoesNotSignalInitialSimple()
            {
                var changes = new List<PropertyChangedEventArgs>();
                var item1 = new Fake { Name = "1" };
                var item2 = new Fake { Name = "2" };
                var source = new ObservableCollection<Fake> { item1, item2 };
                using (source.ObserveItemPropertyChangedSlim(x => x.Name, signalInitial: false)
                             .Subscribe(changes.Add))
                {
                    CollectionAssert.IsEmpty(changes);
                }

                CollectionAssert.IsEmpty(changes);
            }

            [Test]
            public static void DoesNotSignalInitialNested()
            {
                var changes = new List<PropertyChangedEventArgs>();
                var item1 = new Fake { Level1 = new Level1 { Name = "1" } };
                var item2 = new Fake { Level1 = new Level1 { Name = "2" } };
                var source = new ObservableCollection<Fake> { item1, item2 };
                using (source.ObserveItemPropertyChangedSlim(x => x.Name, signalInitial: false)
                             .Subscribe(changes.Add))
                {
                    CollectionAssert.IsEmpty(changes);
                }

                CollectionAssert.IsEmpty(changes);
            }

            [Test]
            public static void AddSimple()
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
            public static void AddNullSimple()
            {
                var source = new ObservableCollection<Fake>();
                var changes = new List<PropertyChangedEventArgs>();
                using (source.ObserveItemPropertyChangedSlim(x => x.Name, signalInitial: false)
                             .Subscribe(x => changes.Add(x)))
                {
                    CollectionAssert.IsEmpty(changes);
                    source.Add(null);
                    Assert.AreEqual(string.Empty, changes.Single().PropertyName);
                }
            }

            [Test]
            public static void AddThenUpdateSimple()
            {
                var changes = new List<PropertyChangedEventArgs>();
                var item1 = new Fake { Name = "1" };
                var item2 = new Fake { Name = "2" };
                var source = new ObservableCollection<Fake> { item1, item2 };
                using (source.ObserveItemPropertyChangedSlim(x => x.Name, signalInitial: false)
                             .Subscribe(changes.Add))
                {
                    CollectionAssert.IsEmpty(changes);

                    var item3 = new Fake { Name = "3" };
                    source.Add(item3);
                    Assert.AreEqual(string.Empty, changes.Single().PropertyName);

                    item3.Name = "new";
                    Assert.AreEqual(2, changes.Count);
                    Assert.AreEqual("Name", changes.Last().PropertyName);
                }
            }

            [Test]
            public static void AddNested()
            {
                var changes = new List<PropertyChangedEventArgs>();
                var item1 = new Fake { Level1 = new Level1 { Name = "1" } };
                var item2 = new Fake { Level1 = new Level1 { Name = "2" } };
                var source = new ObservableCollection<Fake> { item1, item2 };
                using (source.ObserveItemPropertyChangedSlim(x => x.Level1.Name, signalInitial: false)
                             .Subscribe(changes.Add))
                {
                    CollectionAssert.IsEmpty(changes);

                    var item3 = new Fake { Level1 = new Level1 { Name = "3" } };
                    source.Add(item3);
                    Assert.AreEqual(string.Empty, changes.Single().PropertyName);

                    item3.Level1.Name += "new";
                    Assert.AreEqual(2, changes.Count);
                    Assert.AreEqual("Name", changes.Last().PropertyName);

                    item2.Level1.Name += "new";
                    Assert.AreEqual(3, changes.Count);
                    Assert.AreEqual("Name", changes.Last().PropertyName);

                    item1.Level1.Name += "new";
                    Assert.AreEqual(4, changes.Count);
                    Assert.AreEqual("Name", changes.Last().PropertyName);
                }
            }

            [Test]
            public static void ReplaceSimple()
            {
                var changes = new List<PropertyChangedEventArgs>();
                var item1 = new Fake { Name = "1" };
                var item2 = new Fake { Name = "2" };
                var source = new ObservableCollection<Fake> { item1, item2 };
                using (source.ObserveItemPropertyChangedSlim(x => x.Name, signalInitial: false)
                             .Subscribe(changes.Add))
                {
                    CollectionAssert.IsEmpty(changes);

                    var item3 = new Fake { Name = "3" };
                    source[0] = item3;
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
            public static void ReplaceWithNullSimple()
            {
                var changes = new List<PropertyChangedEventArgs>();
                var item1 = new Fake { Name = "1" };
                var item2 = new Fake { Name = "2" };
                var source = new ObservableCollection<Fake> { item1, item2 };
                using (source.ObserveItemPropertyChangedSlim(x => x.Name, signalInitial: false)
                             .Subscribe(changes.Add))
                {
                    CollectionAssert.IsEmpty(changes);

                    source[0] = null;
                    Assert.AreEqual(string.Empty, changes.Single().PropertyName);

                    item1.Name = "1.1";
                    Assert.AreEqual(1, changes.Count);
                }

                item1.Name = "1.2";
                Assert.AreEqual(1, changes.Count);
            }

            [Test]
            public static void ReplaceNullWithNullSimple()
            {
                var changes = new List<PropertyChangedEventArgs>();
                var source = new ObservableCollection<Fake> { null };
                using (source.ObserveItemPropertyChangedSlim(x => x.Name, signalInitial: false)
                             .Subscribe(changes.Add))
                {
                    CollectionAssert.IsEmpty(changes);
                    source[0] = null;
                    CollectionAssert.IsEmpty(changes);
                }

                CollectionAssert.IsEmpty(changes);
            }

            [Test]
            public static void ReplaceNullWithItemSimple()
            {
                var changes = new List<PropertyChangedEventArgs>();
                var source = new ObservableCollection<Fake> { null };
                using (source.ObserveItemPropertyChangedSlim(x => x.Name, signalInitial: false)
                             .Subscribe(changes.Add))
                {
                    CollectionAssert.IsEmpty(changes);
                    var item = new Fake { Name = "3" };
                    source[0] = item;
                    Assert.AreEqual(string.Empty, changes.Single().PropertyName);

                    item.Name = "new";
                    Assert.AreEqual(2, changes.Count);
                    Assert.AreEqual("Name", changes.Last().PropertyName);
                }

                Assert.AreEqual(2, changes.Count);
            }

            [Test]
            public static void ReplaceWithSameSimple()
            {
                var changes = new List<PropertyChangedEventArgs>();
                var item1 = new Fake { Name = "1" };
                var item2 = new Fake { Name = "2" };
                var source = new ObservableCollection<Fake> { item1, item2 };
                using (source.ObserveItemPropertyChangedSlim(x => x.Value, signalInitial: false)
                             .Subscribe(changes.Add))
                {
                    CollectionAssert.IsEmpty(changes);

                    source[0] = item1;
                    CollectionAssert.IsEmpty(changes);

                    item1.Value++;
                    Assert.AreEqual("Value", changes.Single().PropertyName);
                }

                Assert.AreEqual(1, changes.Count);
            }

            [Test]
            public static void ReplaceNested()
            {
                var changes = new List<PropertyChangedEventArgs>();
                var item1 = new Fake { Level1 = new Level1 { Name = "1" } };
                var item2 = new Fake { Level1 = new Level1 { Name = "2" } };
                var source = new ObservableCollection<Fake> { item1, item2 };
                using (source.ObserveItemPropertyChangedSlim(x => x.Level1.Name, signalInitial: false)
                             .Subscribe(changes.Add))
                {
                    CollectionAssert.IsEmpty(changes);

                    var item3 = new Fake { Level1 = new Level1 { Name = "3" } };
                    source[0] = item3;
                    Assert.AreEqual(string.Empty, changes.Single().PropertyName);

                    item3.Level1.Name = "new";
                    Assert.AreEqual(2, changes.Count);
                    Assert.AreEqual("Name", changes.Last().PropertyName);

                    item1.Level1.Name = "new1";
                    Assert.AreEqual(2, changes.Count);
                }

                Assert.AreEqual(2, changes.Count);
            }

            [Test]
            public static void ReplaceWithNullNested()
            {
                var changes = new List<PropertyChangedEventArgs>();
                var item1 = new Fake { Level1 = new Level1 { Name = "1" } };
                var item2 = new Fake { Level1 = new Level1 { Name = "2" } };
                var source = new ObservableCollection<Fake> { item1, item2 };
                using (source.ObserveItemPropertyChangedSlim(x => x.Level1.Name, signalInitial: false)
                             .Subscribe(changes.Add))
                {
                    CollectionAssert.IsEmpty(changes);

                    source[0] = null;
                    Assert.AreEqual(string.Empty, changes.Single().PropertyName);
                }

                Assert.AreEqual(1, changes.Count);
            }

            [Test]
            public static void ReplaceWithSameNested()
            {
                var changes = new List<PropertyChangedEventArgs>();
                var item1 = new Fake { Level1 = new Level1 { Name = "1" } };
                var item2 = new Fake { Level1 = new Level1 { Name = "2" } };
                var source = new ObservableCollection<Fake> { item1, item2 };
                using (source.ObserveItemPropertyChangedSlim(x => x.Level1.Value, signalInitial: false)
                             .Subscribe(changes.Add))
                {
                    CollectionAssert.IsEmpty(changes);

                    source[0] = item1;
                    CollectionAssert.IsEmpty(changes);

                    item1.Level1.Value++;
                    Assert.AreEqual("Value", changes.Single().PropertyName);
                }

                Assert.AreEqual(1, changes.Count);
            }

            [Test]
            public static void RemoveSimple()
            {
                var changes = new List<PropertyChangedEventArgs>();
                var item1 = new Fake { Name = "1" };
                var item2 = new Fake { Name = "2" };
                var source = new ObservableCollection<Fake> { item1, item2 };
                using (source.ObserveItemPropertyChangedSlim(x => x.Name, signalInitial: false)
                             .Subscribe(changes.Add))
                {
                    CollectionAssert.IsEmpty(changes);

                    source.Remove(item2);
                    Assert.AreEqual(string.Empty, changes.Single().PropertyName);

                    item2.Name = "new";
                    Assert.AreEqual(1, changes.Count);
                }

                Assert.AreEqual(1, changes.Count);
            }

            [Test]
            public static void RemoveNullSimple()
            {
                var changes = new List<PropertyChangedEventArgs>();
                var item1 = new Fake { Name = "1" };
                var source = new ObservableCollection<Fake> { item1, null };
                using (source.ObserveItemPropertyChangedSlim(x => x.Name, signalInitial: false)
                             .Subscribe(changes.Add))
                {
                    CollectionAssert.IsEmpty(changes);
                    source.RemoveAt(1);
                    Assert.AreEqual(string.Empty, changes.Single().PropertyName);
                }

                Assert.AreEqual(1, changes.Count);
            }

            [Test]
            public static void RemoveNested()
            {
                var changes = new List<PropertyChangedEventArgs>();
                var item1 = new Fake { Level1 = new Level1 { Name = "1" } };
                var item2 = new Fake { Level1 = new Level1 { Name = "2" } };
                var source = new ObservableCollection<Fake> { item1, item2 };
                using (source.ObserveItemPropertyChangedSlim(x => x.Level1.Name, signalInitial: false)
                             .Subscribe(changes.Add))
                {
                    CollectionAssert.IsEmpty(changes);

                    source.Remove(item2);
                    Assert.AreEqual(string.Empty, changes.Single().PropertyName);

                    item2.Level1.Name = "new";
                    Assert.AreEqual(1, changes.Count);
                }

                Assert.AreEqual(1, changes.Count);
            }

            [Test]
            public static void RemoveNullNested()
            {
                var changes = new List<PropertyChangedEventArgs>();
                var item1 = new Fake { Level1 = new Level1 { Name = "1" } };
                var source = new ObservableCollection<Fake> { item1, null };
                using (source.ObserveItemPropertyChangedSlim(x => x.Level1.Name, signalInitial: false)
                             .Subscribe(changes.Add))
                {
                    CollectionAssert.IsEmpty(changes);
                    source.RemoveAt(1);
                    Assert.AreEqual(string.Empty, changes.Single().PropertyName);
                }

                Assert.AreEqual(1, changes.Count);
            }

            [Test]
            public static void MoveSimple()
            {
                var changes = new List<PropertyChangedEventArgs>();
                var item1 = new Fake { Name = "1" };
                var item2 = new Fake { Name = "2" };
                var source = new ObservableCollection<Fake> { item1, item2 };
                using (source.ObserveItemPropertyChangedSlim(x => x.Name, signalInitial: false)
                             .Subscribe(changes.Add))
                {
                    CollectionAssert.IsEmpty(changes);

                    source.Move(0, 1);
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
            public static void MoveNested()
            {
                var changes = new List<PropertyChangedEventArgs>();
                var item1 = new Fake { Level1 = new Level1 { Name = "1" } };
                var item2 = new Fake { Level1 = new Level1 { Name = "2" } };
                var source = new ObservableCollection<Fake> { item1, item2 };
                using (source.ObserveItemPropertyChangedSlim(x => x.Level1.Name, signalInitial: false)
                             .Subscribe(changes.Add))
                {
                    CollectionAssert.IsEmpty(changes);

                    source.Move(0, 1);
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
            public static void ReadOnlyObservableCollectionCount()
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
            public static void ReactsWhenPropertyChangesSimple()
            {
                var changes = new List<PropertyChangedEventArgs>();
                var item1 = new Fake { Name = "1" };
                var item2 = new Fake { Name = "2" };
                var source = new ObservableCollection<Fake> { item1, item2 };
                using (source.ObserveItemPropertyChangedSlim(x => x.Name, signalInitial: false)
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
            public static void DoesNotReactWhenOtherPropertyChangesSimple()
            {
                var changes = new List<PropertyChangedEventArgs>();
                var item1 = new Fake { Name = "1" };
                var item2 = new Fake { Name = "2" };
                var source = new ObservableCollection<Fake> { item1, item2 };
                using (source.ObserveItemPropertyChangedSlim(x => x.Name, signalInitial: false)
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
            public static void SameInstanceTwiceSimple()
            {
                var changes = new List<PropertyChangedEventArgs>();
                var item = new Fake { Name = "1" };
                var source = new ObservableCollection<Fake> { item };
                using (source.ObserveItemPropertyChangedSlim(x => x.Name, signalInitial: false)
                             .Subscribe(changes.Add))
                {
                    CollectionAssert.IsEmpty(changes);

                    item.Name = "new1";
                    Assert.AreEqual("Name", changes.Single().PropertyName);

                    source.Add(item);
                    Assert.AreEqual(2, changes.Count);

                    item.Name = "new2";
                    Assert.AreEqual(3, changes.Count);
                    Assert.AreEqual("Name", changes.Last().PropertyName);

                    source.RemoveAt(1);
                    Assert.AreEqual(4, changes.Count);

                    item.Name = "new3";
                    Assert.AreEqual(5, changes.Count);
                    Assert.AreEqual("Name", changes.Last().PropertyName);
                }

                Assert.AreEqual(5, changes.Count);
            }

            [Test]
            public static void SameInstanceTwiceNested()
            {
                var changes = new List<PropertyChangedEventArgs>();
                var item = new Fake { Level1 = new Level1 { Name = "1" } };
                var source = new ObservableCollection<Fake> { item };
                using (source.ObserveItemPropertyChangedSlim(x => x.Level1.Name, signalInitial: false)
                             .Subscribe(changes.Add))
                {
                    CollectionAssert.IsEmpty(changes);
                    item.Level1.Name = "new1";
                    Assert.AreEqual("Name", changes.Single().PropertyName);

                    source.Add(item);
                    Assert.AreEqual(2, changes.Count);

                    item.Level1.Name = "new2";
                    Assert.AreEqual(3, changes.Count);
                    Assert.AreEqual("Name", changes.Last().PropertyName);

                    source.RemoveAt(1);
                    Assert.AreEqual(4, changes.Count);

                    item.Level1.Name = "new3";
                    Assert.AreEqual(5, changes.Count);
                    Assert.AreEqual("Name", changes.Last().PropertyName);
                }

                Assert.AreEqual(5, changes.Count);
            }

            [Test]
            public static void OneObservableTwoSubscriptions()
            {
                var changes1 = new List<PropertyChangedEventArgs>();
                var changes2 = new List<PropertyChangedEventArgs>();
                var item1 = new Fake { Name = "1" };
                var item2 = new Fake { Name = "2" };
                var source = new ObservableCollection<Fake> { item1, item2 };
                var observable = source.ObserveItemPropertyChangedSlim(x => x.Name, signalInitial: false);
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
            public static void ReactsWhenPropertyChangesView()
            {
                var changes = new List<PropertyChangedEventArgs>();
                var item1 = new Fake { Name = "1" };
                var item2 = new Fake { Name = "2" };
                var source = new ObservableCollection<Fake> { item1, item2 };
                using (var view = source.AsReadOnlyFilteredView(x => true))
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
            public static void ReactsNested()
            {
                var changes = new List<PropertyChangedEventArgs>();
                var item1 = new Fake { Next = new Level { Name = "1" } };
                var item2 = new Fake();
                var source = new ObservableCollection<Fake> { item1, item2 };
                using (source.ObserveItemPropertyChangedSlim(x => x.Next.Name, signalInitial: false)
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
            public static void DoesNotReactToOtherPropertyNested()
            {
                var changes = new List<PropertyChangedEventArgs>();
                var item1 = new Fake { Next = new Level { Name = "1" } };
                var item2 = new Fake();
                var source = new ObservableCollection<Fake> { item1, item2 };
                using (source.ObserveItemPropertyChangedSlim(x => x.Next.Name, signalInitial: false)
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
            public static void SameItemTwiceNotifiesOnceSimple()
            {
                var changes = new List<PropertyChangedEventArgs>();
                var item = new Fake { Name = "1" };
                var source = new ObservableCollection<Fake> { item, item };
                using (source.ObserveItemPropertyChangedSlim(x => x.Name, signalInitial: false)
                                 .Subscribe(changes.Add))
                {
                    CollectionAssert.IsEmpty(changes);

                    item.Name = "new";
                    Assert.AreEqual("Name", changes.Single().PropertyName);
                }

                Assert.AreEqual(1, changes.Count);
            }

            [Test]
            public static void SameItemTwiceNotifiesOnceNested()
            {
                var changes = new List<PropertyChangedEventArgs>();
                var item = new Fake { Next = new Level { Name = "1" } };
                var source = new ObservableCollection<Fake> { item, item };
                using (source.ObserveItemPropertyChangedSlim(x => x.Next.Name, signalInitial: false)
                             .Subscribe(changes.Add))
                {
                    CollectionAssert.IsEmpty(changes);

                    item.Next.Name = "new";
                    Assert.AreEqual("Name", changes.Single().PropertyName);
                }

                Assert.AreEqual(1, changes.Count);
            }

            [Test]
            public static void DisposeStopsSubscribing()
            {
                var changes = new List<PropertyChangedEventArgs>();
                var item1 = new Fake { Name = "1" };
                var item2 = new Fake { Name = "2" };
                var source = new ObservableCollection<Fake> { item1, item2 };
                using (source.ObserveItemPropertyChangedSlim(x => x.Name, signalInitial: false)
                             .Subscribe(changes.Add))
                {
                }

                source.Add(new Fake { Name = "3" });
                CollectionAssert.IsEmpty(changes);
            }

            [Test]
            [SuppressMessage("ReSharper", "HeuristicUnreachableCode")]
            public static void MemoryLeakDisposeTest()
            {
#if DEBUG
                Assert.Inconclusive("Debugger keeps things alive for the scope of the method.");
#endif
                var changes = new List<PropertyChangedEventArgs>();
                var source = new ObservableCollection<Fake> { new Fake { Name = "1" }, new Fake { Name = "2" } };

                var sourceRef = new WeakReference(source);
                var item1Ref = new WeakReference(source[0]);
                var observable = source.ObserveItemPropertyChangedSlim(x => x.Name, signalInitial: false);
                Assert.IsTrue(sourceRef.IsAlive);
                //// http://stackoverflow.com/a/579001/1069200
                using (observable.Subscribe(changes.Add))
                {
                    CollectionAssert.IsEmpty(changes);
                }

                GC.Collect();
                Assert.IsFalse(sourceRef.IsAlive);
                Assert.IsFalse(item1Ref.IsAlive);
            }

            [Test]
            [SuppressMessage("ReSharper", "HeuristicUnreachableCode")]
            public static void MemoryLeakNoDisposeTest()
            {
#if DEBUG
                Assert.Inconclusive("Debugger keeps things alive for the scope of the method.");
#endif
                var changes = new List<PropertyChangedEventArgs>();
                var source = new ObservableCollection<Fake> { new Fake { Name = "1" }, new Fake { Name = "2" } };

                var sourceRef = new WeakReference(source);
                var item1Ref = new WeakReference(source[0]);
                var observable = source.ObserveItemPropertyChangedSlim(x => x.Name, signalInitial: false);
                Assert.IsTrue(sourceRef.IsAlive);
                //// http://stackoverflow.com/a/579001/1069200
#pragma warning disable IDISP001  // Dispose created.
                var subscription = observable.Subscribe(changes.Add);
#pragma warning restore IDISP001  // Dispose created.
                GC.KeepAlive(observable);
                GC.KeepAlive(subscription);
                CollectionAssert.IsEmpty(changes);

                GC.Collect();

                Assert.IsFalse(sourceRef.IsAlive);
                Assert.IsFalse(item1Ref.IsAlive);
            }
        }
    }
}
