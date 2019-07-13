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

    public static class ObserveItemPropertyChanged
    {
        [Test]
        public static void SignalsInitialSimple()
        {
            var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
            var item1 = new Fake { Name = "1" };
            var item2 = new Fake { Name = "2" };
            var source = new ObservableCollection<Fake> { item1, item2 };
            using (source.ObserveItemPropertyChanged(x => x.Name, signalInitial: true)
                         .Subscribe(changes.Add))
            {
                Assert.AreEqual(2, changes.Count);
                EventPatternAssert.AreEqual(item1, source, item1, Maybe.Some("1"), string.Empty, changes[0]);
                EventPatternAssert.AreEqual(item2, source, item2, Maybe.Some("2"), string.Empty, changes[1]);
            }

            Assert.AreEqual(2, changes.Count);
        }

        [Test]
        public static void SignalsInitialNested()
        {
            var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
            var item1 = new Fake { Level1 = new Level1 { Name = "1" } };
            var item2 = new Fake { Level1 = new Level1 { Name = "2" } };
            var source = new ObservableCollection<Fake> { item1, item2 };
            using (source.ObserveItemPropertyChanged(x => x.Level1.Name, signalInitial: true)
                         .Subscribe(changes.Add))
            {
                Assert.AreEqual(2, changes.Count);
                EventPatternAssert.AreEqual(item1, source, item1.Level1, Maybe.Some("1"), string.Empty, changes[0]);
                EventPatternAssert.AreEqual(item2, source, item2.Level1, Maybe.Some("2"), string.Empty, changes[1]);
            }

            Assert.AreEqual(2, changes.Count);
        }

        [Test]
        public static void DoesNotSignalInitialSimple()
        {
            var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
            var item1 = new Fake { Name = "1" };
            var item2 = new Fake { Name = "2" };
            var source = new ObservableCollection<Fake> { item1, item2 };
            using (source.ObserveItemPropertyChanged(x => x.Name, signalInitial: false)
                         .Subscribe(changes.Add))
            {
                CollectionAssert.IsEmpty(changes);
            }

            CollectionAssert.IsEmpty(changes);
        }

        [Test]
        public static void DoesNotSignalInitialNested()
        {
            var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
            var item1 = new Fake { Level1 = new Level1 { Name = "1" } };
            var item2 = new Fake { Level1 = new Level1 { Name = "2" } };
            var source = new ObservableCollection<Fake> { item1, item2 };
            using (source.ObserveItemPropertyChanged(x => x.Next.Name, signalInitial: false)
                         .Subscribe(changes.Add))
            {
                CollectionAssert.IsEmpty(changes);
            }

            CollectionAssert.IsEmpty(changes);
        }

        [Test]
        public static void AddSimple()
        {
            var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
            var item1 = new Fake { Name = "1" };
            var item2 = new Fake { Name = "2" };
            var source = new ObservableCollection<Fake> { item1, item2 };
            using (source.ObserveItemPropertyChanged(x => x.Name, signalInitial: true)
                         .Subscribe(changes.Add))
            {
                Assert.AreEqual(2, changes.Count);
                EventPatternAssert.AreEqual(item1, source, item1, Maybe.Some("1"), string.Empty, changes[0]);
                EventPatternAssert.AreEqual(item2, source, item2, Maybe.Some("2"), string.Empty, changes[1]);

                var item3 = new Fake { Name = "3" };
                source.Add(item3);
                Assert.AreEqual(3, changes.Count);
                EventPatternAssert.AreEqual(item3, source, item3, Maybe.Some("3"), string.Empty, changes.Last());
            }

            Assert.AreEqual(3, changes.Count);
        }

        [Test]
        public static void AddNullSimple()
        {
            var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
            var item1 = new Fake { Name = "1" };
            var item2 = new Fake { Name = "2" };
            var source = new ObservableCollection<Fake> { item1, item2 };
            using (source.ObserveItemPropertyChanged(x => x.Name, signalInitial: true)
                         .Subscribe(changes.Add))
            {
                Assert.AreEqual(2, changes.Count);
                EventPatternAssert.AreEqual(item1, source, item1, Maybe.Some("1"), string.Empty, changes[0]);
                EventPatternAssert.AreEqual(item2, source, item2, Maybe.Some("2"), string.Empty, changes[1]);

                source.Add(null);
                Assert.AreEqual(3, changes.Count);
                EventPatternAssert.AreEqual(null, source, null, Maybe.None<string>(), string.Empty, changes.Last());
            }

            Assert.AreEqual(3, changes.Count);
        }

        [Test]
        public static void AddThenUpdateSimple()
        {
            var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
            var item1 = new Fake { Name = "1" };
            var item2 = new Fake { Name = "2" };
            var source = new ObservableCollection<Fake> { item1, item2 };
            using (source.ObserveItemPropertyChanged(x => x.Name, signalInitial: false)
                         .Subscribe(changes.Add))
            {
                CollectionAssert.IsEmpty(changes);

                var item3 = new Fake { Name = "3" };
                source.Add(item3);
                EventPatternAssert.AreEqual(item3, source, item3, Maybe.Some("3"), string.Empty, changes.Single());

                item3.Name = "new";
                Assert.AreEqual(2, changes.Count);
                EventPatternAssert.AreEqual(item3, item3, item3, Maybe.Some("new"), "Name", changes.Last());
            }
        }

        [Test]
        public static void AddNested()
        {
            var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
            var item1 = new Fake { Level1 = new Level1 { Name = "1" } };
            var item2 = new Fake { Level1 = new Level1 { Name = "2" } };
            var source = new ObservableCollection<Fake> { item1, item2 };
            using (source.ObserveItemPropertyChanged(x => x.Level1.Name, signalInitial: false)
                         .Subscribe(changes.Add))
            {
                CollectionAssert.IsEmpty(changes);

                var item3 = new Fake { Level1 = new Level1 { Name = "3" } };
                source.Add(item3);
                EventPatternAssert.AreEqual(item3, source, item3.Level1, Maybe.Some("3"), string.Empty, changes.Single());

                item3.Level1.Name = "new";
                Assert.AreEqual(2, changes.Count);
                EventPatternAssert.AreEqual(item3, item3.Level1, item3.Level1, Maybe.Some("new"), "Name", changes.Last());
            }
        }

        [Test]
        public static void MoveSimple()
        {
            var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
            var item1 = new Fake { Name = "1" };
            var item2 = new Fake { Name = "2" };
            var source = new ObservableCollection<Fake> { item1, item2 };
            using (source.ObserveItemPropertyChanged(x => x.Name, signalInitial: false)
                         .Subscribe(changes.Add))
            {
                CollectionAssert.IsEmpty(changes);

                source.Move(0, 1);
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
        public static void MoveNested()
        {
            var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
            var item1 = new Fake { Level1 = new Level1 { Name = "1" } };
            var item2 = new Fake { Level1 = new Level1 { Name = "2" } };
            var source = new ObservableCollection<Fake> { item1, item2 };
            using (source.ObserveItemPropertyChanged(x => x.Level1.Name, signalInitial: false)
                         .Subscribe(changes.Add))
            {
                CollectionAssert.IsEmpty(changes);

                source.Move(0, 1);
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
        public static void ReplaceSimple()
        {
            var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
            var item1 = new Fake { Name = "1" };
            var item2 = new Fake { Name = "2" };
            var source = new ObservableCollection<Fake> { item1, item2 };
            using (source.ObserveItemPropertyChanged(x => x.Name, signalInitial: false)
                         .Subscribe(changes.Add))
            {
                CollectionAssert.IsEmpty(changes);

                var item3 = new Fake { Name = "3" };
                source[0] = item3;
                Assert.AreEqual(2, changes.Count);
                EventPatternAssert.AreEqual(null, source, item1, Maybe.Some("1"), string.Empty, changes[0]);
                EventPatternAssert.AreEqual(item3, source, item3, Maybe.Some("3"), string.Empty, changes[1]);

                item3.Name = "new";
                Assert.AreEqual(3, changes.Count);
                EventPatternAssert.AreEqual(item3, item3, item3, Maybe.Some("new"), "Name", changes.Last());

                item1.Name = "new1";
                Assert.AreEqual(3, changes.Count);
            }

            Assert.AreEqual(3, changes.Count);
        }

        [Test]
        public static void ReplaceWithNullSimple()
        {
            var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
            var item1 = new Fake { Name = "1" };
            var item2 = new Fake { Name = "2" };
            var source = new ObservableCollection<Fake> { item1, item2 };
            using (source.ObserveItemPropertyChanged(x => x.Name, signalInitial: false)
                         .Subscribe(changes.Add))
            {
                CollectionAssert.IsEmpty(changes);

                source[0] = null;
                Assert.AreEqual(2, changes.Count);
                EventPatternAssert.AreEqual(null, source, item1, Maybe.Some("1"), string.Empty, changes[0]);
                EventPatternAssert.AreEqual(null, source, null, Maybe.None<string>(), string.Empty, changes[1]);

                item1.Name = "new";
                Assert.AreEqual(2, changes.Count);
            }

            Assert.AreEqual(2, changes.Count);
        }

        [Test]
        public static void ReplaceNested()
        {
            var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
            var item1 = new Fake { Next = new Level { Name = "1" } };
            var item2 = new Fake { Next = new Level { Name = "2" } };
            var source = new ObservableCollection<Fake> { item1, item2 };
            using (source.ObserveItemPropertyChanged(x => x.Next.Name, signalInitial: false)
                         .Subscribe(changes.Add))
            {
                CollectionAssert.IsEmpty(changes);

                var item3 = new Fake { Next = new Level { Name = "3" } };
                source[0] = item3;
                Assert.AreEqual(2, changes.Count);
                EventPatternAssert.AreEqual(null, source, item1.Next, Maybe.Some("1"), string.Empty, changes[0]);
                EventPatternAssert.AreEqual(item3, source, item3.Next, Maybe.Some("3"), string.Empty, changes[1]);

                item3.Next.Name = "new";
                Assert.AreEqual(3, changes.Count);
                EventPatternAssert.AreEqual(item3, item3.Next, item3.Next, Maybe.Some("new"), "Name", changes.Last());

                item1.Next.Name = "new1";
                Assert.AreEqual(3, changes.Count);
            }

            Assert.AreEqual(3, changes.Count);
        }

        [Test]
        public static void ReplaceWithNullNested()
        {
            var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
            var item1 = new Fake { Level1 = new Level1 { Name = "1" } };
            var item2 = new Fake { Level1 = new Level1 { Name = "2" } };
            var source = new ObservableCollection<Fake> { item1, item2 };
            using (source.ObserveItemPropertyChanged(x => x.Level1.Name, signalInitial: false)
                         .Subscribe(changes.Add))
            {
                CollectionAssert.IsEmpty(changes);

                source[0] = null;
                Assert.AreEqual(2, changes.Count);
                EventPatternAssert.AreEqual(null, source, item1.Level1, Maybe.Some("1"), string.Empty, changes[0]);
                EventPatternAssert.AreEqual(null, source, null, Maybe.None<string>(), string.Empty, changes[1]);

                item1.Level1.Name = "new1";
                Assert.AreEqual(2, changes.Count);
            }

            Assert.AreEqual(2, changes.Count);
        }

        [Test]
        public static void ReplaceWithSameSimple()
        {
            var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, int>>>();
            var item1 = new Fake { Name = "1" };
            var item2 = new Fake { Name = "2" };
            var source = new ObservableCollection<Fake> { item1, item2 };
            using (source.ObserveItemPropertyChanged(x => x.Value, signalInitial: false)
                         .Subscribe(changes.Add))
            {
                CollectionAssert.IsEmpty(changes);

                source[0] = item1;
                CollectionAssert.IsEmpty(changes);

                item1.Value++;
                EventPatternAssert.AreEqual(item1, item1, item1, Maybe.Some(item1.Value), "Value", changes.Single());
            }

            Assert.AreEqual(1, changes.Count);
        }

        [Test]
        public static void ReplaceWithSameNested()
        {
            var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
            var item1 = new Fake { Level1 = new Level1 { Name = "1" } };
            var item2 = new Fake { Level1 = new Level1 { Name = "2" } };
            var source = new ObservableCollection<Fake> { item1, item2 };
            using (source.ObserveItemPropertyChanged(x => x.Level1.Name, signalInitial: false)
                         .Subscribe(changes.Add))
            {
                CollectionAssert.IsEmpty(changes);

                source[0] = item1;
                CollectionAssert.IsEmpty(changes);

                item1.Level1.Name = "3";
                EventPatternAssert.AreEqual(item1, item1.Level1, item1.Level1, Maybe.Some("3"), "Name", changes.Single());
            }

            Assert.AreEqual(1, changes.Count);
        }

        [Test]
        public static void RemoveSimple()
        {
            var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
            var item1 = new Fake { Name = "1" };
            var item2 = new Fake { Name = "2" };
            var source = new ObservableCollection<Fake> { item1, item2 };
            using (source.ObserveItemPropertyChanged(x => x.Name, signalInitial: false)
                         .Subscribe(changes.Add))
            {
                CollectionAssert.IsEmpty(changes);

                source.Remove(item2);
                EventPatternAssert.AreEqual(null, source, item2, Maybe.Some("2"), string.Empty, changes.Single());

                item2.Name = "new";
                Assert.AreEqual(1, changes.Count);
            }

            Assert.AreEqual(1, changes.Count);
        }

        [Test]
        public static void RemoveNested()
        {
            var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
            var item1 = new Fake { Level1 = new Level1 { Name = "1" } };
            var item2 = new Fake { Level1 = new Level1 { Name = "2" } };
            var source = new ObservableCollection<Fake> { item1, item2 };
            using (source.ObserveItemPropertyChanged(x => x.Level1.Name, signalInitial: false)
                         .Subscribe(changes.Add))
            {
                CollectionAssert.IsEmpty(changes);

                source.Remove(item2);
                EventPatternAssert.AreEqual(null, source, item2.Level1, Maybe.Some("2"), string.Empty, changes.Single());

                item2.Level1.Name = "new";
                Assert.AreEqual(1, changes.Count);
            }

            Assert.AreEqual(1, changes.Count);
        }

        [Test]
        public static void ReadOnlyObservableCollectionCount()
        {
            var source = new ObservableCollection<ReadOnlyObservableCollection<int>>();
            var changes = new List<EventPattern<ItemPropertyChangedEventArgs<ReadOnlyObservableCollection<int>, int>>>();
            using (source.ObserveItemPropertyChanged(x => x.Count, signalInitial: false)
                         .Subscribe(x => changes.Add(x)))
            {
                CollectionAssert.IsEmpty(changes);

                var item1 = new ReadOnlyObservableCollection<int>(new ObservableCollection<int> { 1 });
                source.Add(item1);
                EventPatternAssert.AreEqual(item1, source, item1, Maybe.Some(1), string.Empty, changes.Single());
            }
        }

        [Test]
        public static void ReactsWhenPropertyChangesSimple()
        {
            var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
            var item1 = new Fake { Name = "1" };
            var item2 = new Fake { Name = "2" };
            var source = new ObservableCollection<Fake> { item1, item2 };
            using (source.ObserveItemPropertyChanged(x => x.Name, signalInitial: false)
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
        public static void DoesNotReactWhenOtherPropertyChangesSimple()
        {
            var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
            var item1 = new Fake { Name = "1" };
            var item2 = new Fake { Name = "2" };
            var source = new ObservableCollection<Fake> { item1, item2 };
            using (source.ObserveItemPropertyChanged(x => x.Name, signalInitial: false)
                         .Subscribe(changes.Add))
            {
                CollectionAssert.IsEmpty(changes);

                item1.Name = "new1";
                Assert.AreEqual(1, changes.Count);
                EventPatternAssert.AreEqual(item1, item1, item1, Maybe.Some("new1"), "Name", changes.Last());

                item1.Value++;
                Assert.AreEqual(1, changes.Count);
                EventPatternAssert.AreEqual(item1, item1, item1, Maybe.Some("new1"), "Name", changes.Last());

                item2.Name = "new2";
                Assert.AreEqual(2, changes.Count);
                EventPatternAssert.AreEqual(item2, item2, item2, Maybe.Some("new2"), "Name", changes.Last());

                item2.Value++;
                Assert.AreEqual(2, changes.Count);
                EventPatternAssert.AreEqual(item2, item2, item2, Maybe.Some("new2"), "Name", changes.Last());
            }

            Assert.AreEqual(2, changes.Count);
        }

        [Test]
        public static void SameInstanceTwiceSimple()
        {
            var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
            var item = new Fake { Name = "1" };
            var source = new ObservableCollection<Fake> { item };
            using (source.ObserveItemPropertyChanged(x => x.Name, signalInitial: false)
                         .Subscribe(changes.Add))
            {
                CollectionAssert.IsEmpty(changes);

                item.Name = "new1";
                EventPatternAssert.AreEqual(item, item, item, Maybe.Some("new1"), "Name", changes.Single());

                source.Add(item);
                Assert.AreEqual(2, changes.Count);
                EventPatternAssert.AreEqual(item, source, item, Maybe.Some("new1"), string.Empty, changes.Last());

                item.Name = "new2";
                Assert.AreEqual(3, changes.Count);
                EventPatternAssert.AreEqual(item, item, item, Maybe.Some("new2"), "Name", changes.Last());

                source.RemoveAt(1);
                Assert.AreEqual(4, changes.Count);
                EventPatternAssert.AreEqual(null, source, item, Maybe.Some("new2"), string.Empty, changes.Last());

                item.Name = "new3";
                Assert.AreEqual(5, changes.Count);
                EventPatternAssert.AreEqual(item, item, item, Maybe.Some("new3"), "Name", changes.Last());
            }

            Assert.AreEqual(5, changes.Count);
        }

        [Test]
        public static void SameInstanceTwiceNested()
        {
            var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
            var item = new Fake { Level1 = new Level1 { Name = "1" } };
            var source = new ObservableCollection<Fake> { item };
            using (source.ObserveItemPropertyChanged(x => x.Level1.Name, signalInitial: false)
                         .Subscribe(changes.Add))
            {
                CollectionAssert.IsEmpty(changes);
                item.Level1.Name = "new1";
                EventPatternAssert.AreEqual(item, item.Level1, item.Level1, Maybe.Some("new1"), "Name", changes.Single());

                source.Add(item);
                Assert.AreEqual(2, changes.Count);
                EventPatternAssert.AreEqual(item, source, item.Level1, Maybe.Some("new1"), string.Empty, changes.Last());

                item.Level1.Name = "new2";
                Assert.AreEqual(3, changes.Count);
                EventPatternAssert.AreEqual(item, item.Level1, item.Level1, Maybe.Some("new2"), "Name", changes.Last());

                source.RemoveAt(1);
                Assert.AreEqual(4, changes.Count);
                EventPatternAssert.AreEqual(null, source, item.Level1, Maybe.Some("new2"), string.Empty, changes.Last());

                item.Level1.Name = "new3";
                Assert.AreEqual(5, changes.Count);
                EventPatternAssert.AreEqual(item, item.Level1, item.Level1, Maybe.Some("new3"), "Name", changes.Last());
            }

            Assert.AreEqual(5, changes.Count);
        }

        [Test]
        public static void OneObservableTwoSubscriptions()
        {
            var changes1 = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
            var changes2 = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
            var item1 = new Fake { Name = "1" };
            var item2 = new Fake { Name = "2" };
            var source = new ObservableCollection<Fake> { item1, item2 };
            var observable = source.ObserveItemPropertyChanged(x => x.Name, signalInitial: false);
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
        public static void ReactsWhenPropertyChangesView()
        {
            var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
            var item1 = new Fake { Name = "1" };
            var item2 = new Fake { Name = "2" };
            var source = new ObservableCollection<Fake> { item1, item2 };
            using (var view = source.AsReadOnlyFilteredView(x => true))
            {
                using (view.ObserveItemPropertyChanged(x => x.Name, signalInitial: false)
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
        public static void ReactsNested()
        {
            var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
            var item1 = new Fake { Next = new Level { Name = "1" } };
            var item2 = new Fake();
            var source = new ObservableCollection<Fake> { item1, item2 };
            using (source.ObserveItemPropertyChanged(x => x.Next.Name, signalInitial: false)
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
        public static void DoesNotReactToOtherPropertyNested()
        {
            var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
            var item1 = new Fake { Next = new Level { Name = "1" } };
            var item2 = new Fake();
            var source = new ObservableCollection<Fake> { item1, item2 };
            using (source.ObserveItemPropertyChanged(x => x.Next.Name, signalInitial: false)
                         .Subscribe(changes.Add))
            {
                CollectionAssert.IsEmpty(changes);
                item1.Next.Name = "new1";
                EventPatternAssert.AreEqual(item1, item1.Next, item1.Next, Maybe.Some("new1"), "Name", changes.Single());

                item1.Next.Value++;
                EventPatternAssert.AreEqual(item1, item1.Next, item1.Next, Maybe.Some("new1"), "Name", changes.Single());

                item2.Next = new Level { Name = "new2" };
                Assert.AreEqual(2, changes.Count);
                EventPatternAssert.AreEqual(item2, item2, item2.Next, Maybe.Some("new2"), "Next", changes.Last());
            }

            Assert.AreEqual(2, changes.Count);
        }

        [Test]
        public static void ReactsOnceWhenSameItemIsTwoElementsInCollection()
        {
            var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
            var item = new Fake { Name = "1" };
            var source = new ObservableCollection<Fake> { item, item };
            using (source.ObserveItemPropertyChanged(x => x.Name, signalInitial: false)
                         .Subscribe(changes.Add))
            {
                CollectionAssert.IsEmpty(changes);

                item.Name = "new";
                EventPatternAssert.AreEqual(item, item, item, Maybe.Some("new"), "Name", changes.Single());
            }

            Assert.AreEqual(1, changes.Count);
        }

        [Test]
        public static void NullItemSimple()
        {
            var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
            var item = new Fake { Name = "1" };
            var source = new ObservableCollection<Fake> { item, null };
            using (source.ObserveItemPropertyChanged(x => x.Name, signalInitial: false)
                         .Subscribe(changes.Add))
            {
                CollectionAssert.IsEmpty(changes);

                source.Add(null);
                EventPatternAssert.AreEqual(null, source, null, Maybe.None<string>(), string.Empty, changes.Single());

                item.Name = "new1";
                Assert.AreEqual(2, changes.Count);
                EventPatternAssert.AreEqual(item, item, item, Maybe.Some("new1"), "Name", changes.Last());

                var item2 = new Fake { Name = "2" };
                source[1] = item2;
                Assert.AreEqual(4, changes.Count);
                EventPatternAssert.AreEqual(null, source, null, Maybe.None<string>(), string.Empty, changes[2]);
                EventPatternAssert.AreEqual(item2, source, item2, Maybe.Some("2"), string.Empty, changes[3]);
            }

            Assert.AreEqual(4, changes.Count);
        }

        [Test]
        public static void NullItemNested()
        {
            var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
            var item = new Fake { Next = new Level { Name = "1" } };
            var source = new ObservableCollection<Fake> { item, null };
            using (source.ObserveItemPropertyChanged(x => x.Next.Name, signalInitial: false)
                         .Subscribe(changes.Add))
            {
                CollectionAssert.IsEmpty(changes);

                source.Add(null);
                EventPatternAssert.AreEqual(null, source, null, Maybe.None<string>(), string.Empty, changes.Single());

                item.Next.Name = "new1";
                Assert.AreEqual(2, changes.Count);
                EventPatternAssert.AreEqual(item, item.Next, item.Next, Maybe.Some("new1"), "Name", changes.Last());

                var item2 = new Fake { Next = new Level { Name = "2" } };
                source[1] = item2;
                Assert.AreEqual(4, changes.Count);
                EventPatternAssert.AreEqual(null, source, null, Maybe.None<string>(), string.Empty, changes[2]);
                EventPatternAssert.AreEqual(item2, source, item2.Next, Maybe.Some("2"), string.Empty, changes[3]);
            }

            Assert.AreEqual(4, changes.Count);
        }

        [Test]
        public static void DisposeStopsSubscribing()
        {
            var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
            var item1 = new Fake { Name = "1" };
            var item2 = new Fake { Name = "2" };
            var source = new ObservableCollection<Fake> { item1, item2 };
            using (source.ObserveItemPropertyChanged(x => x.Name, signalInitial: false)
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
            var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
            var source = new ObservableCollection<Fake> { new Fake { Name = "1" }, new Fake { Name = "2" } };

            var sourceRef = new WeakReference(source);
            var item1Ref = new WeakReference(source[0]);
            var observable = source.ObserveItemPropertyChanged(x => x.Name, signalInitial: false);
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
            var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
            var source = new ObservableCollection<Fake> { new Fake { Name = "1" }, new Fake { Name = "2" } };

            var sourceRef = new WeakReference(source);
            var item1Ref = new WeakReference(source[0]);
            var observable = source.ObserveItemPropertyChanged(x => x.Name, signalInitial: false);
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
