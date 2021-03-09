namespace Gu.Wpf.Reactive.Tests.Views.FilterTests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Reactive.Subjects;
    using System.Threading.Tasks;
    using System.Windows;
    using Gu.Reactive;
    using Gu.Reactive.Tests.Helpers;
    using Gu.Wpf.Reactive.Tests.FakesAndHelpers;
    using Microsoft.Reactive.Testing;
    using NUnit.Framework;

    public class Triggers
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            App.Start();
        }

        [Test]
        public void ManyOnNextOneReset()
        {
            var source = new ObservableCollection<int> { 1, 2, 3 };
            var scheduler = new TestScheduler();
            using var trigger = new Subject<object?>();
            using var view = new FilteredView<int>(source, x => true, TimeSpan.FromMilliseconds(10), scheduler, leaveOpen: true, triggers: trigger);
            using var actual = view.SubscribeAll();
            source.Clear();
            for (var i = 0; i < 10; i++)
            {
                trigger.OnNext(null);
            }

            CollectionAssert.IsEmpty(actual);
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, view);
            scheduler.Start();

            var expected = new EventArgs[]
            {
                CachedEventArgs.CountPropertyChanged,
                CachedEventArgs.IndexerPropertyChanged,
                CachedEventArgs.NotifyCollectionReset,
            };
            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
            CollectionAssert.IsEmpty(view);
        }

        [Test]
        public void ManyOnNextOneAdd()
        {
            using var trigger = new Subject<object?>();
            var source = new ObservableCollection<int> { 1, 2, 3 };
            var scheduler = new TestScheduler();
            using var view = new FilteredView<int>(source, x => true, TimeSpan.FromMilliseconds(10), scheduler, leaveOpen: true, triggers: trigger);
            using var actual = view.SubscribeAll();
            source.Add(4);
            for (var i = 0; i < 10; i++)
            {
                trigger.OnNext(null);
            }

            CollectionAssert.IsEmpty(actual);
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, view);

            scheduler.Start();

            var expected = new EventArgs[]
            {
                CachedEventArgs.CountPropertyChanged,
                CachedEventArgs.IndexerPropertyChanged,
                Diff.CreateAddEventArgs(4, 3),
            };
            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
            CollectionAssert.AreEqual(new[] { 1, 2, 3, 4 }, view);
        }

        [Test]
        public void AddTriggerThenManyOnNextOneAdd()
        {
            var source = new ObservableCollection<int> { 1, 2, 3 };
            var scheduler = new TestScheduler();
            using var trigger = new Subject<object?>();
            using var view = new FilteredView<int>(source, x => true, TimeSpan.FromMilliseconds(10), scheduler, leaveOpen: true, triggers: trigger);
            using var actual = view.SubscribeAll();
            source.Add(4);
            for (var i = 0; i < 10; i++)
            {
                trigger.OnNext(null);
            }

            CollectionAssert.IsEmpty(actual);
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, view);

            scheduler.Start();

            var expected = new EventArgs[]
            {
                CachedEventArgs.CountPropertyChanged,
                CachedEventArgs.IndexerPropertyChanged,
                Diff.CreateAddEventArgs(4, 3),
            };
            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
            CollectionAssert.AreEqual(new[] { 1, 2, 3, 4 }, view);
        }

        [Test]
        public void Add()
        {
            var source = new ObservableCollection<int> { 1, 2, 3 };
            using var expected = source.SubscribeAll();
            var scheduler = new TestScheduler();
            using var view = new FilteredView<int>(source, x => true, TimeSpan.FromMilliseconds(10), scheduler, leaveOpen: true);
            using var actual = view.SubscribeAll();
            source.Add(4);
            CollectionAssert.IsEmpty(actual);
            scheduler.Start();
            CollectionAssert.AreEqual(source, view);
            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
        }

        [Test]
        public void Remove()
        {
            var source = new ObservableCollection<int> { 1, 2, 3 };
            using var expected = source.SubscribeAll();
            var scheduler = new TestScheduler();
            using var view = new FilteredView<int>(source, x => true, TimeSpan.FromMilliseconds(10), scheduler, leaveOpen: true);
            using var actual = view.SubscribeAll();
            source.RemoveAt(0);
            CollectionAssert.IsEmpty(actual);
            scheduler.Start();
            CollectionAssert.AreEqual(source, view);
            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
        }

        [Test]
        public void Replace()
        {
            var source = new ObservableCollection<int> { 1, 2, 3 };
            using var expected = source.SubscribeAll();
            var scheduler = new TestScheduler();
            using var view = new FilteredView<int>(source, x => true, TimeSpan.FromMilliseconds(10), scheduler, leaveOpen: true);
            using var actual = view.SubscribeAll();
            source[0] = 4;
            CollectionAssert.IsEmpty(actual);
            scheduler.Start();
            CollectionAssert.AreEqual(source, view);
            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
        }

        [Test]
        public void Move()
        {
            var source = new ObservableCollection<int> { 1, 2, 3 };
            var scheduler = new TestScheduler();
            using var view = new FilteredView<int>(source, x => true, TimeSpan.FromMilliseconds(10), scheduler, leaveOpen: true);
            using var actual = view.SubscribeAll();
            source.Move(0, 2);
            CollectionAssert.IsEmpty(actual);
            scheduler.Start();
            CollectionAssert.AreEqual(source, view);
            var expected = new List<EventArgs>
            {
                CachedEventArgs.CountPropertyChanged,
                CachedEventArgs.IndexerPropertyChanged,
                CachedEventArgs.NotifyCollectionReset,
            };
            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
        }

        [Test]
        public void Clear()
        {
            var source = new ObservableCollection<int> { 1, 2, 3 };
            using var expected = source.SubscribeAll();
            var scheduler = new TestScheduler();
            using var view = new FilteredView<int>(source, x => true, TimeSpan.FromMilliseconds(10), scheduler, leaveOpen: true);
            using var actual = view.SubscribeAll();
            source.Clear();
            CollectionAssert.IsEmpty(actual);
            scheduler.Start();
            CollectionAssert.AreEqual(source, view);
            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
        }

        [Test]
        public async Task UpdatesAndNotifiesOnObservableCollectionChangedWhenFiltered()
        {
            var source = new ObservableCollection<int> { 1, 2 };
            using var view = source.AsFilteredView(x => true);
            source.Add(1);
            await Application.Current.Dispatcher.SimulateYield();
            using var actual = view.SubscribeAll();
            view.Filter = x => x < 2;
            await Application.Current.Dispatcher.SimulateYield();
            var expected = new EventArgs[]
            {
                CachedEventArgs.GetOrCreatePropertyChangedEventArgs("Filter"),
                CachedEventArgs.CountPropertyChanged,
                CachedEventArgs.IndexerPropertyChanged,
                Diff.CreateRemoveEventArgs(2, 1),
            };
            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
            CollectionAssert.AreEqual(new[] { 1, 1 }, view);
        }
    }
}
