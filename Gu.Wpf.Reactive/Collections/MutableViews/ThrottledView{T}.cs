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

        /// <summary>
        /// Initializes a new instance of the <see cref="ThrottledView{T}"/> class.
        /// </summary>
        public ThrottledView(ObservableCollection<T> source, TimeSpan bufferTime, bool leaveOpen)
            : this(source, bufferTime, WpfSchedulers.Dispatcher, leaveOpen)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThrottledView{T}"/> class.
        /// </summary>
        public ThrottledView(IObservableCollection<T> source, TimeSpan bufferTime, bool leaveOpen)
            : this((IList<T>)source, bufferTime, WpfSchedulers.Dispatcher, leaveOpen)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThrottledView{T}"/> class.
        /// Exposing this for tests.
        /// </summary>
        internal ThrottledView(ObservableCollection<T> source, TimeSpan bufferTime, IScheduler scheduler, bool leaveOpen)
            : this((IList<T>)source, bufferTime, scheduler, leaveOpen)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThrottledView{T}"/> class.
        /// Exposing this for tests.
        /// </summary>
        /// <param name="source">The source collection.</param>
        /// <param name="bufferTime">The time to defer updates, useful if many triggers fire in short time. Then it will be only one Reset</param>
        /// <param name="scheduler">The scheduler used when throttling. The collection changed events are raised on this scheduler</param>
        /// <param name="leaveOpen">True means that the <paramref name="source"/> is not disposed when this instance is diposed.</param>
        internal ThrottledView(IObservableCollection<T> source, TimeSpan bufferTime, IScheduler scheduler, bool leaveOpen)
            : this((IList<T>)source, bufferTime, scheduler, leaveOpen)
        {
        }

        private ThrottledView(IList<T> source, TimeSpan bufferTime, IScheduler scheduler, bool leaveOpen)
            : base(source, leaveOpen, true)
        {
            this.chunk = new Chunk<NotifyCollectionChangedEventArgs>(bufferTime, scheduler ?? DefaultScheduler.Instance);
            this.refreshSubscription = ((INotifyCollectionChanged)source).ObserveCollectionChangedSlim(false)
                                                                         .Where(this.IsSourceChange)
                                                                         .Slide(this.chunk)
                                                                         .ObserveOn(scheduler ?? ImmediateScheduler.Instance)
                                                                         .StartWith(this.chunk.Add(CachedEventArgs.NotifyCollectionReset))
                                                                         .Subscribe(this.Update);
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

        /// <summary>
        /// Protected implementation of Dispose pattern.
        /// </summary>
        /// <param name="disposing">true: safe to free managed resources</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.refreshSubscription.Dispose();
                this.chunk.ClearItems();
            }

            base.Dispose(disposing);
        }

        /// <inheritdoc/>
        protected sealed override void Refresh(IReadOnlyList<NotifyCollectionChangedEventArgs> changes)
        {
            base.Refresh(changes);
        }

        private void Update(Chunk<NotifyCollectionChangedEventArgs> changes)
        {
            using (changes.ClearTransaction())
            {
                this.Refresh(changes);
            }
        }
    }
}
