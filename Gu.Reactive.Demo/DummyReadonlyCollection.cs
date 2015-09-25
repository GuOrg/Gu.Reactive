namespace Gu.Reactive.Demo
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class DummyReadonlyCollection<T> : ICollection, IList
    {
        private readonly IList<T> _items;

        public DummyReadonlyCollection(IList<T> items)
        {
            _items = items;
        }

        public IEnumerator GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        void ICollection.CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        int ICollection.Count => _items.Count;

        object ICollection.SyncRoot
        {
            get { throw new NotImplementedException(); }
        }
        bool ICollection.IsSynchronized
        {
            get { throw new NotImplementedException(); }
        }

        int IList.Add(object value)
        {
            throw new NotImplementedException();
        }

        bool IList.Contains(object value)
        {
            throw new NotImplementedException();
        }

        void IList.Clear()
        {
            throw new NotImplementedException();
        }

        int IList.IndexOf(object value)
        {
            return _items.IndexOf((T)value);
        }

        void IList.Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        void IList.Remove(object value)
        {
            throw new NotImplementedException();
        }

        void IList.RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public object this[int index]
        {
            get { return _items[index]; }
            set { throw new NotImplementedException(); }
        }
        bool IList.IsReadOnly => true;

        bool IList.IsFixedSize => true;
    }
}
