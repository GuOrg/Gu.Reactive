namespace Gu.Reactive
{
    /// <summary>
    /// Factory methods for creating <see cref="ReadOnlyIListView{T}"/>.
    /// </summary>
    public static class ReadonlyIListView
    {
        /// <summary>
        /// Create a <see cref="ReadOnlyIListView{T}"/> from <paramref name="source"/>.
        /// </summary>
        /// <typeparam name="T">The type of the items in the collection.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="leaveOpen">True means that the <paramref name="source"/> is not disposed when this instance is disposed.</param>
        /// <returns>A <see cref="ReadOnlyIListView{T}"/>.</returns>
        public static ReadOnlyIListView<T> AsReadonlyIListView<T>(this IObservableCollection<T> source, bool leaveOpen = false)
        {
            return new ReadOnlyIListView<T>(source, leaveOpen);
        }

        /// <summary>
        /// Create a <see cref="ReadOnlyIListView{T}"/> from <paramref name="source"/>.
        /// </summary>
        /// <typeparam name="T">The type of the items in the collection.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="leaveOpen">True means that the <paramref name="source"/> is not disposed when this instance is disposed.</param>
        /// <returns>A <see cref="ReadOnlyIListView{T}"/>.</returns>
        public static ReadOnlyIListView<T> AsReadonlyIListView<T>(this IReadOnlyObservableCollection<T> source, bool leaveOpen = false)
        {
            return new ReadOnlyIListView<T>(source, leaveOpen);
        }
    }
}
