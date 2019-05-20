// ReSharper disable All
namespace Gu.Wpf.Reactive.Tests.Collections.Views.FilterTests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Reactive.Concurrency;
    using Gu.Reactive;
    using Gu.Reactive.Tests.Helpers;
    using NUnit.Framework;

    public abstract class FilterTests
    {
        private static readonly PropertyChangedEventArgs FilterChangedEventArgs = new PropertyChangedEventArgs("Filter");

        protected VirtualTimeSchedulerBase<long, long> Scheduler { get; set; }

        protected IFilteredView<int> View { get; set; }

        protected ObservableCollection<int> Source { get; set; }

        [SetUp]
        public virtual void SetUp()
        {
            this.Source = new ObservableCollection<int> { 1, 2, 3 };
        }

        [Test]
        public void FilterRemoveOne()
        {
            using (var actual = this.View.SubscribeAll())
            {
                this.View.Filter = x => x < 3;
                this.Scheduler?.Start();

                CollectionAssert.AreEqual(new[] { 1, 2 }, this.View);
                var expected = new List<EventArgs>
                {
                    CachedEventArgs.CountPropertyChanged,
                    CachedEventArgs.IndexerPropertyChanged,
                    Diff.CreateRemoveEventArgs(3, 2),
                };

                Assert.AreEqual(1, actual.Count(x => EventArgsComparer.Equals(x, FilterChangedEventArgs)));
                CollectionAssert.AreEqual(expected, actual.Where(x => !EventArgsComparer.Equals(x, FilterChangedEventArgs)), EventArgsComparer.Default);
            }
        }

        [Test]
        public void FilterRemoveOneThenAdd()
        {
            using (var actual = this.View.SubscribeAll())
            {
                this.View.Filter = x => x < 3;
                this.Scheduler?.Start();
                CollectionAssert.AreEqual(new[] { 1, 2 }, this.View);
                var expected = new List<EventArgs>
                {
                    CachedEventArgs.CountPropertyChanged,
                    CachedEventArgs.IndexerPropertyChanged,
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, 3, 2),
                };

                Assert.AreEqual(1, actual.Count(x => EventArgsComparer.Equals(x, FilterChangedEventArgs)));
                CollectionAssert.AreEqual(expected, actual.Where(x => !EventArgsComparer.Equals(x, FilterChangedEventArgs)), EventArgsComparer.Default);

                this.View.Filter = x => true;
                this.Scheduler?.Start();
                CollectionAssert.AreEqual(new[] { 1, 2, 3 }, this.View);
                expected.Add(CachedEventArgs.CountPropertyChanged);
                expected.Add(CachedEventArgs.IndexerPropertyChanged);
                expected.Add(Diff.CreateAddEventArgs(3, 2));
                Assert.AreEqual(2, actual.Count(x => EventArgsComparer.Equals(x, FilterChangedEventArgs)));
                CollectionAssert.AreEqual(expected, actual.Where(x => !EventArgsComparer.Equals(x, FilterChangedEventArgs)), EventArgsComparer.Default);
            }
        }

        [Test]
        public void FilterReset()
        {
            using (var actual = this.View.SubscribeAll())
            {
                this.View.Filter = x => x < 0;
                this.Scheduler?.Start();
                CollectionAssert.IsEmpty(this.View);
                var expected = new List<EventArgs>();
                expected.AddRange(
                    new EventArgs[]
                    {
                        CachedEventArgs.CountPropertyChanged,
                        CachedEventArgs.IndexerPropertyChanged,
                        CachedEventArgs.NotifyCollectionReset,
                    });
                Assert.AreEqual(1, actual.Count(x => EventArgsComparer.Equals(x, FilterChangedEventArgs)));
                CollectionAssert.AreEqual(expected, actual.Where(x => !EventArgsComparer.Equals(x, FilterChangedEventArgs)), EventArgsComparer.Default);
            }
        }
    }
}
