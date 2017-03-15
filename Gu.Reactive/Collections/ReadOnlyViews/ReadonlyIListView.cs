namespace Gu.Reactive
{
    /// <summary>
    /// Factory methods for creating <see cref="ReadOnlyIListView{T}"/>
    /// </summary>
    public static class ReadonlyIListView
    {
        /// <summary>
        /// Create a <see cref="ReadOnlyIListView{T}"/> from <paramref name="source"/>
        /// </summary>
        public static ReadOnlyIListView<T> AsReadonlyIListView<T>(this IObservableCollection<T> source)
        {
            return new ReadOnlyIListView<T>(source);
        }

        /// <summary>
        /// Create a <see cref="ReadOnlyIListView{T}"/> from <paramref name="source"/>
        /// </summary>
        public static ReadOnlyIListView<T> AsReadonlyIListView<T>(this IReadOnlyObservableCollection<T> source)
        {
            return new ReadOnlyIListView<T>(source);
        }
    }
}