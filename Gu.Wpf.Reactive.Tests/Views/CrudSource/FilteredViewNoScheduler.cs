namespace Gu.Wpf.Reactive.Tests.Collections.Views.CrudSource
{
    using System;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using System.Windows;
    using Gu.Reactive;
    using Gu.Reactive.Tests.Collections.ReadOnlyViews;
    using Gu.Reactive.Tests.Helpers;
    using Gu.Wpf.Reactive.Tests.FakesAndHelpers;
    using NUnit.Framework;

    public class FilteredViewNoScheduler : CrudSourceTests
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            App.Start();
        }

        public override void SetUp()
        {
            this.Scheduler = new TestDispatcherScheduler();
            base.SetUp();
#pragma warning disable IDISP007 // Don't dispose injected.
            (this.View as IDisposable)?.Dispose();
#pragma warning restore IDISP007 // Don't dispose injected.
            this.View = new FilteredView<int>(this.Source, x => true, TimeSpan.Zero, leaveOpen: false, triggers: null);
        }

        [Test]
        public async Task AddFiltered()
        {
            var source = new ObservableCollection<int>();
            using var view = source.AsFilteredView(x => x % 2 == 0);
            using var changes = view.SubscribeAll();
            source.Add(1);
            await Application.Current.Dispatcher.SimulateYield();
            CollectionAssert.IsEmpty(view);
            CollectionAssert.IsEmpty(changes);
        }

        [Test]
        public async Task AddVisibleWhenFiltered()
        {
            var source = new ObservableCollection<int>();
            using var view = source.AsFilteredView(x => x % 2 == 0);
            using var expected = source.SubscribeAll();
            using var changes = view.SubscribeAll();
            source.Add(2);
            await Application.Current.Dispatcher.SimulateYield();
            CollectionAssert.AreEqual(source, view);
            CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);
        }

        [Test]
        public async Task RemoveFiltered()
        {
            var source = new ObservableCollection<int> { 1 };
            using var view = source.AsFilteredView(x => x % 2 == 0);
            using var changes = view.SubscribeAll();
            source.Remove(1);
            await Application.Current.Dispatcher.SimulateYield();
            CollectionAssert.IsEmpty(view);
            CollectionAssert.IsEmpty(changes);
        }

        [Test]
        public async Task RemoveVisible()
        {
            var source = new ObservableCollection<int> { 1, 2, 3 };
            using var view = source.AsFilteredView(x => x % 2 == 0);
            using var actual = view.SubscribeAll();
            source.Remove(2);
            await Application.Current.Dispatcher.SimulateYield();
            CollectionAssert.IsEmpty(view);
            var expected = new EventArgs[]
            {
                CachedEventArgs.CountPropertyChanged,
                CachedEventArgs.IndexerPropertyChanged,
                Diff.CreateRemoveEventArgs(2, 0),
            };
            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
        }

        [Test]
        public async Task ReplaceFiltered()
        {
            var source = new ObservableCollection<int> { 1 };
            using var view = source.AsFilteredView(x => x % 2 == 0);
            using var changes = view.SubscribeAll();
            source[0] = 3;
            await Application.Current.Dispatcher.SimulateYield();
            CollectionAssert.IsEmpty(view);
            CollectionAssert.IsEmpty(changes);
        }

        [Test]
        public async Task ReplaceFilteredWithVisible()
        {
            var source = new ObservableCollection<int> { 1 };
            using var view = source.AsFilteredView(x => x % 2 == 0);
            using var changes = view.SubscribeAll();
            source[0] = 2;
            await Application.Current.Dispatcher.SimulateYield();
            CollectionAssert.AreEqual(new[] { 2 }, view);
            var expected = new EventArgs[]
            {
                CachedEventArgs.CountPropertyChanged,
                CachedEventArgs.IndexerPropertyChanged,
                Diff.CreateAddEventArgs(2, 0),
            };
            CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);
        }
    }
}
