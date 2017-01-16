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
        [Test]
        public void ManyOnNextsOneReset()
        {
            var ints = new List<int>(new[] { 1, 2, 3 });
            var scheduler = new TestScheduler();
            using (var view = ints.AsFilteredView(x => true, TimeSpan.FromMilliseconds(10), scheduler, new Subject<object>()))
            {
                var actualChanges = view.SubscribeAll();
                var subject = new Subject<object>();
                view.Triggers.Add(subject);
                ints.Clear();
                for (var i = 0; i < 10; i++)
                {
                    subject.OnNext(null);
                }

                CollectionAssert.IsEmpty(actualChanges);
                CollectionAssert.AreEqual(new[] { 1, 2, 3 }, view);
                scheduler.Start();

                CollectionAssert.AreEqual(Diff.ResetEventArgsCollection, actualChanges, EventArgsComparer.Default);
                CollectionAssert.IsEmpty(view);
            }
        }

        [Test]
        public void ManyOnNextsOneAdd()
        {
            var subject = new Subject<object>();
            var ints = new List<int>(new[] { 1, 2, 3 });
            var scheduler = new TestScheduler();
            using (var view = ints.AsFilteredView(x => true, TimeSpan.FromMilliseconds(10), scheduler, subject))
            {
                var actualChanges = view.SubscribeAll();
                view.Triggers.Add(subject);
                ints.Add(4);
                for (int i = 0; i < 10; i++)
                {
                    subject.OnNext(null);
                }

                CollectionAssert.IsEmpty(actualChanges);
                CollectionAssert.AreEqual(new[] { 1, 2, 3 }, view);

                scheduler.Start();

                var expected = new EventArgs[]
                                   {
                                       Notifier.CountPropertyChangedEventArgs,
                                       Notifier.IndexerPropertyChangedEventArgs,
                                       Diff.CreateAddEventArgs(4, 3)
                                   };
                CollectionAssert.AreEqual(expected, actualChanges, EventArgsComparer.Default);
                CollectionAssert.AreEqual(new[] { 1, 2, 3, 4 }, view);
            }
        }

        [Test]
        public void AddTriggerThenManyOnNextsOneAdd()
        {
            var ints = new List<int>(new[] { 1, 2, 3 });
            var scheduler = new TestScheduler();
            using (var view = ints.AsFilteredView(x => true, TimeSpan.FromMilliseconds(10), scheduler, new Subject<object>()))
            {
                var actualChanges = view.SubscribeAll();
                var subject = new Subject<object>();
                view.Triggers.Add(subject);
                ints.Add(4);
                for (int i = 0; i < 10; i++)
                {
                    subject.OnNext(null);
                }

                CollectionAssert.IsEmpty(actualChanges);
                CollectionAssert.AreEqual(new[] { 1, 2, 3 }, view);

                scheduler.Start();

                var expected = new EventArgs[]
                                   {
                                       Notifier.CountPropertyChangedEventArgs,
                                       Notifier.IndexerPropertyChangedEventArgs,
                                       Diff.CreateAddEventArgs(4, 3)
                                   };
                CollectionAssert.AreEqual(expected, actualChanges, EventArgsComparer.Default);
                CollectionAssert.AreEqual(new[] { 1, 2, 3, 4 }, view);
            }
        }

        [Test]
        public void UpdatesAndNotifiesOnCollectionChanged()
        {
            var ints = new ObservableCollection<int>(new[] { 1, 2, 3 });
            var expected = ints.SubscribeAll();
            var scheduler = new TestScheduler();
            using (var view = ints.AsFilteredView(x => true, TimeSpan.FromMilliseconds(10), scheduler))
            {
                var actual = view.SubscribeAll();

                ints.Add(4);
                CollectionAssert.IsEmpty(actual);
                scheduler.Start();
                CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
            }
        }

        [Test]
        public void UpdatesAndNotifiesOnObservableCollectionChangedWhenFiltered()
        {
            var ints = new ObservableCollection<int>(new List<int> { 1, 2 });
            using (var view = ints.AsFilteredView(x => true))
            {
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
}