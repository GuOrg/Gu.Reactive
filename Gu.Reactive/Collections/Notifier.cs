namespace Gu.Reactive
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
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

        public static void Notify(
            object sender,
            IReadOnlyList<NotifyCollectionChangedEventArgs> changes,
            IScheduler scheduler,
            PropertyChangedEventHandler propHandler,
            NotifyCollectionChangedEventHandler colHandler)
        {
            if (changes == null || changes.Count == 0)
            {
                return;
            }
            if (changes.Count == 1)
            {
                Notify(sender, changes[0], scheduler, propHandler, colHandler);
                return;
            }
            NotifyReset(sender, scheduler, propHandler, colHandler);
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

        public static bool IsSingleNewItem(this NotifyCollectionChangedEventArgs e)
        {
            if (e == null)
            {
                return false;
            }
            var items = e.NewItems;
            if (items == null)
            {
                return false;
            }
            return items.Count == 1;
        }

        public static T NewItem<T>(this NotifyCollectionChangedEventArgs e)
        {
            if (!e.IsSingleNewItem())
            {
                throw new InvalidOperationException("Expected single new item");
            }
            return (T)e.NewItems[0];
        }

        public static bool IsSingleOldItem(this NotifyCollectionChangedEventArgs e)
        {
            if (e == null)
            {
                return false;
            }
            var items = e.OldItems;
            if (items == null)
            {
                return false;
            }
            return items.Count == 1;
        }

        public static T OldItem<T>(this NotifyCollectionChangedEventArgs e)
        {
            if (!e.IsSingleOldItem())
            {
                throw new InvalidOperationException("Expected single old item");
            }
            return (T)e.OldItems[0];
        }
    }
}