namespace Gu.Reactive
{
    public static class ReadonlyIListView
    {
        public static ReadOnlyIListView<T> AsReadonlyIListView<T>(this IObservableCollection<T> source)
        {
            return new ReadOnlyIListView<T>(source);
        }

        public static ReadOnlyIListView<T> AsReadonlyIListView<T>(this IReadOnlyObservableCollection<T> source)
        {
            return new ReadOnlyIListView<T>(source);
        }
    }
}