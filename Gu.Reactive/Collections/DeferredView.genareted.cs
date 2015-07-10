namespace Gu.Reactive
{
    using System;
    using System.Collections.ObjectModel;
    using System.Reactive.Concurrency;

    public static partial class DeferredView
    {
        public static DeferredView<TItem> AsDeferredView<TItem>(this IObservableCollection<TItem> collection, TimeSpan bufferTime)
        {
            return new DeferredView<TItem>(collection, bufferTime, null);
        }

        public static DeferredView<TItem> AsDeferredView<TItem>(this IObservableCollection<TItem> collection, TimeSpan bufferTime, IScheduler scheduler)
        {
            return new DeferredView<TItem>(collection, bufferTime, scheduler);
        }

        public static DeferredView<TItem> AsDeferredView<TItem>(this ObservableCollection<TItem> collection, TimeSpan bufferTime)
        {
            return new DeferredView<TItem>(collection, bufferTime, null);
        }

        public static DeferredView<TItem> AsDeferredView<TItem>(this ObservableCollection<TItem> collection, TimeSpan bufferTime, IScheduler scheduler)
        {
            return new DeferredView<TItem>(collection, bufferTime, scheduler);
        }

        public static ReadOnlyDeferredView<TItem> AsDeferredView<TItem>(this ReadOnlyObservableCollection<TItem> collection, TimeSpan bufferTime)
        {
            return new ReadOnlyDeferredView<TItem>(collection, bufferTime, null);
        }

        public static ReadOnlyDeferredView<TItem> AsDeferredView<TItem>(this ReadOnlyObservableCollection<TItem> collection, TimeSpan bufferTime, IScheduler scheduler)
        {
            return new ReadOnlyDeferredView<TItem>(collection, bufferTime, scheduler);
        }

        public static ReadOnlyDeferredView<TItem> AsDeferredView<TItem>(this IReadOnlyObservableCollection<TItem> collection, TimeSpan bufferTime)
        {
            return new ReadOnlyDeferredView<TItem>(collection, bufferTime, null);
        }

        public static ReadOnlyDeferredView<TItem> AsDeferredView<TItem>(this IReadOnlyObservableCollection<TItem> collection, TimeSpan bufferTime, IScheduler scheduler)
        {
            return new ReadOnlyDeferredView<TItem>(collection, bufferTime, scheduler);
        }
    }
}