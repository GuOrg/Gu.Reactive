namespace Gu.Reactive
{
    using System;
    using System.Collections.Specialized;

    /// <summary>
    /// Factory methods for creating <see cref="NotifyCollectionChangedEventArgs{T}"/>
    /// </summary>
    public static class NotifyCollectionChangedEventArgsExt
    {
        internal static bool IsSingleNewItem(this NotifyCollectionChangedEventArgs e)
        {
            return e?.NewItems?.Count == 1;
        }

        internal static T SingleNewItem<T>(this NotifyCollectionChangedEventArgs e)
        {
            if (!e.IsSingleNewItem())
            {
                throw new InvalidOperationException("Expected single new item");
            }

            return (T)e.NewItems[0];
        }

        internal static bool IsSingleOldItem(this NotifyCollectionChangedEventArgs e)
        {
            return e?.OldItems?.Count == 1;
        }

        internal static T SingleOldItem<T>(this NotifyCollectionChangedEventArgs e)
        {
            if (!e.IsSingleOldItem())
            {
                throw new InvalidOperationException("Expected single old item");
            }

            return (T)e.OldItems[0];
        }

        /// <summary>
        /// Create a generic wrapper for <paramref name="args"/>
        /// </summary>
        public static NotifyCollectionChangedEventArgs<T> As<T>(this NotifyCollectionChangedEventArgs args)
        {
            return new NotifyCollectionChangedEventArgs<T>(args);
        }
    }
}
