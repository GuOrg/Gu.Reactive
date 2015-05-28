namespace Gu.Wpf.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;

    public static class DeferredView
    {
        public static IDeferredView<TItem> Create<TCollection, TItem>(TCollection collection, TimeSpan bufferTime)
            where TCollection : IList<TItem>, INotifyCollectionChanged, INotifyPropertyChanged
        {
            return new DeferredView<TCollection, TItem>(collection, bufferTime);
        }

        public static IDeferredView<TItem> Create<TItem>(ObservableCollection<TItem> collection, TimeSpan bufferTime)
        {
            return new DeferredView<ObservableCollection<TItem>, TItem>(collection, bufferTime);
        }

        public static IDeferredView<TItem> Create<TItem>(ReadOnlyObservableCollection<TItem> collection, TimeSpan bufferTime)
        {
            return new DeferredView<ReadOnlyObservableCollection<TItem>, TItem>(collection, bufferTime);
        }

        public static IDeferredView<TItem> Create<TItem>(IObservableCollection<TItem> collection, TimeSpan bufferTime)
        {
            return new DeferredView<IObservableCollection<TItem>, TItem>(collection, bufferTime);
        }

        public static IDeferredView<TItem> AsDeferredView<TCollection, TItem>(this TCollection collection, TimeSpan bufferTime)
            where TCollection : IList<TItem>, INotifyCollectionChanged, INotifyPropertyChanged
        {
            return new DeferredView<TCollection, TItem>(collection, bufferTime);
        }

        public static IDeferredView<TItem> AsDeferredView<TItem>(this ObservableCollection<TItem> collection, TimeSpan bufferTime)
        {
            return new DeferredView<ObservableCollection<TItem>, TItem>(collection, bufferTime);
        }

        public static IDeferredView<TItem> AsDeferredView<TItem>(this ReadOnlyObservableCollection<TItem> collection, TimeSpan bufferTime)
        {
            return new DeferredView<ReadOnlyObservableCollection<TItem>, TItem>(collection, bufferTime);
        }

        public static IDeferredView<TItem> AsDeferredView<TItem>(this IObservableCollection<TItem> collection, TimeSpan bufferTime)
        {
            return new DeferredView<IObservableCollection<TItem>, TItem>(collection, bufferTime);
        }
    }
}