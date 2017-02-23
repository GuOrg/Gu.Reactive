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
    public class ObserveItemPropertyChanged_ChainedTests
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
                AssertRx.AreEqual(collection[0], "Name", collection[0], "Johan", changes.Last());
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
                AssertRx.AreEqual(collection[0], "Name", collection[0], "Johan", changes.Last());
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
                AssertRx.AreEqual(collection[0].Next, "Name", collection[0], "Johan", changes.Last());
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
                    AssertRx.AreEqual(collection[0], "Name", collection[0], "Johan", changes1.Last());
                    AssertRx.AreEqual(collection[0], "Name", collection[0], "Johan", changes2.Last());
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
                    AssertRx.AreEqual(collection[0].Next, "Name", collection[0], "Johan", changes1.Last());
                    AssertRx.AreEqual(collection[0].Next, "Name", collection[0], "Johan", changes2.Last());

                    collection[0].Next.Name = "Erik";
                    Assert.AreEqual(2, changes1.Count);
                    Assert.AreEqual(2, changes2.Count);
                    AssertRx.AreEqual(collection[0].Next, "Name", collection[0], "Erik", changes1.Last());
                    AssertRx.AreEqual(collection[0].Next, "Name", collection[0], "Erik", changes2.Last());
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
                AssertRx.AreEqual(collection1[0], "Name", collection1[0], "Johan", changes.Last());

                fake.Collection = null;
                Assert.AreEqual(1, changes.Count);
                collection1[0].Name = "new";
                Assert.AreEqual(1, changes.Count);

                var collection2 = new ObservableCollection<Fake> { new Fake { Name = "Kurt" } };
                fake.Collection = collection2;
                Assert.AreEqual(2, changes.Count);
                AssertRx.AreEqual(collection2[0], "Name", collection2[0], "Kurt", changes.Last());
            }
        }
    }
}