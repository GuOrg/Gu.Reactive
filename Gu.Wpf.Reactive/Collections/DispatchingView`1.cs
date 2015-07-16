namespace Gu.Wpf.Reactive
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;

    using Gu.Reactive;

    [DebuggerTypeProxy(typeof(CollectionDebugView<>))]
    [DebuggerDisplay("Count = {Count}")] 
    public class DispatchingView<T> : ThrottledView<T>
    {
        public DispatchingView(ObservableCollection<T> source)
            : base(source, TimeSpan.Zero, Schedulers.DispatcherOrCurrentThread)
        {
        }

        public DispatchingView(ObservableCollection<T> source, TimeSpan bufferTime)
            : base(source, bufferTime, Schedulers.DispatcherOrCurrentThread)
        {
        }

        public DispatchingView(IObservableCollection<T> source)
            : base(source, TimeSpan.Zero, Schedulers.DispatcherOrCurrentThread)
        {
        }

        public DispatchingView(IObservableCollection<T> source, TimeSpan bufferTime)
            : base(source, bufferTime, Schedulers.DispatcherOrCurrentThread)
        {
        }
    }
}
