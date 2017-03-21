#pragma warning disable CS0618 // Type or member is obsolete
namespace Gu.Wpf.Reactive.Tests.Collections.MutableViews
{
    using System;
    using System.Collections.ObjectModel;
    using System.Reactive.Subjects;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using Gu.Wpf.Reactive.Tests.FakesAndHelpers;
    using NUnit.Framework;

    public partial class FilteredViewTests
    {
        [Apartment(ApartmentState.STA)]
        public class Wpf
        {
            [OneTimeSetUp]
            public void OneTimeSetUp()
            {
                App.Start();
            }

            [Test]
            public void TwoViewsNotSame()
            {
                var ints = new ObservableCollection<int> { 1, 2, 3 };
                using (var view1 = ints.AsFilteredView(x => true))
                {
                    using (var view2 = ints.AsFilteredView(x => true))
                    {
                        Assert.AreNotSame(view1, view2);

                        var colView1 = CollectionViewSource.GetDefaultView(view1);
                        var colView2 = CollectionViewSource.GetDefaultView(view2);
                        Assert.AreNotSame(colView1, colView2);
                    }
                }
            }

            [Test]
            public async Task BindItemsSource()
            {
                var listBox = new ListBox();
                var source = new ObservableCollection<int> { 1, 2, 3 };
                using (var view = new FilteredView<int>(source, x => x == 2, TimeSpan.Zero, WpfSchedulers.Dispatcher, true, new Subject<object>()))
                {
                    var binding = new Binding { Source = view, };
                    BindingOperations.SetBinding(listBox, ItemsControl.ItemsSourceProperty, binding);
                    view.Refresh();
                    await Application.Current.Dispatcher.SimulateYield();
                    CollectionAssert.AreEqual(new[] { 2 }, listBox.Items); // Filtered
                }
            }
        }
    }
}