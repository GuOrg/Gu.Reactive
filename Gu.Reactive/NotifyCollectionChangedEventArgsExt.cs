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
    }
}
