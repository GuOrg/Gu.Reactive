namespace Gu.Reactive
{
    using System;

    /// <summary>
    /// Buffers collectionchange notifications for an ObservableCollection.
    /// Example ten adds during BufferTime results in one Reset notification.
    /// This is useful if the view has expensive Layout
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IReadOnlyDeferredView<out T> : IReadOnlyObservableCollection<T>, IDisposable
    {
        /// <summary>
        /// The time while the collections buffers changes from the inner collection.
        /// This means the the DeferredView raises it's collection changed event after BufferTime has passed since the last collectionchange notification from the inner collection.
        /// </summary>
        TimeSpan BufferTime { get; }

        /// <summary>
        /// Synchronizes the view with the inner collection and signals collection changed..
        /// </summary>
        void Refresh();
    }
}