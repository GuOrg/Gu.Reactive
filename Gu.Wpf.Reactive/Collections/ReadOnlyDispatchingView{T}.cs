namespace Gu.Wpf.Reactive
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;

    using Gu.Reactive;

    /// <summary>
    /// A view of an <see cref="ObservableCollection{T}"/> that notifies on the dispatcher.
    /// </summary>
    /// <typeparam name="T">The type of the items in the source collection.</typeparam>
    [DebuggerTypeProxy(typeof(CollectionDebugView<>))]
    // ReSharper disable once UseNameofExpression
    [DebuggerDisplay("Count = {Count}")]
    public class ReadOnlyDispatchingView<T> : ReadOnlyThrottledView<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyDispatchingView{T}"/> class.
        /// </summary>
        /// <param name="inner">The source collection.</param>
        /// <param name="bufferTime">The time to buffer changes before notifying.</param>
        public ReadOnlyDispatchingView(ObservableCollection<T> inner, TimeSpan bufferTime)
            : base(inner, bufferTime, WpfSchedulers.Dispatcher)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyDispatchingView{T}"/> class.
        /// </summary>
        /// <param name="inner">The source collection.</param>
        /// <param name="bufferTime">The time to buffer changes before notifying.</param>
        public ReadOnlyDispatchingView(IObservableCollection<T> inner, TimeSpan bufferTime)
            : base(inner, bufferTime, WpfSchedulers.Dispatcher)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyDispatchingView{T}"/> class.
        /// </summary>
        /// <param name="inner">The source collection.</param>
        /// <param name="bufferTime">The time to buffer changes before notifying.</param>
        public ReadOnlyDispatchingView(ReadOnlyObservableCollection<T> inner, TimeSpan bufferTime)
            : base(inner, bufferTime, WpfSchedulers.Dispatcher)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyDispatchingView{T}"/> class.
        /// </summary>
        /// <param name="inner">The source collection.</param>
        /// <param name="bufferTime">The time to buffer changes before notifying.</param>
        public ReadOnlyDispatchingView(IReadOnlyObservableCollection<T> inner, TimeSpan bufferTime)
            : base(inner, bufferTime, WpfSchedulers.Dispatcher)
        {
        }
    }
}