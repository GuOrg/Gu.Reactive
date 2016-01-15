namespace Gu.Reactive
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Reactive.Disposables;

    public class ReadOnlyIListView<T> : IReadOnlyObservableCollection<T>, IList, IDisposable
    {
        private readonly IReadOnlyList<T> _source;
        private readonly IDisposable _subscriptions;

        private bool _disposed;

        public ReadOnlyIListView(IObservableCollection<T> source)
        {
            _source = source.AsReadOnly();
            _subscriptions = Subscribe(source);
        }

        public ReadOnlyIListView(IReadOnlyObservableCollection<T> source)
        {
            _source = source;
            _subscriptions = Subscribe(source);
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        public int Count => VerifyDisposed(_source.Count);

        public T this[int index] => VerifyDisposed(_source[index]);

        bool IList.IsReadOnly => VerifyDisposed(true);

        bool IList.IsFixedSize => VerifyDisposed(true);

        object IList.this[int index]
        {
            get { return this[index]; }
            set { ThrowHelper.ThrowCollectionIsReadonly(); }
        }

        object ICollection.SyncRoot => VerifyDisposed(((ICollection)_source).SyncRoot);

        bool ICollection.IsSynchronized => VerifyDisposed(((ICollection)_source).IsSynchronized);

        public IEnumerator<T> GetEnumerator() => VerifyDisposed(_source.GetEnumerator());

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        void ICollection.CopyTo(Array array, int index) => _source.CopyTo(array, index);

        int IList.Add(object value) => ThrowHelper.ThrowCollectionIsReadonly<int>();

        bool IList.Contains(object value) => _source.Contains(value);

        void IList.Clear() => ThrowHelper.ThrowCollectionIsReadonly();

        int IList.IndexOf(object value) => _source.IndexOf(value);

        void IList.Insert(int index, object value) => ThrowHelper.ThrowCollectionIsReadonly();

        void IList.Remove(object value) => ThrowHelper.ThrowCollectionIsReadonly();

        void IList.RemoveAt(int index) => ThrowHelper.ThrowCollectionIsReadonly();

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