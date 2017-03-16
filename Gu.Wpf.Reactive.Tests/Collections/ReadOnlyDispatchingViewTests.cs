namespace Gu.Wpf.Reactive.Tests.Collections
{
    using System;
    using System.Collections.ObjectModel;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;

    using Gu.Reactive;
    using Gu.Reactive.Tests.Helpers;
    using Gu.Wpf.Reactive.Tests.FakesAndHelpers;

    using NUnit.Framework;

    [Apartment(ApartmentState.STA)]
    public class ReadOnlyDispatchingViewTests
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            App.Start();
        }

        [Test]
        public async Task WhenAddToSource()
        {
            var source = new ObservableCollection<int>();
            using (var expected = source.SubscribeAll())
            {
                using (var view = new ReadOnlyDispatchingView<int>(source, TimeSpan.Zero))
                {
                    await Application.Current.Dispatcher.SimulateYield();
                    using (var actual = view.SubscribeAll())
                    {
                        source.Add(1);
                        await Application.Current.Dispatcher.SimulateYield();

                        CollectionAssert.AreEqual(source, view);
                        CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                    }
                }
            }
        }

        [Test]
        public async Task WhenAddToSourceWithBufferTime()
        {
            var source = new ObservableCollection<int>();
            using (var expected = source.SubscribeAll())
            {
                var bufferTime = TimeSpan.FromMilliseconds(20);
                using (var view = new ReadOnlyDispatchingView<int>(source, bufferTime))
                {
                    await Application.Current.Dispatcher.SimulateYield();
                    using (var actual = view.SubscribeAll())
                    {
                        source.Add(1);
                        await Application.Current.Dispatcher.SimulateYield();
                        CollectionAssert.IsEmpty(view);
                        CollectionAssert.IsEmpty(actual);

                        await Task.Delay(bufferTime);
                        await Task.Delay(bufferTime);
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
            using (var view = new ReadOnlyDispatchingView<int>(source, bufferTime))
            {
                await Application.Current.Dispatcher.SimulateYield();
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