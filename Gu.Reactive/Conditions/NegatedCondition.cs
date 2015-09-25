namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    /// <summary>
    /// The negated condition. Calling Negate on it returns the original condition.
    /// </summary>
    public sealed class NegatedCondition : ICondition
    {
        private readonly FixedSizedQueue<ConditionHistoryPoint> _history = new FixedSizedQueue<ConditionHistoryPoint>(100);
        private readonly ICondition _condition;
        private readonly ICondition _innerNegated;
        private readonly IDisposable _subscription;
        private string _name;

        private NegatedCondition(ICondition condition, ICondition negated)
        {
            _condition = condition;
            _innerNegated = negated;
            _subscription = condition.ObservePropertyChanged(x => x.IsSatisfied, false)
                                          .Subscribe(
                                              x =>
                                                  {
                                                      _history.Enqueue(new ConditionHistoryPoint(DateTime.UtcNow, IsSatisfied));
                                                      OnPropertyChanged(x.EventArgs.PropertyName);
                                                  });
            Name = $"Not_{_condition.Name}";
        }

        public NegatedCondition(Condition condition)
            : this(
                (ICondition)condition,
                new Condition(
                    condition.ObservePropertyChanged(x => x.IsSatisfied),
                    () => condition.IsSatisfied == null
                              ? (bool?)null
                              : !condition.IsSatisfied.Value))
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool? IsSatisfied => _innerNegated.IsSatisfied;

        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                if (value == _name)
                {
                    return;
                }

                _name = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the prerequisites.
        /// </summary>
        public IReadOnlyList<ICondition> Prerequisites => _condition.Prerequisites;

        /// <summary>
        /// Gets the history.
        /// </summary>
        public IEnumerable<ConditionHistoryPoint> History => _history;

        public ICondition Negate()
        {
            return _condition;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _innerNegated.Dispose();
                _subscription.Dispose();
            }
        }

        private void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
