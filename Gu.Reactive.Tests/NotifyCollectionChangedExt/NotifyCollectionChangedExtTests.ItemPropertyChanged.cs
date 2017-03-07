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
                    Assert.AreEqual(1, changes.Count);
                    EventPatternAssert.AreEqual(collection[0], string.Empty, collection[0], "Johan", changes.Last());
                }
            }

            [Test]
            public void SignalsOnSubscribe()
            {
                var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
                var collection = new ObservableCollection<Fake> { new Fake { Name = "Johan" } };
                var fake = new FakeWithCollection { Collection = collection };
                using (fake.ObservePropertyChangedWithValue(x => x.Collection, true)
                           .ItemPropertyChanged(x => x.Name)
                           .Subscribe(changes.Add))
                {
                    Assert.AreEqual(1, changes.Count);
                    EventPatternAssert.AreEqual(collection[0], string.Empty, collection[0], "Johan", changes.Last());
                }
            }

            [Test]
            public void ReactsNested()
            {
                var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
                var collection = new ObservableCollection<Fake> { new Fake { Next = new Level { Name = "Johan" } } };
                var fake = new FakeWithCollection { Collection = collection };
                using (fake.ObservePropertyChangedWithValue(x => x.Collection, true)
                           .ItemPropertyChanged(x => x.Next.Name)
                           .Subscribe(changes.Add))
                {
                    Assert.AreEqual(1, changes.Count);
                    EventPatternAssert.AreEqual(collection[0].Next, string.Empty, collection[0], "Johan", changes.Last());

                    fake.Collection[0].Next.Name = "Erik";
                    Assert.AreEqual(2, changes.Count);
                    EventPatternAssert.AreEqual(collection[0].Next, "Name", collection[0], "Erik", changes.Last());

                    fake.Collection.Add(fake.Collection[0]);
                    Assert.AreEqual(2, changes.Count);

                    fake.Collection[0].Next.Name = "Max";
                    Assert.AreEqual(3, changes.Count);
                    EventPatternAssert.AreEqual(collection[0].Next, "Name", collection[0], "Max", changes.Last());
                }
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
                        var collection = new ObservableCollection<Fake> { new Fake { Name = "Johan" } };
                        fake.Collection = collection;
                        Assert.AreEqual(1, changes1.Count);
                        Assert.AreEqual(1, changes2.Count);
                        EventPatternAssert.AreEqual(collection[0], string.Empty, collection[0], "Johan", changes1.Last());
                        EventPatternAssert.AreEqual(collection[0], string.Empty, collection[0], "Johan", changes2.Last());

                        fake.Collection.Add(new Fake { Name = "Erik" });

                        Assert.AreEqual(2, changes1.Count);
                        Assert.AreEqual(2, changes2.Count);
                        EventPatternAssert.AreEqual(collection[1], string.Empty, collection[1], "Erik", changes1.Last());
                        EventPatternAssert.AreEqual(collection[1], string.Empty, collection[1], "Erik", changes2.Last());

                        fake.Collection[1].Name = "Max";
                        Assert.AreEqual(3, changes1.Count);
                        Assert.AreEqual(3, changes2.Count);
                        EventPatternAssert.AreEqual(collection[1], "Name", collection[1], "Max", changes1.Last());
                        EventPatternAssert.AreEqual(collection[1], "Name", collection[1], "Max", changes2.Last());
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
                var observable = fake.ObservePropertyChangedWithValue(x => x.Collection, true)
                                     .ItemPropertyChanged(x => x.Next.Name);
                using (observable.Subscribe(changes1.Add))
                {
                    using (observable.Subscribe(changes2.Add))
                    {
                        Assert.AreEqual(1, changes1.Count);
                        Assert.AreEqual(1, changes2.Count);
                        EventPatternAssert.AreEqual(collection[0].Next, string.Empty, collection[0], "Johan", changes1.Last());
                        EventPatternAssert.AreEqual(collection[0].Next, string.Empty, collection[0], "Johan", changes2.Last());

                        collection[0].Next.Name = "Erik";
                        Assert.AreEqual(2, changes1.Count);
                        Assert.AreEqual(2, changes2.Count);
                        EventPatternAssert.AreEqual(collection[0].Next, "Name", collection[0], "Erik", changes1.Last());
                        EventPatternAssert.AreEqual(collection[0].Next, "Name", collection[0], "Erik", changes2.Last());
                    }
                }
            }

            [Test]
            public void StopsSubscribing()
            {
                var changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
                var collection1 = new ObservableCollection<Fake> { new Fake { Name = "Johan" } };
                var fake = new FakeWithCollection { Collection = collection1 };
                using (fake.ObservePropertyChangedWithValue(x => x.Collection, true)
                           .ItemPropertyChanged(x => x.Name)
                           .Subscribe(changes.Add))
                {
                    Assert.AreEqual(1, changes.Count);
                    EventPatternAssert.AreEqual(collection1[0], string.Empty, collection1[0], "Johan", changes.Last());

                    fake.Collection = null;
                    Assert.AreEqual(1, changes.Count);
                    collection1[0].Name = "new";
                    Assert.AreEqual(1, changes.Count);

                    var collection2 = new ObservableCollection<Fake> { new Fake { Name = "Kurt" } };
                    fake.Collection = collection2;
                    Assert.AreEqual(2, changes.Count);
                    EventPatternAssert.AreEqual(collection2[0], string.Empty, collection2[0], "Kurt", changes.Last());
                }
            }
        }
    }
}