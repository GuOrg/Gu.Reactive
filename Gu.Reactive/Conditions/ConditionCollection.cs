namespace Gu.Reactive
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;

    using Gu.Reactive.Annotations;

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
                this._innerConditions.Add(condition);
                var subscription = condition.ToObservable(x => x.IsSatisfied, false)
                                            .Subscribe(o => this.UpdateIsSatisfied());
                this._subscriptions.Add(subscription);
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
                if (this._isSatisfied == value)
                    return;
                this._isSatisfied = value;
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
            return string.Format("IsSatisfied: {0} {{{1}}}", this.IsSatisfied, string.Join(", ", this._innerConditions.Select(x => x.Name)));
        }
        public IEnumerator<ICondition> GetEnumerator()
        {
            return this._innerConditions.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this._innerConditions.Any())
                {
                    foreach (var innerCondition in this._innerConditions)
                    {
                        innerCondition.Dispose();
                    }
                    this._innerConditions.Clear();
                }
                if (this._subscriptions.Any())
                {
                    foreach (var subscription in this._subscriptions)
                    {
                        subscription.Dispose();
                    }
                    this._subscriptions.Clear();
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