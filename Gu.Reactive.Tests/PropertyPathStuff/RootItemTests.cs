namespace Gu.Reactive.Tests.PropertyPathStuff
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reactive;

    using Gu.Reactive.PropertyPathStuff;

    using NUnit.Framework;

    public class RootItemTests
    {
        private List<EventPattern<PropertyChangedEventArgs>> changes;

        [SetUp]
        public void SetUp()
        {
            this.changes = new List<EventPattern<PropertyChangedEventArgs>>();
        }

        [Test]
        public void SignalsValue()
        {
            var item = new RootItem(null);
            item.ObservePropertyChanged(x => x.Value, false)
                .Subscribe(this.changes.Add);
            Assert.AreEqual(0, this.changes.Count);
            item.Value = new object();
            Assert.AreEqual(1, this.changes.Count);
        }

        [Test]
        public void Signals()
        {
            int count = 0;
            var item = new RootItem(null);
            item.ObservePropertyChanged(x => x.Source, false)
                .Subscribe(_ => count++);
            Assert.AreEqual(0, count);
            item.Value = new object();
            Assert.AreEqual(1, count);
        }
    }
}
