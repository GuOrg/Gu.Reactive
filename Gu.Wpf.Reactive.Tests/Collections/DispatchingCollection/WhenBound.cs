namespace Gu.Wpf.Reactive.Tests.Collections.DispatchingCollection
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using Gu.Wpf.Reactive.Tests.FakesAndHelpers;
    using NUnit.Framework;

    [Apartment(ApartmentState.STA)]
    [Obsolete("Remove when removing DispatchingCollection")]
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
        public static async Task WhenAddOnOtherThread()
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
