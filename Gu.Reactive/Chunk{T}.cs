namespace Gu.Reactive
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reactive.Concurrency;
    using System.Runtime.CompilerServices;
    using System.Threading;

    /// <summary>
    /// A batch of changes.
    /// </summary>
    /// <typeparam name="T">The type of the items in the collection.</typeparam>
    public class Chunk<T> : IReadOnlyList<T>, INotifyPropertyChanged
    {
        private readonly List<T> items = new List<T>();
        private readonly object gate = new object();

        private TimeSpan bufferTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="Chunk{T}"/> class.
        /// </summary>
        /// <param name="bufferTime">The time to buffer changes.</param>
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        public Chunk(TimeSpan bufferTime, IScheduler scheduler)
        {
            this.bufferTime = bufferTime;
            this.Scheduler = scheduler;
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets the scheduler to throttle changes on.
        /// </summary>
        public IScheduler Scheduler { get; }

        /// <inheritdoc/>
        public int Count => this.items.Count;

        /// <summary>
        /// Gets or sets the time to buffer changes.
        /// </summary>
        public TimeSpan BufferTime
        {
            get => this.bufferTime;

            set
            {
                if (value == this.bufferTime)
                {
                    return;
                }

                this.bufferTime = value;
                this.OnPropertyChanged();
            }
        }

        /// <inheritdoc/>
        public T this[int index]
        {
            get
            {
                lock (this.gate)
                {
                    return this.items[index];
                }
            }
        }

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator()
        {
            lock (this.gate)
            {
                return this.items.GetEnumerator();
            }
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)this.items).GetEnumerator();

        /// <summary>
        /// Create a transaction that locks <see cref="gate"/>
        /// On dispose the inner collection is cleared and the lock is released.
        /// </summary>
        /// <returns>A transaction that locks this instance for editing until disposed.</returns>
        public IDisposable ClearTransaction()
        {
            return new ClearTransactionImpl(this);
        }

        /// <summary>
        /// Clear the inner collection.
        /// </summary>
        public void ClearItems()
        {
            lock (this.gate)
            {
                this.items.Clear();
            }
        }

        /// <summary>
        /// Add  an item to the inner collection.
        /// Returns self.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The same instance.</returns>
        public Chunk<T> Add(T item)
        {
            lock (this.gate)
            {
                this.items.Add(item);
            }

            return this;
        }

        /// <summary>
        /// Notify about <see cref="PropertyChanged"/> for this instance.
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private sealed class ClearTransactionImpl : IDisposable
        {
            private readonly Chunk<T> chunk;

            private bool disposed;

            internal ClearTransactionImpl(Chunk<T> chunk)
            {
                this.chunk = chunk;
                Monitor.Enter(chunk.gate);
            }

            public void Dispose()
            {
                if (this.disposed)
                {
                    return;
                }

                this.disposed = true;
                this.chunk.items.Clear();
                Monitor.Exit(this.chunk.gate);
            }
        }
    }
}
