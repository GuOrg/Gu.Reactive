namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    public sealed class NegatedCondition : ICondition
    {
        private readonly FixedSizedQueue<ConditionHistoryPoint> _history = new FixedSizedQueue<ConditionHistoryPoint>(100);
        private ICondition _condition;

        private ICondition _innerNegated;
        private IDisposable _subscription;

        private string _name;

        private NegatedCondition(ICondition condition, ICondition negated)
        {
            _condition = condition;
            _innerNegated = negated;
            _subscription = condition.ToObservable(x => x.IsSatisfied)
                                          .Subscribe(
                                              x =>
                                                  {
                                                      _history.Enqueue(new ConditionHistoryPoint(DateTime.UtcNow, IsSatisfied));
                                                      OnPropertyChanged(x.EventArgs.PropertyName);
                                                  });
            Name = string.Format("Not_{0}", _condition.Name);
        }

        internal NegatedCondition(Condition condition)
            : this(
            (ICondition)condition,
            new Condition(
                condition.ToObservable(x => x.IsSatisfied),
                () => condition.IsSatisfied == null ? (bool?)null : !condition.IsSatisfied.Value))
        {
        }

        //internal NegatedCondition(OrCondition condition)
        //    : this((ICondition)condition)
        //{
        //    throw new NotImplementedException("Create inner");
        //}

        //internal NegatedCondition(AndCondition condition)
        //    : this((ICondition)condition)
        //{
        //    throw new NotImplementedException("Create inner");
        //}

        public event PropertyChangedEventHandler PropertyChanged;

        public bool? IsSatisfied
        {
            get
            {
                return _innerNegated.IsSatisfied;
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
                this.OnPropertyChanged();
            }
        }

        public IEnumerable<ICondition> Prerequisites
        {
            get
            {
                return _condition.Prerequisites;
            }
        }

        public IEnumerable<ConditionHistoryPoint> History
        {
            get
            {
                return _history;
            }
        }

        public ICondition Negate()
        {
            return _condition;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_condition != null)
                {
                    _condition.Dispose();
                    _condition = null;
                }
                if (_subscription != null)
                {
                    _subscription.Dispose();
                    _subscription = null;
                }
            }
        }

        private void OnPropertyChanged(string propertyName = null)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
