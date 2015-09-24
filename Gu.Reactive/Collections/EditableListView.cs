namespace Gu.Reactive
{
    public static class EditableListView
    {
        public static EditableListView<T> AsEditableListView<T>(this IObservableCollection<T> source)
        {
            return new EditableListView<T>(source);
        }
    }
}