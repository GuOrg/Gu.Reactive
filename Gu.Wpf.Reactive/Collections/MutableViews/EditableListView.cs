namespace Gu.Wpf.Reactive
{
    using Gu.Reactive;

    /// <summary>
    /// Factory methods for creating <see cref="EditableListView{T}"/>
    /// </summary>
    public static class EditableListView
    {
        /// <summary>
        /// Creates an <see cref="EditableListView{T}"/> for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <returns>An <see cref="EditableListView{T}"/></returns>
        public static EditableListView<T> AsEditableListView<T>(this IObservableCollection<T> source)
        {
            return new EditableListView<T>(source);
        }
    }
}