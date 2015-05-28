namespace Gu.Wpf.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Threading;

    using Gu.Reactive;
    using Gu.Reactive.Internals;

    public class DeferredView<TCollection, TItem> : ObservableCollectionWrapperBase<TCollection, TItem>, IDeferredView<TItem>, IDisposable
        where TCollection : IList<TItem>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private readonly object _eventsLock = new object();
        private readonly List<NotifyCollectionChangedEventArgs> _collectionChangedArgs = new List<NotifyCollectionChangedEventArgs>();
        private readonly List<PropertyChangedEventArgs> _countChangedArgs = new List<PropertyChangedEventArgs>();
        private readonly List<PropertyChangedEventArgs> _indexerChangedArgs = new List<PropertyChangedEventArgs>();
        private TimeSpan _bufferTime;
        private bool _disposed = false;
        private IDisposable _timerSubscription;

        public DeferredView(TCollection inner, TimeSpan bufferTime)
            : base(inner)
        {
            _bufferTime = bufferTime;
            this.ObservePropertyChanged(x => x.BufferTime, false)
                .Subscribe(_ => ResetTimer());
            InnerCollectionChangedObservable.Subscribe(x => AddArgs(_collectionChangedArgs, x.EventArgs));
            InnerCountChangedObservable.Subscribe(x => AddArgs(_countChangedArgs, x.EventArgs));
            InnerIndexerChangedObservable.Subscribe(x => AddArgs(_indexerChangedArgs, x.EventArgs));
        }

        public TimeSpan BufferTime
        {
            get
            {
                return _bufferTime;
            }
            set
            {
                if (value == _bufferTime)
                {
                    return;
                }
                _bufferTime = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Dispose(true); //I am calling you from Dispose, it's safe
        /// GC.SuppressFinalize(this); //Hey, GC: don't bother calling finalize later
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public override int Add(object value)
        {
            var i = base.Add(value);
            Notify();
            return i;
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
                lock (_eventsLock)
                {
                    if (_timerSubscription != null)
                    {
                        _timerSubscription.Dispose();
                    }
                }
            }
        }

        protected void VerifyDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }

        private void AddArgs<TArgs>(List<TArgs> col, TArgs eventArgs) where TArgs : EventArgs
        {
            lock (_eventsLock)
            {
                if (_timerSubscription != null)
                {
                    _timerSubscription.Dispose();
                }
                _timerSubscription = Observable.Timer(BufferTime)
                                               .Subscribe(_ => Notify());
                col.Add(eventArgs);
            }
        }

        private void ResetTimer()
        {
            lock (_eventsLock)
            {
                if (_timerSubscription != null)
                {
                    _timerSubscription.Dispose();
                }
                _timerSubscription = Observable.Timer(BufferTime)
                                               .Subscribe(_ => Notify());
            }
        }

        private void Notify()
        {
            VerifyDisposed();
            lock (_eventsLock)
            {
                if (_timerSubscription != null)
                {
                    _timerSubscription.Dispose();
                }

                if (_countChangedArgs.Any())
                {
                    OnPropertyChanged(CountEventArgs);
                    _countChangedArgs.Clear();
                }

                if (_indexerChangedArgs.Any())
                {
                    OnPropertyChanged(IndexerEventArgs);
                    _indexerChangedArgs.Clear();
                }

                if (_collectionChangedArgs.Any())
                {
                    OnCollectionChanged(_collectionChangedArgs);
                    _collectionChangedArgs.Clear();
                }
            }
        }

        private void OnCollectionChanged(IReadOnlyList<NotifyCollectionChangedEventArgs> es)
        {
            if (es == null || es.Count == 0)
            {
                return;
            }
            if (es.Count == 1)
            {
                OnCollectionChanged(es[0]);
            }
            else
            {
                OnCollectionChanged(ResetEventArgs);
            }
        }
    }
}
