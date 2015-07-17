namespace Gu.Reactive.Tests.Collections
{
    using System;
    using System.Collections.ObjectModel;
    using Gu.Reactive.Tests.Helpers;
    using Microsoft.Reactive.Testing;
    using NUnit.Framework;

    public class FilteredView : CrudSourceTests
    {
        public override void SetUp()
        {
            base.SetUp();
            _scheduler = new TestScheduler();
            _view = new FilteredView<int>(_ints, x => true, TimeSpan.FromMilliseconds(10), _scheduler);
            _scheduler.Start();
            _actual = _view.SubscribeAll();
        }

        [Test]
        public void InitializeFiltered()
        {
            var ints = new ObservableCollection<int>(new[] { 1, 2 });
            var view = ints.AsFilteredView(x => x < 2);
            CollectionAssert.AreEqual(new[] { 1 }, view);
        }

        [Test]
        public void AddFiltered()
        {
            var ints = new ObservableCollection<int>();
            var view = ints.AsFilteredView(x => x % 2 == 0);
            var changes = view.SubscribeAll();
            ints.Add(1);
            CollectionAssert.IsEmpty(view);
            CollectionAssert.IsEmpty(changes);
        }

        [Test]
        public void RemoveFiltered()
        {
            var ints = new ObservableCollection<int>(new[] { 1 });
            var view = ints.AsFilteredView(x => x % 2 == 0);
            var changes = view.SubscribeAll();

            ints.Remove(1);
            CollectionAssert.IsEmpty(view);
            CollectionAssert.IsEmpty(changes);
        }

        [Test]
        public void RemoveNonFiltered()
        {
            var ints = new ObservableCollection<int>(new[] { 1, 2, 3 });
            var view = ints.AsFilteredView(x => x % 2 == 0);
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
            var view = ints.AsFilteredView(x => x % 2 == 0);
            var changes = view.SubscribeAll();

            ints[0] = 3;
            CollectionAssert.IsEmpty(view);
            CollectionAssert.IsEmpty(changes);
        }

        [Test]
        public void ReplaceFilteredWithVisible()
        {
            var ints = new ObservableCollection<int>(new[] { 1 });
            var view = ints.AsFilteredView(x => x % 2 == 0);
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