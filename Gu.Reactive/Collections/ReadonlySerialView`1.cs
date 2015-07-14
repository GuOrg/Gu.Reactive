namespace Gu.Reactive
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Reactive.Disposables;

    public sealed class ReadOnlySerialView<T> : IReadOnlyObservableCollection<T>, IUpdater, IDisposable
    {
        private static readonly ReadOnlyObservableCollection<T> Empty = new ReadOnlyObservableCollection<T>(new ObservableCollection<T>());
        private IReadOnlyList<T> _collection;
        private CollectionSynchronizer<T> _tracker;
        private readonly SerialDisposable _refreshSubscription = new SerialDisposable();
        private bool _disposed;


        public ReadOnlySerialView(ObservableCollection<T> collection)
            : this((IReadOnlyList<T>)collection)
        {
        }

        public ReadOnlySerialView(ReadOnlyObservableCollection<T> collection)
            : this((IReadOnlyList<T>)collection)
        {
        }

        public ReadOnlySerialView(IObservableCollection<T> collection)
            : this((IReadOnlyList<T>)collection)
        {
        }

        public ReadOnlySerialView(IReadOnlyObservableCollection<T> collection)
            : this((IReadOnlyList<T>)collection)
        {
        }

        private ReadOnlySerialView(IReadOnlyList<T> collection)
        {
            _collection = collection ?? Empty;
            _tracker = new CollectionSynchronizer<T>(collection);
            _refreshSubscription.Disposable = ThrottledRefresher.Create(this, collection, TimeSpan.Zero, null, false)
                                                                .Subscribe(Refresh);
        }


        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        object IUpdater.IsUpdatingSourceItem
        {
            get { return null; }
        }

        public  void SetSource(ObservableCollection<T> collection)
        {
            SetSource((IReadOnlyList<T>)collection);
        }

        public  void SetSource(ReadOnlyObservableCollection<T> collection)
        {
            SetSource((IReadOnlyList<T>)collection);
        }

        public  void SetSource(IObservableCollection<T> collection)
        {
            SetSource((IReadOnlyList<T>)collection);
        }

        public void SetSource(IReadOnlyObservableCollection<T> collection)
        {
            SetSource((IReadOnlyList<T>)collection);
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
            _tracker.Reset(this, _collection, null, PropertyChanged, CollectionChanged);
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

        private void SetSource(IReadOnlyList<T> source)
        {
            _collection = source ?? Empty;
            _tracker = new CollectionSynchronizer<T>(Empty);
            _refreshSubscription.Disposable = ThrottledRefresher.Create(this, _collection, TimeSpan.Zero, null, false)
                                                                .Subscribe(Refresh);
            Refresh();
        }

        private void Refresh(IReadOnlyList<NotifyCollectionChangedEventArgs> changes)
        {
            _tracker.Refresh(this, _collection, changes, null, PropertyChanged, CollectionChanged);
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