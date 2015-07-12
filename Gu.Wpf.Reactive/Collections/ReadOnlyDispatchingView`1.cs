namespace Gu.Wpf.Reactive
{
    using System;
    using System.Collections.ObjectModel;
    using Gu.Reactive;

    public class ReadOnlyDispatchingView<T> : ReadOnlyThrottledView<T>
    {
        public ReadOnlyDispatchingView(ObservableCollection<T> inner, TimeSpan bufferTime)
            : base(inner, bufferTime, Schedulers.DispatcherOrCurrentThread)
        {
        }

        public ReadOnlyDispatchingView(IObservableCollection<T> inner, TimeSpan bufferTime)
            : base(inner, bufferTime, Schedulers.DispatcherOrCurrentThread)
        {
        }

        public ReadOnlyDispatchingView(ReadOnlyObservableCollection<T> inner, TimeSpan bufferTime)
            : base(inner, bufferTime, Schedulers.DispatcherOrCurrentThread)
        {
        }

        public ReadOnlyDispatchingView(IReadOnlyObservableCollection<T> inner, TimeSpan bufferTime)
            : base(inner, bufferTime, Schedulers.DispatcherOrCurrentThread)
        {
        }
    }
}