namespace Gu.Wpf.Reactive.Tests.CollectionViews
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Threading;

    using FakesAndHelpers;
    using Gu.Reactive;

    using NUnit.Framework;

    [Apartment(ApartmentState.STA), Explicit("Not testable as it uses the dispatcher")]
    public class DispatchingViewTests
    {
        [Test]
        public void ViewSignalsSameAsCollectionWhenCollectionIsChanged()
        {
            var ints = new ObservableCollection<int>();
            var collectionChanges = SubscribeAll(ints);

            var view = ints.AsDispatchingView();
            var viewChanges = SubscribeAll(view);
            
            ints.Add(1);
            
            CollectionAssert.AreEqual(ints, view);
            CollectionAssert.AreEqual(collectionChanges, viewChanges, new EventArgsComparer());
        }

        [Test]
        public void ViewSignalsSameAsCollectionWhenViewIsChanged()
        {
            var ints = new ObservableCollection<int>();
            var collectionChanges = SubscribeAll(ints);

            var view = ints.AsDispatchingView();
            var viewChanges = SubscribeAll(view);
           
            view.Add(1);
            
            CollectionAssert.AreEqual(ints, view);
            CollectionAssert.AreEqual(collectionChanges, viewChanges, new EventArgsComparer());
        }

        protected List<EventArgs> SubscribeAll<T>(T view)
            where T : IEnumerable, INotifyCollectionChanged, INotifyPropertyChanged
        {
            var changes = new List<EventArgs>();
            view.ObserveCollectionChanged(false)
                .Subscribe(x => changes.Add(x.EventArgs));
            view.ObservePropertyChanged()
                .Subscribe(x => changes.Add(x.EventArgs));
            return changes;
        }
    }
}
