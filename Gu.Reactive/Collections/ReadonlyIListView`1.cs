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
        private readonly IReadOnlyList<T> source;
        private readonly IDisposable subscriptions;

        private bool disposed;

        public ReadOnlyIListView(IObservableCollection<T> source)
        {
            this.source = source.AsReadOnly();
            this.subscriptions = this.Subscribe(source);
        }

        public ReadOnlyIListView(IReadOnlyObservableCollection<T> source)
        {
            this.source = source;
            this.subscriptions = this.Subscribe(source);
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        public int Count => this.VerifyDisposed(this.source.Count);

        public T this[int index] => this.VerifyDisposed(this.source[index]);

        bool IList.IsReadOnly => this.VerifyDisposed(true);

        bool IList.IsFixedSize => this.VerifyDisposed(true);

        object IList.this[int index]
        {
            get { return this[index]; }
            set { ThrowHelper.ThrowCollectionIsReadonly(); }
        }

        object ICollection.SyncRoot => this.VerifyDisposed(((ICollection)this.source).SyncRoot);

        bool ICollection.IsSynchronized => this.VerifyDisposed(((ICollection)this.source).IsSynchronized);

        public IEnumerator<T> GetEnumerator() => this.VerifyDisposed(this.source.GetEnumerator());

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        void ICollection.CopyTo(Array array, int index) => this.source.CopyTo(array, index);

        int IList.Add(object value) => ThrowHelper.ThrowCollectionIsReadonly<int>();

        bool IList.Contains(object value) => this.source.Contains(value);

        void IList.Clear() => ThrowHelper.ThrowCollectionIsReadonly();

        int IList.IndexOf(object value) => this.source.IndexOf(value);

        void IList.Insert(int index, object value) => ThrowHelper.ThrowCollectionIsReadonly();

        void IList.Remove(object value) => ThrowHelper.ThrowCollectionIsReadonly();

        void IList.RemoveAt(int index) => ThrowHelper.ThrowCollectionIsReadonly();

        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            this.subscriptions.Dispose();
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e) => this.CollectionChanged?.Invoke(this, e);

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e) => this.PropertyChanged?.Invoke(this, e);

        protected void VerifyDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }

        protected void VerifyDisposed(Action action)
        {
            this.VerifyDisposed();
            action();
        }

        protected TResult VerifyDisposed<TResult>(TResult result)
        {
            this.VerifyDisposed();
            return result;
        }

        private IDisposable Subscribe<TCol>(TCol col) where TCol : INotifyCollectionChanged, INotifyPropertyChanged
        {
            var subscriptions = new CompositeDisposable(2) { col.ObservePropertyChangedSlim()
                                                                .Subscribe(this.OnPropertyChanged),
                                                             col.ObserveCollectionChangedSlim(false)
                                                                .Subscribe(this.OnCollectionChanged) };
            return subscriptions;
        }
    }
}