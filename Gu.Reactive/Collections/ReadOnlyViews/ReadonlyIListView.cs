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
        /// <param name="source">The source collection.</param>
        public static ReadOnlyIListView<T> AsReadonlyIListView<T>(this IObservableCollection<T> source, bool leaveOpen = false)
        {
            return new ReadOnlyIListView<T>(source, leaveOpen);
        }

        /// <summary>
        /// Create a <see cref="ReadOnlyIListView{T}"/> from <paramref name="source"/>.
        /// </summary>
        /// <param name="source">The source collection.</param>
        public static ReadOnlyIListView<T> AsReadonlyIListView<T>(this IReadOnlyObservableCollection<T> source, bool leaveOpen = false)
        {
            return new ReadOnlyIListView<T>(source, leaveOpen);
        }
    }
}
