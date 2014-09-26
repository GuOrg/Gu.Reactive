namespace Gu.Reactive
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Threading;
    using System.Windows;

    public class FixedSizedQueue<T> : IEnumerable<T>, INotifyCollectionChanged
    {
        private readonly ConcurrentQueue<T> _innerQueue = new ConcurrentQueue<T>();
        public FixedSizedQueue(int size)
        {
            Size = size;
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public int Size { get; set; }
        public IEnumerator<T> GetEnumerator()
        {
            return _innerQueue.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public void Enqueue(T item)
        {
            _innerQueue.Enqueue(item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, _innerQueue.Count - 1));
            if (_innerQueue.Count > Size)
            {
                lock (this)
                {
                    T overflow;
                    while (_innerQueue.Count > Size && _innerQueue.TryDequeue(out overflow))
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
                if(SynchronizationContext.Current != null)
                {
                    var scheduler = new SynchronizationContextScheduler(SynchronizationContext.Current);
                    scheduler.Schedule(() => handler(this, e));
                    //System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() => handler(this, e)));
                }
                else
                {
                    handler(this, e);
                }
            }
        }
    }
}
