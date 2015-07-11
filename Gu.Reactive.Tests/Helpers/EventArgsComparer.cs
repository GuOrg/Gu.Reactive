namespace Gu.Reactive.Tests.Helpers
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;

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
                return Compare(collectionChangedEventArgs, y);
            }
            return Compare((PropertyChangedEventArgs)x, y);
        }

        public int Compare(NotifyCollectionChangedEventArgs x, object y)
        {
            AssertEx.AreEqual(x, y);
            return 0;
        }

        public int Compare(PropertyChangedEventArgs x, object y)
        {
            AssertEx.AreEqual(x, y);
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
