namespace Gu.Reactive.Tests.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using Gu.Reactive.Tests.Fakes;
    using NUnit.Framework;

    public class DiffTests
    {
        private List<EventArgs> _changes = new List<EventArgs>();

        private ObservableCollection<int> _ints;

        private IReadOnlyList<int> _before;

        [SetUp]
        public void SetUp()
        {
            _changes.Clear();
            _ints = new ObservableCollection<int>(new[] { 1, 2, 3 });
            _changes = SubscribeAll(_ints);
            _before = _ints.ToArray();
        }

        [Test]
        public void NoChange()
        {
            var actual = Diff.Changes(_before, _ints);
            CollectionAssert.IsEmpty(actual);
        }

        [Test]
        public void AddToEmpty()
        {
            var ints = new ObservableCollection<int>();
            var expected = SubscribeAll(ints);
            ints.Add(1);

            var actual = Diff.Changes(new int[0], ints);

            CollectionAssert.IsNotEmpty(actual);
            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
        }

        [Test]
        public void Add()
        {
            _ints.Add(4);
            var actual = Diff.Changes(_before, _ints);
            CollectionAssert.AreEqual(_changes, actual, EventArgsComparer.Default);
        }

        [Test]
        public void AddAndRemove()
        {
            _ints.Add(4);
            _ints.RemoveAt(0);
            var actual = Diff.Changes(_before, _ints);
            CollectionAssert.AreEqual(Diff.ResetEventArgsCollection, actual, EventArgsComparer.Default);
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public void Insert(int index)
        {
            _ints.Insert(index, 4);
            var actual = Diff.Changes(_before, _ints);
            CollectionAssert.AreEqual(_changes, actual, EventArgsComparer.Default);
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public void Remove(int index)
        {
            _ints.RemoveAt(index);
            var actual = Diff.Changes(_before, _ints);
            CollectionAssert.AreEqual(_changes, actual, EventArgsComparer.Default);
        }

        [Test]
        public void RemoveLast()
        {
            var ints = new ObservableCollection<int>(new[] { 1 });
            var expected = SubscribeAll(ints);

            ints.Remove(1);

            var actual = Diff.Changes(new[] { 1 }, ints);
            CollectionAssert.IsNotEmpty(actual);
            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
        }

        [Test]
        public void Move()
        {
            _ints.Move(1, 2);
            var actual = Diff.Changes(_before, _ints);
            CollectionAssert.AreEqual(_changes, actual, EventArgsComparer.Default);
        }

        [Test]
        public void Replace()
        {
            _ints[0] = 5;
            var actual = Diff.Changes(_before, _ints);

            CollectionAssert.IsNotEmpty(actual);
            CollectionAssert.AreEqual(_changes, actual, EventArgsComparer.Default);
        }

        [Test]
        public void Clear()
        {
            _ints.Clear();

            var actual = Diff.Changes(_before, _ints);
            CollectionAssert.IsNotEmpty(actual);
            CollectionAssert.AreEqual(_changes, actual, EventArgsComparer.Default);
        }

        [Test]
        public void ClearOne()
        {
            var ints = new ObservableCollection<int>(new[] { 1 });
            var expected = SubscribeAll(ints);

            ints.Clear();

            var actual = Diff.Changes(new[] { 1 }, ints);
            CollectionAssert.IsNotEmpty(actual);
            Assert.Inconclusive("I think we like remove more here");
            //CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
        }

        protected List<EventArgs> SubscribeAll<T>(T view)
            where T : IEnumerable, INotifyCollectionChanged, INotifyPropertyChanged
        {
            var changes = new List<EventArgs>();
            view.ObserveCollectionChanged(false)
                .Subscribe(x => changes.Add(x.EventArgs));
            view.ObservePropertyChanged()
                .Subscribe(x => changes.Add(x.EventArgs));
            return changes;
        }
    }
}
