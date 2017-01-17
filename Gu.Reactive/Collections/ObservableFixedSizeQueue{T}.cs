namespace Gu.Reactive
{
    using System;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Reactive.Concurrency;

    /// <summary>
    /// A fixed size queue that notifies.
    /// </summary>
    [DebuggerTypeProxy(typeof(CollectionDebugView<>))]
    [Serializable]
    public class ObservableFixedSizeQueue<T> : FixedSizedQueue<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private readonly IScheduler scheduler;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableFixedSizeQueue{T}"/> class.
        /// </summary>
        /// <param name="size">The maximum number of elements in the queue.</param>
        public ObservableFixedSizeQueue(int size)
            : base(size)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableFixedSizeQueue{T}"/> class.
        /// </summary>
        /// <param name="size">The maximum number of elements in the queue.</param>
        /// <param name="scheduler">The schedluer to notify collection changes on.</param>
        public ObservableFixedSizeQueue(int size, IScheduler scheduler)
            : base(size)
        {
            this.scheduler = scheduler;
        }

        /// <inheritdoc/>
        [field: NonSerialized]
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <inheritdoc/>
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc/>
        public override void Enqueue(T item)
        {
            var count = this.Count;
            var overflow = default(T);
            if (this.Count >= this.Size)
            {
                while (this.Count >= this.Size && this.TryDequeue(out overflow))
                {
                }
            }

            base.Enqueue(item);
            if (this.Count != count)
            {
                this.OnPropertyChanged(Notifier.CountPropertyChangedEventArgs);
            }

            if (count >= this.Size)
            {
                this.CollectionChanged.Notify(this, Diff.CreateRemoveEventArgs(overflow, 0), this.scheduler);
            }

            this.CollectionChanged.Notify(this, Diff.CreateAddEventArgs(item, this.Count - 1), this.scheduler);
        }

        /// <summary>
        /// Raise PropertyChanged event to any listeners.
        /// Properties/methods modifying this <see cref="ObservableFixedSizeQueue{T}"/> will raise
        /// a property changed event through this virtual method.
        /// </summary>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            this.PropertyChanged?.Invoke(this, e);
        }
    }
}
