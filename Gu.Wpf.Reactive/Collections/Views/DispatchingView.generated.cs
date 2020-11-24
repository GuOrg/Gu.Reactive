#nullable enable
namespace Gu.Wpf.Reactive
{
    using System;
    using System.Collections.ObjectModel;

    using Gu.Reactive;

    /// <summary>
    /// Factory methods for creating <see cref="DispatchingView{T}"/>
    /// </summary>
    public static partial class DispatchingView
    {
        /// <summary>
        /// Create a <see cref="DispatchingView{T}"/> for <paramref name="collection"/>
        /// </summary>
        /// <typeparam name="TItem">The type of the items in <paramref name="collection"/></typeparam>
        /// <param name="collection">The source collection.</param>
        /// <param name="leaveOpen">True means that the <paramref name="collection"/> is not disposed when this instance is disposed.</param>
        /// <returns>A <see cref="DispatchingView{T}"/></returns>
        public static DispatchingView<TItem> AsDispatchingView<TItem>(this IObservableCollection<TItem> collection, bool leaveOpen = false)
        {
            return new DispatchingView<TItem>(collection, TimeSpan.Zero, leaveOpen);
        }

        /// <summary>
        /// Create a <see cref="DispatchingView{T}"/> for <paramref name="collection"/>
        /// </summary>
        /// <typeparam name="TItem">The type of the items in <paramref name="collection"/></typeparam>
        /// <param name="collection">The source collection.</param>
        /// <param name="bufferTime">The time to buffer changes in <paramref name="collection"/></param>
        /// <param name="leaveOpen">True means that the <paramref name="collection"/> is not disposed when this instance is disposed.</param>
        /// <returns>A <see cref="DispatchingView{T}"/></returns>
        public static DispatchingView<TItem> AsDispatchingView<TItem>(this IObservableCollection<TItem> collection, TimeSpan bufferTime, bool leaveOpen = false)
        {
            return new DispatchingView<TItem>(collection, bufferTime, leaveOpen);
        }

        /// <summary>
        /// Create a <see cref="DispatchingView{T}"/> for <paramref name="collection"/>
        /// </summary>
        /// <typeparam name="TItem">The type of the items in <paramref name="collection"/></typeparam>
        /// <param name="collection">The source collection.</param>
        /// <param name="leaveOpen">True means that the <paramref name="collection"/> is not disposed when this instance is disposed.</param>
        /// <returns>A <see cref="DispatchingView{T}"/></returns>
        public static DispatchingView<TItem> AsDispatchingView<TItem>(this ObservableCollection<TItem> collection, bool leaveOpen = false)
        {
            return new DispatchingView<TItem>(collection, TimeSpan.Zero, leaveOpen);
        }

        /// <summary>
        /// Create a <see cref="DispatchingView{T}"/> for <paramref name="collection"/>
        /// </summary>
        /// <typeparam name="TItem">The type of the items in <paramref name="collection"/></typeparam>
        /// <param name="collection">The source collection.</param>
        /// <param name="bufferTime">The time to buffer changes in <paramref name="collection"/></param>
        /// <param name="leaveOpen">True means that the <paramref name="collection"/> is not disposed when this instance is disposed.</param>
        /// <returns>A <see cref="DispatchingView{T}"/></returns>
        public static DispatchingView<TItem> AsDispatchingView<TItem>(this ObservableCollection<TItem> collection, TimeSpan bufferTime, bool leaveOpen = false)
        {
            return new DispatchingView<TItem>(collection, bufferTime, leaveOpen);
        }
    }
}