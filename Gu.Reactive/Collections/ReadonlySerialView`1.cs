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
        private readonly CollectionSynchronizer<T> _tracker;
        private IReadOnlyList<T> _source;
        private readonly SerialDisposable _refreshSubscription = new SerialDisposable();
        private bool _disposed;

        public ReadOnlySerialView()
            : this((IReadOnlyList<T>)null)
        {
        }

        public ReadOnlySerialView(ObservableCollection<T> source)
            : this((IReadOnlyList<T>)source)
        {
        }

        public ReadOnlySerialView(ReadOnlyObservableCollection<T> source)
            : this((IReadOnlyList<T>)source)
        {
        }

        public ReadOnlySerialView(IObservableCollection<T> source)
            : this((IReadOnlyList<T>)source)
        {
        }

        public ReadOnlySerialView(IReadOnlyObservableCollection<T> source)
            : this((IReadOnlyList<T>)source)
        {
        }

        private ReadOnlySerialView(IReadOnlyList<T> source)
        {
            _source = source ?? Empty;
            _tracker = new CollectionSynchronizer<T>(source);
            _refreshSubscription.Disposable = ThrottledRefresher.Create(this, source, TimeSpan.Zero, null, false)
                                                                .Subscribe(Refresh);
        }


        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        object IUpdater.IsUpdatingSourceItem
        {
            get { return null; }
        }

        public void SetSource(ObservableCollection<T> collection)
        {
            SetSource((IReadOnlyList<T>)collection);
        }

        public void SetSource(ReadOnlyObservableCollection<T> collection)
        {
            SetSource((IReadOnlyList<T>)collection);
        }

        public void SetSource(IObservableCollection<T> collection)
        {
            SetSource((IReadOnlyList<T>)collection);
        }

        public void SetSource(IReadOnlyObservableCollection<T> collection)
        {
            SetSource((IReadOnlyList<T>)collection);
        }

        public void ClearSource()
        {
            SetSource((IReadOnlyList<T>)null);
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
            _tracker.Reset(this, _source, null, PropertyChanged, CollectionChanged);
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
            _source = source ?? Empty;
            _refreshSubscription.Disposable = ThrottledRefresher.Create(this, _source, TimeSpan.Zero, null, false)
                                                                .Subscribe(Refresh);
            Refresh();
        }

        private void Refresh(IReadOnlyList<NotifyCollectionChangedEventArgs> changes)
        {
            _tracker.Refresh(this, _source, changes, null, PropertyChanged, CollectionChanged);
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