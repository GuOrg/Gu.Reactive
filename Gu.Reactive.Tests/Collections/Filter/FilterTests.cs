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
            this._ints = new ObservableCollection<int>(new[] { 1, 2, 3 });
            this._expected.Clear();
        }

        [Test]
        public void FilterRemoveOne()
        {
            this._view.Filter = x => x < 3;
            if (this._scheduler != null)
            {
                this._scheduler.Start();
            }

            CollectionAssert.AreEqual(new[] { 1, 2 }, this._view);
            var expected = new EventArgs[]
                               {
                                   Notifier.CountPropertyChangedEventArgs,
                                   Notifier.IndexerPropertyChangedEventArgs,
                                   Diff.CreateRemoveEventArgs(3, 2),
                               };
            this._actual.RemoveAll(
                x =>
                x is PropertyChangedEventArgs
                && ((PropertyChangedEventArgs)x).PropertyName == FilterChangedEventArgs.PropertyName);
            CollectionAssert.AreEqual(expected, this._actual, EventArgsComparer.Default);
        }

        [Test]
        public void FilterRemoveOneThenAdd()
        {
            this._view.Filter = x => x < 3;
            this._scheduler?.Start();
            CollectionAssert.AreEqual(new[] { 1, 2 }, this._view);
            this._expected.Add(Notifier.CountPropertyChangedEventArgs);
            this._expected.Add(Notifier.IndexerPropertyChangedEventArgs);
            this._expected.Add(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, 3, 2));

            this._actual.RemoveAll(
                x =>
                x is PropertyChangedEventArgs
                && ((PropertyChangedEventArgs)x).PropertyName == FilterChangedEventArgs.PropertyName);
            CollectionAssert.AreEqual(this._expected, this._actual, EventArgsComparer.Default);

            this._view.Filter = x => true;
            this._scheduler?.Start();
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, this._view);
            this._expected.Add(Notifier.CountPropertyChangedEventArgs);
            this._expected.Add(Notifier.IndexerPropertyChangedEventArgs);
            this._expected.Add(Diff.CreateAddEventArgs(3, 2));

            this._actual.RemoveAll(
                x =>
                x is PropertyChangedEventArgs
                && ((PropertyChangedEventArgs)x).PropertyName == FilterChangedEventArgs.PropertyName);
            CollectionAssert.AreEqual(this._expected, this._actual, EventArgsComparer.Default);
        }

        [Test]
        public void FilterReset()
        {
            this._view.Filter = x => x < 0;

            this._scheduler?.Start();
            CollectionAssert.IsEmpty(this._view);
            this._expected.AddRange(Diff.ResetEventArgsCollection);

            this._actual.RemoveAll(
                x =>
                x is PropertyChangedEventArgs
                && ((PropertyChangedEventArgs)x).PropertyName == FilterChangedEventArgs.PropertyName);
            CollectionAssert.AreEqual(this._expected, this._actual, EventArgsComparer.Default);
        }
    }
}