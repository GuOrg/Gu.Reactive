namespace Gu.Reactive.Internals
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;

    internal static class ItemsTracker
    {
        internal static ItemsTracker<TCollection, TItem, TProperty> Create<TCollection, TItem, TProperty>(
            TCollection collection, NotifyingPath<TItem, TProperty> path)
            where TCollection : class, IEnumerable<TItem>, INotifyCollectionChanged
            where TItem : class, INotifyPropertyChanged
        {
            return new ItemsTracker<TCollection, TItem, TProperty>(collection, path);
        }
    }
}