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

    using Gu.Reactive;
    using Gu.Wpf.Reactive.Annotations;

    /// <summary>
    /// Typed CollectionView for intellisense in xaml
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FilteredView<T> : IEnumerable<T>, IList, INotifyCollectionChanged, INotifyPropertyChanged, IDisposable
    {
        private const string CountString = "Count";
        private const string IndexerName = "Item[]";
        private static readonly Subject<object> DummyTrigger = new Subject<object>();
        private static readonly NotifyCollectionChangedEventArgs ResetEventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
        private readonly IEnumerable<T> _inner;
        private readonly bool _isReadonly;
        private readonly ObservableCollection<IObservable<object>> _triggers;

        private IDisposable _triggerSubscription;
        private Func<T, bool> _filter;
        private bool _disposed = false;
        private TimeSpan _deferTime;

        /// <summary>
        /// For manual Refresh()
        /// </summary>
        /// <param name="collection">The collection to wrap</param>
        /// <param name="filter"></param>
        /// <param name="deferTime">The time to defer updates, useful if many triggers fire in short time. Then it will be only one Reset</param>
        /// <param name="triggers">Triggers when to re evaluate the filter</param>
        public FilteredView(IEnumerable<T> collection, Func<T, bool> filter, TimeSpan deferTime, params IObservable<object>[] triggers)
        {
            _deferTime = deferTime;
            _filter = filter;
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }
            _inner = collection;
            var notifyCollectionChanged = _inner as INotifyCollectionChanged;
            if (notifyCollectionChanged != null)
            {
                CollectionChangedEventManager.AddHandler(notifyCollectionChanged, OnInnerCollectionChanged);
            }
            _triggers = new ObservableCollection<IObservable<object>>(triggers);
            _triggers.CollectionChanged += (_, __) => OnTriggersChanged();
            OnTriggersChanged();
        }

        public FilteredView(IEnumerable<T> collection)
            : this(collection, null, TimeSpan.Zero)
        {
        }

        public FilteredView(IEnumerable<T> collection, TimeSpan deferTime, params IObservable<object>[] triggers)
            : this(collection, null, deferTime, triggers)
        {
        }

        public FilteredView(IEnumerable<T> collection, Func<T, bool> filter, params IObservable<object>[] triggers)
            : this(collection, filter, TimeSpan.Zero, triggers)
        {
        }

        public FilteredView(ObservableCollection<T> collection, Func<T, bool> filter, TimeSpan deferTime, params IObservable<object>[] triggers)
            : this(collection.AsDeferredView(deferTime), filter, deferTime, triggers)
        {
            _isReadonly = false;
        }

        public FilteredView(ObservableCollection<T> collection, TimeSpan deferTime, params IObservable<object>[] triggers)
            : this(collection.AsDeferredView(deferTime), null, deferTime, triggers)
        {
            _isReadonly = false;
        }

        public FilteredView(ObservableCollection<T> collection, params IObservable<object>[] triggers)
            : this(collection.AsDispatchingView(), null, TimeSpan.Zero, triggers)
        {
            _isReadonly = false;
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        public Func<T, bool> Filter
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
                Schedulers.DispatcherOrCurrentThread.Schedule(
                    () =>
                    {
                        OnCollectionChanged(ResetEventArgs);
                    });
            }
        }

        public ObservableCollection<IObservable<object>> Triggers
        {
            get { return _triggers; }
        }

        public TimeSpan DeferTime
        {
            get
            {
                return _deferTime;
            }
            set
            {
                if (value.Equals(_deferTime))
                {
                    return;
                }
                _deferTime = value;
                var deferredView = _inner as IDeferredView<T>;
                if (deferredView != null)
                {
                    deferredView.BufferTime = _deferTime;
                }
                OnPropertyChanged();
                OnTriggersChanged();
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (_filter == null)
            {
                return _inner.GetEnumerator();
            }
            return _inner.Where(_filter)
                         .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
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
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            var handler = CollectionChanged;
            if (handler != null)
            {
                OnPropertyChanged(CountString);
                OnPropertyChanged(IndexerName);
                handler(this, ResetEventArgs);
            }
        }

        private static IObservable<object> CreateTrigger(TimeSpan? deferTime, IList<IObservable<object>> triggers)
        {
            if (!triggers.Any())
            {
                return DummyTrigger;
            }
            var observable = triggers.Merge();
            if (deferTime.HasValue && deferTime.Value > TimeSpan.Zero)
            {
                return observable
                    .Throttle(deferTime.Value)
                    .ObserveOnDispatcherOrCurrentThread();
            }
            return observable.ObserveOnDispatcherOrCurrentThread();
        }

        private void OnTriggersChanged()
        {
            if (_triggerSubscription != null)
            {
                _triggerSubscription.Dispose();
            }
            if (_triggers != null && _triggers.Any())
            {
                _triggerSubscription = CreateTrigger(DeferTime, Triggers)
                    .ObserveOnDispatcherOrCurrentThread()
                    .Subscribe(x => OnCollectionChanged(ResetEventArgs));
            }
            Schedulers.DispatcherOrCurrentThread.Schedule(() => OnCollectionChanged(ResetEventArgs));
        }

        private void OnInnerCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            //if (_filter == null)
            //{
            //    OnCollectionChanged(notifyCollectionChangedEventArgs);
            //}
            //else
            //{
            //    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            //}
            // keeping it simple and resetting here.
            OnPropertyChanged(CountString);
            OnPropertyChanged(IndexerName);
            OnCollectionChanged(ResetEventArgs);
        }

        #region IList implementing this to make items editable in datagrid. Hardcoding fixed size so no add/remove.

        void ICollection.CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        int ICollection.Count
        {
            get
            {
                int i = 0;
                using (var enumerator = GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        i++;
                    }
                }
                return i;
            }
        }

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
            throw new NotImplementedException("Was tricky to get eventargs correct");
            ((IDeferredView<T>)_inner).Add((T)value);
            var indexOf = ((IList)this).IndexOf(value);
            return indexOf;
        }

        bool IList.Contains(object value)
        {
            var isValueType = typeof(T).IsValueType;
            if (isValueType)
            {
                using (var enumerator = GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        if (Equals(value, enumerator.Current))
                        {
                            return true;
                        }
                    }
                }
            }
            else
            {
                using (var enumerator = GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        if (ReferenceEquals(value, enumerator.Current))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        void IList.Clear()
        {
            throw new NotImplementedException();
        }

        int IList.IndexOf(object value)
        {
            int i = 0;
            var isValueType = typeof(T).IsValueType;
            if (isValueType)
            {
                using (var enumerator = GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        if (Equals(value, enumerator.Current))
                        {
                            return i;
                        }
                        i++;
                    }
                }
            }
            else
            {
                using (var enumerator = GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        if (ReferenceEquals(value, enumerator.Current))
                        {
                            return i;
                        }
                        i++;
                    }
                }
            }
            return -1;
        }

        void IList.Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        void IList.Remove(object value)
        {
            throw new NotImplementedException("Was tricky to get eventargs correct");
            ((IDeferredView<T>)_inner).Remove((T)value);
        }

        void IList.RemoveAt(int index)
        {
            throw new NotImplementedException("Was tricky to get eventargs correct");
            int i = 0;
            var isValueType = typeof(T).IsValueType;
            if (isValueType)
            {
                using (var enumerator = GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        if (index == i)
                        {
                            ((IDeferredView<T>)_inner).Remove(enumerator.Current);
                        }
                        i++;
                    }
                }
            }
            else
            {
                using (var enumerator = GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        if (index == i)
                        {
                            ((IDeferredView<T>)_inner).Remove(enumerator.Current);
                        }
                        i++;
                    }
                }
            }
        }

        public object this[int index]
        {
            get
            {
                int i = 0;
                using (var enumerator = GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        if (i == index)
                        {
                            return enumerator.Current;
                        }
                        i++;
                    }
                }
                throw new ArgumentOutOfRangeException("index");
            }
            set { throw new NotImplementedException(); }
        }

        bool IList.IsReadOnly
        {
            get { return true; /* _isReadonly; */ }
        }

        bool IList.IsFixedSize
        {
            get { return true; /* _isReadonly; */}
        }

        #endregion IList
    }
}
