namespace Gu.Wpf.Reactive
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Runtime.CompilerServices;

    using Gu.Reactive.Annotations;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DeferredView<T> : ICollection<T>, IList, INotifyCollectionChanged, INotifyPropertyChanged, IDisposable
    {
        private readonly ObservableCollection<T> _inner;
        private readonly IDisposable _changeSubscription;
        private bool _disposed = false;

        public DeferredView(ObservableCollection<T> inner, TimeSpan deferTime)
        {
            _inner = inner;
            var colChanges = Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                h => inner.CollectionChanged += h,
                h => inner.CollectionChanged -= h);
            _changeSubscription = colChanges.Buffer(colChanges.Throttle(deferTime))
                                            .ObserveOnDispatcherOrCurrentThread()
                                            .Subscribe(x => OnCollectionChanged(x.Select(ep => ep.EventArgs)));
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        #region ICollection<T>

        public IEnumerator<T> GetEnumerator()
        {
            return _inner.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            _inner.Add(item);
        }

        public void Clear()
        {
            _inner.Clear();
        }

        public bool Contains(T item)
        {
            return _inner.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _inner.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return _inner.Remove(item);
        }

        public int Count
        {
            get { return _inner.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        #endregion  ICollection<T>

        #region IList

        int IList.Add(object value)
        {
            return ((IList)_inner).Add(value);
        }

        void IList.Clear()
        {
            ((IList)_inner).Clear();
        }

        bool IList.Contains(object value)
        {
            return ((IList)_inner).Contains(value);
        }

        int IList.IndexOf(object value)
        {
            return ((IList)_inner).IndexOf(value);
        }

        void IList.Insert(int index, object value)
        {
            ((IList)_inner).Insert(index, value);
        }

        bool IList.IsFixedSize
        {
            get { return ((IList)_inner).IsFixedSize; }
        }

        bool IList.IsReadOnly
        {
            get { return ((IList)_inner).IsReadOnly; }
        }

        void IList.Remove(object value)
        {
            ((IList)_inner).Remove(value);
        }

        void IList.RemoveAt(int index)
        {
            ((IList)_inner).RemoveAt(index);
        }

        object IList.this[int index]
        {
            get { return ((IList)_inner)[index]; }
            set { ((IList)_inner)[index] = value; }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            ((IList)_inner).CopyTo(array, index);
        }

        int ICollection.Count
        {
            get { return ((IList)_inner).Count; }
        }

        bool ICollection.IsSynchronized
        {
            get { return ((IList)_inner).IsSynchronized; }
        }

        object ICollection.SyncRoot
        {
            get { return ((IList)_inner).SyncRoot; }
        }

        #endregion IList

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
                _changeSubscription.Dispose();
                // Free any other managed objects here. 
            }

            // Free any unmanaged objects here. 
            _disposed = true;
        }

        protected virtual void OnCollectionChanged(IEnumerable<NotifyCollectionChangedEventArgs> es)
        {
            int count = 0;
            foreach (var _ in es)
            {
                count++;
                if (count > 1)
                {
                    break;
                }
            }
            if (count == 0)
            {
                return;
            }
            if (count == 1)
            {
                OnCollectionChanged(es.Single());
            }
            else
            {
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            this.OnPropertyChanged("Count");
            this.OnPropertyChanged("Item[]");
            var handler = CollectionChanged;
            if (handler != null)
            {
                handler(this, e);
            }
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
    }
}
