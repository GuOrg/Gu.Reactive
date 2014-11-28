namespace Gu.Wpf.Reactive.Tests
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Reactive.Subjects;

    using NUnit.Framework;
    [RequiresSTA]
    public class CollectionViewTests
    {
        [Test]
        public void Filter()
        {
            var ints = new List<int> { 1, 2, 3 };
            var view = new CollectionView<int>(ints);
            CollectionAssert.AreEqual(ints, view);
            var argses = new List<NotifyCollectionChangedEventArgs>();
            ((INotifyCollectionChanged)view).CollectionChanged += (sender, args) => argses.Add(args);
            view.Filter = x => x < 3;
            CollectionAssert.AreEqual(new[] { 1, 2 }, view);
            Assert.AreEqual(1, argses.Count);
        }

        [Test]
        public void UpdatesAndNotifiesOnObservable()
        {
            var subject = new Subject<object>();
            var ints = new List<int> { 1, 2, 3 };
            var view = new CollectionView<int>(ints, subject);
            var argses = new List<NotifyCollectionChangedEventArgs>();
            ((INotifyCollectionChanged)view).CollectionChanged += (sender, args) => argses.Add(args);
            subject.OnNext(new object());
            Assert.AreEqual(1, argses.Count);
            Assert.AreEqual(NotifyCollectionChangedAction.Reset, argses.Single().Action);
        }

        [Test]
        public void UpdatesAndNotifiesOnObservableCollectionChanged()
        {
            var dummy = new ObservableCollection<int>(new List<int> { 1, 2 });
            var view = new CollectionView<int>(dummy);
            var argses = new List<NotifyCollectionChangedEventArgs>();
            ((INotifyCollectionChanged)view).CollectionChanged += (sender, args) => argses.Add(args);
            dummy.Add(3);
            Assert.AreEqual(1, argses.Count);
            Assert.AreEqual(NotifyCollectionChangedAction.Add, argses.Single().Action);
            CollectionAssert.AreEqual(dummy, view);
        }

        [Test]
        public void DoesNotUpdatesAndNotifiesOnObservableCollectionChangedWhenFiltered()
        {
            var dummy = new ObservableCollection<int>(new List<int> { 1, 2 });
            var view = new CollectionView<int>(dummy);
            view.Filter = x => x < 3;
            var argses = new List<NotifyCollectionChangedEventArgs>();
            ((INotifyCollectionChanged)view).CollectionChanged += (sender, args) => argses.Add(args);
            dummy.Add(3);
            Assert.AreEqual(0, argses.Count);
            CollectionAssert.AreEqual(new[] { 1, 2 }, view);
        }

        [Test]
        public void NotifiesOnFilterChanged()
        {
            var dummy = new ObservableCollection<int>(new List<int> { 1, 2 });
            var view = new CollectionView<int>(dummy);
            view.Filter = x => x < 3;
            var argses = new List<NotifyCollectionChangedEventArgs>();
            ((INotifyCollectionChanged)view).CollectionChanged += (sender, args) => argses.Add(args);
            dummy.Add(3);
            Assert.AreEqual(0, argses.Count);
            CollectionAssert.AreEqual(new[] { 1, 2 }, view);
            view.Filter = _ => true;
            Assert.AreEqual(1, argses.Count);
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, view);
        }

        [Test]
        public void UpdatesAndNotifiesOnPropertyChanged()
        {
            var dummy = new DummyClassWithCollection();
            var ints = new[] { 1, 2, 3 };
            dummy.Items.AddRange(ints);
            var view = CollectionView<int>.Create(dummy, x => x.Items);
            var argses = new List<NotifyCollectionChangedEventArgs>();
            ((INotifyCollectionChanged)view).CollectionChanged += (sender, args) => argses.Add(args);
            dummy.OnPropertyChanged("Items");
            Assert.AreEqual(1, argses.Count);
            Assert.AreEqual(NotifyCollectionChangedAction.Reset, argses.Single().Action);
            CollectionAssert.AreEqual(ints, view);
        }
    }
}
