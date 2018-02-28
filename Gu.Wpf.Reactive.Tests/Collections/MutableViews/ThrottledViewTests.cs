#pragma warning disable 618
namespace Gu.Wpf.Reactive.Tests.Collections.MutableViews
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;

    using Gu.Reactive;
    using Gu.Reactive.Tests.Helpers;
    using Gu.Wpf.Reactive;
    using Gu.Wpf.Reactive.Tests.FakesAndHelpers;

    using Microsoft.Reactive.Testing;

    using NUnit.Framework;

    [Apartment(ApartmentState.STA)]
    public class ThrottledViewTests
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            App.Start();
        }

        [Test]
        public void AddToSourceTestScheduler()
        {
            var source = new ObservableCollection<int> { 1, 2, 3 };
            var scheduler = new TestScheduler();
            using (var view = new ThrottledView<int>(source, TimeSpan.FromMilliseconds(10), scheduler, leaveOpen: true))
            {
                scheduler.Start();
                using (var expected = source.SubscribeAll())
                {
                    using (var actual = view.SubscribeAll())
                    {
                        source.Add(4);
                        scheduler.Start();
                        CollectionAssert.AreEqual(source, view);
                        CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                    }
                }
            }
        }

        [Test]
        public void AddToSourceExplicitRefresh()
        {
            var source = new ObservableCollection<int> { 1, 2, 3 };
            using (var view = source.AsThrottledView(TimeSpan.FromMilliseconds(10)))
            {
                using (var expected = source.SubscribeAll())
                {
                    using (var actual = view.SubscribeAll())
                    {
                        source.Add(4);
                        view.Refresh();
                        CollectionAssert.AreEqual(source, view);
                        CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                    }
                }
            }
        }

        [Test]
        public async Task AddToSourceAwait()
        {
            var source = new ObservableCollection<int> { 1, 2, 3 };
            using (var view = source.AsThrottledView(TimeSpan.FromMilliseconds(10)))
            {
                using (var expected = source.SubscribeAll())
                {
                    using (var actual = view.SubscribeAll())
                    {
                        source.Add(4);
                        await Task.Delay(TimeSpan.FromMilliseconds(40)).ConfigureAwait(false);
                        CollectionAssert.AreEqual(source, view);
                        CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                    }
                }
            }
        }

        [Test]
        public void ManyAddsOneResetThrottledExplicitRefresh()
        {
            var source = new ObservableCollection<int> { 1, 2, 3 };
            var scheduler = new TestScheduler();
            using (var view = new ThrottledView<int>(source, TimeSpan.FromMilliseconds(100), scheduler, leaveOpen: true))
            {
                using (var actual = view.SubscribeAll())
                {
                    for (var i = 4; i < 10; i++)
                    {
                        source.Add(i);
                    }

                    CollectionAssert.AreEqual(new[] { 1, 2, 3 }, view);
                    CollectionAssert.IsEmpty(actual);
                    view.Refresh();
                    CollectionAssert.AreEqual(source, view);
                    var expected = new EventArgs[]
                    {
                        CachedEventArgs.CountPropertyChanged,
                        CachedEventArgs.IndexerPropertyChanged,
                        CachedEventArgs.NotifyCollectionReset
                    };
                    CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                }
            }
        }

        [Test]
        public void ManyAddsOneResetThrottledTestScheduler()
        {
            var source = new ObservableCollection<int> { 1, 2, 3 };
            var scheduler = new TestScheduler();
            using (var view = new ThrottledView<int>(source, TimeSpan.FromMilliseconds(100), scheduler, leaveOpen: true))
            {
                scheduler.Start();
                CollectionAssert.AreEqual(source, view);
                using (var actual = view.SubscribeAll())
                {
                    for (var i = 4; i < 10; i++)
                    {
                        source.Add(i);
                    }

                    CollectionAssert.AreEqual(new[] { 1, 2, 3 }, view);
                    CollectionAssert.IsEmpty(actual);
                    scheduler.Start();
                    CollectionAssert.AreEqual(source, view);
                    var expected = new EventArgs[]
                    {
                        CachedEventArgs.CountPropertyChanged,
                        CachedEventArgs.IndexerPropertyChanged,
                        CachedEventArgs.NotifyCollectionReset
                    };
                    CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                }
            }
        }

        [Test]
        public void TwoBurstsTwoResets()
        {
            var changes = new List<NotifyCollectionChangedEventArgs>();
            var source = new ObservableCollection<int> { 1, 2, 3 };
            var deferTime = TimeSpan.FromMilliseconds(10);
            using (var throttledView = source.AsThrottledView(deferTime))
            {
                throttledView.CollectionChanged += (_, e) => changes.Add(e);
                for (var i = 0; i < 10; i++)
                {
                    source.Add(i);
                }

                throttledView.Refresh();
                for (var i = 0; i < 10; i++)
                {
                    source.Add(i);
                }

                throttledView.Refresh();
                CollectionAssert.AreEqual(source, throttledView);

                var expected = new[]
                                   {
                                       new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset),
                                       new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset)
                                   };
                CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);
            }
        }

        [Test]
        public async Task WhenAddToSourceWithBufferTime()
        {
            var source = new ObservableCollection<int>();
            using (var expected = source.SubscribeAll())
            {
                var bufferTime = TimeSpan.FromMilliseconds(20);
                using (var view = source.AsThrottledView(bufferTime))
                {
                    using (var actual = view.SubscribeAll())
                    {
                        source.Add(1);
                        ////await Application.Current.Dispatcher.SimulateYield();
                        CollectionAssert.IsEmpty(view);
                        CollectionAssert.IsEmpty(actual);

                        await Task.Delay(bufferTime);
                        await Application.Current.Dispatcher.SimulateYield();
                        await Application.Current.Dispatcher.SimulateYield();

                        CollectionAssert.AreEqual(source, view);
                        CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                    }
                }
            }
        }

        [Test]
        public async Task WhenManyAddsToSourceWithBufferTime()
        {
            var source = new ObservableCollection<int>();
            var bufferTime = TimeSpan.FromMilliseconds(20);
            using (var view = source.AsThrottledView(bufferTime))
            {
                using (var actual = view.SubscribeAll())
                {
                    source.Add(1);
                    source.Add(1);
                    await Application.Current.Dispatcher.SimulateYield();
                    CollectionAssert.IsEmpty(view);
                    CollectionAssert.IsEmpty(actual);

                    await Task.Delay(bufferTime);
                    await Task.Delay(bufferTime);
                    await Application.Current.Dispatcher.SimulateYield();

                    CollectionAssert.AreEqual(source, view);
                    var expected = new EventArgs[]
                                       {
                                           CachedEventArgs.CountPropertyChanged,
                                           CachedEventArgs.IndexerPropertyChanged,
                                           CachedEventArgs.NotifyCollectionReset
                                       };
                    CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                }
            }
        }
    }
}
