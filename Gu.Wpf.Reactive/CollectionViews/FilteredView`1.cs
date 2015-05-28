namespace Gu.Wpf.Reactive
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Windows.Threading;

    using Gu.Reactive;
    using Gu.Wpf.Reactive.Annotations;

    /// <summary>
    /// Typed CollectionView for intellisense in xaml
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public class FilteredView<TItem> : IFilteredView<TItem>, IDisposable
    {
        private const string CountName = "Count";
        private const string IndexerName = "Item[]";

        internal static readonly TimeSpan DefaultBufferTime = TimeSpan.FromMilliseconds(10);
        private static readonly NotifyCollectionChangedEventArgs ResetEventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
        private static readonly PropertyChangedEventArgs CountEventArgs = new PropertyChangedEventArgs(CountName);
        private static readonly PropertyChangedEventArgs IndexerEventArgs = new PropertyChangedEventArgs(IndexerName);

        private readonly IEnumerable<TItem> _inner;
        private readonly List<TItem> _filtered = new List<TItem>();
        private readonly bool _isReadonly;
        private readonly ObservableCollection<IObservable<object>> _triggers;
        private readonly DispatcherTimer _timer;

        private IDisposable _triggerSubscription;
        private Func<TItem, bool> _filter;
        private bool _disposed = false;
        private TimeSpan _bufferTime;

        /// <summary>
        /// For manual Refresh()
        /// </summary>
        /// <param name="collection">The collection to wrap</param>
        /// <param name="filter"></param>
        /// <param name="bufferTime">The time to defer updates, useful if many triggers fire in short time. Then it will be only one Reset</param>
        /// <param name="triggers">Triggers when to re evaluate the filter</param>
        public FilteredView(IEnumerable<TItem> collection, Func<TItem, bool> filter, TimeSpan bufferTime, params IObservable<object>[] triggers)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }
            _timer = new DispatcherTimer(DispatcherPriority.DataBind);
            _timer.Tick += new EventHandler(Refresh);
            _bufferTime = bufferTime;
            _filter = filter;

            _isReadonly = !((collection is INotifyCollectionChanged) && (collection is IList<TItem>));
            _inner = collection;
            var notifyCollectionChanged = _inner as INotifyCollectionChanged;
            if (notifyCollectionChanged != null)
            {
                CollectionChangedEventManager.AddHandler(notifyCollectionChanged, OnInnerCollectionChanged);
            }

            _triggers = new ObservableCollection<IObservable<object>>(triggers);
            _triggers.ObserveCollectionChanged(true).Subscribe(_ => OnTriggersChanged());
            Refresh();
        }

        public FilteredView(IEnumerable<TItem> collection)
            : this(collection, null, DefaultBufferTime)
        {
        }

        public FilteredView(IEnumerable<TItem> collection, TimeSpan deferTime, params IObservable<object>[] triggers)
            : this(collection, null, deferTime, triggers)
        {
        }

        public FilteredView(IEnumerable<TItem> collection, Func<TItem, bool> filter, params IObservable<object>[] triggers)
            : this(collection, filter, DefaultBufferTime, triggers)
        {
        }

        public FilteredView(ObservableCollection<TItem> collection, Func<TItem, bool> filter, TimeSpan bufferTime, params IObservable<object>[] triggers)
            : this((IEnumerable<TItem>)collection, filter, bufferTime, triggers)
        {
        }

        public FilteredView(ObservableCollection<TItem> collection, TimeSpan deferTime, params IObservable<object>[] triggers)
            : this(collection, null, deferTime, triggers)
        {
            _isReadonly = false;
        }

        public FilteredView(ObservableCollection<TItem> collection, params IObservable<object>[] triggers)
            : this(collection, null, DefaultBufferTime, triggers)
        {
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        public Func<TItem, bool> Filter
        {
            get
            {
                return _filter;
            }
            set
            {
                if (Equals(value, _filter))
                {
                    return;
                }
                _filter = value;
                OnPropertyChanged();
                DeferredRefresh();
            }
        }

        public ObservableCollection<IObservable<object>> Triggers
        {
            get { return _triggers; }
        }

        public TimeSpan BufferTime
        {
            get
            {
                return _bufferTime;
            }
            set
            {
                if (value.Equals(_bufferTime))
                {
                    return;
                }
                _bufferTime = value;
                OnPropertyChanged();
                DeferredRefresh();
            }
        }

        /// <summary>
        /// Dispose(true); //I am calling you from Dispose, it's safe
        /// GC.SuppressFinalize(this); //Hey, GC: don't bother calling finalize later
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Refresh()
        {
            _timer.Stop();
            _filtered.Clear();
            if (_filter != null)
            {
                _filtered.AddRange(_inner.Where(_filter));
            }
            else
            {
                _filtered.AddRange(_inner);
            }
            Notify(ResetEventArgs);
        }

        protected void DeferredRefresh()
        {
            if (BufferTime > TimeSpan.Zero)
            {
                _timer.Interval = BufferTime;
                _timer.Start();
            }
            else
            {
                _timer.Stop();
                Refresh();
            }
        }

        /// <summary>
        /// Protected implementation of Dispose pattern. 
        /// </summary>
        /// <param name="disposing">true: safe to free managed resources</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                if (_triggerSubscription != null)
                {
                    _triggerSubscription.Dispose();
                }

                var notifyCollectionChanged = _inner as INotifyCollectionChanged;
                if (notifyCollectionChanged != null)
                {
                    CollectionChangedEventManager.RemoveHandler(notifyCollectionChanged, OnInnerCollectionChanged);
                }
            }

            // Free any unmanaged objects here. 
            _disposed = true;
        }

        [NotifyPropertyChangedInvocator]
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected void OnPropertyChanged(PropertyChangedEventArgs e)
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
        protected void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
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

        private void OnTriggersChanged()
        {
            if (_triggerSubscription != null)
            {
                _triggerSubscription.Dispose();
            }
            if (_triggers != null && _triggers.Any())
            {
                _triggerSubscription = _triggers.Merge()
                                                .Subscribe(x => DeferredRefresh());
            }
        }

        private void OnInnerCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            DeferredRefresh();
        }

        private void VerifyEditable([CallerMemberName] string caller = null)
        {
            if (IsReadOnly)
            {
                throw new InvalidOperationException(string.Format("Collection is readonly, cannot perform {0}", caller));
            }
        }

        private void Notify(NotifyCollectionChangedEventArgs collectionChangedEventArgs)
        {
            var handler = CollectionChanged;
            if (handler != null)
            {
                OnPropertyChanged(CountEventArgs);
                OnPropertyChanged(IndexerEventArgs);
                OnCollectionChanged(collectionChangedEventArgs);
            }
        }

        private void Refresh(object sender, EventArgs e)
        {
            Refresh();
        }

        #region IList<TItem>

        public int Count
        {
            get
            {
                return _filtered.Count;
            }
        }

        public bool IsReadOnly
        {
            get { return _isReadonly; }
        }

        public TItem this[int index]
        {
            get { return _filtered[index]; }
            set
            {
                RemoveAt(index);
                Insert(index, value);
            }
        }

        public IEnumerator<TItem> GetEnumerator()
        {
            return _filtered.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(TItem item)
        {
            VerifyEditable();
            ((IList<TItem>)_inner).Add(item);
        }

        public void Clear()
        {
            VerifyEditable();
            ((IList<TItem>)_inner).Clear();
        }

        public bool Contains(TItem item)
        {
            return _filtered.Contains(item);
        }

        public int IndexOf(TItem value)
        {
            VerifyEditable();
            return _filtered.IndexOf(value);
        }

        public void Insert(int index, TItem value)
        {
            VerifyEditable();
            var i = ((IList<TItem>)_inner).IndexOf(_filtered[index]) + 1;
            ((IList<TItem>)_inner).Insert(i, value);
        }

        public bool Remove(TItem item)
        {
            VerifyEditable();
            return ((IList<TItem>)_inner).Remove(item);
        }

        public void RemoveAt(int index)
        {
            VerifyEditable();
            ((IList<TItem>)_inner).Remove(_filtered[index]);
        }

        public void CopyTo(TItem[] array, int arrayIndex)
        {
            _filtered.CopyTo(array, arrayIndex);
        }

        #endregion  IList<TItem>

        #region IList

        int IList.Add(object value)
        {
            ((IList)_inner).Add(value); // Adding to inner
            var index = ((IList)_filtered).Add(value); // Adding explicitly to filtered to get correct index.
            Notify(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, index)); // IList.Add happens when new row is added in DataGrid, we need to notify here to avoid out of sync exception
            return index;
        }

        bool IList.Contains(object value)
        {
            return ((IList)_filtered).Contains(value);
        }

        int IList.IndexOf(object value)
        {
            return ((IList)_filtered).IndexOf(value);
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, (TItem)value);
        }

        void IList.Remove(object value)
        {
            Remove((TItem)value);
        }

        object IList.this[int index]
        {
            get { return this[index]; }
            set { this[index] = (TItem)value; }
        }

        public bool IsFixedSize
        {
            get { return _isReadonly; }
        }

        #endregion IList

        #region ICollection

        void ICollection.CopyTo(Array array, int index)
        {
            ((ICollection)_filtered).CopyTo(array, index);
        }

        public object SyncRoot
        {
            get { return ((ICollection)_inner).SyncRoot; }
        }

        public bool IsSynchronized
        {
            get { return ((ICollection)_inner).IsSynchronized; }
        }

        #endregion ICollection
    }
}
