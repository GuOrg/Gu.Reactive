namespace Gu.Wpf.Reactive.Tests.CollectionViews
{
    using System;
    using System.Collections.ObjectModel;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;

    using Gu.Reactive;
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
            var ourceChanges = source.SubscribeAll();

            var view = new ReadOnlyDispatchingView<int>(source, TimeSpan.Zero);
            var viewChanges = view.SubscribeAll();

            source.Add(1);
            await Application.Current.Dispatcher.SimulateYield();

            CollectionAssert.AreEqual(source, view);
            CollectionAssert.AreEqual(ourceChanges, viewChanges, EventArgsComparer.Default);
        }

        [Test]
        public async Task WhenAddToSourceWithBufferTime()
        {
            var source = new ObservableCollection<int>();
            var sourceChanges = source.SubscribeAll();

            var bufferTime = TimeSpan.FromMilliseconds(20);
            var view = new ReadOnlyDispatchingView<int>(source, bufferTime);
            var viewChanges = view.SubscribeAll();

            source.Add(1);
            await Application.Current.Dispatcher.SimulateYield();
            CollectionAssert.IsEmpty(view);
            CollectionAssert.IsEmpty(viewChanges);

            await Task.Delay(bufferTime);
            await Task.Delay(bufferTime);
            await Application.Current.Dispatcher.SimulateYield();

            CollectionAssert.AreEqual(source, view);
            CollectionAssert.AreEqual(sourceChanges, viewChanges, EventArgsComparer.Default);
        }

        [Test]
        public async Task WhenManyAddsToSourceWithBufferTime()
        {
            var source = new ObservableCollection<int>();
            var bufferTime = TimeSpan.FromMilliseconds(20);
            var view = new ReadOnlyDispatchingView<int>(source, bufferTime);
            var viewChanges = view.SubscribeAll();

            source.Add(1);
            source.Add(1);
            await Application.Current.Dispatcher.SimulateYield();
            CollectionAssert.IsEmpty(view);
            CollectionAssert.IsEmpty(viewChanges);

            await Task.Delay(bufferTime);
            await Application.Current.Dispatcher.SimulateYield();

            CollectionAssert.AreEqual(source, view);
            CollectionAssert.AreEqual(CachedEventArgs.ResetEventArgsCollection, viewChanges, EventArgsComparer.Default);
        }
    }
}