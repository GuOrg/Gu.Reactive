namespace Gu.Reactive.Tests.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    using Gu.Reactive.Tests.Helpers;

    using Microsoft.Reactive.Testing;

    using NUnit.Framework;

    public class ReadOnlyFilteredView : CrudSourceTests
    {
        public override void SetUp()
        {
            base.SetUp();
            _scheduler = new TestScheduler();
            _view = new ReadOnlyFilteredView<int>(_ints, x => true, TimeSpan.FromMilliseconds(10), _scheduler);
            _scheduler.Start();
            _actual = _view.SubscribeAll();
        }

        [Test]
        public void InitializeFiltered()
        {
            var ints = new ObservableCollection<int>(new[] { 1, 2 });
            var view = ints.AsReadOnlyFilteredView(x => x < 2);
            CollectionAssert.AreEqual(new[] { 1 }, view);
        }

        [Test]
        public void NotifiesWithOnlyPropertyChangedSubscription()
        {
            var ints = new ObservableCollection<int>();
            var view = ints.AsReadOnlyFilteredView(x => true);
            var changes = new List<EventArgs>();
            view.ObservePropertyChanged()
                .Subscribe(x => changes.Add(x.EventArgs));
            ints.Add(1);
            var expected = new[]
                               {
                                   Notifier.CountPropertyChangedEventArgs, 
                                   Notifier.IndexerPropertyChangedEventArgs
                               };
            CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);
        }

        [Test]
        public void NotifiesWithOnlyCollectionChangedSubscription()
        {
            var ints = new ObservableCollection<int>();
            var view = ints.AsReadOnlyFilteredView(x => true);
            var changes = new List<EventArgs>();
            view.ObserveCollectionChangedSlim(false)
                .Subscribe(x => changes.Add(x));
            ints.Add(1);
            var expected = new[] { Diff.CreateAddEventArgs(1, 0) };
            CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);
        }

        [Test]
        public void AddFiltered()
        {
            var ints = new ObservableCollection<int>();
            var view = ints.AsReadOnlyFilteredView(x => x % 2 == 0);
            var changes = view.SubscribeAll();
            ints.Add(1);
            CollectionAssert.IsEmpty(view);
            CollectionAssert.IsEmpty(changes);
        }

        [Test]
        public void AddVisibleWhenFiltered()
        {
            var ints = new ObservableCollection<int>();
            var view = ints.AsFilteredView(x => x % 2 == 0);
            var changes = view.SubscribeAll();
            int countChanges = 0;
            view.ObservePropertyChanged(x => x.Count, false)
                .Subscribe(_ => countChanges++);
            Assert.AreEqual(0, countChanges);

            ints.Add(2);
            CollectionAssert.AreEqual(new[] { 2 }, view);
            var expected = new EventArgs[]
                               {
                                   Notifier.CountPropertyChangedEventArgs,
                                   Notifier.IndexerPropertyChangedEventArgs,
                                   Diff.CreateAddEventArgs(2, 0)
                               };
            CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);
            Assert.AreEqual(1, countChanges);
        }

        [Test]
        public void RemoveFiltered()
        {
            var ints = new ObservableCollection<int>(new[] { 1 });
            var view = ints.AsReadOnlyFilteredView(x => x % 2 == 0);
            var changes = view.SubscribeAll();

            ints.Remove(1);
            CollectionAssert.IsEmpty(view);
            CollectionAssert.IsEmpty(changes);
        }

        [Test]
        public void RemoveNonFiltered()
        {
            var ints = new ObservableCollection<int>(new[] { 1, 2, 3 });
            var view = ints.AsReadOnlyFilteredView(x => x % 2 == 0);
            var changes = view.SubscribeAll();

            ints.Remove(2);
            CollectionAssert.IsEmpty(view);
            var expected = new EventArgs[]
                               {
                                   Notifier.CountPropertyChangedEventArgs,
                                   Notifier.IndexerPropertyChangedEventArgs,
                                   Diff.CreateRemoveEventArgs(2, 0)
                               };
            CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);
        }

        [Test]
        public void ReplaceFiltered()
        {
            var ints = new ObservableCollection<int>(new[] { 1 });
            var view = ints.AsReadOnlyFilteredView(x => x % 2 == 0);
            var changes = view.SubscribeAll();

            ints[0] = 3;
            CollectionAssert.IsEmpty(view);
            CollectionAssert.IsEmpty(changes);
        }

        [Test]
        public void ReplaceFilteredWithVisible()
        {
            var ints = new ObservableCollection<int>(new[] { 1 });
            var view = ints.AsReadOnlyFilteredView(x => x % 2 == 0);
            var changes = view.SubscribeAll();

            ints[0] = 2;
            CollectionAssert.AreEqual(new[] { 2 }, view);
            var expected = new EventArgs[]
                               {
                                   Notifier.CountPropertyChangedEventArgs,
                                   Notifier.IndexerPropertyChangedEventArgs,
                                   Diff.CreateAddEventArgs(2, 0)
                               };
            CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);
        }
    }
}