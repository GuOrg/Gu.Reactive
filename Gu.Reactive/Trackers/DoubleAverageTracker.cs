namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;

    public sealed class DoubleAverageTracker : Tracker<double>
    {
        private double sum;

        public DoubleAverageTracker(
            IReadOnlyList<double> source,
            IObservable<NotifyCollectionChangedEventArgs> onRefresh,
            double whenEmpty)
            : base(source, onRefresh, whenEmpty)
        {
            this.Reset();
        }

        protected override void OnAdd(double value)
        {
            this.sum += value;
            this.Value = this.sum / this.Source.Count;
        }

        protected override void OnRemove(double value)
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

        protected override void OnReplace(double oldValue, double newValue)
        {
            this.sum -= oldValue;
            this.sum += newValue;
            this.Value = this.sum / this.Source.Count;
        }

        protected override double GetValue(IReadOnlyList<double> source)
        {
            this.sum = source.Sum();
            return this.sum / source.Count;
        }
    }
}