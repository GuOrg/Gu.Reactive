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
    [DebuggerDisplay("Count = {Count}")]
    public class DispatchingView<T> : ThrottledView<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DispatchingView{T}"/> class.
        /// This uses buffer time <see cref="TimeSpan.Zero"/> and <see cref="WpfSchedulers.Dispatcher"/>
        /// </summary>
        /// <param name="source">The source collection.</param>
        public DispatchingView(ObservableCollection<T> source)
            : base(source, TimeSpan.Zero, WpfSchedulers.Dispatcher)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DispatchingView{T}"/> class.
        /// This uses <see cref="WpfSchedulers.Dispatcher"/>
        /// </summary>
        /// <param name="source">The source collection.</param>
        /// <param name="bufferTime">The time to buffer changes.</param>
        public DispatchingView(ObservableCollection<T> source, TimeSpan bufferTime)
            : base(source, bufferTime, WpfSchedulers.Dispatcher)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DispatchingView{T}"/> class.
        /// This uses buffer time <see cref="TimeSpan.Zero"/> and <see cref="WpfSchedulers.Dispatcher"/>
        /// </summary>
        /// <param name="source">The source collection.</param>
        public DispatchingView(IObservableCollection<T> source)
            : base(source, TimeSpan.Zero, WpfSchedulers.Dispatcher)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DispatchingView{T}"/> class.
        /// This uses <see cref="WpfSchedulers.Dispatcher"/>
        /// </summary>
        /// <param name="source">The source collection.</param>
        /// <param name="bufferTime">The time to buffer changes.</param>
        public DispatchingView(IObservableCollection<T> source, TimeSpan bufferTime)
            : base(source, bufferTime, WpfSchedulers.Dispatcher)
        {
        }
    }
}
