namespace Gu.Wpf.Reactive
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;

    using Gu.Reactive;

    [DebuggerTypeProxy(typeof(CollectionDebugView<>))]
    [DebuggerDisplay("Count = {Count}")]
    public class ReadOnlyDispatchingView<T> : ReadOnlyThrottledView<T>
    {
        public ReadOnlyDispatchingView(ObservableCollection<T> inner, TimeSpan bufferTime)
            : base(inner, bufferTime, WpfSchedulers.Dispatcher)
        {
        }

        public ReadOnlyDispatchingView(IObservableCollection<T> inner, TimeSpan bufferTime)
            : base(inner, bufferTime, WpfSchedulers.Dispatcher)
        {
        }

        public ReadOnlyDispatchingView(ReadOnlyObservableCollection<T> inner, TimeSpan bufferTime)
            : base(inner, bufferTime, WpfSchedulers.Dispatcher)
        {
        }

        public ReadOnlyDispatchingView(IReadOnlyObservableCollection<T> inner, TimeSpan bufferTime)
            : base(inner, bufferTime, WpfSchedulers.Dispatcher)
        {
        }
    }
}