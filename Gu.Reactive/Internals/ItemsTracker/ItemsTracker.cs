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
            if (path.Count == 1)
            {
                return new SimpleItemsTracker<TCollection, TItem, TProperty>(
                    collection,
                   (Getter<TItem, TProperty>)path.Path[0].Getter);
            }

            return new NestedItemsTracker<TCollection, TItem, TProperty>(collection, path);
        }
    }
}