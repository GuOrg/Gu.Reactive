namespace Gu.Wpf.Reactive
{
    using System;
    using System.Collections.ObjectModel;
    using System.Reactive.Concurrency;

    using Gu.Reactive;

    public static partial class DispatchingView
    {
        public static DispatchingView<TItem> AsDispatchingView<TItem>(this IObservableCollection<TItem> collection)
        {
            return new DispatchingView<TItem>(collection, TimeSpan.Zero);
        }

        public static DispatchingView<TItem> AsDispatchingView<TItem>(this IObservableCollection<TItem> collection, TimeSpan bufferTime)
        {
            return new DispatchingView<TItem>(collection, bufferTime);
        }

        public static DispatchingView<TItem> AsDispatchingView<TItem>(this ObservableCollection<TItem> collection)
        {
            return new DispatchingView<TItem>(collection, TimeSpan.Zero);
        }

        public static DispatchingView<TItem> AsDispatchingView<TItem>(this ObservableCollection<TItem> collection, TimeSpan bufferTime)
        {
            return new DispatchingView<TItem>(collection, bufferTime);
        }

        public static ReadOnlyDispatchingView<TItem> AsDispatchingView<TItem>(this ReadOnlyObservableCollection<TItem> collection)
        {
            return new ReadOnlyDispatchingView<TItem>(collection, TimeSpan.Zero);
        }

        public static ReadOnlyDispatchingView<TItem> AsDispatchingView<TItem>(this ReadOnlyObservableCollection<TItem> collection, TimeSpan bufferTime, IScheduler scheduler)
        {
            return new ReadOnlyDispatchingView<TItem>(collection, bufferTime);
        }

        public static ReadOnlyDispatchingView<TItem> AsDispatchingView<TItem>(this IReadOnlyObservableCollection<TItem> collection)
        {
            return new ReadOnlyDispatchingView<TItem>(collection, TimeSpan.Zero);
        }

        public static ReadOnlyDispatchingView<TItem> AsDispatchingView<TItem>(this IReadOnlyObservableCollection<TItem> collection, TimeSpan bufferTime, IScheduler scheduler)
        {
            return new ReadOnlyDispatchingView<TItem>(collection, bufferTime);
        }
    }
}