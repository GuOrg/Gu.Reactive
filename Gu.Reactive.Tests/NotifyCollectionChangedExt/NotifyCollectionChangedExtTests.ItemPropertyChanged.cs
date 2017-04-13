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

    public partial class NotifyCollectionChangedExtTests
    {
        public class ItemPropertyChanged
        {
            [Test]
            public void SignalsOnNewCollection()
            {
                var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
                var fake = new FakeWithCollection();
                using (fake.ObservePropertyChangedWithValue(x => x.Collection)
                           .ItemPropertyChanged(x => x.Name)
                           .Subscribe(changes.Add))
                {
                    CollectionAssert.IsEmpty(changes);
                    var collection = new ObservableCollection<Fake> { new Fake { Name = "Johan" } };
                    fake.Collection = collection;
                    EventPatternAssert.AreEqual(collection[0], collection[0], collection[0], Maybe.Some("Johan"), string.Empty, changes.Single());
                }
            }

            [Test]
            public void SignalsInitial()
            {
                var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
                var collection = new ObservableCollection<Fake> { new Fake { Name = "Johan" } };
                var fake = new FakeWithCollection { Collection = collection };
                using (fake.ObservePropertyChangedWithValue(x => x.Collection, signalInitial: true)
                           .ItemPropertyChanged(x => x.Name)
                           .Subscribe(changes.Add))
                {
                    EventPatternAssert.AreEqual(collection[0], collection[0], collection[0], Maybe.Some("Johan"), string.Empty, changes.Single());
                }
            }

            [Test]
            public void ReactsNested()
            {
                var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
                var collection = new ObservableCollection<Fake>
                                     {
                                         new Fake { Next = new Level { Name = "Johan" } },
                                         new Fake { Next = new Level { Name = "Reed" } }
                                     };
                var fake = new FakeWithCollection { Collection = collection };
                using (fake.ObservePropertyChangedWithValue(x => x.Collection, signalInitial: true)
                           .ItemPropertyChanged(x => x.Next.Name)
                           .Subscribe(changes.Add))
                {
                    Assert.AreEqual(2, changes.Count);
                    EventPatternAssert.AreEqual(collection[0], collection[0].Next, collection[0].Next, Maybe.Some("Johan"), string.Empty, changes[0]);
                    EventPatternAssert.AreEqual(collection[1], collection[1].Next, collection[1].Next, Maybe.Some("Reed"), string.Empty, changes[1]);

                    fake.Collection[0].Next.Name = "Erik";
                    Assert.AreEqual(3, changes.Count);
                    EventPatternAssert.AreEqual(collection[0], collection[0].Next, collection[0].Next, Maybe.Some("Erik"), "Name", changes.Last());

                    fake.Collection.Add(fake.Collection[0]);
                    Assert.AreEqual(3, changes.Count);

                    fake.Collection[0].Next.Name = "Max";
                    Assert.AreEqual(4, changes.Count);
                    EventPatternAssert.AreEqual(collection[0], collection[0].Next, collection[0].Next, Maybe.Some("Max"), "Name", changes.Last());

                    fake.Collection[1].Next.Name = "Tom";
                    Assert.AreEqual(5, changes.Count);
                    EventPatternAssert.AreEqual(collection[1], collection[1].Next, collection[1].Next, Maybe.Some("Tom"), "Name", changes.Last());
                }

                Assert.AreEqual(5, changes.Count);
            }

            [Test]
            public void ReplacingCollectionWithNewWithSameItems()
            {
                var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
                var collection = new ObservableCollection<Fake>
                                     {
                                         new Fake { Next = new Level { Name = "Johan" } },
                                         new Fake { Next = new Level { Name = "Reed" } }
                                     };
                var fake = new FakeWithCollection { Collection = collection };
                using (fake.ObservePropertyChangedWithValue(x => x.Collection, signalInitial: true)
                           .ItemPropertyChanged(x => x.Next.Name)
                           .Subscribe(changes.Add))
                {
                    Assert.AreEqual(2, changes.Count);
                    EventPatternAssert.AreEqual(collection[0], collection[0].Next, collection[0].Next, Maybe.Some("Johan"), string.Empty, changes[0]);
                    EventPatternAssert.AreEqual(collection[1], collection[1].Next, collection[1].Next, Maybe.Some("Reed"), string.Empty, changes[1]);

                    fake.Collection = new ObservableCollection<Fake>(collection);
                    Assert.AreEqual(2, changes.Count);

                    collection.Add(new Fake());
                    Assert.AreEqual(2, changes.Count);
                }

                Assert.AreEqual(2, changes.Count);
            }

            [Test]
            public void ReplacingCollectionWithNewWithNewItems()
            {
                var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
                var collection = new ObservableCollection<Fake>
                                     {
                                         new Fake { Next = new Level { Name = "1" } },
                                         new Fake { Next = new Level { Name = "2" } }
                                     };
                var fake = new FakeWithCollection { Collection = collection };
                using (fake.ObservePropertyChangedWithValue(x => x.Collection, signalInitial: true)
                           .ItemPropertyChanged(x => x.Next.Name)
                           .Subscribe(changes.Add))
                {
                    Assert.AreEqual(2, changes.Count);
                    EventPatternAssert.AreEqual(collection[0], collection[0].Next, collection[0].Next, Maybe.Some("1"), string.Empty, changes[0]);
                    EventPatternAssert.AreEqual(collection[1], collection[1].Next, collection[1].Next, Maybe.Some("2"), string.Empty, changes[1]);

                    fake.Collection = new ObservableCollection<Fake>
                                     {
                                         new Fake { Next = new Level { Name = "3" } },
                                         new Fake { Next = new Level { Name = "4" } }
                                     };

                    Assert.AreEqual(4, changes.Count);
                    EventPatternAssert.AreEqual(fake.Collection[0], fake.Collection[0].Next, fake.Collection[0].Next, Maybe.Some("3"), string.Empty, changes[2]);
                    EventPatternAssert.AreEqual(fake.Collection[1], fake.Collection[1].Next, fake.Collection[1].Next, Maybe.Some("4"), string.Empty, changes[3]);

                    fake.Collection[0].Next.Name = "5";
                    Assert.AreEqual(5, changes.Count);
                    EventPatternAssert.AreEqual(fake.Collection[0], fake.Collection[0].Next, fake.Collection[0].Next, Maybe.Some("5"), "Name", changes[4]);

                    collection.Add(new Fake());
                    Assert.AreEqual(5, changes.Count);
                }

                Assert.AreEqual(5, changes.Count);
            }

            [Test]
            public void OneObservableTwoSubscriptions()
            {
                var changes1 = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
                var changes2 = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
                var fake = new FakeWithCollection();
                var observable = fake.ObservePropertyChangedWithValue(x => x.Collection)
                                     .ItemPropertyChanged(x => x.Name);
                using (observable.Subscribe(changes1.Add))
                {
                    using (observable.Subscribe(changes2.Add))
                    {
                        CollectionAssert.IsEmpty(changes1);
                        CollectionAssert.IsEmpty(changes2);

                        var collection = new ObservableCollection<Fake> { new Fake { Name = "Johan" } };
                        fake.Collection = collection;
                        EventPatternAssert.AreEqual(collection[0], collection[0], collection[0], Maybe.Some("Johan"), string.Empty, changes1.Single());
                        EventPatternAssert.AreEqual(collection[0], collection[0], collection[0], Maybe.Some("Johan"), string.Empty, changes2.Single());

                        fake.Collection.Add(new Fake { Name = "Erik" });

                        Assert.AreEqual(2, changes1.Count);
                        Assert.AreEqual(2, changes2.Count);
                        EventPatternAssert.AreEqual(collection[1], collection[1], collection[1], Maybe.Some("Erik"), string.Empty, changes1.Last());
                        EventPatternAssert.AreEqual(collection[1], collection[1], collection[1], Maybe.Some("Erik"), string.Empty, changes2.Last());

                        fake.Collection[1].Name = "Max";
                        Assert.AreEqual(3, changes1.Count);
                        Assert.AreEqual(3, changes2.Count);
                        EventPatternAssert.AreEqual(collection[1], collection[1], collection[1], Maybe.Some("Max"), "Name", changes1.Last());
                        EventPatternAssert.AreEqual(collection[1], collection[1], collection[1], Maybe.Some("Max"), "Name", changes2.Last());
                    }
                }
            }

            [Test]
            public void OneObservableTwoSubscriptionsNested()
            {
                var changes1 = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
                var changes2 = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
                var collection = new ObservableCollection<Fake> { new Fake { Next = new Level { Name = "Johan" } } };
                var fake = new FakeWithCollection { Collection = collection };
                var observable = fake.ObservePropertyChangedWithValue(x => x.Collection, signalInitial: true)
                                     .ItemPropertyChanged(x => x.Next.Name);
                using (observable.Subscribe(changes1.Add))
                {
                    using (observable.Subscribe(changes2.Add))
                    {
                        fake.Collection = collection;
                        EventPatternAssert.AreEqual(collection[0], collection[0].Next, collection[0].Next, Maybe.Some("Johan"), string.Empty, changes1.Single());
                        EventPatternAssert.AreEqual(collection[0], collection[0].Next, collection[0].Next, Maybe.Some("Johan"), string.Empty, changes2.Single());

                        fake.Collection.Add(new Fake { Next = new Level { Name = "Erik" } });

                        Assert.AreEqual(2, changes1.Count);
                        Assert.AreEqual(2, changes2.Count);
                        EventPatternAssert.AreEqual(collection[1], collection[1].Next, collection[1].Next, Maybe.Some("Erik"), string.Empty, changes1.Last());
                        EventPatternAssert.AreEqual(collection[1], collection[1].Next, collection[1].Next, Maybe.Some("Erik"), string.Empty, changes2.Last());

                        fake.Collection[1].Next.Name = "Max";
                        Assert.AreEqual(3, changes1.Count);
                        Assert.AreEqual(3, changes2.Count);
                        EventPatternAssert.AreEqual(collection[1], collection[1].Next, collection[1].Next, Maybe.Some("Max"), "Name", changes1.Last());
                        EventPatternAssert.AreEqual(collection[1], collection[1].Next, collection[1].Next, Maybe.Some("Max"), "Name", changes2.Last());
                    }
                }
            }

            [Test]
            public void StopsSubscribing()
            {
                var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
                var collection1 = new ObservableCollection<Fake> { new Fake { Name = "Johan" } };
                var fake = new FakeWithCollection { Collection = collection1 };
                using (fake.ObservePropertyChangedWithValue(x => x.Collection, signalInitial: true)
                           .ItemPropertyChanged(x => x.Name)
                           .Subscribe(changes.Add))
                {
                    EventPatternAssert.AreEqual(collection1[0], collection1[0], collection1[0], Maybe.Some("Johan"), string.Empty, changes.Single());

                    fake.Collection = null;
                    Assert.AreEqual(1, changes.Count);

                    collection1[0].Name = "new";
                    Assert.AreEqual(1, changes.Count);

                    var collection2 = new ObservableCollection<Fake> { new Fake { Name = "Johan" } };
                    fake.Collection = collection2;
                    Assert.AreEqual(2, changes.Count);
                    EventPatternAssert.AreEqual(collection2[0], collection2[0], collection2[0], Maybe.Some("Johan"), string.Empty, changes.Last());
                }
            }
        }
    }
}