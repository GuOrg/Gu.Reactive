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
    [TestFixture]
    public class ObserveItemPropertyChanged_ChainedTests
    {
        private List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>> changes;

        [SetUp]
        public void SetUp()
        {
            this.changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
        }

        [Test]
        public void SignalsOnNewCollection()
        {
            var fake = new FakeWithCollection();
            fake.ObservePropertyChangedWithValue(x => x.Collection)
                .ItemPropertyChanged(x => x.Name)
                .Subscribe(this.changes.Add);
            CollectionAssert.IsEmpty(this.changes);
            var collection = new ObservableCollection<Fake> { new Fake { Name = "Johan" } };
            fake.Collection = collection;
            Assert.AreEqual(1, this.changes.Count);
            AssertRx.AreEqual(collection[0], "Name", collection[0], "Johan", this.changes.Last());
        }

        [Test]
        public void SignalsOnSubscribe()
        {
            var collection = new ObservableCollection<Fake> { new Fake { Name = "Johan" } };
            var fake = new FakeWithCollection { Collection = collection };
            fake.ObservePropertyChangedWithValue(x => x.Collection, true)
                .ItemPropertyChanged(x => x.Name)
                            .Subscribe(this.changes.Add);
            Assert.AreEqual(1, this.changes.Count);
            AssertRx.AreEqual(collection[0], "Name", collection[0], "Johan", this.changes.Last());
        }

        [Test]
        public void ReactsNested()
        {
            var collection = new ObservableCollection<Fake> { new Fake { Next = new Level { Name = "Johan" } } };
            var fake = new FakeWithCollection { Collection = collection };
            fake.ObservePropertyChangedWithValue(x => x.Collection, true)
                .ItemPropertyChanged(x => x.Next.Name)
                .Subscribe(this.changes.Add);
            Assert.AreEqual(1, this.changes.Count);
            AssertRx.AreEqual(collection[0].Next, "Name", collection[0], "Johan", this.changes.Last());
        }

        [Test]
        public void StopsSubscribing()
        {
            var collection1 = new ObservableCollection<Fake> { new Fake { Name = "Johan" } };
            var fake = new FakeWithCollection { Collection = collection1 };
            fake.ObservePropertyChangedWithValue(x => x.Collection, true)
                .ItemPropertyChanged(x => x.Name)
                .Subscribe(this.changes.Add);
            Assert.AreEqual(1, this.changes.Count);
            AssertRx.AreEqual(collection1[0], "Name", collection1[0], "Johan", this.changes.Last());

            fake.Collection = null;
            Assert.AreEqual(1, this.changes.Count);
            collection1[0].Name = "new";
            Assert.AreEqual(1, this.changes.Count);

            var collection2 = new ObservableCollection<Fake> { new Fake { Name = "Kurt" } };
            fake.Collection = collection2;
            Assert.AreEqual(2, this.changes.Count);
            AssertRx.AreEqual(collection2[0], "Name", collection2[0], "Kurt", this.changes.Last());
        }
    }
}