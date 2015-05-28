namespace Gu.Wpf.Reactive.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Gu.Reactive;
    using NUnit.Framework;

    [RequiresSTA]
    public class DispatchingViewTests
    {
        [Test]
        public void ViewSignalsSameAsCollectionWhenCollectionIsChanged()
        {
            var collectionChanges = new List<EventArgs>();
            var viewChanges = new List<EventArgs>();
            var ints = new ObservableCollection<int>();
            ints.ObservePropertyChanged().Subscribe(x => collectionChanges.Add(x.EventArgs));
            ints.ObserveCollectionChanged().Subscribe(x => collectionChanges.Add(x.EventArgs));
            var view = ints.AsDispatchingView();
            view.ObservePropertyChanged().Subscribe(x => viewChanges.Add(x.EventArgs));
            view.ObserveCollectionChanged().Subscribe(x => viewChanges.Add(x.EventArgs));
            ints.Add(1);
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
            var view = ints.AsDispatchingView();
            view.ObservePropertyChanged().Subscribe(x => viewChanges.Add(x.EventArgs));
            view.ObserveCollectionChanged().Subscribe(x => viewChanges.Add(x.EventArgs));
            view.Add(1);
            CollectionAssert.AreEqual(ints, view);
            CollectionAssert.AreEqual(collectionChanges, viewChanges, new EventArgsComparer());
        }
    }
}
