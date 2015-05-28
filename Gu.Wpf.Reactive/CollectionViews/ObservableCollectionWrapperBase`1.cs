namespace Gu.Wpf.Reactive
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Reactive;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Windows.Threading;

    using Gu.Reactive;
    using Gu.Reactive.Internals;
    using Gu.Wpf.Reactive.Annotations;

    public abstract class ObservableCollectionWrapperBase<TCollection, TItem> : IObservableCollection<TItem>
        where TCollection : IList<TItem>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private const string CountName = "Count";
        private const string IndexerName = "Item[]";

        protected static readonly NotifyCollectionChangedEventArgs ResetEventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
        protected static readonly PropertyChangedEventArgs CountEventArgs = new PropertyChangedEventArgs(CountName);
        protected static readonly PropertyChangedEventArgs IndexerEventArgs = new PropertyChangedEventArgs(IndexerName);

        private readonly TCollection _inner;

        public ObservableCollectionWrapperBase(TCollection inner)
        {
            _inner = inner;
            InnerCollectionChangedObservable = inner.ObserveCollectionChanged(false);
            InnerCountChangedObservable = inner.ObservePropertyChanged(CountName, false);
            InnerIndexerChangedObservable = inner.ObservePropertyChanged(IndexerName, false);
        }

        [field: NonSerialized]
        public virtual event PropertyChangedEventHandler PropertyChanged;

        [field: NonSerialized]
        public virtual event NotifyCollectionChangedEventHandler CollectionChanged;

        public IObservable<EventPattern<NotifyCollectionChangedEventArgs>> InnerCollectionChangedObservable { get; private set; }

        public IObservable<EventPattern<PropertyChangedEventArgs>> InnerCountChangedObservable { get; private set; }

        public IObservable<EventPattern<PropertyChangedEventArgs>> InnerIndexerChangedObservable { get; private set; }

        #region IList<TItem>

        public int Count
        {
            get
            {
                return _inner.Count;
            }
        }

        public bool IsReadOnly
        {
            get { return _inner.IsReadOnly; }
        }

        public TItem this[int index]
        {
            get
            {
                return _inner[index];
            }
            set { SetItem(index, value); }
        }

        public IEnumerator<TItem> GetEnumerator()
        {
            return _inner.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(TItem item)
        {
            InsertItem(-1, item);
        }

        public void Clear()
        {
            ClearItems();
        }

        public bool Contains(TItem item)
        {
            return _inner.Contains(item);
        }

        public int IndexOf(TItem value)
        {
            return _inner.IndexOf(value);
        }

        public void Insert(int index, TItem value)
        {
            InsertItem(index, value);
        }

        public bool Remove(TItem item)
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

        public void CopyTo(TItem[] array, int arrayIndex)
        {
            _inner.CopyTo(array, arrayIndex);
        }

        #endregion  IList<TItem>

        #region IList

        object IList.this[int index]
        {
            get
            {
                return _inner[index];
            }
            set { SetItem(index, (TItem)value); }
        }

        bool IList.IsFixedSize
        {
            get { return ((IList)_inner).IsFixedSize; }
        }

        public virtual int Add(object value)
        {
            Add((TItem)value);
            return _inner.Count - 1;
        }

        bool IList.Contains(object value)
        {
            return Contains((TItem)value);
        }

        int IList.IndexOf(object value)
        {
            return IndexOf((TItem)value);
        }

        void IList.Insert(int index, object value)
        {
            InsertItem(index, (TItem)value);
        }

        void IList.Remove(object value)
        {
            Remove((TItem)value);
        }

        #endregion IList

        #region ICollection

        void ICollection.CopyTo(Array array, int index)
        {
            ((IList)_inner).CopyTo(array, index);
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

        protected virtual void InsertItem(int index, TItem item)
        {
            if (index == -1)
            {
                index = _inner.Count;
            }
            _inner.Insert(index, item);
        }

        protected virtual void SetItem(int index, TItem item)
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

        /// <summary>
        /// Can be called from any thread. Raises on Dispatcher if needed.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            var handler = CollectionChanged;
            if (handler != null)
            {
                var invocationList = handler.GetInvocationList().OfType<NotifyCollectionChangedEventHandler>();
                foreach (var invocation in invocationList)
                {
                    var dispatcherObject = invocation.Target as DispatcherObject;

                    if (dispatcherObject != null && !dispatcherObject.CheckAccess())
                    {
                        dispatcherObject.Dispatcher.Invoke(DispatcherPriority.DataBind, invocation, this, e);
                    }
                    else
                    {
                        invocation(this, e); // note : this does not execute invocation in target thread's context
                    }
                }
            }
        }
    }
}
