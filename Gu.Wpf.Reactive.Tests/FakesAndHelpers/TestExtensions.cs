namespace Gu.Wpf.Reactive.Tests.FakesAndHelpers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Windows.Threading;

    using Gu.Reactive;

    public static class TestExtensions
    {
        public static List<EventArgs> SubscribeAll<T>(this T observableCollection)
            where T : IEnumerable, INotifyCollectionChanged, INotifyPropertyChanged
        {
            var changes = new List<EventArgs>();
            observableCollection.ObserveCollectionChangedSlim(false)
                                .Subscribe(x => changes.Add(x));
            observableCollection.ObservePropertyChanged()
                                .Subscribe(x => changes.Add(x.EventArgs));
            return changes;
        }

        public static DispatcherOperation SimulateYield(this Dispatcher dispatcher) => dispatcher.InvokeAsync(() => { }, DispatcherPriority.Background);
    }
}