#pragma warning disable 618
namespace Gu.Wpf.Reactive.Tests.Views
{
    using System.Collections.ObjectModel;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using Gu.Reactive.Tests.Helpers;
    using Gu.Wpf.Reactive.Tests.FakesAndHelpers;
    using NUnit.Framework;

    [Apartment(ApartmentState.STA)]
    public class DispatchingViewTests
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
            using var expected = source.SubscribeAll();
            using var view = source.AsDispatchingView();
            await Application.Current.Dispatcher.SimulateYield();
            using var actual = view.SubscribeAll();
            source.Add(1);
            await Application.Current.Dispatcher.SimulateYield();

            CollectionAssert.AreEqual(source, view);
            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
        }

        [Test]
        public async Task WhenAddToSourceExplicitZero()
        {
            var source = new ObservableCollection<int>();
            using var expected = source.SubscribeAll();
            using var view = source.AsDispatchingView();
            await Application.Current.Dispatcher.SimulateYield();
            using var actual = view.SubscribeAll();
            source.Add(1);
            await Application.Current.Dispatcher.SimulateYield();

            CollectionAssert.AreEqual(source, view);
            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
        }

        [Test]
        public async Task WhenAddToView()
        {
            var source = new ObservableCollection<int>();
            using var expected = source.SubscribeAll();
            using var view = source.AsDispatchingView();
            await Application.Current.Dispatcher.SimulateYield();
            using var actual = view.SubscribeAll();
            view.Add(1);
            await Application.Current.Dispatcher.SimulateYield();

            CollectionAssert.AreEqual(source, view);
            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
        }

        [Test]
        public async Task WhenAddToViewExplicitZero()
        {
            var source = new ObservableCollection<int>();
            using var expected = source.SubscribeAll();
            using var view = source.AsDispatchingView();
            using var actual = view.SubscribeAll();
            view.Add(1);
            await Application.Current.Dispatcher.SimulateYield();

            CollectionAssert.AreEqual(source, view);
            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
        }
    }
}
