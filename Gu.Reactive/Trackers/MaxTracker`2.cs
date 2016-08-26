namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;

    public sealed class MaxTracker<TValue> : Tracker<TValue>
        where TValue : struct, IComparable<TValue>
    {
        public MaxTracker(
            IReadOnlyList<TValue> source,
            IObservable<NotifyCollectionChangedEventArgs> onRefresh,
            TValue? whenEmpty)
            : base(source, onRefresh, whenEmpty)
        {
            this.Reset();
        }

        protected override void OnAdd(TValue value)
        {
            var current = this.Value;
            if (current == null)
            {
                this.Value = value;
                return;
            }

            if (Comparer<TValue>.Default.Compare(value, current.Value) > 0)
            {
                this.Value = value;
            }
        }

        protected override void OnRemove(TValue value)
        {
            var current = this.Value;
            if (current == null)
            {
                throw new InvalidOperationException();
            }

            if (Comparer<TValue>.Default.Compare(value, current.Value) == 0)
            {
                this.Reset();
            }
        }

        protected override void OnReplace(TValue oldValue, TValue newValue)
        {
            var current = this.Value;
            if (current == null)
            {
                throw new InvalidOperationException();
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

        protected override TValue GetValue(IReadOnlyList<TValue> source)
        {
            return source.Max();
        }
    }
}