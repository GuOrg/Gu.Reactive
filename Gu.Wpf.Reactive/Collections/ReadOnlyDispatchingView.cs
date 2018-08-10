namespace Gu.Wpf.Reactive
{
    using System;
    using System.Collections.ObjectModel;

    using Gu.Reactive;

    /// <summary>
    /// Factory methods for creating <see cref="ReadOnlyDispatchingView{T}"/>.
    /// </summary>
    public static class ReadOnlyDispatchingView
    {
        /// <summary>
        /// Create a <see cref="ReadOnlyDispatchingView{T}"/> for <paramref name="source"/>.
        /// </summary>
        /// <typeparam name="TItem">The type of the items in <paramref name="source"/>.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="leaveOpen">True means that the <paramref name="source"/> is not disposed when this instance is diposed.</param>
        /// <returns>A <see cref="ReadOnlyDispatchingView{T}"/>.</returns>
        public static ReadOnlyDispatchingView<TItem> AsReadOnlyDispatchingView<TItem>(this ObservableCollection<TItem> source, bool leaveOpen = false)
        {
            return new ReadOnlyDispatchingView<TItem>(source, TimeSpan.Zero, leaveOpen);
        }

        /// <summary>
        /// Create a <see cref="ReadOnlyDispatchingView{T}"/> for <paramref name="source"/>.
        /// </summary>
        /// <typeparam name="TItem">The type of the items in <paramref name="source"/>.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="bufferTime">The time to buffer changes before notifying.</param>
        /// <param name="leaveOpen">True means that the <paramref name="source"/> is not disposed when this instance is diposed.</param>
        /// <returns>A <see cref="ReadOnlyDispatchingView{T}"/>.</returns>
        public static ReadOnlyDispatchingView<TItem> AsReadOnlyDispatchingView<TItem>(this ObservableCollection<TItem> source, TimeSpan bufferTime, bool leaveOpen = false)
        {
            return new ReadOnlyDispatchingView<TItem>(source, bufferTime, leaveOpen);
        }

        /// <summary>
        /// Create a <see cref="ReadOnlyDispatchingView{T}"/> for <paramref name="source"/>.
        /// </summary>
        /// <typeparam name="TItem">The type of the items in <paramref name="source"/>.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="leaveOpen">True means that the <paramref name="source"/> is not disposed when this instance is diposed.</param>
        /// <returns>A <see cref="ReadOnlyDispatchingView{T}"/>.</returns>
        public static ReadOnlyDispatchingView<TItem> AsReadOnlyDispatchingView<TItem>(this ReadOnlyObservableCollection<TItem> source, bool leaveOpen = false)
        {
            return new ReadOnlyDispatchingView<TItem>(source, TimeSpan.Zero, leaveOpen);
        }

        /// <summary>
        /// Create a <see cref="ReadOnlyDispatchingView{T}"/> for <paramref name="source"/>.
        /// </summary>
        /// <typeparam name="TItem">The type of the items in <paramref name="source"/>.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="bufferTime">The time to buffer changes before notifying.</param>
        /// <param name="leaveOpen">True means that the <paramref name="source"/> is not disposed when this instance is diposed.</param>
        /// <returns>A <see cref="ReadOnlyDispatchingView{T}"/>.</returns>
        public static ReadOnlyDispatchingView<TItem> AsReadOnlyDispatchingView<TItem>(this ReadOnlyObservableCollection<TItem> source, TimeSpan bufferTime, bool leaveOpen = false)
        {
            return new ReadOnlyDispatchingView<TItem>(source, bufferTime, leaveOpen);
        }

        /// <summary>
        /// Create a <see cref="ReadOnlyDispatchingView{T}"/> for <paramref name="source"/>.
        /// </summary>
        /// <typeparam name="TItem">The type of the items in <paramref name="source"/>.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="leaveOpen">True means that the <paramref name="source"/> is not disposed when this instance is diposed.</param>
        /// <returns>A <see cref="ReadOnlyDispatchingView{T}"/>.</returns>
        public static ReadOnlyDispatchingView<TItem> AsReadOnlyDispatchingView<TItem>(this IReadOnlyObservableCollection<TItem> source, bool leaveOpen = false)
        {
            return new ReadOnlyDispatchingView<TItem>(source, TimeSpan.Zero, leaveOpen);
        }

        /// <summary>
        /// Create a <see cref="ReadOnlyDispatchingView{T}"/> for <paramref name="source"/>.
        /// </summary>
        /// <typeparam name="TItem">The type of the items in <paramref name="source"/>.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="bufferTime">The time to buffer changes before notifying.</param>
        /// <param name="leaveOpen">True means that the <paramref name="source"/> is not disposed when this instance is diposed.</param>
        /// <returns>A <see cref="ReadOnlyDispatchingView{T}"/>.</returns>
        public static ReadOnlyDispatchingView<TItem> AsReadOnlyDispatchingView<TItem>(this IReadOnlyObservableCollection<TItem> source, TimeSpan bufferTime, bool leaveOpen = false)
        {
            return new ReadOnlyDispatchingView<TItem>(source, bufferTime, leaveOpen);
        }
    }
}