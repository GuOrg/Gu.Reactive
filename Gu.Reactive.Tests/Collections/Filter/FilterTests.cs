// ReSharper disable All
namespace Gu.Reactive.Tests.Collections.Filter
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;

    using Gu.Reactive;
    using Gu.Reactive.Tests.Helpers;

    using Microsoft.Reactive.Testing;

    using NUnit.Framework;

    public abstract class FilterTests
    {
        private static readonly PropertyChangedEventArgs FilterChangedEventArgs = new PropertyChangedEventArgs("Filter");
        protected List<EventArgs> _actual;
        private readonly List<EventArgs> _expected = new List<EventArgs>();
        protected TestScheduler _scheduler;
        protected IFilteredView<int> _view;
        protected ObservableCollection<int> _ints;

        [SetUp]
        public virtual void SetUp()
        {
            _ints = new ObservableCollection<int>(new[] { 1, 2, 3 });
            _expected.Clear();
        }

        [Test]
        public void FilterRemoveOne()
        {
            _view.Filter = x => x < 3;
            if (_scheduler != null)
            {
                _scheduler.Start();
            }
            CollectionAssert.AreEqual(new[] { 1, 2 }, _view);
            var expected = new EventArgs[]
                               {
                                   Notifier.CountPropertyChangedEventArgs,
                                   Notifier.IndexerPropertyChangedEventArgs,
                                   Diff.CreateRemoveEventArgs(3, 2),
                               };
            _actual.RemoveAll(
                x =>
                x is PropertyChangedEventArgs
                && ((PropertyChangedEventArgs)x).PropertyName == FilterChangedEventArgs.PropertyName);
            CollectionAssert.AreEqual(expected, _actual, EventArgsComparer.Default);
        }

        [Test]
        public void FilterRemoveOneThenAdd()
        {
            _view.Filter = x => x < 3;
            _scheduler?.Start();
            CollectionAssert.AreEqual(new[] { 1, 2 }, _view);
            _expected.Add(Notifier.CountPropertyChangedEventArgs);
            _expected.Add(Notifier.IndexerPropertyChangedEventArgs);
            _expected.Add(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, 3, 2));

            _actual.RemoveAll(
                x =>
                x is PropertyChangedEventArgs
                && ((PropertyChangedEventArgs)x).PropertyName == FilterChangedEventArgs.PropertyName);
            CollectionAssert.AreEqual(_expected, _actual, EventArgsComparer.Default);

            _view.Filter = x => true;
            _scheduler?.Start();
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, _view);
            _expected.Add(Notifier.CountPropertyChangedEventArgs);
            _expected.Add(Notifier.IndexerPropertyChangedEventArgs);
            _expected.Add(Diff.CreateAddEventArgs(3, 2));

            _actual.RemoveAll(
                x =>
                x is PropertyChangedEventArgs
                && ((PropertyChangedEventArgs)x).PropertyName == FilterChangedEventArgs.PropertyName);
            CollectionAssert.AreEqual(_expected, _actual, EventArgsComparer.Default);
        }

        [Test]
        public void FilterReset()
        {
            _view.Filter = x => x < 0;

            _scheduler?.Start();
            CollectionAssert.IsEmpty(_view);
            _expected.AddRange(Diff.ResetEventArgsCollection);

            _actual.RemoveAll(
                x =>
                x is PropertyChangedEventArgs
                && ((PropertyChangedEventArgs)x).PropertyName == FilterChangedEventArgs.PropertyName);
            CollectionAssert.AreEqual(_expected, _actual, EventArgsComparer.Default);
        }
    }
}