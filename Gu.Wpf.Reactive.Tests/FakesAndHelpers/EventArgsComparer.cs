namespace Gu.Wpf.Reactive.Tests.FakesAndHelpers
{
    using System.Collections;
    using System.Collections.Specialized;
    using System.ComponentModel;

    public class EventArgsComparer : IComparer
    {
        public static readonly EventArgsComparer Instance = new EventArgsComparer();
        public int Compare(object x, object y)
        {
            if (Equals(x, y))
            {
                return 0;
            }
            var collectionChangedEventArgs = x as NotifyCollectionChangedEventArgs;
            if (collectionChangedEventArgs != null)
            {
                return Compare(collectionChangedEventArgs, y as NotifyCollectionChangedEventArgs);
            }
            return ((PropertyChangedEventArgs)x).PropertyName.CompareTo(((PropertyChangedEventArgs)y).PropertyName);
        }

        public int Compare(NotifyCollectionChangedEventArgs x, NotifyCollectionChangedEventArgs y)
        {
            if (x.Action != y.Action)
            {
                return -1;
            }

            if (x.NewStartingIndex != y.NewStartingIndex)
            {
                return -1;
            }

            if (!ListsEquals(x.NewItems, y.NewItems))
            {
                return -1;
            }

            if (x.OldStartingIndex != y.OldStartingIndex)
            {
                return -1;
            }

            if (!ListsEquals(x.OldItems, y.OldItems))
            {
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