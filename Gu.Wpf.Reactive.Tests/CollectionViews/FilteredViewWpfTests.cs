namespace Gu.Wpf.Reactive.Tests.CollectionViews
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Reactive.Subjects;
    using System.Threading;
    using System.Windows.Controls;
    using System.Windows.Data;

    using Gu.Reactive;

    using NUnit.Framework;

    [Apartment(ApartmentState.STA)]
    public class FilteredViewWpfTests
    {
        [Test]
        public void TwoViewsNotSame()
        {
            var ints = new ObservableCollection<int>(new[] { 1, 2, 3 });
            var view1 = ints.AsFilteredView(x => true);
            var view2 = ints.AsFilteredView(x => true);
            Assert.AreNotSame(view1, view2);

            var colView1 = CollectionViewSource.GetDefaultView(view1);
            var colView2 = CollectionViewSource.GetDefaultView(view2);
            Assert.AreNotSame(colView1, colView2);
        }

        [Test, Explicit("Not sure this is relevant any more")]
        public void IsDefaultView()
        {
            Assert.Inconclusive("Not sure this is relevant any more");
            //var ints = new List<int> { 1, 2, 3 };
            //var source = ReactiveViewSource.Create(ints);
            //var view = CollectionViewSource.GetDefaultView(source.View);
            //Assert.AreSame(view, source.View);
        }

        [Test]
        public void BindItemsSource()
        {
            var listBox = new ListBox();
            var ints = new List<int> { 1, 2, 3 };
            var view = ints.AsFilteredView(x => x == 2, new Subject<object>());
            var binding = new Binding { Source = view, };
            BindingOperations.SetBinding(listBox, ItemsControl.ItemsSourceProperty, binding);
            view.Refresh();
            CollectionAssert.AreEqual(new[] { 2 }, listBox.Items); // Filtered
        }

    }
}