namespace Gu.Reactive
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;

    using Gu.Reactive.Annotations;

    /// <summary>
    /// Base class for collections
    /// </summary>
    public abstract class ConditionCollection : IEnumerable<ICondition>, INotifyPropertyChanged, IDisposable
    {
        private readonly List<ICondition> _innerConditions = new List<ICondition>();
        private readonly List<IDisposable> _subscriptions = new List<IDisposable>();
        private bool? _isSatisfied;

        protected ConditionCollection(params ICondition[] conditions)
        {
            if (conditions.Distinct().Count() != conditions.Length)
                throw new ArgumentException("conditions.Distinct().Count() != conditions.Length");
            foreach (var condition in conditions)
            {
                _innerConditions.Add(condition);
                var subscription = condition.ToObservable(x => x.IsSatisfied, false)
                                            .Subscribe(o => this.UpdateIsSatisfied());
                _subscriptions.Add(subscription);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        
        public virtual bool? IsSatisfied
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
                this.OnPropertyChanged();
            }
        }
       
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        public override string ToString()
        {
            return string.Format("IsSatisfied: {0} {{{1}}}", this.IsSatisfied, string.Join(", ", _innerConditions.Select(x => x.Name)));
        }
        
        public IEnumerator<ICondition> GetEnumerator()
        {
            return _innerConditions.GetEnumerator();
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_innerConditions.Any())
                {
                    foreach (var innerCondition in _innerConditions)
                    {
                        innerCondition.Dispose();
                    }
                    _innerConditions.Clear();
                }
                if (_subscriptions.Any())
                {
                    foreach (var subscription in _subscriptions)
                    {
                        subscription.Dispose();
                    }
                    _subscriptions.Clear();
                }
            }
        }
        
        protected void UpdateIsSatisfied()
        {
            this.IsSatisfied = this.InternalIsSatisfied();
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        
        protected abstract bool? InternalIsSatisfied();
    }
}