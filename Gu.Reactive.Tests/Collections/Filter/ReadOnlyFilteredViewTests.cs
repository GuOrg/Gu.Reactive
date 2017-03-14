namespace Gu.Reactive.Tests.Collections.Filter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Subjects;

    using Gu.Reactive.Tests.Helpers;

    using Microsoft.Reactive.Testing;

    using NUnit.Framework;

    public class ReadOnlyFilteredViewTests
    {
        private Func<int, bool> filter = x => true;

        [Test]
        public void FilterEnumerable()
        {
            this.filter = x => true;
            using (var subject = new Subject<object>())
            {
                var source = Enumerable.Range(1, 3);
                using (var view = source.AsReadOnlyFilteredView(this.Filter, subject))
                {
                    using (var actual = view.SubscribeAll())
                    {
                        this.filter = x => x < 3;
                        subject.OnNext(null);
                        CollectionAssert.AreEqual(new[] { 1, 2 }, view);
                        var expected = new EventArgs[]
                                           {
                                               CachedEventArgs.CountPropertyChanged,
                                               CachedEventArgs.IndexerPropertyChanged,
                                               Diff.CreateRemoveEventArgs(3, 2),
                                           };
                        CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                    }
                }
            }
        }

        [Test]
        public void FilterThrowingEnumerableThrowsAtFirst()
        {
            this.filter = x => true;
            using (var subject = new Subject<object>())
            {
                var source = new ThrowingEnumerable<int>(new[] { 1, 2, 3, 4 }, new Queue<bool>(new[] { true }));
                var scheduler = new TestScheduler();
                using (var view = source.AsReadOnlyFilteredView(this.Filter, scheduler, subject))
                {
                    CollectionAssert.AreEqual(new[] { 1, 2, 3, 4, 0 }, view);
                    using (var actual = view.SubscribeAll())
                    {
                        this.filter = x => x > 0;
                        subject.OnNext(null);
                        scheduler.Start();
                        CollectionAssert.AreEqual(new[] { 1, 2, 3, 4 }, view);
                        var expected = new EventArgs[]
                                           {
                                               CachedEventArgs.CountPropertyChanged,
                                               CachedEventArgs.IndexerPropertyChanged,
                                               Diff.CreateRemoveEventArgs(0, 4),
                                           };
                        CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                    }
                }
            }
        }

        [Test]
        public void FilterThrowingEnumerableThrowsAtSecond()
        {
            this.filter = x => true;
            using (var subject = new Subject<object>())
            {
                var source = new ThrowingEnumerable<int>(new[] { 1, 2, 3, 4 }, new Queue<bool>(new[] { false, true }));
                var scheduler = new TestScheduler();
                using (var view = source.AsReadOnlyFilteredView(this.Filter, scheduler, subject))
                {
                    CollectionAssert.AreEqual(new[] { 1, 2, 3, 4 }, view);
                    using (var actual = view.SubscribeAll())
                    {
                        this.filter = x => x > 0;
                        subject.OnNext(null);
                        scheduler.Start();
                        CollectionAssert.AreEqual(new[] { 1, 2, 3, 4 }, view);
                        CollectionAssert.IsEmpty(actual);
                    }
                }
            }
        }

        private bool Filter(int i)
        {
            return this.filter(i);
        }
    }
}