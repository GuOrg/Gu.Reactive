namespace Gu.Reactive
{
    using System;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Reactive.Concurrency;

    internal static class Notifier
    {
        internal static void Notify(
            object sender,
            NotifyCollectionChangedEventArgs change,
            IScheduler scheduler,
            PropertyChangedEventHandler propHandler,
            NotifyCollectionChangedEventHandler colHandler)
        {
            if ((propHandler == null && colHandler == null) || change == null)
            {
                return;
            }

            switch (change.Action)
            {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Remove:
                    propHandler.Notify(sender, CachedEventArgs.CountPropertyChanged);
                    propHandler.Notify(sender, CachedEventArgs.IndexerPropertyChanged);
                    colHandler.Notify(sender, change, scheduler);
                    break;
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Move:
                    propHandler.Notify(sender, CachedEventArgs.IndexerPropertyChanged);
                    colHandler.Notify(sender, change, scheduler);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    propHandler.Notify(sender, CachedEventArgs.CountPropertyChanged);
                    // not sure if specialcasing is needed here.
                    propHandler.Notify(sender, CachedEventArgs.IndexerPropertyChanged);
                    colHandler.Notify(sender, change, scheduler);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        internal static void Notify(this PropertyChangedEventHandler handler, object sender, PropertyChangedEventArgs e)
        {
            handler?.Invoke(sender, e);
        }

        internal static void Notify(
            this NotifyCollectionChangedEventHandler handler,
            object sender,
            NotifyCollectionChangedEventArgs e,
            IScheduler scheduler)
        {
            if (handler == null)
            {
                return;
            }

            if (scheduler != null)
            {
#pragma warning disable GU0011 // Don't ignore returnvalue
#pragma warning disable GU0033 // Don't ignore returnvalue of type IDisposable.
                scheduler.Schedule(() => handler(sender, e));
#pragma warning restore GU0033
#pragma warning restore GU0011
            }
            else
            {
                handler(sender, e);
            }
        }

        internal static bool IsSingleNewItem(this NotifyCollectionChangedEventArgs e)
        {
            return e?.NewItems?.Count == 1;
        }

        internal static T NewItem<T>(this NotifyCollectionChangedEventArgs e)
        {
            if (!e.IsSingleNewItem())
            {
                throw new InvalidOperationException("Expected single new item");
            }

            return (T)e.NewItems[0];
        }

        internal static bool IsSingleOldItem(this NotifyCollectionChangedEventArgs e)
        {
            return e?.OldItems?.Count == 1;
        }

        internal static T OldItem<T>(this NotifyCollectionChangedEventArgs e)
        {
            if (!e.IsSingleOldItem())
            {
                throw new InvalidOperationException("Expected single old item");
            }

            return (T)e.OldItems[0];
        }
    }
}