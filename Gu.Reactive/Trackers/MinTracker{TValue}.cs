namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;

    /// <summary>
    /// A tracker for minimum value in a collection.
    /// </summary>
    /// <typeparam name="TValue">The type of the items in the collection.</typeparam>
    public sealed class MinTracker<TValue> : Tracker<TValue>
        where TValue : struct, IComparable<TValue>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MinTracker{TValue}"/> class.
        /// </summary>
        /// <param name="source">The source collection.</param>
        /// <param name="onChanged">The </param>
        /// <param name="whenEmpty">The Value to use when <paramref name="source"/> is empty.</param>
        public MinTracker(
            IReadOnlyList<TValue> source,
            IObservable<NotifyCollectionChangedEventArgs> onChanged,
            TValue? whenEmpty)
            : base(source, onChanged, whenEmpty)
        {
            this.Reset();
        }

        /// <inheritdoc/>
        protected override void OnAdd(TValue value)
        {
            var current = this.Value;
            if (current == null)
            {
                this.Value = value;
                return;
            }

            if (Comparer<TValue>.Default.Compare(value, current.Value) < 0)
            {
                this.Value = value;
            }
        }

        /// <inheritdoc/>
        protected override void OnRemove(TValue value)
        {
            var current = this.Value;
            if (current == null)
            {
                this.Reset();
                return;
            }

            if (Comparer<TValue>.Default.Compare(value, current.Value) == 0)
            {
                this.Reset();
            }
        }

        /// <inheritdoc/>
        protected override void OnReplace(TValue oldValue, TValue newValue)
        {
            var current = this.Value;
            if (current == null)
            {
                this.Reset();
                return;
            }

            if (Comparer<TValue>.Default.Compare(oldValue, current.Value) == 0)
            {
                this.Reset();
            }
            else
            {
                this.OnAdd(newValue);
            }
        }

        /// <inheritdoc/>
        protected override TValue GetValue(IReadOnlyList<TValue> source)
        {
            lock (this.Gate)
            {
                return source.Min();
            }
        }
    }
}
