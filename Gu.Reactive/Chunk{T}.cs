namespace Gu.Reactive
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reactive.Concurrency;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// A batch of changes.
    /// </summary>
    public class Chunk<T> : IReadOnlyList<T>, INotifyPropertyChanged
    {
        private readonly List<T> items = new List<T>();
        private TimeSpan bufferTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="Chunk{T}"/> class.
        /// </summary>
        public Chunk(TimeSpan bufferTime, IScheduler scheduler)
        {
            this.bufferTime = bufferTime;
            this.Scheduler = scheduler;
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Lock mutation of this instance
        /// </summary>
        public object Gate { get; } = new object();

        /// <summary>
        /// The scheduler to throttle changes on.
        /// </summary>
        public IScheduler Scheduler { get; }

        /// <inheritdoc/>
        public int Count => this.items.Count;

        /// <summary>
        /// The time to buffer changes.
        /// </summary>
        public TimeSpan BufferTime
        {
            get
            {
                return this.bufferTime;
            }

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
                lock (this.Gate)
                {
                    return this.items[index];
                }
            }
        }

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator()
        {
            lock (this.Gate)
            {
                return this.items.GetEnumerator();
            }
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)this.items).GetEnumerator();

        /// <summary>
        /// Clear the inner collection.
        /// </summary>
        public void Clear()
        {
            lock (this.Gate)
            {
                this.items.Clear();
            }
        }

        /// <summary>
        /// Add  an item to the inner collection.
        /// Returns self
        /// </summary>
        public Chunk<T> Add(T item)
        {
            lock (this.Gate)
            {
                this.items.Add(item);
            }

            return this;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}