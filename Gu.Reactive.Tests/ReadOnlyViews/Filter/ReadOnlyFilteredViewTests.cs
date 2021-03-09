namespace Gu.Reactive.Tests.ReadOnlyViews.Filter
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
            using var subject = new Subject<object?>();
            var source = Enumerable.Range(1, 3);
            using var view = source.AsReadOnlyFilteredView(this.Filter, subject);
            using var actual = view.SubscribeAll();
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

        [TestCase(new[] { true })]
        [TestCase(new[] { false, true })]
        public void FilterThrowingEnumerable(bool[] throws)
        {
            this.filter = x => true;
            using var subject = new Subject<object?>();
            var source = new ThrowingEnumerable<int>(new[] { 1, 2, 3, 4 }, new Queue<bool>(throws));
            var scheduler = new TestScheduler();
            using var view = source.AsReadOnlyFilteredView(this.Filter, scheduler, subject);
            scheduler.Start();
            CollectionAssert.AreEqual(new[] { 1, 2, 3, 4 }, view);
            using var actual = view.SubscribeAll();
            this.filter = x => x > 0;
            subject.OnNext(null);
            scheduler.Start();
            CollectionAssert.AreEqual(new[] { 1, 2, 3, 4 }, view);
            CollectionAssert.IsEmpty(actual);
        }

        private bool Filter(int i)
        {
            return this.filter(i);
        }
    }
}
