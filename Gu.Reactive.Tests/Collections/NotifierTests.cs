namespace Gu.Reactive.Tests.Collections
{
    using System;
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;

    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    public class NotifierTests
    {
        [Test]
        public void NoChangeNoEvent()
        {
            var change = Diff.CollectionChange(new[] { 1, 2, 3 }, new[] { 1, 2, 3 });
            var notifier = new DummyNotifier();
            var actual = notifier.SubscribeAll();

            Notifier.Notify(notifier, change, null, notifier.PropertyChangedEventHandler, notifier.NotifyCollectionChangedEventHandler);

            CollectionAssert.IsEmpty(actual);
        }

        [Test]
        public void AddToEmpty()
        {
            var ints = new ObservableCollection<int>();
            var expected = ints.SubscribeAll();
            ints.Add(1);
            var notifier = new DummyNotifier();
            var actual = notifier.SubscribeAll();

            var change = Diff.CollectionChange(new int[0], ints);
            Notifier.Notify(notifier, change, null, notifier.PropertyChangedEventHandler, notifier.NotifyCollectionChangedEventHandler);

            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
        }

        [Test]
        public void Add()
        {
            var before = new[] { 1, 2, 3 };
            var ints = new ObservableCollection<int>(before);
            var expected = ints.SubscribeAll();
            ints.Add(4);
            var notifier = new DummyNotifier();
            var actual = notifier.SubscribeAll();

            var change = Diff.CollectionChange(before, ints);
            Notifier.Notify(notifier, change, null, notifier.PropertyChangedEventHandler, notifier.NotifyCollectionChangedEventHandler);

            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public void Insert(int index)
        {
            var before = new[] { 1, 2, 3 };
            var ints = new ObservableCollection<int>(before);
            var expected = ints.SubscribeAll();
            var notifier = new DummyNotifier();
            var actual = notifier.SubscribeAll();

            ints.Insert(index, 4);
            var change = Diff.CollectionChange(before, ints);
            Notifier.Notify(notifier, change, null, notifier.PropertyChangedEventHandler, notifier.NotifyCollectionChangedEventHandler);

            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public void Remove(int index)
        {
            var before = new[] { 1, 2, 3 };
            var ints = new ObservableCollection<int>(before);
            var expected = ints.SubscribeAll();
            var notifier = new DummyNotifier();
            var actual = notifier.SubscribeAll();

            ints.RemoveAt(index);
            var change = Diff.CollectionChange(before, ints);
            Notifier.Notify(notifier, change, null, notifier.PropertyChangedEventHandler, notifier.NotifyCollectionChangedEventHandler);

            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
        }

        [Test]
        public void RemoveLast()
        {
            var before = new[] { 1 };
            var ints = new ObservableCollection<int>(before);
            var expected = ints.SubscribeAll();
            var notifier = new DummyNotifier();
            var actual = notifier.SubscribeAll();

            ints.Remove(1);
            var change = Diff.CollectionChange(before, ints);
            Notifier.Notify(notifier, change, null, notifier.PropertyChangedEventHandler, notifier.NotifyCollectionChangedEventHandler);

            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
        }

        [Test]
        public void Move()
        {
            var before = new[] { 1, 2, 3 };
            var ints = new ObservableCollection<int>(before);
            var expected = ints.SubscribeAll();
            var notifier = new DummyNotifier();
            var actual = notifier.SubscribeAll();

            ints.Move(1, 2);
            var change = Diff.CollectionChange(before, ints);
            Notifier.Notify(notifier, change, null, notifier.PropertyChangedEventHandler, notifier.NotifyCollectionChangedEventHandler);

            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
        }

        [Test]
        public void Replace()
        {
            var before = new[] { 1, 2, 3 };
            var ints = new ObservableCollection<int>(before);
            var expected = ints.SubscribeAll();
            var notifier = new DummyNotifier();
            var actual = notifier.SubscribeAll();

            ints[0] = 5;
            var change = Diff.CollectionChange(before, ints);
            Notifier.Notify(notifier, change, null, notifier.PropertyChangedEventHandler, notifier.NotifyCollectionChangedEventHandler);

            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
        }

        [Test]
        public void Clear()
        {
            var before = new[] { 1, 2, 3 };
            var ints = new ObservableCollection<int>(before);
            var expected = ints.SubscribeAll();
            var notifier = new DummyNotifier();
            var actual = notifier.SubscribeAll();

            ints.Clear();
            var change = Diff.CollectionChange(before, ints);
            Notifier.Notify(notifier, change, null, notifier.PropertyChangedEventHandler, notifier.NotifyCollectionChangedEventHandler);

            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
        }

        [Test]
        public void ClearOne()
        {
            var before = new[] { 1 };
            var ints = new ObservableCollection<int>(before);
            var notifier = new DummyNotifier();
            var actual = notifier.SubscribeAll();

            ints.Clear();
            var change = Diff.CollectionChange(before, ints);
            Notifier.Notify(notifier, change, null, notifier.PropertyChangedEventHandler, notifier.NotifyCollectionChangedEventHandler);

            var dummy = new ObservableCollection<int>(new[] { 1 });
            var expected = dummy.SubscribeAll();
            dummy.RemoveAt(0);
            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
        }

        private class DummyNotifier : INotifyPropertyChanged, INotifyCollectionChanged, IEnumerable
        {
            public event PropertyChangedEventHandler PropertyChanged;
            public event NotifyCollectionChangedEventHandler CollectionChanged;

            public PropertyChangedEventHandler PropertyChangedEventHandler => PropertyChanged;

            public NotifyCollectionChangedEventHandler NotifyCollectionChangedEventHandler => CollectionChanged;

            public IEnumerator GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }
    }
}
