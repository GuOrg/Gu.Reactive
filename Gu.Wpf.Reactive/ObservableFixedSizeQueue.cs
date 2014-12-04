namespace Gu.Wpf.Reactive
{
    using System.Collections.Specialized;
    using System.Reactive.Concurrency;

    using Gu.Reactive;

    public class ObservableFixedSizeQueue<T> : FixedSizedQueue<T>, INotifyCollectionChanged
    {
        public ObservableFixedSizeQueue(int size)
            : base(size)
        {
        }


        public event NotifyCollectionChangedEventHandler CollectionChanged;


        public override void Enqueue(T item)
        {
            base.Enqueue(item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, InnerQueue.Count - 1));
            if (InnerQueue.Count > Size)
            {
                lock (this)
                {
                    T overflow;
                    while (InnerQueue.Count > Size && InnerQueue.TryDequeue(out overflow))
                    {
                        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, overflow, 0));
                    }
                }
            }
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            var handler = CollectionChanged;
            if (handler != null)
            {
                Schedulers.DispatcherOrCurrentThread.Schedule(() => handler(this, e));
            }
        }
    }
}
