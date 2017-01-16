namespace Gu.Reactive.Tests.Collections.Filter
{
    using System;
    using System.Linq;
    using System.Reactive.Subjects;

    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    public class ReadOnlyFilteredViewTests
    {
        private Func<int, bool> filter = x => true;

        [Test]
        public void FilterEnumerable()
        {
            using (var subject = new Subject<object>())
            {
                var source = Enumerable.Range(1, 3);
                using (var view = source.AsReadOnlyFilteredView(this.Filter, subject))
                {
                    var actual = view.SubscribeAll();
                    this.filter = x => x < 3;
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
            }
        }

        private bool Filter(int i)
        {
            return this.filter(i);
        }
    }
}