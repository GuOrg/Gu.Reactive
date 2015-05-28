namespace Gu.Wpf.Reactive.Tests
{
    using System.Collections;
    using System.Collections.Specialized;
    using System.ComponentModel;

    public class EventArgsComparer : IComparer
    {
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

            if (x.OldStartingIndex != y.OldStartingIndex)
            {
                return -1;
            }
            return 0;
        }
    }
}