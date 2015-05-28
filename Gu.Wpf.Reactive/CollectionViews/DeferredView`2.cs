namespace Gu.Wpf.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Windows.Threading;

    using Gu.Reactive;
    using Gu.Reactive.Internals;

    public class DeferredView<TCollection, TItem> : ObservableCollectionWrapperBase<TCollection, TItem>, IDeferredView<TItem>, IDisposable
        where TCollection : IList<TItem>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        internal static readonly TimeSpan DefaultBufferTime = TimeSpan.FromMilliseconds(10);
        private readonly List<NotifyCollectionChangedEventArgs> _collectionChangedArgs = new List<NotifyCollectionChangedEventArgs>();
        private readonly List<PropertyChangedEventArgs> _countChangedArgs = new List<PropertyChangedEventArgs>();
        private readonly List<PropertyChangedEventArgs> _indexerChangedArgs = new List<PropertyChangedEventArgs>();
        private readonly DispatcherTimer _timer;
        private TimeSpan _bufferTime;
        private bool _disposed = false;

        public DeferredView(TCollection inner)
            : this(inner, DefaultBufferTime)
        {
        }
        public DeferredView(TCollection inner, TimeSpan bufferTime)
            : base(inner)
        {
            _bufferTime = bufferTime;
            _timer = new DispatcherTimer(DispatcherPriority.DataBind);
            _timer.Tick += Refresh;
            this.ObservePropertyChanged(x => x.BufferTime, false)
                .Subscribe(_ => DeferredRefresh());
            InnerCollectionChangedObservable.Subscribe(x => AddArgs(_collectionChangedArgs, x.EventArgs));
            InnerCountChangedObservable.Subscribe(x => AddArgs(_countChangedArgs, x.EventArgs));
            InnerIndexerChangedObservable.Subscribe(x => AddArgs(_indexerChangedArgs, x.EventArgs));
        }

        public void Refresh()
        {
            _timer.Stop();
            Notify();
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
            DeferredRefresh();
            col.Add(eventArgs);
        }

        private void DeferredRefresh()
        {
            if (BufferTime > TimeSpan.Zero)
            {
                _timer.Interval = BufferTime;
                _timer.Start();
            }
            else
            {
                _timer.Stop();
                Refresh();
            }
        }

        private void Notify()
        {
            VerifyDisposed();
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

        private void Refresh(object sender, EventArgs e)
        {
            Refresh();
        }
    }
}
