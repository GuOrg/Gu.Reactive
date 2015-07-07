namespace Gu.Wpf.Reactive
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;

    public static class DispatchingView
    {
        public static IObservableCollection<TItem> Create<TCollection, TItem>(TCollection collection) 
            where TCollection : IList<TItem>, INotifyCollectionChanged, INotifyPropertyChanged
        {
            return new DispatchingView<TCollection, TItem>(collection);
        }

        public static IObservableCollection<TItem> Create<TItem>(ObservableCollection<TItem> collection)
        {
            return new DispatchingView<ObservableCollection<TItem>, TItem>(collection);
        }

        public static IObservableCollection<TItem> Create<TItem>(ReadOnlyObservableCollection<TItem> collection)
        {
            return new DispatchingView<ReadOnlyObservableCollection<TItem>, TItem>(collection);
        }

        public static IObservableCollection<TItem> Create<TItem>(IObservableCollection<TItem> collection)
        {
            return new DispatchingView<IObservableCollection<TItem>, TItem>(collection);
        }

        public static IObservableCollection<TItem> AsDispatchingView<TCollection, TItem>(this TCollection collection)
            where TCollection : IList<TItem>, INotifyCollectionChanged, INotifyPropertyChanged
        {
            return new DispatchingView<TCollection, TItem>(collection);
        }

        public static IObservableCollection<TItem> AsDispatchingView<TItem>(this ObservableCollection<TItem> collection)
        {
            return new DispatchingView<ObservableCollection<TItem>, TItem>(collection);
        }

        public static IObservableCollection<TItem> AsDispatchingView<TItem>(this ReadOnlyObservableCollection<TItem> collection)
        {
            return new DispatchingView<ReadOnlyObservableCollection<TItem>, TItem>(collection);
        }

        public static IObservableCollection<TItem> AsDispatchingView<TItem>(this IObservableCollection<TItem> collection)
        {
            return new DispatchingView<IObservableCollection<TItem>, TItem>(collection);
        }
    }
}