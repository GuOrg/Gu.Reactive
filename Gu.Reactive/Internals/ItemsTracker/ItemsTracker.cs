namespace Gu.Reactive.Internals
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;

    internal static class ItemsTracker
    {
        internal static ItemsTracker<TCollection, TItem, TProperty> Create<TCollection, TItem, TProperty>(
            TCollection? source,
            NotifyingPath<TItem, TProperty> path)
            where TCollection : class, IEnumerable<TItem>, INotifyCollectionChanged
            where TItem : class, INotifyPropertyChanged
        {
            if (path.Count == 1)
            {
                return new SimpleItemsTracker<TCollection, TItem, TProperty>(
                    source,
                    (Getter<TItem, TProperty>)path[0]);
            }

            return new NestedItemsTracker<TCollection, TItem, TProperty>(source, path);
        }
    }
}
