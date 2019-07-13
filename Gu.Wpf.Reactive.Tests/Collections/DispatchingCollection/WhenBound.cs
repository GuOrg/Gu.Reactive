namespace Gu.Wpf.Reactive.Tests.Collections
{
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;

    using Gu.Wpf.Reactive.Tests.FakesAndHelpers;

    using NUnit.Framework;

    [Apartment(ApartmentState.STA)]
    public static class WhenBound
    {
        [OneTimeSetUp]
        public static void SetUp()
        {
            App.Start();
        }

        [Test]
        public static void Add()
        {
            var collection = new DispatchingCollection<int>();
            var itemsControl = new ItemsControl { ItemsSource = collection };
            CollectionAssert.IsEmpty(itemsControl.Items);

            collection.Add(1);
            CollectionAssert.AreEqual(new[] { 1 }, itemsControl.Items);
        }

        [Test]
        public static async Task AddOnOtherThread()
        {
            var collection = new DispatchingCollection<int>();
            var itemsControl = new ItemsControl { ItemsSource = collection };
            CollectionAssert.IsEmpty(itemsControl.Items);

            await Task.Run(() => collection.Add(1)).ConfigureAwait(false);
            await Application.Current.Dispatcher.SimulateYield();
            CollectionAssert.AreEqual(new[] { 1 }, itemsControl.Items);
        }
    }
}
