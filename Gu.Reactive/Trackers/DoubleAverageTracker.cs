namespace Gu.Reactive
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    /// <summary>
    /// A tracker for average value in a collection.
    /// </summary>
    public sealed class DoubleAverageTracker : Tracker<double>
    {
        private readonly ObservableCollection<double> source;
        private double sum;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoubleAverageTracker"/> class.
        /// </summary>
        /// <param name="source">The source collection.</param>
        public DoubleAverageTracker(ObservableCollection<double> source)
            : base(new SimpleChanges<ObservableCollection<double>, double>(source))
        {
            this.source = source;
            this.Reset();
        }

        /// <inheritdoc/>
        protected override void OnAdd(double value)
        {
            this.sum += value;
            this.Value = this.sum / this.source.Count;
        }

        /// <inheritdoc/>
        protected override void OnRemove(double value)
        {
            if (this.source.Count == 0)
            {
                this.sum = 0;
                this.Value = null;
                return;
            }

            this.sum -= value;
            this.Value = this.sum / this.source.Count;
        }

        /// <inheritdoc/>
        protected override double? GetValueOrDefault(IEnumerable<double> _)
        {
            if (this.source.Count == 0)
            {
                return null;
            }

            this.sum = this.source.Sum();
            return this.sum / this.source.Count;
        }
    }
}