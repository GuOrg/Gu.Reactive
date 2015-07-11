namespace Gu.Reactive.Tests.Helpers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;

    public static class TestExtensions
    {
        public static List<EventArgs> SubscribeAll<T>(this T observableCollection)
            where T : IEnumerable, INotifyCollectionChanged, INotifyPropertyChanged
        {
            var changes = new List<EventArgs>();
            observableCollection.ObserveCollectionChanged(false)
                                .Subscribe(x => changes.Add(x.EventArgs));
            observableCollection.ObservePropertyChanged()
                                .Subscribe(x => changes.Add(x.EventArgs));
            return changes;
        }
    }
}
