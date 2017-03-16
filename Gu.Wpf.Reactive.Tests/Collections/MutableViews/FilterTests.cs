// ReSharper disable All
namespace Gu.Wpf.Reactive.Tests.Collections.MutableViews
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;

    using Gu.Reactive;
    using Gu.Reactive.Tests.Helpers;

    using Microsoft.Reactive.Testing;

    using NUnit.Framework;

    public abstract class FilterTests
    {
        protected TestScheduler scheduler;
        protected IFilteredView<int> view;
        protected ObservableCollection<int> ints;

        private static readonly PropertyChangedEventArgs FilterChangedEventArgs = new PropertyChangedEventArgs("Filter");

        [SetUp]
        public virtual void SetUp()
        {
            this.ints = new ObservableCollection<int> { 1, 2, 3 };
        }

        [Test]
        public void FilterRemoveOne()
        {
            var actual = this.view.SubscribeAll();
            this.view.Filter = x => x < 3;
            this.scheduler?.Start();

            CollectionAssert.AreEqual(new[] { 1, 2 }, this.view);
            var expected = new List<EventArgs>
                               {
                                   CachedEventArgs.CountPropertyChanged,
                                   CachedEventArgs.IndexerPropertyChanged,
                                   Diff.CreateRemoveEventArgs(3, 2),
                               };

            Assert.AreEqual(1, actual.Count(x => EventArgsComparer.Equals(x, FilterChangedEventArgs)));
            CollectionAssert.AreEqual(expected, actual.Where(x => !EventArgsComparer.Equals(x, FilterChangedEventArgs)), EventArgsComparer.Default);
        }

        [Test]
        public void FilterRemoveOneThenAdd()
        {
            var actual = this.view.SubscribeAll();
            this.view.Filter = x => x < 3;
            this.scheduler?.Start();
            CollectionAssert.AreEqual(new[] { 1, 2 }, this.view);
            var expected = new List<EventArgs>();
            expected.Add(CachedEventArgs.CountPropertyChanged);
            expected.Add(CachedEventArgs.IndexerPropertyChanged);
            expected.Add(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, 3, 2));

            Assert.AreEqual(1, actual.Count(x => EventArgsComparer.Equals(x, FilterChangedEventArgs)));
            CollectionAssert.AreEqual(expected, actual.Where(x => !EventArgsComparer.Equals(x, FilterChangedEventArgs)), EventArgsComparer.Default);

            this.view.Filter = x => true;
            this.scheduler?.Start();
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, this.view);
            expected.Add(CachedEventArgs.CountPropertyChanged);
            expected.Add(CachedEventArgs.IndexerPropertyChanged);
            expected.Add(Diff.CreateAddEventArgs(3, 2));
            Assert.AreEqual(2, actual.Count(x => EventArgsComparer.Equals(x, FilterChangedEventArgs)));
            CollectionAssert.AreEqual(expected, actual.Where(x => !EventArgsComparer.Equals(x, FilterChangedEventArgs)), EventArgsComparer.Default);
        }

        [Test]
        public void FilterReset()
        {
            var actual = this.view.SubscribeAll();
            this.view.Filter = x => x < 0;
            this.scheduler?.Start();
            CollectionAssert.IsEmpty(this.view);
            var expected = new List<EventArgs>();
            expected.AddRange(
                new EventArgs[]
                    {
                        CachedEventArgs.CountPropertyChanged,
                        CachedEventArgs.IndexerPropertyChanged,
                        CachedEventArgs.NotifyCollectionReset
                    });
            Assert.AreEqual(1, actual.Count(x => EventArgsComparer.Equals(x, FilterChangedEventArgs)));
            CollectionAssert.AreEqual(expected, actual.Where(x => !EventArgsComparer.Equals(x, FilterChangedEventArgs)), EventArgsComparer.Default);
        }
    }
}