#pragma warning disable CS0618 // Type or member is obsolete
namespace Gu.Wpf.Reactive.Tests.Collections.Views.FilterTests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using System.Windows;
    using Gu.Reactive;
    using Gu.Reactive.Tests.Helpers;
    using Gu.Wpf.Reactive.Tests.FakesAndHelpers;
    using NUnit.Framework;

    public partial class FilteredViewTests
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
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
    }
}
