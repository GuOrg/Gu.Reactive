namespace Gu.Reactive
{
    using System;
    using System.Collections.Specialized;

    internal static class Filtered
    {
        internal static bool AffectsFilteredOnly<T>(NotifyCollectionChangedEventArgs e, Func<T, bool> filter)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Move:
                    return e.TryGetSingleNewItem<T>(out var item) &&
                           !filter(item);
                case NotifyCollectionChangedAction.Remove:
                    return e.TryGetSingleOldItem<T>(out var removed) &&
                           !filter(removed);
                case NotifyCollectionChangedAction.Replace:
                    return e.TryGetSingleNewItem<T>(out var newItem) &&
                           !filter(newItem) &&
                           e.TryGetSingleOldItem<T>(out var oldItem) &&
                           !filter(oldItem);
                case NotifyCollectionChangedAction.Reset:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException(nameof(e));
            }
        }
    }
}
