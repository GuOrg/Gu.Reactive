namespace Gu.Reactive
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Reactive.Disposables;

    using Gu.Reactive.Internals;

    public class ReadOnlyIListView<T> : IReadonlyIListView<T>
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

        void ICollection.CopyTo(Array array, int index)
        {
            Ensure.NotNull(array, nameof(array));
            Ensure.That(index >= 0, nameof(index), "Index must be greater than or equal to 0");
            Ensure.That(index < array.Length, nameof(index), "Index must be less than array.Length");

            for (int i = index; i < _source.Count; i++)
            {
                array.SetValue(_source[i], i);
            }
        }

        int IList.Add(object value) => ThrowHelper.ThrowCollectionIsReadonly<int>();

        bool IList.Contains(object value) => ThrowHelper.ThrowNotSupportedException<bool>();

        void IList.Clear() => ThrowHelper.ThrowCollectionIsReadonly();

        int IList.IndexOf(object value) => ThrowHelper.ThrowNotSupportedException<int>();

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