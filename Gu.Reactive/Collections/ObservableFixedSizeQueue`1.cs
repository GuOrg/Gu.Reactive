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
            var eventArgses = new List<EventArgs> { Diff.IndexerPropertyChangedEventArgs };
            if (Count >= Size)
            {
                T overflow;
                while (Count >= Size && TryDequeue(out overflow))
                {
                    eventArgses.Add(Diff.CreateRemoveEventArgs(overflow, 0));
                }
            }
            base.Enqueue(item);
            eventArgses.Add(Diff.CreateAddEventArgs(item, Count - 1));
            if (Count != count)
            {
                eventArgses.Insert(0, Diff.CountPropertyChangedEventArgs);
            }
            CollectionSynchronizer<T>.Notify(this, eventArgses, _scheduler, PropertyChanged, CollectionChanged);
        }
    }
}
