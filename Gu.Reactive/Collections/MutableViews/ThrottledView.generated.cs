namespace Gu.Reactive
{
    using System;
    using System.Collections.ObjectModel;
    using System.Reactive.Concurrency;

    /// <summary>
    /// Factory methods for creating <see cref="ThrottledView{T}"/>
    /// </summary>
    [Obsolete("Candidate for removal, broken. Prefer the read only version.")]
    public static partial class ThrottledView
    {
        /// <summary>
        /// Create a <see cref="ThrottledView{T}"/> view for <paramref name="collection"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <param name="collection">The source collection</param>
        /// <param name="bufferTime">The time to buffer changes in <paramref name="collection"/></param>
        /// <returns>A <see cref="ThrottledView{T}"/></returns>
        [Obsolete("Candidate for removal, broken. Prefer the read only version.")]
        public static ThrottledView<T> AsThrottledView<T>(this ObservableCollection<T> collection, TimeSpan bufferTime)
        {
            return new ThrottledView<T>(collection, bufferTime, null);
        }

        /// <summary>
        /// Create a <see cref="ThrottledView{T}"/> view for <paramref name="collection"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <param name="collection">The source collection</param>
        /// <param name="bufferTime">The time to buffer changes in <paramref name="collection"/></param>
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        /// <returns>A <see cref="ThrottledView{T}"/></returns>
        [Obsolete("Candidate for removal, broken. Prefer the read only version.")]
        public static ThrottledView<T> AsThrottledView<T>(this ObservableCollection<T> collection, TimeSpan bufferTime, IScheduler scheduler)
        {
            return new ThrottledView<T>(collection, bufferTime, scheduler);
        }

        /// <summary>
        /// Create a <see cref="ThrottledView{T}"/> view for <paramref name="collection"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <param name="collection">The source collection</param>
        /// <param name="bufferTime">The time to buffer changes in <paramref name="collection"/></param>
        /// <returns>A <see cref="ThrottledView{T}"/></returns>
        [Obsolete("Candidate for removal, broken. Prefer the read only version.")]
        public static ThrottledView<T> AsThrottledView<T>(this IObservableCollection<T> collection, TimeSpan bufferTime)
        {
            return new ThrottledView<T>(collection, bufferTime, null);
        }

        /// <summary>
        /// Create a <see cref="ThrottledView{T}"/> view for <paramref name="collection"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <param name="collection">The source collection</param>
        /// <param name="bufferTime">The time to buffer changes in <paramref name="collection"/></param>
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        /// <returns>A <see cref="ThrottledView{T}"/></returns>
        [Obsolete("Candidate for removal, broken. Prefer the read only version.")]
        public static ThrottledView<T> AsThrottledView<T>(this IObservableCollection<T> collection, TimeSpan bufferTime, IScheduler scheduler)
        {
            return new ThrottledView<T>(collection, bufferTime, scheduler);
        }
    }
}