namespace Gu.Reactive.Tests.Collections.Filter
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Reactive.Subjects;

    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    public class ReadOnlyFilteredViewTests
    {
        private Func<int, bool> _filter = x => true;

        [Test]
        public void FilterEnumerable()
        {
            var subject = new Subject<object>();
            var source = Enumerable.Range(1, 3);
            var view = source.AsReadOnlyFilteredView(Filter, subject);
            var actual = view.SubscribeAll();
            _filter = x => x < 3;
            subject.OnNext(null);
            CollectionAssert.AreEqual(new[] { 1, 2 }, view);
            var expected = new EventArgs[]
                               {
                                   Notifier.CountPropertyChangedEventArgs,
                                   Notifier.IndexerPropertyChangedEventArgs,
                                   Diff.CreateRemoveEventArgs(3, 2),
                               };
            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
        }

        private bool Filter(int i)
        {
            return _filter(i);
        }
    }
}