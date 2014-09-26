namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;

    using Gu.Reactive.Annotations;

    public class Condition : ICondition
    {
        private readonly ConditionCollection _prerequisites = new AndConditionCollection(); // Default empty to avoid null checking
        private readonly Func<bool?> _criteria;
        private readonly List<IDisposable> _subscriptions = new List<IDisposable>();
        private readonly FixedSizedQueue<ConditionHistoryPoint> _history = new FixedSizedQueue<ConditionHistoryPoint>(100);
        private bool? _isSatisfied;
        private string _name;

        public Condition(IObservable<object> observable, Func<bool?> criteria)
            : this(observable, criteria, new AndConditionCollection())
        {
            this.Name = this.GetType().Name;
        }
        protected Condition(IObservable<object> observable, Func<bool?> criteria, ConditionCollection prerequisites = null)
            : this(prerequisites)
        {
            this._criteria = criteria;
            this.Name = this.GetType().Name;
            this._subscriptions.Add(observable.Subscribe(
                x =>
                {
                    this.UpdateIsSatisfied();
                }));
            this.UpdateIsSatisfied();
        }
        protected Condition(ConditionCollection prerequisites)
        {
            this.Name = this.GetType().Name;
            if (prerequisites != null && prerequisites.Any())
            {
                this._prerequisites = prerequisites;
                var subscription = this._prerequisites.ToObservable(x => x.IsSatisfied, false)
                                                 .Subscribe(x => this.UpdateIsSatisfied());
                this._subscriptions.Add(subscription);
                this.UpdateIsSatisfied();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public bool? IsSatisfied
        {
            get
            {
                return this.InternalIsSatisfied(); // No caching
            }
            private set // This is only to raise inpc, value is always calculated
            {
                if (this._isSatisfied == value)
                    return;
                this._isSatisfied = value;
                this.History.Enqueue(new ConditionHistoryPoint(DateTime.UtcNow, this._isSatisfied));
                this.OnPropertyChanged();
            }
        }
        public string Name
        {
            get
            {
                return this._name;
            }
            set
            {
                if (value == this._name)
                {
                    return;
                }
                this._name = value;
                this.OnPropertyChanged();
            }
        }
        public FixedSizedQueue<ConditionHistoryPoint> History
        {
            get
            {
                return this._history;
            }
        }
        public IEnumerable<ICondition> Prerequisites
        {
            get
            {
                return this._prerequisites;
            }
        }
        public ICondition Negate()
        {
            return new NegatedCondition(this);
        }
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        public override string ToString()
        {
            return string.Format("Name: {0}, IsSatisfied: {1}", this.GetType().Name, this.IsSatisfied == null ? "null" : this.IsSatisfied.ToString());
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this._subscriptions.Any())
                {
                    foreach (var subscription in this._subscriptions)
                    {
                        subscription.Dispose();
                    }
                    this._subscriptions.Clear();
                }
                if (this._prerequisites.Any())
                {
                    this._prerequisites.Dispose();
                }
            }
        }
        protected void UpdateIsSatisfied()
        {
            this.IsSatisfied = this.InternalIsSatisfied();
        }

        [NotifyPropertyChangedInvocator]
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
      
        private bool? InternalIsSatisfied()
        {
            if (this._criteria != null)
            {
                var criteria = this._criteria();
                var conditions = new[] { criteria, this._prerequisites.IsSatisfied };
                if (conditions.All(x => x == true))
                    return true;
                if (conditions.Any(x => x == false))
                    return false;
                return null;
            }
            return this._prerequisites.IsSatisfied;
        }
    }
}
