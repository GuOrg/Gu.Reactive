namespace Gu.Wpf.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;

    public static class FilteredView
    {
        public static IFilteredView<TItem> AsFilteredView<TCollection, TItem>(this TCollection collection)
            where TCollection : IList<TItem>, INotifyCollectionChanged, INotifyPropertyChanged
        {
            return new FilteredView<TItem>(collection);
        }

        public static IFilteredView<TItem> AsFilteredView<TItem>(this ObservableCollection<TItem> collection)
        {
            return new FilteredView<TItem>(collection);
        }

        public static IFilteredView<TItem> AsFilteredView<TItem>(this ReadOnlyObservableCollection<TItem> collection)
        {
            return new FilteredView<TItem>(collection);
        }

        public static IFilteredView<TItem> AsFilteredView<TItem>(this IObservableCollection<TItem> collection)
        {
            return new FilteredView<TItem>(collection);
        }

        public static IFilteredView<TItem> AsFilteredView<TItem>(this ObservableCollection<TItem> collection, Func<TItem, bool> filter)
        {
            return new FilteredView<TItem>(collection, filter);
        }

        public static IFilteredView<TItem> AsFilteredView<TItem>(this ReadOnlyObservableCollection<TItem> collection, Func<TItem, bool> filter)
        {
            return new FilteredView<TItem>(collection, filter);
        }

        public static IFilteredView<TItem> AsFilteredView<TItem>(this IObservableCollection<TItem> collection, Func<TItem, bool> filter)
        {
            return new FilteredView<TItem>(collection, filter);
        }

        public static IFilteredView<TItem> AsFilteredView<TItem>(this ObservableCollection<TItem> collection, Func<TItem, bool> filter, params IObservable<object>[] triggers)
        {
            return new FilteredView<TItem>(collection, filter, triggers);
        }

        public static IFilteredView<TItem> AsFilteredView<TItem>(this ReadOnlyObservableCollection<TItem> collection, Func<TItem, bool> filter, params IObservable<object>[] triggers)
        {
            return new FilteredView<TItem>(collection, filter, triggers);
        }

        public static IFilteredView<TItem> AsFilteredView<TItem>(this IObservableCollection<TItem> collection, Func<TItem, bool> filter, params IObservable<object>[] triggers)
        {
            return new FilteredView<TItem>(collection, filter, triggers);
        }
    }
}