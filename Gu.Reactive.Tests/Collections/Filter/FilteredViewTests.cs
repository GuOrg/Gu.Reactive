namespace Gu.Reactive.Tests.Collections.Filter
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Reactive.Subjects;

    using Gu.Reactive.Tests.Helpers;

    using Microsoft.Reactive.Testing;

    using NUnit.Framework;

    public class FilteredViewTests
    {
        private static readonly NotifyCollectionChangedEventArgs ResetEventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);

        [Test]
        public void Refresh()
        {
            var ints = new List<int> { 1, 2, 3 };
            var scheduler = new TestScheduler();
            var view = ints.AsFilteredView(x => x < 2, scheduler, new Subject<object>());
            var changes = SubscribeAll(view);
            view.Refresh();
            scheduler.Start();
            var expected = new List<EventArgs>();
            expected.AddRange(Diff.ResetEventArgsCollection);
            CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);
            CollectionAssert.AreEqual(new[] { 1 }, view);

            view.Refresh();
            scheduler.Start();
            //expected.AddRange(Diff.ResetEventArgsCollection);
            CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);
            CollectionAssert.AreEqual(new[] { 1 }, view);
        }

        private List<EventArgs> SubscribeAll<T>(T view)
    where T : IEnumerable, INotifyCollectionChanged, INotifyPropertyChanged
        {
            var changes = new List<EventArgs>();
            view.ObserveCollectionChanged(false)
                .Subscribe(x => changes.Add(x.EventArgs));
            view.ObservePropertyChanged()
                .Subscribe(x => changes.Add(x.EventArgs));
            return changes;
        }
    }
}
