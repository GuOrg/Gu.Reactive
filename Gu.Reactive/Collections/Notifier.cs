namespace Gu.Reactive
{
    using System;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Reactive.Concurrency;

    public static class Notifier
    {
        private const string CountName = "Count";
        private const string IndexerName = "Item[]";

        internal static readonly PropertyChangedEventArgs CountPropertyChangedEventArgs = new PropertyChangedEventArgs(CountName);
        internal static readonly PropertyChangedEventArgs IndexerPropertyChangedEventArgs = new PropertyChangedEventArgs(IndexerName);

        internal static void NotifyReset(object sender,
                                    IScheduler scheduler,
                                    PropertyChangedEventHandler propertyChangedEventHandler,
                                    NotifyCollectionChangedEventHandler collectionChangeEventHandler)
        {
            Notify(sender,
                   Diff.NotifyCollectionResetEventArgs,
                   scheduler,
                   propertyChangedEventHandler,
                   collectionChangeEventHandler);
        }

        internal static void Notify(object sender,
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
                    propHandler.Notify(sender, CountPropertyChangedEventArgs);
                    propHandler.Notify(sender, IndexerPropertyChangedEventArgs);
                    colHandler.Notify(sender, change, scheduler);
                    break;
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Move:
                    propHandler.Notify(sender, IndexerPropertyChangedEventArgs);
                    colHandler.Notify(sender, change, scheduler);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    propHandler.Notify(sender, CountPropertyChangedEventArgs); // not sure if specialcasing is needed here.
                    propHandler.Notify(sender, IndexerPropertyChangedEventArgs);
                    colHandler.Notify(sender, change, scheduler);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        internal static void Notify(this PropertyChangedEventHandler handler, object sender, PropertyChangedEventArgs e)
        {
            if (handler == null)
            {
                return;
            }
            handler(sender, e);
        }

        internal static void Notify(this NotifyCollectionChangedEventHandler handler, object sender, NotifyCollectionChangedEventArgs e, IScheduler scheduler)
        {
            if (handler == null)
            {
                return;
            }
            if (scheduler != null)
            {
                scheduler.Schedule(() => handler(sender, e));
            }
            else
            {
                handler(sender, e);
            }
        }

        internal static object NewItem(this NotifyCollectionChangedEventArgs e)
        {
            if (e == null || e.NewItems == null || e.NewItems.Count != 1)
            {
                return null;
            }
            return e.NewItems[0];
        }

        internal static object OldItem(this NotifyCollectionChangedEventArgs e)
        {
            if (e == null || e.OldItems == null || e.OldItems.Count != 1)
            {
                return null;
            }
            return e.OldItems[0];
        }
    }
}