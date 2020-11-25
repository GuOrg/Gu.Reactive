#nullable enable
#pragma warning disable RS0026 // Do not add multiple public overloads with optional parameters
namespace Gu.Reactive
{
    using System;
    using System.Collections.ObjectModel;
    using System.Reactive.Concurrency;

    /// <summary>
    /// Factory methods for creating <see cref="ReadOnlyThrottledView{T}"/>
    /// </summary>
    public static partial class ReadOnlyThrottledView
    {
        /// <summary>
        /// Create a <see cref="ReadOnlyThrottledView{T}"/> view for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="bufferTime">The time to buffer changes in <paramref name="source"/></param>
        /// <param name="leaveOpen">True means that the <paramref name="source"/> is not disposed when this instance is disposed.</param>
        /// <returns>A <see cref="ReadOnlyThrottledView{T}"/></returns>
        public static ReadOnlyThrottledView<T> AsReadOnlyThrottledView<T>(this ObservableCollection<T> source, TimeSpan bufferTime, bool leaveOpen = false)
        {
            return new ReadOnlyThrottledView<T>(source, bufferTime, null, leaveOpen);
        }

        /// <summary>
        /// Create a <see cref="ReadOnlyThrottledView{T}"/> view for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="bufferTime">The time to buffer changes in <paramref name="source"/></param>
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        /// <param name="leaveOpen">True means that the <paramref name="source"/> is not disposed when this instance is disposed.</param>
        /// <returns>A <see cref="ReadOnlyThrottledView{T}"/></returns>
        public static ReadOnlyThrottledView<T> AsReadOnlyThrottledView<T>(this ObservableCollection<T> source, TimeSpan bufferTime, IScheduler scheduler, bool leaveOpen = false)
        {
            return new ReadOnlyThrottledView<T>(source, bufferTime, scheduler, leaveOpen);
        }

        /// <summary>
        /// Create a <see cref="ReadOnlyThrottledView{T}"/> view for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="bufferTime">The time to buffer changes in <paramref name="source"/></param>
        /// <param name="leaveOpen">True means that the <paramref name="source"/> is not disposed when this instance is disposed.</param>
        /// <returns>A <see cref="ReadOnlyThrottledView{T}"/></returns>
        public static ReadOnlyThrottledView<T> AsReadOnlyThrottledView<T>(this ReadOnlyObservableCollection<T> source, TimeSpan bufferTime, bool leaveOpen = false)
        {
            return new ReadOnlyThrottledView<T>(source, bufferTime, null, leaveOpen);
        }

        /// <summary>
        /// Create a <see cref="ReadOnlyThrottledView{T}"/> view for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="bufferTime">The time to buffer changes in <paramref name="source"/></param>
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        /// <param name="leaveOpen">True means that the <paramref name="source"/> is not disposed when this instance is disposed.</param>
        /// <returns>A <see cref="ReadOnlyThrottledView{T}"/></returns>
        public static ReadOnlyThrottledView<T> AsReadOnlyThrottledView<T>(this ReadOnlyObservableCollection<T> source, TimeSpan bufferTime, IScheduler scheduler, bool leaveOpen = false)
        {
            return new ReadOnlyThrottledView<T>(source, bufferTime, scheduler, leaveOpen);
        }

        /// <summary>
        /// Create a <see cref="ReadOnlyThrottledView{T}"/> view for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="bufferTime">The time to buffer changes in <paramref name="source"/></param>
        /// <param name="leaveOpen">True means that the <paramref name="source"/> is not disposed when this instance is disposed.</param>
        /// <returns>A <see cref="ReadOnlyThrottledView{T}"/></returns>
        public static ReadOnlyThrottledView<T> AsReadOnlyThrottledView<T>(this IReadOnlyObservableCollection<T> source, TimeSpan bufferTime, bool leaveOpen = false)
        {
            return new ReadOnlyThrottledView<T>(source, bufferTime, null, leaveOpen);
        }

        /// <summary>
        /// Create a <see cref="ReadOnlyThrottledView{T}"/> view for <paramref name="source"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <param name="source">The source collection</param>
        /// <param name="bufferTime">The time to buffer changes in <paramref name="source"/></param>
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        /// <param name="leaveOpen">True means that the <paramref name="source"/> is not disposed when this instance is disposed.</param>
        /// <returns>A <see cref="ReadOnlyThrottledView{T}"/></returns>
        public static ReadOnlyThrottledView<T> AsReadOnlyThrottledView<T>(this IReadOnlyObservableCollection<T> source, TimeSpan bufferTime, IScheduler scheduler, bool leaveOpen = false)
        {
            return new ReadOnlyThrottledView<T>(source, bufferTime, scheduler, leaveOpen);
        }
    }
}