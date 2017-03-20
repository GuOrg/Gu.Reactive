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
                                                                         .Slide(this.chunk)
                                                                         .ObserveOn(scheduler ?? ImmediateScheduler.Instance)
                                                                         .StartWith(this.chunk.Add(CachedEventArgs.NotifyCollectionReset))
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
            using (this.chunk.ClearTransaction())
            {
                base.Refresh();
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
                this.chunk.ClearItems();
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

        private void Refresh(Chunk<NotifyCollectionChangedEventArgs> changes)
        {
            using (changes.ClearTransaction())
            {
                base.Refresh(changes);
            }
        }
    }
}
