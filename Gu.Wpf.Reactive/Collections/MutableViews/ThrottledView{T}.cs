namespace Gu.Wpf.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;

    using Gu.Reactive;

    /// <summary>
    /// A view of a collection that buffers changes before notifying.
    /// </summary>
    public class ThrottledView<T> : SynchronizedEditableView<T>, IThrottledView<T>, IReadOnlyThrottledView<T>
    {
        private readonly IDisposable refreshSubscription;
        private readonly Chunk<NotifyCollectionChangedEventArgs> chunk;

        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThrottledView{T}"/> class.
        /// </summary>
        public ThrottledView(ObservableCollection<T> source, TimeSpan bufferTime)
            : this(source, bufferTime, WpfSchedulers.Dispatcher)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThrottledView{T}"/> class.
        /// </summary>
        public ThrottledView(IObservableCollection<T> source, TimeSpan bufferTime)
            : this((IList<T>)source, bufferTime, WpfSchedulers.Dispatcher)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThrottledView{T}"/> class.
        /// Exposing this for tests.
        /// </summary>
        internal ThrottledView(ObservableCollection<T> source, TimeSpan bufferTime, IScheduler scheduler)
            : this((IList<T>)source, bufferTime, scheduler)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThrottledView{T}"/> class.
        /// Exposing this for tests.
        /// </summary>
        internal ThrottledView(IObservableCollection<T> source, TimeSpan bufferTime, IScheduler scheduler)
            : this((IList<T>)source, bufferTime, scheduler)
        {
        }

        private ThrottledView(IList<T> source, TimeSpan bufferTime, IScheduler scheduler)
            : base(source, true)
        {
            this.chunk = new Chunk<NotifyCollectionChangedEventArgs>(bufferTime, scheduler ?? DefaultScheduler.Instance);
            this.refreshSubscription = ((INotifyCollectionChanged)source).ObserveCollectionChangedSlim(false)
                                                                         .Where(this.IsSourceChange)
                                                                         .AsSlidingChunk(this.chunk)
                                                                         .ObserveOn(scheduler ?? ImmediateScheduler.Instance)
                                                                         .StartWith(Chunk.One(CachedEventArgs.NotifyCollectionReset))
                                                                         .Subscribe(this.Refresh);
        }

        /// <summary>
        /// The time to buffer changes before notifying.
        /// </summary>
        public TimeSpan BufferTime
        {
            get
            {
                return this.chunk.BufferTime;
            }

            set
            {
                if (value == this.chunk.BufferTime)
                {
                    return;
                }

                this.chunk.BufferTime = value;
                this.OnPropertyChanged();
            }
        }

        /// <inheritdoc/>
        public override void Refresh()
        {
            this.ThrowIfDisposed();
            lock (this.chunk.Gate)
            {
                base.Refresh();
                this.chunk.Clear();
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Refresh(Chunk<NotifyCollectionChangedEventArgs> changes)
        {
            lock (changes.Gate)
            {
                base.Refresh(changes);
                changes.Clear();
            }
        }

        /// <summary>
        /// Protected implementation of Dispose pattern.
        /// </summary>
        /// <param name="disposing">true: safe to free managed resources</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            if (disposing)
            {
                this.refreshSubscription.Dispose();
                lock (this.chunk.Gate)
                {
                    this.chunk.Clear();
                }
            }
        }

        /// <summary>
        /// Throws an <see cref="ObjectDisposedException"/> if the instance is disposed.
        /// </summary>
        protected void ThrowIfDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }
    }
}
