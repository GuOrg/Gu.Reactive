namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;

    public sealed class DoubleAverageTracker : Tracker<double>
    {
        private double _sum;
        public DoubleAverageTracker(
            IReadOnlyList<double> source,
            IObservable<NotifyCollectionChangedEventArgs> onRefresh,
            double whenEmpty)
            : base(source, onRefresh, whenEmpty)
        {
            Reset();
        }

        protected override void OnAdd(double value)
        {
            _sum += value;
            Value = _sum / Source.Count;
        }

        protected override void OnRemove(double value)
        {
            if (Source.Count == 0)
            {
                _sum = 0;
                Value = WhenEmpty;
                return;
            }
            _sum -= value;
            Value = _sum / Source.Count;
        }

        protected override void OnReplace(double oldValue, double newValue)
        {
            _sum -= oldValue;
            _sum += newValue;
            Value = _sum / Source.Count;
        }

        protected override double GetValue(IReadOnlyList<double> source)
        {
            _sum = source.Sum();
            return _sum / source.Count;
        }
    }
}