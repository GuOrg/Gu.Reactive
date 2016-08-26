namespace Gu.Reactive
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Reactive.Disposables;

    public class EditableListView<T> : IObservableCollection<T>, IList, IDisposable
    {
        private readonly IObservableCollection<T> source;
        private readonly IDisposable subscriptions;

        private bool disposed;

        public EditableListView(IObservableCollection<T> source)
        {
            this.source = source;
            this.subscriptions = this.Subscribe(source);
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        public int Count => this.VerifyDisposed(this.source.Count);

        public bool IsReadOnly => this.VerifyDisposed(false);

        bool IList.IsFixedSize => this.VerifyDisposed(false);

        public T this[int index]
        {
            get { return this.source[index]; }
            set { this.source[index] = value; }
        }

        object IList.this[int index]
        {
            get { return this[index]; }
            set { throw new NotImplementedException("we must notify immediately here"); }
        }

        public IEnumerator<T> GetEnumerator() => this.VerifyDisposed(this.source.GetEnumerator());

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public void Add(T item) => this.VerifyDisposed(() => this.source.Add(item));

        int IList.Add(object value)
        {
            var count = this.source.Count;
            this.source.Add((T)value);
            (this.source as IRefreshAble)?.Refresh();
            return count;
        }

        public bool Remove(T item) => this.source.Remove(item);

        void IList.Remove(object value)
        {
            this.Remove((T)value);
            (this.source as IRefreshAble)?.Refresh();
        }

        public void RemoveAt(int index) => this.source.RemoveAt(index);

        public void Clear()
        {
            this.source.Clear();
        }

        public void Insert(int index, T item) => this.source.Insert(index, item);

        void IList.Insert(int index, object value)
        {
            this.Insert(index, (T)value);
            (this.source as IRefreshAble)?.Refresh();
        }

        object ICollection.SyncRoot => this.VerifyDisposed(((ICollection)this.source).SyncRoot);

        bool ICollection.IsSynchronized => this.VerifyDisposed(((ICollection)this.source).IsSynchronized);

        public void CopyTo(T[] array, int arrayIndex) => this.source.CopyTo(array, arrayIndex);

        void ICollection.CopyTo(Array array, int index) => ListExt.CopyTo(this, array, index);

        public bool Contains(T item) => this.source.Contains(item);

        bool IList.Contains(object value) => this.Contains((T)value);

        public int IndexOf(T item) => this.source.IndexOf(item);

        int IList.IndexOf(object value) => this.IndexOf((T)value);

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