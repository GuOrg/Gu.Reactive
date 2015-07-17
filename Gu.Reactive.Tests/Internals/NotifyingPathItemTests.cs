namespace Gu.Reactive.Tests.Internals
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reactive;
    using System.Reflection;

    using Gu.Reactive.Internals;
    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;
    using PropertyPathStuff;

    public class NotifyingPathItemTests
    {
        private List<EventPattern<PropertyChangedEventArgs>> _changes;

        [SetUp]
        public void SetUp()
        {
            _changes = new List<EventPattern<PropertyChangedEventArgs>>();
        }

        [Test]
        public void ThrowsOnStruct()
        {
            var propertyInfo = typeof(StructLevel).GetProperty(NameOf.Property<StructLevel>(x => x.Name));
            Assert.NotNull(propertyInfo);
            var pathItem = new PathProperty(null, propertyInfo);
            var item = new NotifyingPathItem(null, pathItem);
            Assert.Throws<ArgumentException>(() => new NotifyingPathItem(item, pathItem));
        }

        [Test]
        public void ThrowsOnNotINotifyPropertyChanged()
        {
            var propertyInfo = typeof(NotInpc).GetProperty(NameOf.Property<NotInpc>(x => x.Name));
            Assert.NotNull(propertyInfo);
            var pathItem = new PathProperty(null, propertyInfo);
            var item = new NotifyingPathItem(null, pathItem);
            Assert.Throws<ArgumentException>(() => new NotifyingPathItem(item, pathItem));
        }

        [Test]
        public void ThrowsOnSettingSourceToWrongType()
        {
            var propertyName = NameOf.Property<Fake>(x => x.IsTrue);
            var propertyInfo = typeof(Fake).GetProperty(propertyName);
            var pathItem = new NotifyingPathItem(null, new PathProperty(null, propertyInfo));
            Assert.Throws<TargetException>(() => pathItem.Source = new StructLevel());
        }

        [Test]
        public void SetSourceToCorrectType()
        {
            var propertyName = NameOf.Property<Fake>(x => x.IsTrue);
            var propertyInfo = typeof(Fake).GetProperty(propertyName);
            var fakeInpc = new Fake();
            var pathItem = new NotifyingPathItem(null, new PathProperty(null, propertyInfo));
            pathItem.Source = fakeInpc;
            Assert.AreSame(fakeInpc, pathItem.Source); // Really just scheking that we don't throw here
        }

        [Test]
        public void SetSourceToSubtype()
        {
            var propertyName = NameOf.Property<IFake>(x => x.IsTrue);
            var propertyInfo = typeof(IFake).GetProperty(propertyName);
            var fakeInpc = new Fake();
            var pathItem = new NotifyingPathItem(null, new PathProperty(null, propertyInfo));
            pathItem.Source = fakeInpc;
            Assert.AreSame(fakeInpc, pathItem.Source); // Really just scheking that we don't throw here
        }

        [Test]
        public void SetSourceToIncorrectTypeThrows()
        {
            var propertyName = NameOf.Property<Fake>(x => x.IsTrue);
            var propertyInfo = typeof(Fake).GetProperty(propertyName);
            var level = new Level();
            var pathItem = new NotifyingPathItem(null, new PathProperty(null, propertyInfo));
            Assert.Throws<TargetException>(() => pathItem.Source = level);
        }

        [Test]
        public void NotifiesOnNewSourceAffectingProp()
        {
            var propertyName = NameOf.Property<Fake>(x => x.IsTrue);
            var propertyInfo = typeof(Fake).GetProperty(propertyName);
            var fakeInpc = new Fake();
            var pathItem = new NotifyingPathItem(null, new PathProperty(null, propertyInfo));
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
            var propertyName = NameOf.Property<Fake>(x => x.IsTrue);
            var propertyInfo = typeof(Fake).GetProperty(propertyName);
            var fakeInpc = new Fake();
            var pathItem = new NotifyingPathItem(null, new PathProperty(null, propertyInfo));
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
            var propertyName = NameOf.Property<Fake>(x => x.Name);
            var propertyInfo = typeof(Fake).GetProperty(propertyName);
            var fakeInpc = new Fake();
            var pathItem = new NotifyingPathItem(null, new PathProperty(null, propertyInfo));
            pathItem.ObservePropertyChanged().Subscribe(_changes.Add);
            Assert.AreEqual(0, _changes.Count);
            pathItem.Source = fakeInpc;
            Assert.AreEqual(0, _changes.Count);
        }

        [Test]
        public void DoesNotNotifyOnNewNullSourceWhenPropGoesFromNullToNull()
        {
            var propertyName = NameOf.Property<Fake>(x => x.Name);
            var propertyInfo = typeof(Fake).GetProperty(propertyName);
            var fakeInpc = new Fake();
            var pathItem = new NotifyingPathItem(null, new PathProperty(null, propertyInfo));
            pathItem.ObservePropertyChanged().Subscribe(_changes.Add);
            Assert.AreEqual(0, _changes.Count);
            pathItem.Source = null;
            Assert.AreEqual(0, _changes.Count);
        }

        [Test]
        public void NotifiesOnNewSourceWhenPropGoesFromTrueToTrue()
        {
            var propertyName = NameOf.Property<Fake>(x => x.IsTrue);
            var propertyInfo = typeof(Fake).GetProperty(propertyName);
            var fakeInpc = new Fake { IsTrue = true };
            var pathItem = new NotifyingPathItem(null, new PathProperty(null, propertyInfo));
            pathItem.ObservePropertyChanged().Subscribe(_changes.Add);
            Assert.AreEqual(0, _changes.Count);
            pathItem.Source = fakeInpc;
            Assert.AreEqual(1, _changes.Count);
            pathItem.Source = new Fake { IsTrue = true };
            Assert.AreEqual(2, _changes.Count);
        }

        [Test]
        public void NotifiesOnSourceChanged()
        {
            var propertyName = NameOf.Property<Fake>(x => x.IsTrue);
            var propertyInfo = typeof(Fake).GetProperty(propertyName);
            var fakeInpc = new Fake();
            var pathItem = new NotifyingPathItem(null, new PathProperty(null, propertyInfo));
            pathItem.Source = fakeInpc;
            pathItem.ObservePropertyChanged().Subscribe(_changes.Add);
            Assert.AreEqual(0, _changes.Count);
            fakeInpc.IsTrue = !fakeInpc.IsTrue;
            Assert.AreEqual(1, _changes.Count);
            Assert.AreEqual(propertyName, _changes.Single().EventArgs.PropertyName);
            Assert.AreSame(fakeInpc, _changes.Single().Sender);
        }

        [TestCase("", Description = "Wpf uses string.empty or null to mean all properties changed")]
        [TestCase(null)]
        [TestCase("IsTrue")]
        public void NotifiesOnSourcePropertyChangedEvent(string eventArgsPropName)
        {
            var propertyName = NameOf.Property<Fake>(x => x.IsTrue);
            var propertyInfo = typeof(Fake).GetProperty(propertyName);
            var fakeInpc = new Fake();
            var pathItem = new NotifyingPathItem(null, new PathProperty(null, propertyInfo));
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
            var fakeInpc = new Fake { Next = new Level { Name = "1" } };
            var rootItem = new RootItem(fakeInpc);
            var nextName = NameOf.Property<Fake>(x => x.Next);
            var nextProp = typeof(Fake).GetProperty(nextName);
            var firstPathItem = new PathProperty(null, nextProp);
            var first = new NotifyingPathItem(rootItem, firstPathItem);

            var isTrueName = NameOf.Property<Level>(x => x.IsTrue);
            var isTrueProp = typeof(Level).GetProperty(isTrueName);
            var second = new NotifyingPathItem(first, new PathProperty(firstPathItem, isTrueProp));

            first.Source = fakeInpc;

            Assert.AreSame(fakeInpc.Next, second.Source);
        }

        [Test]
        public void UpdatesNextSourceOnPropertyChange()
        {
            var fakeInpc = new Fake { Next = new Level { Name = "1" } };
            var rootItem = new RootItem(fakeInpc);

            var nextName = NameOf.Property<Fake>(x => x.Next);
            var nextProp = typeof(Fake).GetProperty(nextName);
            var firstProperty = new PathProperty(null, nextProp);
            var first = new NotifyingPathItem(rootItem, firstProperty);

            var isTrueName = NameOf.Property<Level>(x => x.IsTrue);
            var isTrueProp = typeof(Level).GetProperty(isTrueName);
            var secondProperty = new PathProperty(firstProperty, isTrueProp);
            var second = new NotifyingPathItem(first, secondProperty);

            Assert.AreSame(fakeInpc.Next, second.Source);
            fakeInpc.Next = new Level { Name = "2" };
            Assert.AreSame(fakeInpc.Next, second.Source);
        }
    }
}
