#pragma warning disable RS0026 // Do not add multiple public overloads with optional parameters
namespace Gu.Wpf.Reactive
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;

    using Gu.Reactive;

    /// <summary>
    /// A readonly view of an <see cref="ObservableCollection{T}"/> that notifies on the dispatcher.
    /// </summary>
    /// <typeparam name="T">The type of the items in the source collection.</typeparam>
    [DebuggerTypeProxy(typeof(CollectionDebugView<>))]
    [DebuggerDisplay("Count = {this.Count}")]
    [Obsolete("This will be removed in future version. Prefer alternatives taking a scheduler.")]
#pragma warning disable CA1010 // Collections should implement generic interface
    public class ReadOnlyDispatchingView<T> : ReadOnlyThrottledView<T>
#pragma warning restore CA1010 // Collections should implement generic interface
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyDispatchingView{T}"/> class.
        /// </summary>
        /// <param name="source">The source collection.</param>
        /// <param name="bufferTime">The time to buffer changes before notifying.</param>
        /// <param name="leaveOpen">True means that the <paramref name="source"/> is not disposed when this instance is disposed.</param>
        public ReadOnlyDispatchingView(ObservableCollection<T> source, TimeSpan bufferTime, bool leaveOpen = false)
            : base(source, bufferTime, WpfSchedulers.Dispatcher, leaveOpen)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyDispatchingView{T}"/> class.
        /// </summary>
        /// <param name="source">The source collection.</param>
        /// <param name="bufferTime">The time to buffer changes before notifying.</param>
        /// <param name="leaveOpen">True means that the <paramref name="source"/> is not disposed when this instance is disposed.</param>
        public ReadOnlyDispatchingView(IObservableCollection<T> source, TimeSpan bufferTime, bool leaveOpen = false)
            : base(source, bufferTime, WpfSchedulers.Dispatcher, leaveOpen)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyDispatchingView{T}"/> class.
        /// </summary>
        /// <param name="source">The source collection.</param>
        /// <param name="bufferTime">The time to buffer changes before notifying.</param>
        /// <param name="leaveOpen">True means that the <paramref name="source"/> is not disposed when this instance is disposed.</param>
        public ReadOnlyDispatchingView(ReadOnlyObservableCollection<T> source, TimeSpan bufferTime, bool leaveOpen = false)
            : base(source, bufferTime, WpfSchedulers.Dispatcher, leaveOpen)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyDispatchingView{T}"/> class.
        /// </summary>
        /// <param name="source">The source collection.</param>
        /// <param name="bufferTime">The time to buffer changes before notifying.</param>
        /// <param name="leaveOpen">True means that the <paramref name="source"/> is not disposed when this instance is disposed.</param>
        public ReadOnlyDispatchingView(IReadOnlyObservableCollection<T> source, TimeSpan bufferTime, bool leaveOpen = false)
            : base(source, bufferTime, WpfSchedulers.Dispatcher, leaveOpen)
        {
        }
    }
}
