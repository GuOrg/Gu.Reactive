namespace Gu.Wpf.Reactive.Tests.CollectionViews
{
    using System.Net.Mime;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;

    using Gu.Wpf.Reactive.Tests.FakesAndHelpers;

    using NUnit.Framework;

    public partial class DispatchingCollectionTests
    {
        [Apartment(ApartmentState.STA)]
        public class WhenBound
        {
            [OneTimeSetUp]
            public void SetUp()
            {
                App.Start();
            }

            [Test]
            public void Add()
            {
                var collection = new DispatchingCollection<int>();
                var itemsControl = new ItemsControl { ItemsSource = collection };
                CollectionAssert.IsEmpty(itemsControl.Items);

                collection.Add(1);
                CollectionAssert.AreEqual(new[] { 1 }, itemsControl.Items);
            }

            [Test]
            public async Task AddOnOtherThread()
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
}