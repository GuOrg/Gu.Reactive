namespace Gu.Reactive.Tests.Internals
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reactive;

    using NUnit.Framework;
    using PropertyPathStuff;

    public class RootItemTests
    {
        private List<EventPattern<PropertyChangedEventArgs>> _changes;

        [SetUp]
        public void SetUp()
        {
            _changes = new List<EventPattern<PropertyChangedEventArgs>>();
        }

        [Test]
        public void SignalsValue()
        {
            var item = new RootItem(null);
            item.ObservePropertyChanged(x => x.Value, false)
                .Subscribe(_changes.Add);
            Assert.AreEqual(0, _changes.Count);
            item.Value = new object();
            Assert.AreEqual(1, _changes.Count);
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
