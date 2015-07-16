namespace Gu.Reactive
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;

    [Serializable]
    [DebuggerTypeProxy(typeof(CollectionDebugView<>))]
    [DebuggerDisplay("Count = {Count}")] 
    public class FixedSizedQueue<T> : IProducerConsumerCollection<T>
    {
        private readonly ConcurrentQueue<T> _innerQueue = new ConcurrentQueue<T>();

        public FixedSizedQueue(int size)
        {
            Size = size;
        }

        public int Size { get; private set; }

        public int Count
        {
            get { return _innerQueue.Count; }
        }

        object ICollection.SyncRoot
        {
            get { return ((ICollection)_innerQueue).SyncRoot; }
        }

        bool ICollection.IsSynchronized
        {
            get { return ((ICollection)_innerQueue).IsSynchronized; }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _innerQueue.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public virtual void Enqueue(T item)
        {
            _innerQueue.Enqueue(item);
            if (_innerQueue.Count > Size)
            {
                lock (this)
                {
                    T overflow;
                    while (_innerQueue.Count > Size && _innerQueue.TryDequeue(out overflow))
                    {
                    }
                }
            }
        }

        public void CopyTo(Array array, int index)
        {
            ((ICollection)_innerQueue).CopyTo(array, index);
        }

        public void CopyTo(T[] array, int index)
        {
            _innerQueue.CopyTo(array, index);
        }

        public bool TryAdd(T item)
        {
            return ((IProducerConsumerCollection<T>)_innerQueue).TryAdd(item);
        }

        public bool TryTake(out T item)
        {
            return ((IProducerConsumerCollection<T>)_innerQueue).TryTake(out item);
        }

        public T[] ToArray()
        {
            return _innerQueue.ToArray();
        }

        protected bool TryDequeue(out T overflow)
        {
            return _innerQueue.TryDequeue(out overflow);
        }
    }
}
