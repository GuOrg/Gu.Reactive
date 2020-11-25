#nullable enable
#pragma warning disable RS0026 // Do not add multiple public overloads with optional parameters
namespace Gu.Wpf.Reactive
{
    using System;
    using System.Collections.ObjectModel;

    using Gu.Reactive;

    /// <summary>
    /// Factory methods for creating <see cref="ThrottledView{T}"/>
    /// </summary>
    public static partial class ThrottledView
    {
        /// <summary>
        /// Create a <see cref="ThrottledView{T}"/> view for <paramref name="collection"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <param name="collection">The source collection</param>
        /// <param name="bufferTime">The time to buffer changes in <paramref name="collection"/></param>
        /// <param name="leaveOpen">True means that the <paramref name="collection"/> is not disposed when this instance is disposed.</param>
        /// <returns>A <see cref="ThrottledView{T}"/></returns>
        public static ThrottledView<T> AsThrottledView<T>(this ObservableCollection<T> collection, TimeSpan bufferTime, bool leaveOpen = false)
        {
            return new ThrottledView<T>(collection, bufferTime, leaveOpen);
        }

        /// <summary>
        /// Create a <see cref="ThrottledView{T}"/> view for <paramref name="collection"/>
        /// </summary>
        /// <typeparam name="T">The type of the elements in the collection.</typeparam>
        /// <param name="collection">The source collection</param>
        /// <param name="bufferTime">The time to buffer changes in <paramref name="collection"/></param>
        /// <param name="leaveOpen">True means that the <paramref name="collection"/> is not disposed when this instance is disposed.</param>
        /// <returns>A <see cref="ThrottledView{T}"/></returns>
        public static ThrottledView<T> AsThrottledView<T>(this IObservableCollection<T> collection, TimeSpan bufferTime, bool leaveOpen = false)
        {
            return new ThrottledView<T>(collection, bufferTime, leaveOpen);
        }
    }
}