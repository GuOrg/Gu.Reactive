namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Reactive.Concurrency;

    [Serializable]
    public class ObservableFixedSizeQueue<T> : FixedSizedQueue<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private readonly IScheduler _scheduler;

        public ObservableFixedSizeQueue(int size)
            : base(size)
        {
        }

        public ObservableFixedSizeQueue(int size, IScheduler scheduler)
            : base(size)
        {
            _scheduler = scheduler;
        }

        [field: NonSerialized]
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        public override void Enqueue(T item)
        {
            var count = Count;
            T overflow = default(T);
            if (Count >= Size)
            {
                while (Count >= Size && TryDequeue(out overflow)) { }
            }
            base.Enqueue(item);
            if (Count != count)
            {
                OnPropertyChanged(Notifier.CountPropertyChangedEventArgs);
            }
            OnPropertyChanged(Notifier.IndexerPropertyChangedEventArgs);
            if (count >= Size)
            {
                CollectionChanged.Notify(this, Diff.CreateRemoveEventArgs(overflow, 0), _scheduler);
            }
            CollectionChanged.Notify(this, Diff.CreateAddEventArgs(item, Count - 1), _scheduler);
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
