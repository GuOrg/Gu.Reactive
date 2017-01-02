namespace Gu.Reactive.Tests.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Reactive.Subjects;

    using Gu.Reactive.Tests.Helpers;

    using Microsoft.Reactive.Testing;

    using NUnit.Framework;

    public class FilteredViewTriggersTests
    {
        private List<EventArgs> actualChanges;
        private TestScheduler scheduler;
        private FilteredView<int> view;
        private List<int> ints;

        [SetUp]
        public void SetUp()
        {
            this.actualChanges = new List<EventArgs>();
            this.ints = new List<int>(new[] { 1, 2, 3 });

            this.scheduler = new TestScheduler();

            this.view = this.ints.AsFilteredView(x => true, TimeSpan.FromMilliseconds(10), this.scheduler, new Subject<object>());
            this.actualChanges = this.view.SubscribeAll();
        }

        [Test]
        public void ManyOnNextsOneReset()
        {
            var subject = new Subject<object>();
            this.view.Triggers.Add(subject);
            this.ints.Clear();
            for (int i = 0; i < 10; i++)
            {
                subject.OnNext(null);
            }

            CollectionAssert.IsEmpty(this.actualChanges);
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, this.view);
            this.scheduler.Start();

            CollectionAssert.AreEqual(Diff.ResetEventArgsCollection, this.actualChanges, EventArgsComparer.Default);
            CollectionAssert.IsEmpty(this.view);
        }

        [Test]
        public void ManyOnNextsOneAdd()
        {
            var subject = new Subject<object>();
            this.view.Triggers.Add(subject);
            this.ints.Add(4);
            for (int i = 0; i < 10; i++)
            {
                subject.OnNext(null);
            }

            CollectionAssert.IsEmpty(this.actualChanges);
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, this.view);

            this.scheduler.Start();

            var expected = new EventArgs[] { Notifier.CountPropertyChangedEventArgs, Notifier.IndexerPropertyChangedEventArgs, Diff.CreateAddEventArgs(4, 3) };
            CollectionAssert.AreEqual(expected, this.actualChanges, EventArgsComparer.Default);
            CollectionAssert.AreEqual(new[] { 1, 2, 3, 4 }, this.view);
        }

        [Test]
        public void UpdatesAndNotifiesOnCollectionChanged()
        {
            var ints = new ObservableCollection<int>(new[] { 1, 2, 3 });
            var expected = ints.SubscribeAll();
            var view = ints.AsFilteredView(x => true, TimeSpan.FromMilliseconds(10), this.scheduler);
            var actual = view.SubscribeAll();

            ints.Add(4);
            CollectionAssert.IsEmpty(actual);
            this.scheduler.Start();
            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
        }

        [Test]
        public void UpdatesAndNotifiesOnObservableCollectionChangedWhenFiltered()
        {
            var ints = new ObservableCollection<int>(new List<int> { 1, 2 });
            var view = ints.AsFilteredView(x => true);
            ints.Add(1);
            var actual = view.SubscribeAll();
            view.Filter = x => x < 2;
            view.Refresh();
            var expected = new EventArgs[]
                                          {
                                              Notifier.CountPropertyChangedEventArgs,
                                              Notifier.IndexerPropertyChangedEventArgs,
                                              Diff.CreateRemoveEventArgs(2, 1),
                                              new PropertyChangedEventArgs("Filter"),
                                          };
            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
            CollectionAssert.AreEqual(new[] { 1, 1 }, view);
        }
    }
}