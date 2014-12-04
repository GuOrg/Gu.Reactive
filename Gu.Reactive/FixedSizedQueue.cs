﻿namespace Gu.Reactive
{
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    public class FixedSizedQueue<T> : IEnumerable<T>
    {
        protected readonly ConcurrentQueue<T> InnerQueue = new ConcurrentQueue<T>();
        public FixedSizedQueue(int size)
        {
            Size = size;
        }
        
        public int Size { get; private set; }
        
        public IEnumerator<T> GetEnumerator()
        {
            return InnerQueue.GetEnumerator();
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        public virtual void Enqueue(T item)
        {
            InnerQueue.Enqueue(item);
        }
    }
}
