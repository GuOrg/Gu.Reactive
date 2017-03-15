#pragma warning disable CS0618 // Type or member is obsolete
namespace Gu.Reactive.Tests.Collections.Filter
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reactive.Subjects;

    using Gu.Reactive.Tests.Helpers;

    using Microsoft.Reactive.Testing;

    using NUnit.Framework;

    public class FilteredViewTests
    {
        [Test]
        public void Refresh()
        {
            var ints = new List<int> { 1, 2, 3 };
            var scheduler = new TestScheduler();
            using (var view = ints.AsFilteredView(x => true, scheduler, new Subject<object>()))
            {
                using (var changes = view.SubscribeAll())
                {
                    view.Filter = x => x < 2;
                    view.Refresh();
                    scheduler.Start();
                    var expected = new List<EventArgs>();
                    expected.Add(new PropertyChangedEventArgs("Filter"));
                    expected.AddRange(
                        new EventArgs[]
                        {
                            CachedEventArgs.CountPropertyChanged,
                            CachedEventArgs.IndexerPropertyChanged,
                            CachedEventArgs.NotifyCollectionReset
                        });
                    CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);
                    CollectionAssert.AreEqual(new[] { 1 }, view);

                    view.Refresh();
                    scheduler.Start();
                    ////expected.AddRange(Diff.ResetEventArgsCollection);
                    CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);
                    CollectionAssert.AreEqual(new[] { 1 }, view);
                }
            }
        }
    }
}
