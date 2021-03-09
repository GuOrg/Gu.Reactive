namespace Gu.Wpf.Reactive.Tests.Views.CrudView
{
    using System;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using System.Windows;
    using Gu.Reactive;
    using Gu.Reactive.Tests.Helpers;
    using Gu.Wpf.Reactive.Tests.FakesAndHelpers;
    using NUnit.Framework;

    public class FilteredViewNoThrottling : CrudViewTests
    {
        [SetUp]
        public override void SetUp()
        {
            this.Scheduler = new TestDispatcherScheduler();
            base.SetUp();
#pragma warning disable IDISP007 // Don't dispose injected.
            (this.View as IDisposable)?.Dispose();
#pragma warning restore IDISP007 // Don't dispose injected.
            this.View = this.Ints.AsFilteredView(x => true, TimeSpan.Zero);
        }

        [Test]
        public async Task AddFilteredWhenEmpty()
        {
            var source = new ObservableCollection<int>();
            using var view = source.AsFilteredView(x => x % 2 == 0);
            using var actual = view.SubscribeAll();
            view.Add(1);
            await Application.Current.Dispatcher.SimulateYield();
            CollectionAssert.AreEqual(new[] { 1 }, view);
            CollectionAssert.AreEqual(new[] { 1 }, source);
            var expected = new EventArgs[]
            {
                CachedEventArgs.CountPropertyChanged,
                CachedEventArgs.IndexerPropertyChanged,
                Diff.CreateAddEventArgs(1, 0),
            };
            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
        }

        [Test]
        public async Task AddVisibleWhenEmpty()
        {
            var source = new ObservableCollection<int>();
            using var view = source.AsFilteredView(x => true);
            using var actual = view.SubscribeAll();
            view.Add(1);
            await Application.Current.Dispatcher.SimulateYield();
            CollectionAssert.AreEqual(new[] { 1 }, view);
            CollectionAssert.AreEqual(new[] { 1 }, source);
            var expected = new EventArgs[]
            {
                CachedEventArgs.CountPropertyChanged,
                CachedEventArgs.IndexerPropertyChanged,
                Diff.CreateAddEventArgs(1, 0),
            };
            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
        }

        [Test]
        public async Task AddFiltered()
        {
            var source = new ObservableCollection<int> { 1, 2, 3 };
            using var view = source.AsFilteredView(x => x % 2 == 0);
            using var actual = view.SubscribeAll();
            view.Add(4);
            await Application.Current.Dispatcher.SimulateYield();
            CollectionAssert.AreEqual(new[] { 2, 4 }, view);
            CollectionAssert.AreEqual(new[] { 1, 2, 3, 4 }, source);
            var expected = new EventArgs[]
            {
                CachedEventArgs.CountPropertyChanged,
                CachedEventArgs.IndexerPropertyChanged,
                Diff.CreateAddEventArgs(4, 1),
            };
            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
        }

        [Test]
        public async Task InsertFiltered()
        {
            var source = new ObservableCollection<int> { 1, 2, 3 };
            using var view = source.AsFilteredView(x => x % 2 == 0);
            using var actual = view.SubscribeAll();
            view.Insert(0, 4);
            await Application.Current.Dispatcher.SimulateYield();
            CollectionAssert.AreEqual(new[] { 4, 2 }, view);
            CollectionAssert.AreEqual(new[] { 1, 4, 2, 3 }, source);
            var expected = new EventArgs[]
            {
                CachedEventArgs.CountPropertyChanged,
                CachedEventArgs.IndexerPropertyChanged,
                Diff.CreateAddEventArgs(4, 0),
            };
            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
        }

        [Test]
        public async Task RemoveFiltered()
        {
            var source = new ObservableCollection<int> { 1, 2, 3 };
            using var view = source.AsFilteredView(x => x % 2 == 0);
            using var actual = view.SubscribeAll();
            view.RemoveAt(0);
            await Application.Current.Dispatcher.SimulateYield();
            CollectionAssert.IsEmpty(view);
            CollectionAssert.AreEqual(new[] { 1, 3 }, source);
            var expected = new EventArgs[]
            {
                CachedEventArgs.CountPropertyChanged,
                CachedEventArgs.IndexerPropertyChanged,
                Diff.CreateRemoveEventArgs(2, 0),
            };
            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
        }

        [Test]
        public async Task Replace0()
        {
            var source = new ObservableCollection<int> { 1, 2, 3, 2 };
            using var view = source.AsFilteredView(x => x % 2 == 0);
            using var actual = view.SubscribeAll();
            view[0] = 4;
            await Application.Current.Dispatcher.SimulateYield();
            CollectionAssert.AreEqual(new[] { 4, 2 }, view);
            CollectionAssert.AreEqual(new[] { 1, 4, 3, 2 }, source);
            var expected = new EventArgs[]
            {
                CachedEventArgs.IndexerPropertyChanged,
                Diff.CreateReplaceEventArgs(4, 2, 0),
            };
            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
        }

        [Test]
        public async Task Replace1()
        {
            var source = new ObservableCollection<int> { 1, 2, 3, 2 };
            using var view = source.AsFilteredView(x => x % 2 == 0);
            using var actual = view.SubscribeAll();
            view[1] = 4;
            await Application.Current.Dispatcher.SimulateYield();
            CollectionAssert.AreEqual(new[] { 2, 4 }, view);
            CollectionAssert.AreEqual(new[] { 1, 2, 3, 4 }, source);
            var expected = new EventArgs[]
            {
                CachedEventArgs.IndexerPropertyChanged,
                Diff.CreateReplaceEventArgs(4, 2, 1),
            };
            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
        }
    }
}
