namespace Gu.Wpf.Reactive.Tests.FakesAndHelpers
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using Gu.Reactive.Tests.Helpers;

    public class EventArgsComparer : IComparer
    {
        public static readonly EventArgsComparer Default = new EventArgsComparer();

        private EventArgsComparer()
        {
        }

        public int Compare(object x, object y)
        {
            if (Equals(x, y))
            {
                return 0;
            }

            var collectionChangedEventArgs = x as NotifyCollectionChangedEventArgs;
            if (collectionChangedEventArgs != null)
            {
                return this.Compare(collectionChangedEventArgs, y);
            }

            return this.Compare((PropertyChangedEventArgs)x, y);
        }

        public int Compare(NotifyCollectionChangedEventArgs x, object y)
        {
            if (y == null)
            {
                DumpIfDebug.WriteLine("NotifyCollectionChangedEventArgs Null");
                return -1;
            }

            var yArgs = y as NotifyCollectionChangedEventArgs;
            if (yArgs == null)
            {
                DumpIfDebug.WriteLine(
                    "NotifyCollectionChangedEventArgs Type, expected: {0} was: {1}",
                    typeof(NotifyCollectionChangedEventArgs),
                    y.GetType());
                return -1;
            }

            if (x.Action != yArgs.Action)
            {
                DumpIfDebug.WriteLine("NotifyCollectionChangedEventArgs Action, expected: {0} was: {1}", x.Action, yArgs.Action);
                return -1;
            }

            if (x.NewStartingIndex != yArgs.NewStartingIndex)
            {
                DumpIfDebug.WriteLine("NotifyCollectionChangedEventArgs NewStartingIndex, expected: {0} was: {1}", x.NewStartingIndex, yArgs.NewStartingIndex);
                return -1;
            }

            if (!ListsEquals(x.NewItems, yArgs.NewItems))
            {
                var xItems = string.Join(", ", x.NewItems);
                var yItems = string.Join(", ", yArgs.NewItems);
                DumpIfDebug.WriteLine("NotifyCollectionChangedEventArgs NewItems, expected: {0} was: {1}", xItems, yItems);
                return -1;
            }

            if (x.OldStartingIndex != yArgs.OldStartingIndex)
            {
                DumpIfDebug.WriteLine("NotifyCollectionChangedEventArgs OldStartingIndex, expected: {0} was: {1}", x.OldStartingIndex, yArgs.OldStartingIndex);
                return -1;
            }

            if (!ListsEquals(x.OldItems, yArgs.OldItems))
            {
                var xItems = string.Join(", ", x.OldItems);
                var yItems = string.Join(", ", yArgs.OldItems);
                DumpIfDebug.WriteLine("NotifyCollectionChangedEventArgs OldItems, expected: {0} was: {1}", xItems, yItems);
                return -1;
            }

            DumpIfDebug.WriteLine("NotifyCollectionChangedEventArgs Success");
            return 0;
        }

        public int Compare(PropertyChangedEventArgs x, object y)
        {
            if (y == null)
            {
                DumpIfDebug.WriteLine("PropertyChangedEventArgs Null");
                return -1;
            }

            var yArgs = y as PropertyChangedEventArgs;
            if (yArgs == null)
            {
                DumpIfDebug.WriteLine(
                    "PropertyChangedEventArgs Type, expected: {0} was: {1}",
                    typeof(PropertyChangedEventArgs),
                    y.GetType());
                return -1;
            }

            if (x.PropertyName != yArgs.PropertyName)
            {
                DumpIfDebug.WriteLine("PropertyChangedEventArgs PropertyName, expected: {0} was: {1}", x.PropertyName, yArgs.PropertyName);
                return -1;
            }

            return 0;
        }

        private static bool ListsEquals(IList newItems, IList oldItems)
        {
            if (newItems == null && oldItems == null)
            {
                return true;
            }

            if (newItems == null || oldItems == null)
            {
                return false;
            }

            if (newItems.Count != oldItems.Count)
            {
                return false;
            }

            for (int i = 0; i < newItems.Count; i++)
            {
                if (!Equals(newItems[i], oldItems[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}