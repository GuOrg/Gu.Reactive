namespace Gu.Reactive
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Reactive.Disposables;

    using Gu.Reactive.Internals;

    public class EditableListView<T> : IObservableCollection<T>, IList, IDisposable
    {
        private readonly IObservableCollection<T> _source;
        private readonly IDisposable _subscriptions;

        private bool _disposed;

        public EditableListView(IObservableCollection<T> source)
        {
            _source = source;
            _subscriptions = Subscribe(source);
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        public int Count => VerifyDisposed(_source.Count);

        public bool IsReadOnly => VerifyDisposed(false);

        bool IList.IsFixedSize => VerifyDisposed(false);

        public T this[int index]
        {
            get { return _source[index]; }
            set { _source[index] = value; }
        }

        object IList.this[int index]
        {
            get { return this[index]; }
            set { throw new NotImplementedException("we must notify immediately here"); }
        }

        public IEnumerator<T> GetEnumerator() => VerifyDisposed(_source.GetEnumerator());

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(T item) => VerifyDisposed(() => _source.Add(item));

        int IList.Add(object value)
        {
            var count = _source.Count;
            _source.Add((T)value);
            (_source as IRefreshAble)?.Refresh();
            return count;
        }

        public bool Remove(T item) => _source.Remove(item);

        void IList.Remove(object value)
        {
            Remove((T)value);
            (_source as IRefreshAble)?.Refresh();
        }

        public void RemoveAt(int index) => _source.RemoveAt(index);

        public void Clear()
        {
            _source.Clear();
        }

        public void Insert(int index, T item) => _source.Insert(index, item);

        void IList.Insert(int index, object value)
        {
            Insert(index, (T)value);
            (_source as IRefreshAble)?.Refresh();
        }

        object ICollection.SyncRoot => VerifyDisposed(((ICollection)_source).SyncRoot);

        bool ICollection.IsSynchronized => VerifyDisposed(((ICollection)_source).IsSynchronized);

        public void CopyTo(T[] array, int arrayIndex) => _source.CopyTo(array, arrayIndex);

        void ICollection.CopyTo(Array array, int index) => ListExt.CopyTo(this, array, index);

        public bool Contains(T item) => _source.Contains(item);

        bool IList.Contains(object value) => Contains((T)value);

        public int IndexOf(T item) => _source.IndexOf(item);

        int IList.IndexOf(object value) => IndexOf((T)value);

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;
            _subscriptions.Dispose();
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e) => CollectionChanged?.Invoke(this, e);

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e) => PropertyChanged?.Invoke(this, e);

        protected void VerifyDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }

        protected void VerifyDisposed(Action action)
        {
            VerifyDisposed();
            action();
        }

        protected TResult VerifyDisposed<TResult>(TResult result)
        {
            VerifyDisposed();
            return result;
        }

        private IDisposable Subscribe<TCol>(TCol col) where TCol : INotifyCollectionChanged, INotifyPropertyChanged
        {
            var subscriptions = new CompositeDisposable(2) { col.ObservePropertyChangedSlim()
                                                                .Subscribe(OnPropertyChanged),
                                                             col.ObserveCollectionChangedSlim(false)
                                                                .Subscribe(OnCollectionChanged) };
            return subscriptions;
        }
    }
}