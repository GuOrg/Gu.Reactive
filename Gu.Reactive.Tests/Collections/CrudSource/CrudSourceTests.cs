namespace Gu.Reactive.Tests.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using Gu.Reactive.Tests.Helpers;

    using Microsoft.Reactive.Testing;

    using NUnit.Framework;

    public abstract class CrudSourceTests
    {
        private List<EventArgs> _expected;
        protected List<EventArgs> _actual;
        protected TestScheduler _scheduler;
        protected IReadOnlyObservableCollection<int> _view;
        protected ObservableCollection<int> _ints;

        [SetUp]
        public virtual void SetUp()
        {
            _ints = new ObservableCollection<int>(new[] { 1, 2, 3 });
            _expected = _ints.SubscribeAll();
        }

        [Test]
        public void Ctor()
        {
            CollectionAssert.AreEqual(_ints, _view);
        }

        [Test]
        public void NoChangeNoEvent()
        {
            CollectionAssert.AreEqual(_ints, _view);
            (_view as IRefreshAble)?.Refresh();
            _scheduler?.Start();

            CollectionAssert.AreEqual(_ints, _view);
            CollectionAssert.IsEmpty(_actual);
        }

        [Test]
        public void UpdatesBeforeItNotifies()
        {
            int[] actual = null;
            int[] expected = null;
            _view.ObserveCollectionChanged(false)
                 .Subscribe(_ => { actual = _view.ToArray(); });
            _ints.ObserveCollectionChanged(false)
                 .Subscribe(_ => { expected = _ints.ToArray(); });
            _ints.Add(5);
            _scheduler?.Start();
            CollectionAssert.AreEqual(expected, actual);

            _ints.Clear();
            _scheduler?.Start();
            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void Add()
        {
            _ints.Add(4);
            _scheduler?.Start();

            CollectionAssert.AreEqual(_ints, _view);
            CollectionAssert.AreEqual(_expected, _actual, EventArgsComparer.Default);
        }


        [Test]
        public void ManyAddsOneReset()
        {
            if (_scheduler == null)
            {
                Assert.Inconclusive();
            }
            for (int i = 0; i < 10; i++)
            {
                _ints.Add(i);
            }

            _scheduler.Start();

            CollectionAssert.AreEqual(_ints, _view);
            CollectionAssert.AreEqual(Diff.ResetEventArgsCollection, _actual, EventArgsComparer.Default);
        }

        [TestCase(1)]
        [TestCase(2)]
        public void Remove(int toRemove)
        {
            _ints.Remove(toRemove);
            _scheduler?.Start();

            CollectionAssert.AreEqual(_ints, _view);
            CollectionAssert.AreEqual(_expected, _actual, EventArgsComparer.Default);
        }

        [TestCase(2, 1)]
        [TestCase(0, 2)]
        public void Replace(int index, int value)
        {
            _ints[index] = value;
            _scheduler?.Start();

            Assert.AreEqual(value, _view[index]);
            CollectionAssert.AreEqual(_ints, _view);
            CollectionAssert.AreEqual(_expected, _actual, EventArgsComparer.Default);
        }

        [TestCase(0, 1)]
        public void Move(int fromIndex, int toIndex)
        {
            _ints.Move(fromIndex, toIndex);
            _scheduler?.Start();

            CollectionAssert.AreEqual(_ints, _view);
            CollectionAssert.AreEqual(_expected, _actual, EventArgsComparer.Default);
        }
    }
}
