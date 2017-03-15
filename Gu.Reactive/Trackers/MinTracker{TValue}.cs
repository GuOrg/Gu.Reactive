namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;

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
        /// <param name="source">The changes of the source collection.</param>
        public MinTracker(IChanges<TValue> source)
            : base(source)
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
                return;
            }

            if (Comparer<TValue>.Default.Compare(value, current.Value) == 0)
            {
                this.Reset();
            }
        }

        /// <inheritdoc/>
        protected override TValue? GetValueOrDefault(IEnumerable<TValue> source)
        {
            var comparer = Comparer<TValue>.Default;
            TValue? value = null;
            foreach (var x in source)
            {
                if (value != null)
                {
                    if (comparer.Compare(x, value.Value) < 0)
                    {
                        value = x;
                    }
                }
                else
                {
                    value = x;
                }
            }

            return value;
        }
    }
}
