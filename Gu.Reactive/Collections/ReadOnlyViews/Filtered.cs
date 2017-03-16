namespace Gu.Reactive
{
    using System;
    using System.Collections.Specialized;

    internal static class Filtered
    {
        internal static bool AffectsFilteredOnly<T>(NotifyCollectionChangedEventArgs e, Func<T, bool> filter)
        {
            if (filter == null)
            {
                return false;
            }

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Move:
                    return e.TryGetSingleNewItem(out T item) &&
                           !filter(item);
                case NotifyCollectionChangedAction.Remove:
                    return e.TryGetSingleOldItem(out T removed) &&
                           !filter(removed);
                case NotifyCollectionChangedAction.Replace:
                    return e.TryGetSingleNewItem(out T newItem) &&
                           !filter(newItem) &&
                           e.TryGetSingleOldItem(out T oldItem) &&
                           !filter(oldItem);
                case NotifyCollectionChangedAction.Reset:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}