namespace Gu.Reactive
{
    using System.Collections.Specialized;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Factory methods for creating <see cref="NotifyCollectionChangedEventArgs{T}"/>.
    /// </summary>
    public static class NotifyCollectionChangedEventArgsExt
    {
        /// <summary>
        /// Create a generic wrapper for <paramref name="args"/>.
        /// </summary>
        /// <typeparam name="T">The type of the items in the collection.</typeparam>
        /// <param name="args">The <see cref="NotifyCollectionChangedEventArgs"/>.</param>
        /// <returns>The <see cref="NotifyCollectionChangedEventArgs{T}"/>.</returns>
        public static NotifyCollectionChangedEventArgs<T> As<T>(this NotifyCollectionChangedEventArgs args)
        {
            return new NotifyCollectionChangedEventArgs<T>(args);
        }

        internal static bool TryGetSingleNewItem<T>(this NotifyCollectionChangedEventArgs e, [MaybeNullWhen(false)] out T result)
        {
            if (e?.NewItems?.Count == 1 &&
                e.NewItems[0] is T variable)
            {
                result = variable;
                return true;
            }

            result = default!;
            return false;
        }

        internal static bool TryGetSingleOldItem<T>(this NotifyCollectionChangedEventArgs e, [MaybeNullWhen(false)] out T result)
        {
            if (e?.OldItems?.Count == 1 &&
                e.OldItems[0] is T variable)
            {
                result = variable;
                return true;
            }

            result = default!;
            return false;
        }
    }
}
