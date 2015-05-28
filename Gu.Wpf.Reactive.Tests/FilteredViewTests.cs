namespace Gu.Wpf.Reactive.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Reactive.Subjects;
    using System.Threading.Tasks;
    using System.Windows.Controls;
    using System.Windows.Data;

    using Gu.Reactive;

    using NUnit.Framework;

    [RequiresSTA]
    public class FilteredViewTests
    {
        [Test]
        public async Task ManyTriggersOneReset()
        {
            var changes = new List<NotifyCollectionChangedEventArgs>();
            var subject = new Subject<object>();
            var ints = new List<int> { 1, 2, 3 };
            var deferTime = TimeSpan.FromMilliseconds(10);
            var view = new FilteredView<int>(ints, deferTime, subject);
            view.CollectionChanged += (_, e) => changes.Add(e);
            for (int i = 0; i < 100; i++)
            {
                subject.OnNext(null);
            }
            await Task.Delay(5 * deferTime.Milliseconds);
            Assert.AreEqual(NotifyCollectionChangedAction.Reset, changes.Single().Action);
        }

        [Test]
        public void ViewSignalsImmediatelyOnIListAddTest()
        {
            var collectionChanges = new List<EventArgs>();
            var viewChanges = new List<EventArgs>();
            var ints = new ObservableCollection<int>();
            ints.ObservePropertyChanged().Subscribe(x => collectionChanges.Add(x.EventArgs));
            ints.ObserveCollectionChanged().Subscribe(x => collectionChanges.Add(x.EventArgs));
            var view = ints.AsFilteredView();
            view.ObservePropertyChanged().Subscribe(x => viewChanges.Add(x.EventArgs));
            view.ObserveCollectionChanged().Subscribe(x => viewChanges.Add(x.EventArgs));
            ((IList)view).Add(1);
            ((IList)view).Add(2);
            CollectionAssert.AreEqual(ints, view);
            CollectionAssert.AreEqual(collectionChanges, viewChanges, new EventArgsComparer());
        }

        [Test]
        public async Task ManyAddsOneReset()
        {
            var changes = new List<NotifyCollectionChangedEventArgs>();
            var ints = new ObservableCollection<int>();
            var deferTime = TimeSpan.FromMilliseconds(10);
            var view = new FilteredView<int>(ints, deferTime);
            view.CollectionChanged += (_, e) =>
                {
                    changes.Add(e);
                };
            for (int i = 0; i < 100; i++)
            {
                ints.Add(i);
            }

            await Task.Delay(5 * deferTime.Milliseconds);
            CollectionAssert.AreEqual(Enumerable.Range(0, 100), view);
            Assert.AreEqual(NotifyCollectionChangedAction.Reset, changes.Single().Action);
        }

        [Test]
        public void TwoViewsNotSame()
        {
            var ints = new List<int> { 1, 2, 3 };
            var view1 = new FilteredView<int>(ints);
            var view2 = new FilteredView<int>(ints);
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
            var view = new FilteredView<int>(ints) { Filter = x => x == 2 };
            var binding = new Binding { Source = view, };
            BindingOperations.SetBinding(listBox, ItemsControl.ItemsSourceProperty, binding);

            CollectionAssert.AreEqual(new[] { 2 }, listBox.Items); // Filtered
        }

        [Test]
        public async Task Filter()
        {
            var ints = new List<int> { 1, 2, 3 };
            var view = new FilteredView<int>(ints) { Filter = x => x < 3 };
            await Task.Delay(20);
            CollectionAssert.AreEqual(new[] { 1, 2 }, view);
        }

        [Test]
        public void ToArrayTest()
        {
            var ints = new List<int> { 1, 2, 3 };
            var view = new FilteredView<int>(ints);
            CollectionAssert.AreEqual(ints, view.ToArray());
        }

        [Test]
        public async Task UpdatesAndNotifiesOnObservable()
        {
            var subject = new Subject<object>();
            var ints = new List<int> { 1, 2, 3 };
            var view = new FilteredView<int>(ints, TimeSpan.Zero, subject);
            var argses = new List<NotifyCollectionChangedEventArgs>();
            view.CollectionChanged += (sender, args) => argses.Add(args);
            subject.OnNext(new object());
            await Task.Delay(20);
            Assert.AreEqual(1, argses.Count);
            Assert.AreEqual(NotifyCollectionChangedAction.Reset, argses.Single().Action);
        }

        [Test]
        public async Task UpdatesAndNotifiesOnObservableCollectionChanged()
        {
            var ints = new ObservableCollection<int>(new List<int> { 1, 2 });
            var view = new FilteredView<int>(ints);
            var argses = new List<NotifyCollectionChangedEventArgs>();
            view.CollectionChanged += (sender, args) => argses.Add(args);
            ints.Add(3);
            await Task.Delay(20);
            Assert.AreEqual(1, argses.Count);
            Assert.AreEqual(NotifyCollectionChangedAction.Reset, argses.Single().Action);
            CollectionAssert.AreEqual(ints, view);
        }

        [Test]
        public async Task UpdatesAndNotifiesOnObservableCollectionChangedWhenFiltered()
        {
            var ints = new ObservableCollection<int>(new List<int> { 1, 2 });
            var view = new FilteredView<int>(ints) { Filter = x => x < 3 };
            var argses = new List<NotifyCollectionChangedEventArgs>();
            view.CollectionChanged += (sender, args) => argses.Add(args);
            ints.Add(3);
            await Task.Delay(20);
            Assert.AreEqual(NotifyCollectionChangedAction.Reset, argses.Single().Action);
            CollectionAssert.AreEqual(new[] { 1, 2 }, view);
        }

        [Test]
        public async Task NotifiesOnFilterChanged()
        {
            var ints = new ObservableCollection<int>(new List<int> { 1, 2, 3 });
            var view = ints.AsFilteredView(x => x < 3);
            var argses = new List<NotifyCollectionChangedEventArgs>();
            view.CollectionChanged += (sender, args) => argses.Add(args);
            Assert.AreEqual(0, argses.Count);
            CollectionAssert.AreEqual(new[] { 1, 2 }, view);
            view.Filter = _ => true;
            await Task.Delay(40);
            Assert.AreEqual(NotifyCollectionChangedAction.Reset, argses.Single().Action);
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, view);
        }
    }
}
