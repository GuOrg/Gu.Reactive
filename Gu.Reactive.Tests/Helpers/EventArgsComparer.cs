namespace Gu.Reactive.Tests.Helpers
{
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
            return !newItems.Cast<object>()
                            .Where((t, i) => !Equals(t, oldItems[i]))
                            .Any();
        }
    }
}
