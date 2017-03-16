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
    [Obsolete("Candidate for removal, broken. Prefer the read only version.")]
    public class ThrottledView<T> : SynchronizedEditableView<T>, IThrottledView<T>, IReadOnlyThrottledView<T>
    {
        private readonly IDisposable refreshSubscription;
        private TimeSpan bufferTime;
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThrottledView{T}"/> class.
        /// </summary>
        public ThrottledView(ObservableCollection<T> source, TimeSpan bufferTime, IScheduler scheduler)
            : this((IList<T>)source, bufferTime, scheduler)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThrottledView{T}"/> class.
        /// </summary>
        public ThrottledView(IObservableCollection<T> source, TimeSpan bufferTime, IScheduler scheduler)
            : this((IList<T>)source, bufferTime, scheduler)
        {
        }

        private ThrottledView(IList<T> source, TimeSpan bufferTime, IScheduler scheduler)
            : base(source, true)
        {
            this.bufferTime = bufferTime;
            this.refreshSubscription = ((INotifyCollectionChanged)source).ObserveCollectionChangedSlim(false)
                                                                         .Where(this.IsSourceChange)
                                                                         .Publish(
                                                                             shared =>
                                                                                 this.ObservePropertyChangedSlim(nameof(this.BufferTime))
                                                                                     .Select(_ => shared.Chunks(bufferTime, scheduler)
                                                                                                        .ObserveOn(scheduler ?? ImmediateScheduler.Instance))
                                                                                     .Switch())
                                                                         .StartWith(CachedEventArgs.SingleNotifyCollectionReset)
                                                                         .Subscribe(this.Refresh);
        }

        /// <summary>
        /// The time to buffer changes before notifying.
        /// </summary>
        public TimeSpan BufferTime
        {
            get
            {
                return this.bufferTime;
            }

            set
            {
                if (value == this.bufferTime)
                {
                    return;
                }

                this.bufferTime = value;
                this.OnPropertyChanged();
            }
        }

        /// <inheritdoc/>
        public override void Refresh()
        {
            this.ThrowIfDisposed();
            base.Refresh();
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

        /// <inheritdoc/>
        protected sealed override void Refresh(IReadOnlyList<NotifyCollectionChangedEventArgs> changes)
        {
            base.Refresh(changes);
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
