namespace Gu.Reactive.Tests.Helpers
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.ComponentModel;

    public class EventArgsComparer : IComparer
    {
        public static readonly EventArgsComparer Default = new EventArgsComparer();

        private EventArgsComparer()
        {
        }

        public static bool Equals(EventArgs x, EventArgs y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            if (x.GetType() != y.GetType())
            {
                return false;
            }

            if (x is PropertyChangedEventArgs xpc && 
                y is PropertyChangedEventArgs ypc)
            {
                return xpc.PropertyName == ypc.PropertyName;
            }

            return Compare(x, y) == 0;
        }

        public static int Compare(object x, object y)
        {
            if (Equals(x, y))
            {
                return 0;
            }

            if (x is NotifyCollectionChangedEventArgs collectionChangedEventArgs)
            {
                return Compare(collectionChangedEventArgs, y);
            }

            return Compare((PropertyChangedEventArgs)x, y);
        }

        public static int Compare(NotifyCollectionChangedEventArgs x, object y)
        {
            AssertEx.AreEqual(x, y);
            return 0;
        }

        public static int Compare(PropertyChangedEventArgs x, object y)
        {
            AssertEx.AreEqual(x, y);
            return 0;
        }

        int IComparer.Compare(object x, object y) => Compare(x, y);
    }
}
