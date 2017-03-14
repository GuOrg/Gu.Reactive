namespace Gu.Reactive
{
    using System.Collections.Specialized;

    /// <summary>
    /// Factory methods for creating <see cref="NotifyCollectionChangedEventArgs{T}"/>
    /// </summary>
    public static class NotifyCollectionChangedEventArgsExt
    {
        /// <summary>
        /// Create a generic wrapper for <paramref name="args"/>
        /// </summary>
        public static NotifyCollectionChangedEventArgs<T> As<T>(this NotifyCollectionChangedEventArgs args)
        {
            return new NotifyCollectionChangedEventArgs<T>(args);
        }

        internal static bool TryGetSingleNewItem<T>(this NotifyCollectionChangedEventArgs e, out T result)
        {
            if (e?.NewItems?.Count == 1 &&
                e.NewItems[0] is T)
            {
                result = (T)e.NewItems[0];
                return true;
            }

            result = default(T);
            return false;
        }

        internal static bool TryGetSingleOldItem<T>(this NotifyCollectionChangedEventArgs e, out T result)
        {
            if (e?.OldItems?.Count == 1 &&
                e.OldItems[0] is T)
            {
                result = (T)e.OldItems[0];
                return true;
            }

            result = default(T);
            return false;
        }
    }
}
