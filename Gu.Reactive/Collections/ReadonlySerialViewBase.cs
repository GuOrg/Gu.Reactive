namespace Gu.Reactive
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;

    [DebuggerTypeProxy(typeof(CollectionDebugView<>))]
    [DebuggerDisplay("Count = {Count}")]
    public abstract class ReadonlySerialViewBase<T> : IRefreshAble, IList, IDisposable
    {
        private static readonly IReadOnlyList<T> Empty = new T[0];
        private readonly CollectionSynchronizer<T> _tracker;

        private bool _disposed;

        protected ReadonlySerialViewBase()
            : this(null, true, true)
        {
        }

        protected ReadonlySerialViewBase(IEnumerable<T> source)
            : this(source, true, true)
        {
        }

        protected ReadonlySerialViewBase(bool isreadonly, bool isFixedSize)
            : this(null, isreadonly, isFixedSize)
        {
        }

        protected ReadonlySerialViewBase(IEnumerable<T> source, bool isreadonly, bool isFixedSize)
        {
            IsReadOnly = isreadonly;
            IsFixedSize = isFixedSize;
            Source = source ?? Empty;
            _tracker = new CollectionSynchronizer<T>(source ?? Empty);
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        protected IEnumerable<T> Source { get; private set; }

        /// <summary>
        /// Make the class sealed when using this. 
        /// Call VerifyDisposed at the start of all public methods
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;
            Dispose(true);
            // Dispose some stuff now
        }

        public virtual void Refresh()
        {
            VerifyDisposed();
            try
            {
                var source = Source.AsReadOnly();
                _tracker.Reset(this, source, null, PropertyChanged, CollectionChanged);
            }
            catch (InvalidOperationException) // when (e.Message == Environment.GetCollectionWasModifiedText())
            {
                Refresh();
            }
        }

        protected void SetSource(IEnumerable<T> source)
        {
            VerifyDisposed();
            Source = source ?? Empty;
            Refresh();
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
            _disposed = true;
            if (disposing)
            {
                // Free any other managed objects here. 
            }

            // Free any unmanaged objects here. 
        }

        protected void ClearSource()
        {
            VerifyDisposed();
            Source = Empty;
            Refresh();
        }

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

        protected TResult VerifyDisposed<TResult>(Func<TResult> action)
        {
            VerifyDisposed();
            return action();
        }

        protected virtual void Refresh(IReadOnlyList<NotifyCollectionChangedEventArgs> changes)
        {
            try
            {
                var source = Source.AsReadOnly();
                _tracker.Refresh(this, source, changes, null, PropertyChanged, CollectionChanged);
            }
            catch (InvalidOperationException) // when (e.Message == Environment.GetCollectionWasModifiedText())
            {
                Refresh(changes);
            }
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, e);
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        #region IReadOnlyList<T> & IList

        public int Count => VerifyDisposed(() => _tracker.Count);

        public bool IsReadOnly { get; }

        public bool IsFixedSize { get; }

        object ICollection.SyncRoot => _tracker.SyncRoot;

        bool ICollection.IsSynchronized => _tracker.IsSynchronized;

        object IList.this[int index]
        {
            get { return this[index]; }
            set { ThrowHelper.ThrowCollectionIsReadonly(); }
        }

        public T this[int index] => VerifyDisposed(() => _tracker.Current[index]);

        public IEnumerator<T> GetEnumerator() => VerifyDisposed(() => _tracker.GetEnumerator());

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        int IList.Add(object value) => ThrowHelper.ThrowCollectionIsReadonly<int>();

        bool IList.Contains(object value) => _tracker.Contains(value);

        void IList.Clear() => ThrowHelper.ThrowCollectionIsReadonly();

        int IList.IndexOf(object value) => _tracker.IndexOf(value);

        void IList.Insert(int index, object value) => ThrowHelper.ThrowCollectionIsReadonly();

        void IList.Remove(object value) => ThrowHelper.ThrowCollectionIsReadonly();

        void IList.RemoveAt(int index) => ThrowHelper.ThrowCollectionIsReadonly();

        void ICollection.CopyTo(Array array, int index) => _tracker.CopyTo(array, index);

        #endregion IReadOnlyList<T>
    }
}