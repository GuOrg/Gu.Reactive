namespace Gu.Wpf.Reactive.Tests.CollectionViews
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;
    using FakesAndHelpers;
    using Gu.Reactive;

    using NUnit.Framework;

    public class DeferredViewTests
    {
        private ObservableCollection<int> _source;
        private TimeSpan _deferTime;
        private IDeferredView<int> _deferredView;
        private readonly List<NotifyCollectionChangedEventArgs> _changes = new List<NotifyCollectionChangedEventArgs>();

        [SetUp]
        public void SetUp()
        {
            _source = new ObservableCollection<int>(new[] { 1, 2, 3 });
            _deferTime = TimeSpan.FromMilliseconds(10);
            _deferredView = _source.AsDeferredView(_deferTime);
            _changes.Clear();
        }

        [Test]
        public void ViewSignalsSameAsCollectionWhenCollectionIsChanged()
        {
            var collectionChanges = new List<EventArgs>();
            var viewChanges = new List<EventArgs>();
            var ints = new ObservableCollection<int>();
            ints.ObservePropertyChanged().Subscribe(x => collectionChanges.Add(x.EventArgs));
            ints.ObserveCollectionChanged().Subscribe(x => collectionChanges.Add(x.EventArgs));
            var view = ints.AsDeferredView(TimeSpan.FromMilliseconds(10));
            view.ObservePropertyChanged().Subscribe(x => viewChanges.Add(x.EventArgs));
            view.ObserveCollectionChanged().Subscribe(x => viewChanges.Add(x.EventArgs));
            ints.Add(1);
            view.Refresh();
            CollectionAssert.AreEqual(ints, view);
            CollectionAssert.AreEqual(collectionChanges, viewChanges, new EventArgsComparer());
        }

        [Test]
        public void ViewSignalsSameAsCollectionWhenViewIsChanged()
        {
            var collectionChanges = new List<EventArgs>();
            var viewChanges = new List<EventArgs>();
            var ints = new ObservableCollection<int>();
            ints.ObservePropertyChanged().Subscribe(x => collectionChanges.Add(x.EventArgs));
            ints.ObserveCollectionChanged().Subscribe(x => collectionChanges.Add(x.EventArgs));
            var view = ints.AsDeferredView(TimeSpan.FromMilliseconds(10));
            view.ObservePropertyChanged().Subscribe(x => viewChanges.Add(x.EventArgs));
            view.ObserveCollectionChanged().Subscribe(x => viewChanges.Add(x.EventArgs));
            view.Add(1);
            view.Refresh();
            CollectionAssert.AreEqual(ints, view);
            CollectionAssert.AreEqual(collectionChanges, viewChanges, new EventArgsComparer());
        }

        [Test]
        public void ViewSignalsImmediatelyOnIListAddTest()
        {
            var collectionChanges = new List<EventArgs>();
            var viewChanges = new List<EventArgs>();
            var ints = new ObservableCollection<int>();
            ints.ObservePropertyChanged().Subscribe(x => collectionChanges.Add(x.EventArgs));
            ints.ObserveCollectionChanged().Subscribe(x => collectionChanges.Add(x.EventArgs));
            var view = ints.AsDeferredView(TimeSpan.FromMilliseconds(10));
            view.ObservePropertyChanged().Subscribe(x => viewChanges.Add(x.EventArgs));
            view.ObserveCollectionChanged().Subscribe(x => viewChanges.Add(x.EventArgs));
            ((IList)view).Add(1);
            ((IList)view).Add(2);
            CollectionAssert.AreEqual(ints, view);
            CollectionAssert.AreEqual(collectionChanges, viewChanges, new EventArgsComparer());
        }

        [Test]
        public void OneChangeOneNotification()
        {
            _deferredView.CollectionChanged += (_, e) => _changes.Add(e);
            _source.Add(4);
            _deferredView.Refresh();
            CollectionAssert.AreEqual(_source, _deferredView);
            var expected = new[] { new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, 4, 3) };
            CollectionAssert.AreEqual(expected, _changes, new EventArgsComparer());
        }

        [Test]
        public void ManyChangeOneReset()
        {
            _deferredView.CollectionChanged += (_, e) => _changes.Add(e);
            for (int i = 0; i < 10; i++)
            {
                _source.Add(i);
            }
            _deferredView.Refresh();
            CollectionAssert.AreEqual(_source, _deferredView);
            var expected = new[] { new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset) };
            CollectionAssert.AreEqual(expected, _changes, new EventArgsComparer());
        }

        [Test]
        public void TwoBurstsTwoResets()
        {
            _deferredView.CollectionChanged += (_, e) => _changes.Add(e);
            for (int i = 0; i < 10; i++)
            {
                _source.Add(i);
            }
            _deferredView.Refresh();
            for (int i = 0; i < 10; i++)
            {
                _source.Add(i);
            }
            _deferredView.Refresh();
            CollectionAssert.AreEqual(_source, _deferredView);
            var expected = new[]
            {
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset),
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset)
            };
            CollectionAssert.AreEqual(expected, _changes, new EventArgsComparer());
        }
    }
}
