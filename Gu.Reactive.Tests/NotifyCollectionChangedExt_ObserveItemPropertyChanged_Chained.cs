namespace Gu.Reactive.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reactive;

    using Gu.Reactive.Tests.Fakes;

    using NUnit.Framework;

    [Explicit("Implement & test this")]
    public class NotifyCollectionChangedExt_ObserveItemPropertyChanged_Chained
    {
        private List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>> _changes;

        [SetUp]
        public void SetUp()
        {
            _changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
        }

        [Test]
        public void SignalsOnNewCollection()
        {
            var fake = new FakeWithCollection();
            fake.ObservePropertyChangedWithValue(x => x.Collection)
                .ItemPropertyChanged(x => x.Name)
                .Subscribe(_changes.Add);
            CollectionAssert.IsEmpty(_changes);
            var collection = new ObservableCollection<Fake> { new Fake { Name = "Johan" } };
            fake.Collection = collection;
            Assert.AreEqual(1, _changes.Count);
            AssertRx.AreEqual("Name", collection[0], collection[0], "Johan", _changes.Last());
        }

        [Test]
        public void SignalsOnSubscribe()
        {
            var collection = new ObservableCollection<Fake> { new Fake { Name = "Johan" } };
            var fake = new FakeWithCollection { Collection = collection };
            fake.ObservePropertyChangedWithValue(x => x.Collection, true)
                .ItemPropertyChanged(x => x.Name)
                .Subscribe(_changes.Add);
            Assert.AreEqual(1, _changes.Count);
            AssertRx.AreEqual("Name", collection[0], collection[0], "Johan", _changes.Last());
        }
    }
}