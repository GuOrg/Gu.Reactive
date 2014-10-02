namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;

    using Gu.Reactive.Annotations;
    /// <summary>
    /// To be used standalone or derived from. Conditions really starts to sing when you subclass them and use an IoC container to build trees.
    /// </summary>
    public class Condition : ICondition
    {
        private readonly ConditionCollection _prerequisites = new AndConditionCollection(); // Default empty to avoid null checking
        private readonly Func<bool?> _criteria;
        private readonly List<IDisposable> _subscriptions = new List<IDisposable>();
        private readonly FixedSizedQueue<ConditionHistoryPoint> _history = new FixedSizedQueue<ConditionHistoryPoint>(100);
        private bool? _isSatisfied;
        private string _name;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="observable">Can be an event or async, anything that signals. 
        /// This is used to trigger evaluation of IsSatisfied & Notification</param>
        /// <param name="criteria">The criteria to be evaluated</param>
        public Condition(IObservable<object> observable, Func<bool?> criteria)
            : this(observable, criteria, new AndConditionCollection())
        {
            this.Name = this.GetType().Name;
        }
        
        [Obsolete("Not sure this needs to remain")]
        protected Condition(IObservable<object> observable, Func<bool?> criteria, ConditionCollection prerequisites = null)
            : this(prerequisites)
        {
            _criteria = criteria;
            this.Name = this.GetType().Name;
            _subscriptions.Add(observable.Subscribe(
                x =>
                {
                    this.UpdateIsSatisfied();
                }));
            this.UpdateIsSatisfied();
        }

        /// <summary>
        /// When composing conditions this takes a collection of subconditions.
        /// </summary>
        /// <param name="prerequisites"></param>
        protected Condition(ConditionCollection prerequisites)
        {
            this.Name = this.GetType().Name;
            if (prerequisites != null && prerequisites.Any())
            {
                _prerequisites = prerequisites;
                var subscription = _prerequisites.ToObservable(x => x.IsSatisfied, false)
                                                 .Subscribe(x => this.UpdateIsSatisfied());
                _subscriptions.Add(subscription);
                this.UpdateIsSatisfied();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
       
        /// <summary>
        /// Evaluates the criteria and returns if it is satisfied. 
        /// Notifies via PropertyChanged when it changes.
        /// </summary>
        public bool? IsSatisfied
        {
            get
            {
                return this.InternalIsSatisfied(); // No caching
            }
            private set // This is only to raise inpc, value is always calculated
            {
                if (_isSatisfied == value)
                {
                    return;
                }
                _isSatisfied = value;
                _history.Enqueue(new ConditionHistoryPoint(DateTime.UtcNow, _isSatisfied));
                this.OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// The name of the condition, mainly used for tostring & in UI
        /// </summary>
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
       
        /// <summary>
        /// A log of the last n states and times
        /// </summary>
        public IEnumerable<ConditionHistoryPoint> History
        {
            get
            {
                return _history;
            }
        }
        
        /// <summary>
        /// The subconditions for this condition
        /// </summary>
        public IEnumerable<ICondition> Prerequisites
        {
            get
            {
                return _prerequisites;
            }
        }
        
        /// <summary>
        /// Returns this condition negated, negating again returns the original
        /// </summary>
        /// <returns></returns>
        public virtual ICondition Negate()
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
            return string.Format("Name: {0}, IsSatisfied: {1}", 
                string.IsNullOrEmpty(Name) ? this.GetType().Name : Name, 
                this.IsSatisfied == null ? "null" : this.IsSatisfied.ToString());
        }
        
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_subscriptions.Any())
                {
                    foreach (var subscription in _subscriptions)
                    {
                        subscription.Dispose();
                    }
                    _subscriptions.Clear();
                }
                if (_prerequisites.Any())
                {
                    _prerequisites.Dispose();
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
            if (_criteria != null)
            {
                var isSatisfied = _criteria();
                if (!_prerequisites.Any())
                {
                    return isSatisfied;
                }
                var satisfactions = new[] { isSatisfied, _prerequisites.IsSatisfied };
                if (satisfactions.All(x => x == true))
                    return true;
                if (satisfactions.Any(x => x == false))
                    return false;
                return null;
            }
            return _prerequisites.IsSatisfied;
        }
    }
}
