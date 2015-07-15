namespace Gu.Reactive.Tests.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Reactive.Subjects;

    using Gu.Reactive.Tests.Helpers;

    using Microsoft.Reactive.Testing;

    using NUnit.Framework;

    public class FilteredViewTriggersTests
    {
        private List<EventArgs> _actualChanges;
        private List<EventArgs> _expectedChanges;
        private TestScheduler _scheduler;
        private FilteredView<int> _view;
        private List<int> _ints;

        [SetUp]
        public void SetUp()
        {
            _actualChanges = new List<EventArgs>();
            _expectedChanges = new List<EventArgs>();
            _ints = new List<int>(new[] { 1, 2, 3 });

            _scheduler = new TestScheduler();

            _view = _ints.AsFilteredView(x => true, TimeSpan.FromMilliseconds(10), _scheduler, new Subject<object>());
            _actualChanges = SubscribeAll(_view);
        }

        [Test]
        public void ManyOnNextsOneReset()
        {
            var subject = new Subject<object>();
            _view.Triggers.Add(subject);
            _ints.Clear();
            for (int i = 0; i < 10; i++)
            {
                subject.OnNext(null);
            }

            CollectionAssert.IsEmpty(_actualChanges);
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, _view);
            _scheduler.Start();

            CollectionAssert.AreEqual(Diff.ResetEventArgsCollection, _actualChanges, EventArgsComparer.Default);
            CollectionAssert.IsEmpty(_view);
        }

        [Test]
        public void ManyOnNextsOneAdd()
        {
            var subject = new Subject<object>();
            _view.Triggers.Add(subject);
            _ints.Add(4);
            for (int i = 0; i < 10; i++)
            {
                subject.OnNext(null);
            }

            CollectionAssert.IsEmpty(_actualChanges);
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, _view);

            _scheduler.Start();

            var expected = new EventArgs[] { Notifier.CountPropertyChangedEventArgs, Notifier.IndexerPropertyChangedEventArgs, Diff.CreateAddEventArgs(4, 3) };
            CollectionAssert.AreEqual(expected, _actualChanges, EventArgsComparer.Default);
            CollectionAssert.AreEqual(new[] { 1, 2, 3, 4 }, _view);
        }

        [Test]
        public void UpdatesAndNotifiesOnCollectionChanged()
        {
            var ints = new ObservableCollection<int>(new[] { 1, 2, 3 });
            var expected = SubscribeAll(ints);
            var view = ints.AsFilteredView(x => true, TimeSpan.FromMilliseconds(10), _scheduler);
            var actual = SubscribeAll(view);

            ints.Add(4);
            CollectionAssert.IsEmpty(actual);
            _scheduler.Start();
            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
        }

        [Test]
        public void UpdatesAndNotifiesOnObservableCollectionChangedWhenFiltered()
        {
            var ints = new ObservableCollection<int>(new List<int> { 1, 2 });
            var view = ints.AsFilteredView(x => true);
            ints.Add(1);
            var actual = SubscribeAll(view);
            view.Filter = x => x < 2;
            view.Refresh();
            var expected = new EventArgs[]
                                          {
                                              Notifier.IndexerPropertyChangedEventArgs,
                                              Diff.CreateReplaceEventArgs(1, 2, 1),
                                              new PropertyChangedEventArgs("Filter"), 
                                          };
            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
            CollectionAssert.AreEqual(new[] { 1, 1 }, view);
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