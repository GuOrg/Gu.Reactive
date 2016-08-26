namespace Gu.Reactive
{
    using System;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Reactive.Concurrency;

    [Serializable]
    public class ObservableFixedSizeQueue<T> : FixedSizedQueue<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private readonly IScheduler scheduler;

        public ObservableFixedSizeQueue(int size)
            : base(size)
        {
        }

        public ObservableFixedSizeQueue(int size, IScheduler scheduler)
            : base(size)
        {
            this.scheduler = scheduler;
        }

        [field: NonSerialized]
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        public override void Enqueue(T item)
        {
            var count = this.Count;
            T overflow = default(T);
            if (this.Count >= this.Size)
            {
                while (this.Count >= this.Size && this.TryDequeue(out overflow)) { }
            }

            base.Enqueue(item);
            if (this.Count != count)
            {
                this.OnPropertyChanged(Notifier.CountPropertyChangedEventArgs);
            }

            this.OnPropertyChanged(Notifier.IndexerPropertyChangedEventArgs);
            if (count >= this.Size)
            {
                this.CollectionChanged.Notify(this, Diff.CreateRemoveEventArgs(overflow, 0), this.scheduler);
            }

            this.CollectionChanged.Notify(this, Diff.CreateAddEventArgs(item, this.Count - 1), this.scheduler);
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            this.PropertyChanged?.Invoke(this, e);
        }
    }
}
