namespace Gu.Reactive
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// A fixed size queue. Overflow is trimmed when adding more items than max.
    /// Wraps a <see cref="ConcurrentQueue{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the items in the collection.</typeparam>
    [Serializable]
    [DebuggerTypeProxy(typeof(CollectionDebugView<>))]
    [DebuggerDisplay("Count = {this.Count}")]
#pragma warning disable CA1010 // Collections should implement generic interface
    public class FixedSizedQueue<T> : IProducerConsumerCollection<T>
#pragma warning restore CA1010 // Collections should implement generic interface
    {
        private readonly ConcurrentQueue<T> innerQueue = new ConcurrentQueue<T>();
        private readonly object gate = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedSizedQueue{T}"/> class.
        /// </summary>
        /// <param name="size">The maximum number of elements in the queue.</param>
        public FixedSizedQueue(int size)
        {
            this.Size = size;
        }

        /// <summary>
        /// Gets the maximum number of items.
        /// </summary>
        public int Size { get; }

        /// <inheritdoc/>
        public int Count => this.innerQueue.Count;

#pragma warning disable CA1033 // Interface methods should be callable by child types
        /// <inheritdoc/>
        object ICollection.SyncRoot => ((ICollection)this.innerQueue).SyncRoot;

        /// <inheritdoc/>
        bool ICollection.IsSynchronized => ((ICollection)this.innerQueue).IsSynchronized;
#pragma warning restore CA1033 // Interface methods should be callable by child types

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator() => this.innerQueue.GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <summary>Adds an object to the end of the <see cref="FixedSizedQueue{T}"/> overflow is trimmed.</summary>
        /// <param name="item">The object to add to the end of the <see cref="FixedSizedQueue{T}"/>. The value can be a null reference (Nothing in Visual Basic) for reference types.</param>
        public virtual void Enqueue(T item)
        {
            this.innerQueue.Enqueue(item);
            if (this.innerQueue.Count > this.Size)
            {
                lock (this.gate)
                {
                    while (this.innerQueue.Count > this.Size && this.innerQueue.TryDequeue(out var _))
                    {
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void CopyTo(Array array, int index) => ((ICollection)this.innerQueue).CopyTo(array, index);

        /// <inheritdoc/>
        public void CopyTo(T[] array, int index) => this.innerQueue.CopyTo(array, index);

        /// <inheritdoc/>
        public bool TryAdd(T item) => ((IProducerConsumerCollection<T>)this.innerQueue).TryAdd(item);

        /// <inheritdoc/>
        public bool TryTake(out T item) => ((IProducerConsumerCollection<T>)this.innerQueue).TryTake(out item);

        /// <inheritdoc/>
        public T[] ToArray() => this.innerQueue.ToArray();

        /// <summary>Tries to return an object from the beginning of the <see cref="FixedSizedQueue{T}" /> without removing it.</summary>
        /// <returns>true if an object was returned successfully; otherwise, false.</returns>
        /// <param name="result">When this method returns, <paramref name="result" /> contains an object from the beginning of the <see cref="FixedSizedQueue{T}" /> or an unspecified value if the operation failed.</param>
        public bool TryPeek(out T result) => this.innerQueue.TryPeek(out result);

        /// <summary>Tries to remove and return the object at the beginning of the concurrent queue.</summary>
        /// <returns>true if an element was removed and returned from the beginning of the <see cref="FixedSizedQueue{T}" /> successfully; otherwise, false.</returns>
        /// <param name="result">When this method returns, if the operation was successful, <paramref name="result" /> contains the object removed. If no object was available to be removed, the value is unspecified.</param>
        protected bool TryDequeue(out T result) => this.innerQueue.TryDequeue(out result);
    }
}
