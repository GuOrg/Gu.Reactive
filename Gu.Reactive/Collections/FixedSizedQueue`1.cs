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
        private readonly ConcurrentQueue<T> innerQueue = new ConcurrentQueue<T>();

        public FixedSizedQueue(int size)
        {
            this.Size = size;
        }

        public int Size { get; }

        public int Count => this.innerQueue.Count;

        object ICollection.SyncRoot => ((ICollection)this.innerQueue).SyncRoot;

        bool ICollection.IsSynchronized => ((ICollection)this.innerQueue).IsSynchronized;

        public IEnumerator<T> GetEnumerator() => this.innerQueue.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public virtual void Enqueue(T item)
        {
            this.innerQueue.Enqueue(item);
            if (this.innerQueue.Count > this.Size)
            {
                lock (this)
                {
                    T overflow;
                    while (this.innerQueue.Count > this.Size && this.innerQueue.TryDequeue(out overflow))
                    {
                    }
                }
            }
        }

        public void CopyTo(Array array, int index) => ((ICollection)this.innerQueue).CopyTo(array, index);

        public void CopyTo(T[] array, int index) => this.innerQueue.CopyTo(array, index);

        public bool TryAdd(T item) => ((IProducerConsumerCollection<T>)this.innerQueue).TryAdd(item);

        public bool TryTake(out T item) => ((IProducerConsumerCollection<T>)this.innerQueue).TryTake(out item);

        public T[] ToArray() => this.innerQueue.ToArray();

        protected bool TryDequeue(out T overflow) => this.innerQueue.TryDequeue(out overflow);
    }
}
