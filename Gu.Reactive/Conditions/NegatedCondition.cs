namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    public class NegatedCondition : ICondition
    {
        private readonly FixedSizedQueue<ConditionHistoryPoint> _history = new FixedSizedQueue<ConditionHistoryPoint>(100);
        private ICondition _condition;
        private IDisposable _subscription;

        public NegatedCondition(ICondition condition)
        {
            this._condition = condition;
            this._subscription = condition.ToObservable(x => x.IsSatisfied)
                                     .Subscribe(
                                         x =>
                                         {
                                             this._history.Enqueue(new ConditionHistoryPoint(DateTime.UtcNow, this.IsSatisfied));
                                             this.OnPropertyChanged(x.EventArgs.PropertyName);
                                         });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool? IsSatisfied
        {
            get
            {
                if (this._condition.IsSatisfied == null)
                {
                    return null;
                }
                return !this._condition.IsSatisfied.Value;
            }
        }

        public string Name
        {
            get
            {
                return string.Format("Not_{0}", this._condition.Name);
            }
        }

        public IEnumerable<ICondition> Prerequisites
        {
            get
            {
                return this._condition.Prerequisites;
            }
        }
        public FixedSizedQueue<ConditionHistoryPoint> History
        {
            get
            {
                return this._history;
            }
        }
        public ICondition Negate()
        {
            return this._condition;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this._condition != null)
                {
                    this._condition.Dispose();
                    this._condition = null;
                }
                if (this._subscription != null)
                {
                    this._subscription.Dispose();
                    this._subscription = null;
                }
            }
        }
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            var handler = this.PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
