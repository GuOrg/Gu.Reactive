namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;

    /// <summary>
    /// A tracker for average value in a collection.
    /// </summary>
    public sealed class DoubleAverageTracker : Tracker<double>
    {
        private double sum;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoubleAverageTracker"/> class.
        /// </summary>
        /// <param name="source">The source collection.</param>
        /// <param name="onChanged">The change event.</param>
        /// <param name="whenEmpty">The Value to use when <paramref name="source"/> is empty.</param>
        public DoubleAverageTracker(
            IReadOnlyList<double> source,
            IObservable<NotifyCollectionChangedEventArgs> onChanged,
            double whenEmpty)
            : base(source, onChanged, whenEmpty)
        {
            this.Reset();
        }

        /// <inheritdoc/>
        protected override void OnAdd(double value)
        {
            lock (this.Gate)
            {
                this.sum += value;
                this.Value = this.sum / this.Source.Count;
            }
        }

        /// <inheritdoc/>
        protected override void OnRemove(double value)
        {
            lock (this.Gate)
            {
                if (this.Source.Count == 0)
                {
                    this.sum = 0;
                    this.Value = this.WhenEmpty;
                    return;
                }

                this.sum -= value;
                this.Value = this.sum / this.Source.Count;
            }
        }

        /// <inheritdoc/>
        protected override void OnReplace(double oldValue, double newValue)
        {
            lock (this.Gate)
            {
                this.sum -= oldValue;
                this.sum += newValue;
                this.Value = this.sum / this.Source.Count;
            }
        }

        /// <inheritdoc/>
        protected override double GetValue(IReadOnlyList<double> source)
        {
            lock (this.Gate)
            {
                this.sum = source.Sum();
                return this.sum / source.Count;
            }
        }
    }
}