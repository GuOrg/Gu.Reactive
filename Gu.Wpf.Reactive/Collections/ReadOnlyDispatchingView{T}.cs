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
    public class ReadOnlyDispatchingView<T> : ReadOnlyThrottledView<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyDispatchingView{T}"/> class.
        /// </summary>
        /// <param name="source">The source collection.</param>
        /// <param name="bufferTime">The time to buffer changes before notifying.</param>
        public ReadOnlyDispatchingView(ObservableCollection<T> source, TimeSpan bufferTime)
            : base(source, bufferTime, WpfSchedulers.Dispatcher)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyDispatchingView{T}"/> class.
        /// </summary>
        /// <param name="source">The source collection.</param>
        /// <param name="bufferTime">The time to buffer changes before notifying.</param>
        public ReadOnlyDispatchingView(IObservableCollection<T> source, TimeSpan bufferTime)
            : base(source, bufferTime, WpfSchedulers.Dispatcher)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyDispatchingView{T}"/> class.
        /// </summary>
        /// <param name="source">The source collection.</param>
        /// <param name="bufferTime">The time to buffer changes before notifying.</param>
        public ReadOnlyDispatchingView(ReadOnlyObservableCollection<T> source, TimeSpan bufferTime)
            : base(source, bufferTime, WpfSchedulers.Dispatcher)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyDispatchingView{T}"/> class.
        /// </summary>
        /// <param name="source">The source collection.</param>
        /// <param name="bufferTime">The time to buffer changes before notifying.</param>
        public ReadOnlyDispatchingView(IReadOnlyObservableCollection<T> source, TimeSpan bufferTime)
            : base(source, bufferTime, WpfSchedulers.Dispatcher)
        {
        }
    }
}