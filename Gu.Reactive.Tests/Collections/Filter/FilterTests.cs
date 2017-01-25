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
        protected List<EventArgs> actual;
        protected TestScheduler scheduler;
        protected IFilteredView<int> view;
        protected ObservableCollection<int> ints;

        private static readonly PropertyChangedEventArgs FilterChangedEventArgs = new PropertyChangedEventArgs("Filter");

        private readonly List<EventArgs> expected = new List<EventArgs>();

        [SetUp]
        public virtual void SetUp()
        {
            this.ints = new ObservableCollection<int>(new[] { 1, 2, 3 });
            this.expected.Clear();
        }

        [Test]
        public void FilterRemoveOne()
        {
            this.view.Filter = x => x < 3;
            if (this.scheduler != null)
            {
                this.scheduler.Start();
            }

            CollectionAssert.AreEqual(new[] { 1, 2 }, this.view);
            var expected = new EventArgs[]
                               {
                                   CachedEventArgs.CountPropertyChanged,
                                   CachedEventArgs.IndexerPropertyChanged,
                                   Diff.CreateRemoveEventArgs(3, 2),
                               };
            this.actual.RemoveAll(
                x =>
                x is PropertyChangedEventArgs
                && ((PropertyChangedEventArgs)x).PropertyName == FilterChangedEventArgs.PropertyName);
            CollectionAssert.AreEqual(expected, this.actual, EventArgsComparer.Default);
        }

        [Test]
        public void FilterRemoveOneThenAdd()
        {
            this.view.Filter = x => x < 3;
            this.scheduler?.Start();
            CollectionAssert.AreEqual(new[] { 1, 2 }, this.view);
            this.expected.Add(CachedEventArgs.CountPropertyChanged);
            this.expected.Add(CachedEventArgs.IndexerPropertyChanged);
            this.expected.Add(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, 3, 2));

            this.actual.RemoveAll(
                x =>
                x is PropertyChangedEventArgs
                && ((PropertyChangedEventArgs)x).PropertyName == FilterChangedEventArgs.PropertyName);
            CollectionAssert.AreEqual(this.expected, this.actual, EventArgsComparer.Default);

            this.view.Filter = x => true;
            this.scheduler?.Start();
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, this.view);
            this.expected.Add(CachedEventArgs.CountPropertyChanged);
            this.expected.Add(CachedEventArgs.IndexerPropertyChanged);
            this.expected.Add(Diff.CreateAddEventArgs(3, 2));

            this.actual.RemoveAll(
                x =>
                x is PropertyChangedEventArgs
                && ((PropertyChangedEventArgs)x).PropertyName == FilterChangedEventArgs.PropertyName);
            CollectionAssert.AreEqual(this.expected, this.actual, EventArgsComparer.Default);
        }

        [Test]
        public void FilterReset()
        {
            this.view.Filter = x => x < 0;

            this.scheduler?.Start();
            CollectionAssert.IsEmpty(this.view);
            this.expected.AddRange(CachedEventArgs.ResetEventArgsCollection);

            this.actual.RemoveAll(
                x =>
                x is PropertyChangedEventArgs
                && ((PropertyChangedEventArgs)x).PropertyName == FilterChangedEventArgs.PropertyName);
            CollectionAssert.AreEqual(this.expected, this.actual, EventArgsComparer.Default);
        }
    }
}