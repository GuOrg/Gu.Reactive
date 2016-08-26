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
        private readonly IDisposable _subscription;
        private string _name;

        private bool _disposed;

        public NegatedCondition(Condition condition)
        {
            _condition = condition;
            Name = $"Not_{_condition.Name}";

            _subscription = condition.ObserveIsSatisfiedChanged()
                                     .Subscribe(_ => OnPropertyChanged(nameof(IsSatisfied)));

            this.ObservePropertyChangedSlim(nameof(IsSatisfied), true)
                .Subscribe(_ => _history.Enqueue(new ConditionHistoryPoint(DateTime.UtcNow, IsSatisfied)));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool? IsSatisfied
        {
            get
            {
                var isSatisfied = _condition.IsSatisfied;
                if (isSatisfied == null)
                {
                    return null;
                }

                return isSatisfied != true;
            }
        }

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

        public ICondition Negate() => _condition;

        /// <summary>
        /// Dispose(true); //I am calling you from Dispose, it's safe
        /// GC.SuppressFinalize(this); //Hey, GC: don't bother calling finalize later
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            _subscription.Dispose();
            // GC.SuppressFinalize(this);           
        }

        private void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
