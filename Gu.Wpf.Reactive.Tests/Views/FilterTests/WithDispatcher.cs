namespace Gu.Wpf.Reactive.Tests.Collections.Views.FilterTests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Data;
    using Gu.Reactive;
    using Gu.Reactive.Tests.Helpers;
    using Gu.Wpf.Reactive.Tests.FakesAndHelpers;
    using NUnit.Framework;

    [Apartment(ApartmentState.STA)]
    public static class WithDispatcher
    {
        [OneTimeSetUp]
        public static void OneTimeSetUp()
        {
            App.Start();
        }

        [Test]
        public static void TwoViewsNotSame()
        {
            var ints = new ObservableCollection<int> { 1, 2, 3 };
            using var view1 = ints.AsFilteredView(x => true);
            using var view2 = ints.AsFilteredView(x => true);
            Assert.AreNotSame(view1, view2);

            var colView1 = CollectionViewSource.GetDefaultView(view1);
            var colView2 = CollectionViewSource.GetDefaultView(view2);
            Assert.AreNotSame(colView1, colView2);
        }

        [Test]
        public static void InitializeFiltered()
        {
            var source = new ObservableCollection<int> { 1, 2 };
            using var view = source.AsFilteredView(x => x < 2);
            CollectionAssert.AreEqual(new[] { 1 }, view);
        }

        [Test]
        public static async Task UpdateFilter()
        {
            var source = new ObservableCollection<int> { 1, 2, 3 };
            using var view = source.AsFilteredView(x => true);
            using var actual = view.SubscribeAll();
            view.Filter = x => x < 2;
            await Application.Current.Dispatcher.SimulateYield();
            var expected = new List<EventArgs>
            {
                new PropertyChangedEventArgs("Filter"),
                CachedEventArgs.CountPropertyChanged,
                CachedEventArgs.IndexerPropertyChanged,
                CachedEventArgs.NotifyCollectionReset,
            };
            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
            CollectionAssert.AreEqual(new[] { 1 }, view);

            view.Refresh();
            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
            CollectionAssert.AreEqual(new[] { 1 }, view);
        }
    }
}
