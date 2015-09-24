namespace Gu.Reactive
{
    using System.Collections.Specialized;

    public static class NotifyCollectionChangedEventArgsExt
    {
        public static NotifyCollectionChangedEventArgs<T> As<T>(this NotifyCollectionChangedEventArgs args)
        {
            return new NotifyCollectionChangedEventArgs<T>(args);
        }
    }
}
