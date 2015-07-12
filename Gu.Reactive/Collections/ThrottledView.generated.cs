namespace Gu.Reactive
{
    using System;
    using System.Collections.ObjectModel;
    using System.Reactive.Concurrency;

    public static partial class ThrottledView
    {
        public static ThrottledView<TItem> AsThrottledView<TItem>(this ObservableCollection<TItem> collection, TimeSpan bufferTime)
        {
            return new ThrottledView<TItem>(collection, bufferTime, null);
        }

        public static ThrottledView<TItem> AsThrottledView<TItem>(this ObservableCollection<TItem> collection, TimeSpan bufferTime, IScheduler scheduler)
        {
            return new ThrottledView<TItem>(collection, bufferTime, scheduler);
        }

        public static ThrottledView<TItem> AsThrottledView<TItem>(this IObservableCollection<TItem> collection, TimeSpan bufferTime)
        {
            return new ThrottledView<TItem>(collection, bufferTime, null);
        }

        public static ThrottledView<TItem> AsThrottledView<TItem>(this IObservableCollection<TItem> collection, TimeSpan bufferTime, IScheduler scheduler)
        {
            return new ThrottledView<TItem>(collection, bufferTime, scheduler);
        }

        public static ReadOnlyThrottledView<TItem> AsReadOnlyThrottledView<TItem>(this ObservableCollection<TItem> collection, TimeSpan bufferTime)
        {
            return new ReadOnlyThrottledView<TItem>(collection, bufferTime, null);
        }

        public static ReadOnlyThrottledView<TItem> AsReadOnlyThrottledView<TItem>(this ObservableCollection<TItem> collection, TimeSpan bufferTime, IScheduler scheduler)
        {
            return new ReadOnlyThrottledView<TItem>(collection, bufferTime, scheduler);
        }

        public static ReadOnlyThrottledView<TItem> AsReadOnlyThrottledView<TItem>(this ReadOnlyObservableCollection<TItem> collection, TimeSpan bufferTime)
        {
            return new ReadOnlyThrottledView<TItem>(collection, bufferTime, null);
        }

        public static ReadOnlyThrottledView<TItem> AsReadOnlyThrottledView<TItem>(this ReadOnlyObservableCollection<TItem> collection, TimeSpan bufferTime, IScheduler scheduler)
        {
            return new ReadOnlyThrottledView<TItem>(collection, bufferTime, scheduler);
        }

        public static ReadOnlyThrottledView<TItem> AsReadOnlyThrottledView<TItem>(this IReadOnlyObservableCollection<TItem> collection, TimeSpan bufferTime)
        {
            return new ReadOnlyThrottledView<TItem>(collection, bufferTime, null);
        }

        public static ReadOnlyThrottledView<TItem> AsReadOnlyThrottledView<TItem>(this IReadOnlyObservableCollection<TItem> collection, TimeSpan bufferTime, IScheduler scheduler)
        {
            return new ReadOnlyThrottledView<TItem>(collection, bufferTime, scheduler);
        }
    }
}