namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public sealed class ItemPropertyListener<T> : INotifyPropertyChanged, IDisposable where T : INotifyPropertyChanged
    {
        private readonly ObservableCollection<T> _collection;
        private readonly string _propertyName;
        private readonly Dictionary<T, int> _items = new Dictionary<T, int>(new ObjectIdentityComparer());
        public ItemPropertyListener(ObservableCollection<T> collection, string propertyName = "")
        {
            _collection = collection;
            //_propertyName = propertyName ?? "";
            //AddRange(collection);
            //CollectionChangedEventManager.AddHandler(collection, CollectionChanged);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        //private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        //{
        //    switch (e.Action)
        //    {
        //        case NotifyCollectionChangedAction.Add:
        //            AddRange(e.NewItems.Cast<T>());
        //            break;
        //        case NotifyCollectionChangedAction.Remove:
        //            RemoveRange(e.OldItems.Cast<T>());
        //            break;
        //        case NotifyCollectionChangedAction.Replace:
        //            AddRange(e.NewItems.Cast<T>());
        //            RemoveRange(e.OldItems.Cast<T>());
        //            break;
        //        case NotifyCollectionChangedAction.Move:
        //            break;
        //        case NotifyCollectionChangedAction.Reset:
        //            Reset();
        //            break;
        //        default:
        //            throw new ArgumentOutOfRangeException();
        //    }

        //}

        //private void AddRange(IEnumerable<T> newItems)
        //{
        //    foreach (T item in newItems)
        //    {
        //        if (_items.ContainsKey(item))
        //        {
        //            _items[item]++;
        //        }
        //        else
        //        {
        //            _items.Add(item, 1);
        //            PropertyChangedEventManager.AddHandler(item, ChildPropertyChanged, _propertyName);
        //        }
        //    }
        //}

        //private void RemoveRange(IEnumerable<T> oldItems)
        //{
        //    foreach (T item in oldItems)
        //    {
        //        _items[item]--;
        //        if (_items[item] == 0)
        //        {
        //            _items.Remove(item);
        //            PropertyChangedEventManager.RemoveHandler(item, ChildPropertyChanged, _propertyName);
        //        }
        //    }
        //}

        //private void Reset()
        //{
        //    foreach (T item in _items.Keys.ToList())
        //    {
        //        PropertyChangedEventManager.RemoveHandler(item, ChildPropertyChanged, _propertyName);
        //        _items.Remove(item);
        //    }
        //    CollectionChangedEventManager.RemoveHandler(_collection, CollectionChanged);
        //    AddRange(_collection);
        //}

        private void ChildPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private class ObjectIdentityComparer : IEqualityComparer<T>
        {
            public bool Equals(T x, T y)
            {
                return ReferenceEquals(x, y);
            }
            public int GetHashCode(T obj)
            {
                return RuntimeHelpers.GetHashCode(obj);
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}