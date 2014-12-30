// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FixedSizedQueue.cs" company="">
//   
// </copyright>
// <summary>
//   The fixed sized queue.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gu.Reactive
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    /// <summary>
    /// The fixed sized queue.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    [Serializable]
    public class FixedSizedQueue<T> : IEnumerable<T>
    {
        /// <summary>
        /// The inner queue.
        /// </summary>
        protected readonly ConcurrentQueue<T> InnerQueue = new ConcurrentQueue<T>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedSizedQueue{T}"/> class.
        /// </summary>
        /// <param name="size">
        /// The size.
        /// </param>
        public FixedSizedQueue(int size)
        {
            Size = size;
        }

        /// <summary>
        /// Gets the size.
        /// </summary>
        public int Size { get; private set; }

        /// <summary>
        /// The get enumerator.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerator"/>.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            return InnerQueue.GetEnumerator();
        }

        /// <summary>
        /// The get enumerator.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerator"/>.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// The enqueue.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public virtual void Enqueue(T item)
        {
            InnerQueue.Enqueue(item);
            if (InnerQueue.Count > Size)
            {
                lock (this)
                {
                    T overflow;
                    while (InnerQueue.Count > Size && InnerQueue.TryDequeue(out overflow))
                    {
                    }
                }
            }
        }
    }
}
