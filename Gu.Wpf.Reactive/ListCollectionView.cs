namespace Gu.Wpf.Reactive
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Data;

    [Obsolete("Internal until tested")]
    internal class ListCollectionView<T> : ListCollectionView, IEnumerable<T>
    {
        public ListCollectionView(IList list)
            : base(list)
        {
        }

        public new Predicate<T> Filter
        {
            get
            {
                return o => base.Filter(o);
            }
            set
            {
                base.Filter = o => value((T)o);
            }
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return this.Cast<T>().GetEnumerator();
        }
    }
}