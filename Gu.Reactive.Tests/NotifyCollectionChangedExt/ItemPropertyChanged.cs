// ReSharper disable RedundantArgumentDefaultValue
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable once InconsistentNaming
namespace Gu.Reactive.Tests.NotifyCollectionChangedExt
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reactive;

    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    public static class ItemPropertyChanged
    {
        [Test]
        public static void SignalsOnNewCollection()
        {
            var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string?>>>();
            var fake = new With<ObservableCollection<Fake>>();
            using (fake.ObservePropertyChangedWithValue(x => x.Value)
                       .ItemPropertyChanged(x => x.Name)
                       .Subscribe(changes.Add))
            {
                CollectionAssert.IsEmpty(changes);
                var collection = new ObservableCollection<Fake> { new Fake { Name = "1" } };
                fake.Value = collection;
                EventPatternAssert.AreEqual(collection[0], collection, collection[0], Maybe.Some<string?>("1"), string.Empty, changes.Single());
            }
        }

        [Test]
        public static void SignalsOnNewCollectionNullable()
        {
            var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string?>>>();
            var fake = new With<ObservableCollection<Fake>?>();
            using (fake.ObservePropertyChangedWithValue(x => x.Value)
                       .ItemPropertyChanged(x => x.Name)
                       .Subscribe(changes.Add))
            {
                CollectionAssert.IsEmpty(changes);
                var collection = new ObservableCollection<Fake> { new Fake { Name = "1" } };
                fake.Value = collection;
                EventPatternAssert.AreEqual(collection[0], collection, collection[0], Maybe.Some<string?>("1"), string.Empty, changes.Single());
            }
        }

        [Test]
        public static void SignalsInitial()
        {
            var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string?>>>();
            var collection = new ObservableCollection<Fake> { new Fake { Name = "1" } };
            var fake = new FakeWithCollection { Collection = collection };
            using (fake.ObservePropertyChangedWithValue(x => x.Collection, signalInitial: true)
                       .ItemPropertyChanged(x => x.Name)
                       .Subscribe(changes.Add))
            {
                EventPatternAssert.AreEqual(collection[0], collection, collection[0], Maybe.Some<string?>("1"), string.Empty, changes.Single());
            }
        }

        [Test]
        public static void ReactsNested()
        {
            var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string?>>>();
            var collection = new ObservableCollection<Fake>
            {
                new Fake { Next = new Level { Name = "Johan" } },
                new Fake { Next = new Level { Name = "Reed" } },
            };
            var fake = new FakeWithCollection { Collection = collection };
            using (fake.ObservePropertyChangedWithValue(x => x.Collection, signalInitial: true)
                       .ItemPropertyChanged(x => x.Next.Name)
                       .Subscribe(changes.Add))
            {
                Assert.AreEqual(2, changes.Count);
                EventPatternAssert.AreEqual(collection[0], collection, collection[0].Next, Maybe.Some<string?>("Johan"), string.Empty, changes[0]);
                EventPatternAssert.AreEqual(collection[1], collection, collection[1].Next, Maybe.Some<string?>("Reed"), string.Empty, changes[1]);

                fake.Collection[0].Next!.Name = "Erik";
                Assert.AreEqual(3, changes.Count);
                EventPatternAssert.AreEqual(collection[0], collection[0].Next, collection[0].Next, Maybe.Some<string?>("Erik"), "Name", changes.Last());

                fake.Collection.Add(fake.Collection[0]);
                Assert.AreEqual(4, changes.Count);
                EventPatternAssert.AreEqual(collection[0], collection, collection[0].Next, Maybe.Some<string?>("Erik"), string.Empty, changes.Last());

                fake.Collection[0].Next!.Name = "Max";
                Assert.AreEqual(5, changes.Count);
                EventPatternAssert.AreEqual(collection[0], collection[0].Next, collection[0].Next, Maybe.Some<string?>("Max"), "Name", changes.Last());

                fake.Collection[1].Next!.Name = "Tom";
                Assert.AreEqual(6, changes.Count);
                EventPatternAssert.AreEqual(collection[1], collection[1].Next, collection[1].Next, Maybe.Some<string?>("Tom"), "Name", changes.Last());
            }

            Assert.AreEqual(6, changes.Count);
        }

        [Test]
        public static void ReplacingCollectionWithNewWithSameItems()
        {
            var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string?>>>();
            var collection = new ObservableCollection<Fake>
            {
                new Fake { Next = new Level { Name = "1" } },
                new Fake { Next = new Level { Name = "2" } },
            };
            var fake = new FakeWithCollection { Collection = collection };
            using (fake.ObservePropertyChangedWithValue(x => x.Collection, signalInitial: true)
                       .ItemPropertyChanged(x => x.Next.Name)
                       .Subscribe(changes.Add))
            {
                Assert.AreEqual(2, changes.Count);
                EventPatternAssert.AreEqual(collection[0], collection, collection[0].Next, Maybe.Some<string?>("1"), string.Empty, changes[0]);
                EventPatternAssert.AreEqual(collection[1], collection, collection[1].Next, Maybe.Some<string?>("2"), string.Empty, changes[1]);

                fake.Collection = new ObservableCollection<Fake>(collection);
                Assert.AreEqual(6, changes.Count);
                EventPatternAssert.AreEqual(null, collection, collection[0].Next, Maybe.Some<string?>("1"), string.Empty, changes[2]);
                EventPatternAssert.AreEqual(null, collection, collection[1].Next, Maybe.Some<string?>("2"), string.Empty, changes[3]);
                EventPatternAssert.AreEqual(collection[0], fake.Collection, collection[0].Next, Maybe.Some<string?>("1"), string.Empty, changes[4]);
                EventPatternAssert.AreEqual(collection[1], fake.Collection, collection[1].Next, Maybe.Some<string?>("2"), string.Empty, changes[5]);

                collection.Add(new Fake());
                Assert.AreEqual(6, changes.Count);

                fake.Collection.Add(new Fake { Next = new Level { Name = "3" } });
                Assert.AreEqual(7, changes.Count);
                EventPatternAssert.AreEqual(fake.Collection[2], fake.Collection, fake.Collection[2].Next, Maybe.Some("3"), string.Empty, changes.Last());
            }

            Assert.AreEqual(7, changes.Count);
        }

        [Test]
        public static void ReplacingCollectionWithNewWithNewItems()
        {
            var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string?>>>();
            var collection = new ObservableCollection<Fake>
            {
                new Fake { Next = new Level { Name = "1" } },
                new Fake { Next = new Level { Name = "2" } },
            };
            var fake = new FakeWithCollection { Collection = collection };
            using (fake.ObservePropertyChangedWithValue(x => x.Collection, signalInitial: true)
                       .ItemPropertyChanged(x => x.Next.Name)
                       .Subscribe(changes.Add))
            {
                Assert.AreEqual(2, changes.Count);
                EventPatternAssert.AreEqual(collection[0], collection, collection[0].Next, Maybe.Some<string?>("1"), string.Empty, changes[0]);
                EventPatternAssert.AreEqual(collection[1], collection, collection[1].Next, Maybe.Some<string?>("2"), string.Empty, changes[1]);

                fake.Collection = new ObservableCollection<Fake>
                {
                    new Fake { Next = new Level { Name = "3" } },
                    new Fake { Next = new Level { Name = "4" } },
                };

                Assert.AreEqual(6, changes.Count);
                EventPatternAssert.AreEqual(null, collection, collection[0].Next, Maybe.Some<string?>("1"), string.Empty, changes[2]);
                EventPatternAssert.AreEqual(null, collection, collection[1].Next, Maybe.Some<string?>("2"), string.Empty, changes[3]);
                EventPatternAssert.AreEqual(fake.Collection[0], fake.Collection, fake.Collection[0].Next, Maybe.Some<string?>("3"), string.Empty, changes[4]);
                EventPatternAssert.AreEqual(fake.Collection[1], fake.Collection, fake.Collection[1].Next, Maybe.Some<string?>("4"), string.Empty, changes[5]);

                fake.Collection[0].Next!.Name = "5";
                Assert.AreEqual(7, changes.Count);
                EventPatternAssert.AreEqual(fake.Collection[0], fake.Collection[0].Next, fake.Collection[0].Next, Maybe.Some<string?>("5"), "Name", changes.Last());

                collection.Add(new Fake());
                Assert.AreEqual(7, changes.Count);

                fake.Collection.Add(new Fake { Next = new Level { Name = "5" } });
                Assert.AreEqual(8, changes.Count);
                EventPatternAssert.AreEqual(fake.Collection[2], fake.Collection, fake.Collection[2].Next, Maybe.Some<string?>("5"), string.Empty, changes.Last());
            }

            Assert.AreEqual(8, changes.Count);
        }

        [Test]
        public static void OneObservableTwoSubscriptions()
        {
            var changes1 = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string?>>>();
            var changes2 = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string?>>>();
            var fake = new FakeWithCollection();
            var observable = fake.ObservePropertyChangedWithValue(x => x.Collection)
                                 .ItemPropertyChanged(x => x.Name);
            using (observable.Subscribe(changes1.Add))
            {
                using (observable.Subscribe(changes2.Add))
                {
                    CollectionAssert.IsEmpty(changes1);
                    CollectionAssert.IsEmpty(changes2);

                    var source = new ObservableCollection<Fake> { new Fake { Name = "1" } };
                    fake.Collection = source;
                    EventPatternAssert.AreEqual(source[0], source, source[0], Maybe.Some<string?>("1"), string.Empty, changes1.Single());
                    EventPatternAssert.AreEqual(source[0], source, source[0], Maybe.Some<string?>("1"), string.Empty, changes2.Single());

                    fake.Collection.Add(new Fake { Name = "2" });

                    Assert.AreEqual(2, changes1.Count);
                    Assert.AreEqual(2, changes2.Count);
                    EventPatternAssert.AreEqual(source[1], source, source[1], Maybe.Some<string?>("2"), string.Empty, changes2.Last());
                    EventPatternAssert.AreEqual(source[1], source, source[1], Maybe.Some<string?>("2"), string.Empty, changes2.Last());

                    fake.Collection[1].Name = "3";
                    Assert.AreEqual(3, changes1.Count);
                    Assert.AreEqual(3, changes2.Count);
                    EventPatternAssert.AreEqual(source[1], source[1], source[1], Maybe.Some<string?>("3"), "Name", changes1.Last());
                    EventPatternAssert.AreEqual(source[1], source[1], source[1], Maybe.Some<string?>("3"), "Name", changes2.Last());
                }
            }
        }

        [Test]
        public static void OneObservableTwoSubscriptionsNested()
        {
            var changes1 = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string?>>>();
            var changes2 = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string?>>>();
            var collection = new ObservableCollection<Fake> { new Fake { Next = new Level { Name = "1" } } };
            var fake = new FakeWithCollection { Collection = collection };
            var observable = fake.ObservePropertyChangedWithValue(x => x.Collection, signalInitial: true)
                                 .ItemPropertyChanged(x => x.Next.Name);
            using (observable.Subscribe(changes1.Add))
            {
                using (observable.Subscribe(changes2.Add))
                {
                    fake.Collection = collection;
                    EventPatternAssert.AreEqual(collection[0], collection, collection[0].Next, Maybe.Some<string?>("1"), string.Empty, changes1.Single());
                    EventPatternAssert.AreEqual(collection[0], collection, collection[0].Next, Maybe.Some<string?>("1"), string.Empty, changes2.Single());

                    fake.Collection.Add(new Fake { Next = new Level { Name = "2" } });

                    Assert.AreEqual(2, changes1.Count);
                    Assert.AreEqual(2, changes2.Count);
                    EventPatternAssert.AreEqual(collection[1], collection, collection[1].Next, Maybe.Some<string?>("2"), string.Empty, changes1.Last());
                    EventPatternAssert.AreEqual(collection[1], collection, collection[1].Next, Maybe.Some<string?>("2"), string.Empty, changes2.Last());

                    fake.Collection[1].Next!.Name = "3";
                    Assert.AreEqual(3, changes1.Count);
                    Assert.AreEqual(3, changes2.Count);
                    EventPatternAssert.AreEqual(collection[1], collection[1].Next, collection[1].Next, Maybe.Some<string?>("3"), "Name", changes1.Last());
                    EventPatternAssert.AreEqual(collection[1], collection[1].Next, collection[1].Next, Maybe.Some<string?>("3"), "Name", changes2.Last());
                }
            }
        }

        [Test]
        public static void StopsSubscribing()
        {
            var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string?>>>();
            var collection1 = new ObservableCollection<Fake> { new Fake { Name = "1" } };
            var fake = new FakeWithCollection { Collection = collection1 };
            using (fake.ObservePropertyChangedWithValue(x => x.Collection, signalInitial: true)
                       .ItemPropertyChanged(x => x.Name)
                       .Subscribe(changes.Add))
            {
                EventPatternAssert.AreEqual(collection1[0], collection1, collection1[0], Maybe.Some<string?>("1"), string.Empty, changes.Single());

                fake.Collection = null;
                Assert.AreEqual(2, changes.Count);
                EventPatternAssert.AreEqual(null, collection1, collection1[0], Maybe.Some<string?>("1"), string.Empty, changes.Last());

                collection1[0].Name = "new";
                Assert.AreEqual(2, changes.Count);

                var collection2 = new ObservableCollection<Fake> { new Fake { Name = "1" } };
                fake.Collection = collection2;
                Assert.AreEqual(3, changes.Count);
                EventPatternAssert.AreEqual(collection2[0], collection2, collection2[0], Maybe.Some<string?>("1"), string.Empty, changes.Last());
            }
        }
    }
}
