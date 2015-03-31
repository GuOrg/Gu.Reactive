namespace Gu.Reactive.Tests.Internals
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reactive;

    using Gu.Reactive.Internals;

    using NUnit.Framework;

    public class NotifyingPathItemTests
    {
        private List<EventPattern<PropertyChangedEventArgs>> _changes;



        [SetUp]
        public void SetUp()
        {
            _changes = new List<EventPattern<PropertyChangedEventArgs>>();
        }

        [Test]
        public void ThrowsOnWriteOnly()
        {
            var propertyInfo = typeof(FakeInpc).GetProperty("WriteOnly");
            Assert.Throws<ArgumentException>(() => new NotifyingPathItem(null, propertyInfo));
        }

        [Test]
        public void ThrowsOnSettingSourceToWrongType()
        {
            var propertyName = NameOf.Property<FakeInpc>(x => x.IsTrue);
            var propertyInfo = typeof(FakeInpc).GetProperty(propertyName);
            var pathItem = new NotifyingPathItem(null, propertyInfo);
            Assert.Throws<InvalidOperationException>(() => pathItem.Source = new StructLevel());
        }

        [Test]
        public void NotifiesOnNewSourceAffectingProp()
        {
            var propertyName = NameOf.Property<FakeInpc>(x => x.IsTrue);
            var propertyInfo = typeof(FakeInpc).GetProperty(propertyName);
            var fakeInpc = new FakeInpc();
            var pathItem = new NotifyingPathItem(null, propertyInfo);
            pathItem.ObservePropertyChanged().Subscribe(_changes.Add);
            Assert.AreEqual(0, _changes.Count);
            pathItem.Source = fakeInpc;
            Assert.AreEqual(1, _changes.Count);
            Assert.AreEqual(propertyName, _changes.Single().EventArgs.PropertyName);
            Assert.AreSame(fakeInpc, _changes.Single().Sender);
        }

        [Test]
        public void NotifiesOnSourceChangeAffectingPropByBeingSetToNull()
        {
            var propertyName = NameOf.Property<FakeInpc>(x => x.IsTrue);
            var propertyInfo = typeof(FakeInpc).GetProperty(propertyName);
            var fakeInpc = new FakeInpc();
            var pathItem = new NotifyingPathItem(null, propertyInfo);
            pathItem.Source = fakeInpc;
            pathItem.ObservePropertyChanged().Subscribe(_changes.Add);
            Assert.AreEqual(0, _changes.Count);
            pathItem.Source = null;
            Assert.AreEqual(1, _changes.Count);
            Assert.AreEqual(propertyName, _changes.Single().EventArgs.PropertyName);
            Assert.AreSame(null, _changes.Single().Sender);
        }

        [Test]
        public void DoesNotNotifyOnNewSourceWhenPropGoesFromNullToNull()
        {
            var propertyName = NameOf.Property<FakeInpc>(x => x.Name);
            var propertyInfo = typeof(FakeInpc).GetProperty(propertyName);
            var fakeInpc = new FakeInpc();
            var pathItem = new NotifyingPathItem(null, propertyInfo);
            pathItem.ObservePropertyChanged().Subscribe(_changes.Add);
            Assert.AreEqual(0, _changes.Count);
            pathItem.Source = fakeInpc;
            Assert.AreEqual(0, _changes.Count);
        }

        [Test]
        public void DoesNotNotifyOnNewNullSourceWhenPropGoesFromNullToNull()
        {
            var propertyName = NameOf.Property<FakeInpc>(x => x.Name);
            var propertyInfo = typeof(FakeInpc).GetProperty(propertyName);
            var fakeInpc = new FakeInpc();
            var pathItem = new NotifyingPathItem(null, propertyInfo);
            pathItem.ObservePropertyChanged().Subscribe(_changes.Add);
            Assert.AreEqual(0, _changes.Count);
            pathItem.Source = null;
            Assert.AreEqual(0, _changes.Count);
        }

        [Test]
        public void NotifiesOnNewSourceWhenPropGoesFromTrueToTrue()
        {
            var propertyName = NameOf.Property<FakeInpc>(x => x.IsTrue);
            var propertyInfo = typeof(FakeInpc).GetProperty(propertyName);
            var fakeInpc = new FakeInpc { IsTrue = true };
            var pathItem = new NotifyingPathItem(null, propertyInfo);
            pathItem.ObservePropertyChanged().Subscribe(_changes.Add);
            Assert.AreEqual(0, _changes.Count);
            pathItem.Source = fakeInpc;
            Assert.AreEqual(1, _changes.Count);
            pathItem.Source = new FakeInpc { IsTrue = true };
            Assert.AreEqual(2, _changes.Count);
        }

        [Test]
        public void NotifiesOnSourceChanged()
        {
            var propertyName = NameOf.Property<FakeInpc>(x => x.IsTrue);
            var propertyInfo = typeof(FakeInpc).GetProperty(propertyName);
            var fakeInpc = new FakeInpc();
            var pathItem = new NotifyingPathItem(null, propertyInfo);
            pathItem.Source = fakeInpc;
            pathItem.ObservePropertyChanged().Subscribe(_changes.Add);
            Assert.AreEqual(0, _changes.Count);
            fakeInpc.IsTrue = !fakeInpc.IsTrue;
            Assert.AreEqual(1, _changes.Count);
            Assert.AreEqual(propertyName, _changes.Single().EventArgs.PropertyName);
            Assert.AreSame(fakeInpc, _changes.Single().Sender);
        }

        [TestCase("",Description = "Wpf uses string.empty or null to mean all properties changed")]
        [TestCase(null)]
        [TestCase("IsTrue")]
        public void NotifiesOnSourcePropertyChangedEvent(string eventArgsPropName)
        {
            var propertyName = NameOf.Property<FakeInpc>(x => x.IsTrue);
            var propertyInfo = typeof(FakeInpc).GetProperty(propertyName);
            var fakeInpc = new FakeInpc();
            var pathItem = new NotifyingPathItem(null, propertyInfo);
            pathItem.Source = fakeInpc;
            pathItem.ObservePropertyChanged().Subscribe(_changes.Add);
            Assert.AreEqual(0, _changes.Count);
            fakeInpc.OnPropertyChanged(eventArgsPropName);
            Assert.AreEqual(1, _changes.Count);
            Assert.AreEqual(eventArgsPropName, _changes.Single().EventArgs.PropertyName);
            Assert.AreSame(fakeInpc, _changes.Single().Sender);
        }

        [Test]
        public void UpdatesNextSourceOnSourceChange()
        {
            var nextName = NameOf.Property<FakeInpc>(x => x.Next);
            var nextProp = typeof(FakeInpc).GetProperty(nextName);
            var first = new NotifyingPathItem(null, nextProp);

            var isTrueName = NameOf.Property<Level>(x => x.IsTrue);
            var isTrueProp = typeof(Level).GetProperty(isTrueName);
            var second = new NotifyingPathItem(first, isTrueProp);

            var fakeInpc = new FakeInpc { Next = new Level() };
            first.Source = fakeInpc;
            Assert.AreSame(fakeInpc.Next, second.Source);
        }

        [Test]
        public void UpdatesNextSourceOnPropertyChange()
        {
            var nextName = NameOf.Property<FakeInpc>(x => x.Next);
            var nextProp = typeof(FakeInpc).GetProperty(nextName);
            var first = new NotifyingPathItem(null, nextProp);

            var isTrueName = NameOf.Property<Level>(x => x.IsTrue);
            var isTrueProp = typeof(Level).GetProperty(isTrueName);
            var second = new NotifyingPathItem(first, isTrueProp);

            var fakeInpc = new FakeInpc { Next = new Level { Name = "1" } };
            first.Source = fakeInpc;
            Assert.AreSame(fakeInpc.Next, second.Source);
            fakeInpc.Next = new Level { Name = "2" };
            Assert.AreSame(fakeInpc.Next, second.Source);
        }
    }
}
