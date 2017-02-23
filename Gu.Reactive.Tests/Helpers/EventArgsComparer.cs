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

            var xpc = x as PropertyChangedEventArgs;
            var ypc = y as PropertyChangedEventArgs;
            if (xpc != null && ypc != null)
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

            var collectionChangedEventArgs = x as NotifyCollectionChangedEventArgs;
            if (collectionChangedEventArgs != null)
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
