namespace Gu.Reactive.Tests.Internals.PropertyTrackerTests
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reactive;

    using Gu.Reactive.Internals;
    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    public class PathPropertyTrackerTests
    {
        [Test]
        public void ThrowsOnStruct()
        {
            var propertyInfo = typeof(StructLevel).GetProperty("Name");
            var pathItem = new PathProperty(null, propertyInfo);
            //// ReSharper disable once ObjectCreationAsStatement
            var exception = Assert.Throws<ArgumentException>(() => new PathPropertyTracker(null, pathItem));
            var expected = "Property path cannot have structs in it. Copy by value will make subscribing error prone.\r\n" +
                           "The type Gu.Reactive.Tests.Helpers.StructLevel is a value type not so StructLevel.Name subscribing to changes is weird.\r\n" +
                           "Parameter name: pathProperty";
            Assert.AreEqual(expected, exception.Message);
        }

        [Test]
        public void ThrowsOnNotINotifyPropertyChanged()
        {
            var propertyInfo = typeof(NotInpc).GetProperty("Name");
            var pathItem = new PathProperty(null, propertyInfo);
            //// ReSharper disable once ObjectCreationAsStatement
            var exception = Assert.Throws<ArgumentException>(() => new PathPropertyTracker(null, pathItem));
            var expected = "All levels in the path must implement INotifyPropertyChanged.\r\n" +
                           "The type Gu.Reactive.Tests.Helpers.NotInpc does not so the property NotInpc.Name will not notify when value changes.\r\n" +
                           "Parameter name: pathProperty";
            Assert.AreEqual(expected, exception.Message);
        }

        [Test]
        public void ThrowsOnSettingSourceToWrongType()
        {
            var propertyInfo = typeof(Fake).GetProperty("IsTrue");
            using (var pathItem = new PathPropertyTracker(null, new PathProperty(null, propertyInfo)))
            {
                var exception = Assert.Throws<InvalidCastException>(() => pathItem.Source = default(StructLevel));
                var expected = "Unable to cast object of type 'Gu.Reactive.Tests.Helpers.StructLevel' to type 'Gu.Reactive.Tests.Helpers.Fake'.";
                Assert.AreEqual(expected, exception.Message);
            }
        }

        [Test]
        public void SetSourceToCorrectType()
        {
            var propertyInfo = typeof(Fake).GetProperty("IsTrue");
            var fakeInpc = new Fake();
            using (var pathItem = new PathPropertyTracker(null, new PathProperty(null, propertyInfo)))
            {
                pathItem.Source = fakeInpc;
                Assert.AreSame(fakeInpc, pathItem.Source); // Really just scheking that we don't throw here
            }
        }

        [Test]
        public void SetSourceToSubtype()
        {
            var propertyInfo = typeof(IFake).GetProperty("IsTrue");
            var fakeInpc = new Fake();
            using (var pathItem = new PathPropertyTracker(null, new PathProperty(null, propertyInfo)))
            {
                pathItem.Source = fakeInpc;
                Assert.AreSame(fakeInpc, pathItem.Source); // Really just scheking that we don't throw here
            }
        }

        [Test]
        public void SetSourceToIncorrectTypeThrows()
        {
            var propertyInfo = typeof(Fake).GetProperty("IsTrue");
            var level = new Level();
            using (var pathItem = new PathPropertyTracker(null, new PathProperty(null, propertyInfo)))
            {
                var exception = Assert.Throws<InvalidCastException>(() => pathItem.Source = level);
                Assert.AreEqual("Unable to cast object of type 'Gu.Reactive.Tests.Helpers.Level' to type 'Gu.Reactive.Tests.Helpers.Fake'.", exception.Message);
            }
        }

        [Test]
        public void NotifiesOnNewSourceAffectingProp()
        {
            var propertyInfo = typeof(Fake).GetProperty("IsTrue");
            var fakeInpc = new Fake();
            using (var pathItem = new PathPropertyTracker(null, new PathProperty(null, propertyInfo)))
            {
                var changes = new List<EventPattern<PropertyChangedEventArgs>>();
                pathItem.ObservePropertyChanged().Subscribe(changes.Add);
                Assert.AreEqual(0, changes.Count);

                pathItem.Source = fakeInpc;
                Assert.AreEqual(1, changes.Count);
                Assert.AreEqual("IsTrue", changes.Single().EventArgs.PropertyName);
                Assert.AreSame(fakeInpc, changes.Single().Sender);
            }
        }

        [Test]
        public void NotifiesOnSourceChangeAffectingPropByBeingSetToNull()
        {
            var propertyInfo = typeof(Fake).GetProperty("IsTrue");
            var fakeInpc = new Fake();
            using (var pathItem = new PathPropertyTracker(null, new PathProperty(null, propertyInfo)))
            {
                pathItem.Source = fakeInpc;
                var changes = new List<EventPattern<PropertyChangedEventArgs>>();
                pathItem.ObservePropertyChanged().Subscribe(changes.Add);
                Assert.AreEqual(0, changes.Count);
                pathItem.Source = null;
                Assert.AreEqual(1, changes.Count);
                Assert.AreEqual("IsTrue", changes.Single().EventArgs.PropertyName);
                Assert.AreSame(null, changes.Single().Sender);
            }
        }

        [Test]
        public void DoesNotNotifyOnNewSourceWhenPropGoesFromNullToNull()
        {
            var propertyInfo = typeof(Fake).GetProperty("Name");
            var fakeInpc = new Fake();
            using (var pathItem = new PathPropertyTracker(null, new PathProperty(null, propertyInfo)))
            {
                var changes = new List<EventPattern<PropertyChangedEventArgs>>();
                pathItem.ObservePropertyChanged().Subscribe(changes.Add);
                Assert.AreEqual(0, changes.Count);
                pathItem.Source = fakeInpc;
                Assert.AreEqual(0, changes.Count);
            }
        }

        [Test]
        public void DoesNotNotifyOnNewNullSourceWhenPropGoesFromNullToNull()
        {
            var propertyInfo = typeof(Fake).GetProperty("Name");
            using (var pathItem = new PathPropertyTracker(null, new PathProperty(null, propertyInfo)))
            {
                var changes = new List<EventPattern<PropertyChangedEventArgs>>();
                pathItem.ObservePropertyChanged().Subscribe(changes.Add);
                Assert.AreEqual(0, changes.Count);
                pathItem.Source = null;
                Assert.AreEqual(0, changes.Count);
            }
        }

        [Test]
        public void NotifiesOnNewSourceWhenPropGoesFromTrueToTrue()
        {
            var propertyInfo = typeof(Fake).GetProperty("IsTrue");
            var fakeInpc = new Fake { IsTrue = true };
            using (var pathItem = new PathPropertyTracker(null, new PathProperty(null, propertyInfo)))
            {
                var changes = new List<EventPattern<PropertyChangedEventArgs>>();
                pathItem.ObservePropertyChanged().Subscribe(changes.Add);
                Assert.AreEqual(0, changes.Count);
                pathItem.Source = fakeInpc;
                Assert.AreEqual(1, changes.Count);
                pathItem.Source = new Fake { IsTrue = true };
                Assert.AreEqual(2, changes.Count);
            }
        }

        [Test]
        public void NotifiesOnSourceChanged()
        {
            var propertyInfo = typeof(Fake).GetProperty("IsTrue");
            var fakeInpc = new Fake();
            using (var pathItem = new PathPropertyTracker(null, new PathProperty(null, propertyInfo)))
            {
                pathItem.Source = fakeInpc;
                var changes = new List<EventPattern<PropertyChangedEventArgs>>();
                pathItem.ObservePropertyChanged().Subscribe(changes.Add);
                Assert.AreEqual(0, changes.Count);
                fakeInpc.IsTrue = !fakeInpc.IsTrue;
                Assert.AreEqual(1, changes.Count);
                Assert.AreEqual("IsTrue", changes.Single().EventArgs.PropertyName);
                Assert.AreSame(fakeInpc, changes.Single().Sender);
            }
        }

        [TestCase("", Description = "Wpf uses string.empty or null to mean all properties changed")]
        [TestCase(null)]
        [TestCase("IsTrue")]
        public void NotifiesOnSourcePropertyChangedEvent(string eventArgsPropName)
        {
            var propertyInfo = typeof(Fake).GetProperty("IsTrue");
            var fakeInpc = new Fake();
            using (var pathItem = new PathPropertyTracker(null, new PathProperty(null, propertyInfo)))
            {
                pathItem.Source = fakeInpc;
                var changes = new List<EventPattern<PropertyChangedEventArgs>>();
                pathItem.ObservePropertyChanged().Subscribe(changes.Add);
                Assert.AreEqual(0, changes.Count);
                fakeInpc.OnPropertyChanged(eventArgsPropName);
                Assert.AreEqual(1, changes.Count);
                Assert.AreEqual(eventArgsPropName, changes.Single().EventArgs.PropertyName);
                Assert.AreSame(fakeInpc, changes.Single().Sender);
            }
        }

        [Test]
        public void UpdatesNextSourceOnSourceChange()
        {
            var fakeInpc = new Fake { Next = new Level { Name = "1" } };
            using (var rootItem = new RootPropertyTracker(fakeInpc))
            {
                var nextProp = typeof(Fake).GetProperty("Next");
                var firstPathItem = new PathProperty(null, nextProp);
                using (var first = new PathPropertyTracker(rootItem, firstPathItem))
                {
                    var isTrueProp = typeof(Level).GetProperty("IsTrue");
                    using (var second = new PathPropertyTracker(first, new PathProperty(firstPathItem, isTrueProp)))
                    {
                        first.Source = fakeInpc;

                        Assert.AreSame(fakeInpc.Next, second.Source);
                    }
                }
            }
        }

        [Test]
        public void UpdatesNextSourceOnPropertyChange()
        {
            var fakeInpc = new Fake { Next = new Level { Name = "1" } };
            using (var rootItem = new RootPropertyTracker(fakeInpc))
            {
                var nextProp = typeof(Fake).GetProperty(nameof(fakeInpc.Next));
                var firstProperty = new PathProperty(null, nextProp);
                using (var first = new PathPropertyTracker(rootItem, firstProperty))
                {
                    var isTrueProp = typeof(Level).GetProperty("IsTrue");
                    var secondProperty = new PathProperty(firstProperty, isTrueProp);
                    using (var second = new PathPropertyTracker(first, secondProperty))
                    {
                        Assert.AreSame(fakeInpc.Next, second.Source);
                        fakeInpc.Next = new Level { Name = "2" };
                        Assert.AreSame(fakeInpc.Next, second.Source);
                    }
                }
            }
        }
    }
}
