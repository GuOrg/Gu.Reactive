namespace Gu.Reactive
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Reactive.Concurrency;
    using System.Runtime.CompilerServices;
    using Gu.Reactive.Annotations;

    public abstract class ObservableCollectionWrapper<T> : CollectionSynchronizer<T>, IObservableCollection<T>, IList
    {
        private readonly IList<T> _inner;
        private readonly IScheduler _scheduler;

        protected ObservableCollectionWrapper(ObservableCollection<T> inner, IScheduler scheduler)
        {
            _inner = inner;
            _scheduler = scheduler;
        }

        protected ObservableCollectionWrapper(IObservableCollection<T> inner, IScheduler scheduler)
        {
            _inner = inner;
            _scheduler = scheduler;
        }

        [field: NonSerialized]
        public virtual event PropertyChangedEventHandler PropertyChanged;

        [field: NonSerialized]
        public virtual event NotifyCollectionChangedEventHandler CollectionChanged;

        public virtual void Refresh()
        {
            Reset(this, _inner, _scheduler, PropertyChanged, CollectionChanged);
        }

        protected void Refresh(NotifyCollectionChangedEventArgs change)
        {
            Refresh(this, _inner, new[] { change }, _scheduler, PropertyChanged, CollectionChanged);
        }

        protected void Refresh(IReadOnlyList<NotifyCollectionChangedEventArgs> changes)
        {
            Refresh(this, _inner, changes, _scheduler, PropertyChanged, CollectionChanged);
        }

        protected virtual void ClearItems()
        {
            _inner.Clear();
        }

        protected virtual void RemoveItem(int index)
        {
            if (index < 0)
            {
                return;
            }
            _inner.RemoveAt(index);
        }

        protected virtual void InsertItem(int index, T item)
        {
            if (index == -1)
            {
                index = _inner.Count;
            }
            _inner.Insert(index, item);
        }

        protected virtual void SetItem(int index, T item)
        {
            _inner[index] = item;
        }

        protected virtual void MoveItem(int oldIndex, int newIndex)
        {
            var removedItem = _inner[oldIndex];
            _inner.RemoveAt(oldIndex);
            _inner.Insert(newIndex, removedItem);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #region IList<TItem>

        public int Count
        {
            get { return Current.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public T this[int index]
        {
            get { return Current[index]; }
            set { SetItem(index, value); }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Current.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            InsertItem(-1, item);
        }

        public void Clear()
        {
            ClearItems();
        }

        public void Insert(int index, T value)
        {
            InsertItem(index, value);
        }

        public bool Remove(T item)
        {
            var indexOf = IndexOf(item);
            if (indexOf < 0)
            {
                return false;
            }
            RemoveItem(indexOf);
            return true;
        }

        public void RemoveAt(int index)
        {
            RemoveItem(index);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            base.CopyTo(array, arrayIndex);
        }

        #endregion  IList<TItem>

        #region IList

        object IList.this[int index]
        {
            get { return Current[index]; }
            set { SetItem(index, (T)value); }
        }

        bool IList.IsFixedSize
        {
            get { return ((IList)Current).IsFixedSize; }
        }

        public virtual int Add(object value)
        {
            Add((T)value);
            var index = Current.Count - 1;
            Refresh(Diff.CreateAddEventArgs(value, index));
            return index;
        }

        bool IList.Contains(object value)
        {
            return Contains((T)value);
        }

        int IList.IndexOf(object value)
        {
            return IndexOf((T)value);
        }

        void IList.Insert(int index, object value)
        {
            InsertItem(index, (T)value);
        }

        void IList.Remove(object value)
        {
            Remove((T)value);
        }

        #endregion IList

        #region ICollection

        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo(array, index);
        }

        bool ICollection.IsSynchronized
        {
            get { return ((IList)_inner).IsSynchronized; }
        }

        object ICollection.SyncRoot
        {
            get { return ((IList)_inner).SyncRoot; }
        }

        #endregion ICollection
    }
}
