namespace Gu.Reactive.Tests.Helpers
{
    using System.Collections;
    using System.Collections.Specialized;
    using System.ComponentModel;

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
            AssertEx.AreEqual(x, y);
            return 0;
        }

        public int Compare(PropertyChangedEventArgs x, object y)
        {
            AssertEx.AreEqual(x, y);
            return 0;
        }
    }
}
