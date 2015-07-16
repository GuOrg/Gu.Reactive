namespace Gu.Reactive
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Reactive.Disposables;

    [DebuggerTypeProxy(typeof(CollectionDebugView<>))]
    [DebuggerDisplay("Count = {Count}")] 
    public sealed class ReadOnlySerialView<T> : IReadOnlyObservableCollection<T>, IUpdater, IDisposable
    {
        private static readonly IReadOnlyList<T> Empty = new T[0];
        private readonly CollectionSynchronizer<T> _tracker;
        private IEnumerable<T> _source;
        private readonly SerialDisposable _refreshSubscription = new SerialDisposable();
        private bool _disposed;

        public ReadOnlySerialView()
            : this(null)
        {
        }

        public ReadOnlySerialView(IEnumerable<T> source)
        {
            _source = source ?? Empty;
            _tracker = new CollectionSynchronizer<T>(_source);
            _refreshSubscription.Disposable = ThrottledRefresher.Create(this, _source, TimeSpan.Zero, null, false)
                                                                .Subscribe(Refresh);
        }


        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        object IUpdater.IsUpdatingSourceItem
        {
            get { return null; }
        }

        public void SetSource(IEnumerable<T> source)
        {
            _source = source ?? Empty;
            _refreshSubscription.Disposable = ThrottledRefresher.Create(this, _source, TimeSpan.Zero, null, false)
                                                                .Subscribe(Refresh);
            Refresh();
        }

        public void ClearSource()
        {
            _source = Empty;
            _refreshSubscription.Disposable = null;
            Refresh();
        }

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
            _refreshSubscription.Dispose();
            // Dispose some stuff now
        }

        public void Refresh()
        {
            var source = _source as IReadOnlyList<T> ?? _source.ToArray();
            _tracker.Reset(this, source, null, PropertyChanged, CollectionChanged);
        }

        private void VerifyDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(
                    GetType()
                        .FullName);
            }
        }

        private void Refresh(IReadOnlyList<NotifyCollectionChangedEventArgs> changes)
        {
            var source =_source as IReadOnlyList<T> ?? _source.ToArray();
            _tracker.Refresh(this, source, changes, null, PropertyChanged, CollectionChanged);
        }

        #region IReadOnlyList<T>

        public int Count { get { return _tracker.Current.Count; } }

        public T this[int index]
        {
            get
            {
                VerifyDisposed();
                return _tracker.Current[index];
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            VerifyDisposed();
            return _tracker.Current.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            VerifyDisposed();
            return GetEnumerator();
        }

        #endregion IReadOnlyList<T>
    }
}