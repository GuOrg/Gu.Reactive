#pragma warning disable CS0618 // Type or member is obsolete
namespace Gu.Wpf.Reactive.Tests.Collections.MutableViews
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Reactive.Subjects;
    using System.Threading.Tasks;
    using System.Windows;
    using Gu.Reactive;
    using Gu.Reactive.Tests.Helpers;
    using Gu.Wpf.Reactive.Tests.FakesAndHelpers;
    using Microsoft.Reactive.Testing;

    using NUnit.Framework;

    public class FilteredViewTests
    {
        [SetUp]
        public void SetUp()
        {
            App.Start();
        }

        [Test]
        public void InitializeFiltered()
        {
            var source = new ObservableCollection<int> { 1, 2 };
            using (var view = source.AsFilteredView(x => x < 2))
            {
                CollectionAssert.AreEqual(new[] { 1 }, view);
            }
        }

        [Test]
        public async Task AddFilteredToSource()
        {
            var source = new ObservableCollection<int>();
            using (var view = source.AsFilteredView(x => x % 2 == 0))
            {
                using (var changes = view.SubscribeAll())
                {
                    source.Add(1);
                    await Application.Current.Dispatcher.SimulateYield();
                    CollectionAssert.IsEmpty(view);
                    CollectionAssert.IsEmpty(changes);
                }
            }
        }

        [Test]
        public async Task AddVisibleToSourceWhenFiltered()
        {
            var source = new ObservableCollection<int>();
            using (var view = source.AsFilteredView(x => x % 2 == 0))
            {
                using (var expected = source.SubscribeAll())
                {
                    using (var changes = view.SubscribeAll())
                    {
                        source.Add(2);
                        await Application.Current.Dispatcher.SimulateYield();
                        CollectionAssert.AreEqual(source, view);
                        CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);
                    }
                }
            }
        }

        [Test]
        public async Task RemoveFilteredFromSource()
        {
            var source = new ObservableCollection<int> { 1 };
            using (var view = source.AsFilteredView(x => x % 2 == 0))
            {
                using (var changes = view.SubscribeAll())
                {
                    source.Remove(1);
                    await Application.Current.Dispatcher.SimulateYield();
                    CollectionAssert.IsEmpty(view);
                    CollectionAssert.IsEmpty(changes);
                }
            }
        }

        [Test]
        public async Task RemoveVisibleFromSource()
        {
            var source = new ObservableCollection<int> { 1, 2, 3 };
            using (var view = source.AsFilteredView(x => x % 2 == 0))
            {
                using (var actual = view.SubscribeAll())
                {
                    source.Remove(2);
                    await Application.Current.Dispatcher.SimulateYield();
                    CollectionAssert.IsEmpty(view);
                    var expected = new EventArgs[]
                                       {
                                           CachedEventArgs.CountPropertyChanged,
                                           CachedEventArgs.IndexerPropertyChanged,
                                           Diff.CreateRemoveEventArgs(2, 0)
                                       };
                    CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                }
            }
        }

        [Test]
        public async Task ReplaceFiltered()
        {
            var source = new ObservableCollection<int> { 1 };
            using (var view = source.AsFilteredView(x => x % 2 == 0))
            {
                using (var changes = view.SubscribeAll())
                {
                    source[0] = 3;
                    await Application.Current.Dispatcher.SimulateYield();
                    CollectionAssert.IsEmpty(view);
                    CollectionAssert.IsEmpty(changes);
                }
            }
        }

        [Test]
        public async Task ReplaceFilteredWithVisibleInSource()
        {
            var source = new ObservableCollection<int> { 1 };
            using (var view = source.AsFilteredView(x => x % 2 == 0))
            {
                using (var changes = view.SubscribeAll())
                {
                    source[0] = 2;
                    await Application.Current.Dispatcher.SimulateYield();
                    CollectionAssert.AreEqual(new[] { 2 }, view);
                    var expected = new EventArgs[]
                                       {
                                           CachedEventArgs.CountPropertyChanged,
                                           CachedEventArgs.IndexerPropertyChanged,
                                           Diff.CreateAddEventArgs(2, 0)
                                       };
                    CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);
                }
            }
        }

        [Test]
        public async Task UpdateFilter()
        {
            var source = new ObservableCollection<int> { 1, 2, 3 };
            using (var view = source.AsFilteredView(x => true))
            {
                using (var actual = view.SubscribeAll())
                {
                    view.Filter = x => x < 2;
                    await Application.Current.Dispatcher.SimulateYield();
                    var expected = new List<EventArgs>
                    {
                        new PropertyChangedEventArgs("Filter"),
                        CachedEventArgs.CountPropertyChanged,
                        CachedEventArgs.IndexerPropertyChanged,
                        CachedEventArgs.NotifyCollectionReset
                    };
                    CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                    CollectionAssert.AreEqual(new[] { 1 }, view);

                    view.Refresh();
                    CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                    CollectionAssert.AreEqual(new[] { 1 }, view);
                }
            }
        }
    }
}
