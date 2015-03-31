namespace Gu.Reactive
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    internal class WeakCollectionWrapper<T> : IEnumerable<T>
    {
        private WeakReference _wr = new WeakReference(null);
        public WeakCollectionWrapper(ObservableCollection<T> collection)
        {
            _wr.Target = collection;
        }

        public IEnumerable<T> Collection
        {
            get { return (IEnumerable<T>)_wr.Target; }
        }

        public IEnumerator<T> GetEnumerator()
        {
            var source = (ObservableCollection<T>)_wr.Target;
            if (source == null)
            {
                return Enumerable.Empty<T>()
                                 .GetEnumerator();
            }
            return source.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}