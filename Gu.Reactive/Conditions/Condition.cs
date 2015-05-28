namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reactive.Linq;
    using System.Runtime.CompilerServices;

    using Gu.Reactive.Annotations;

    /// <summary>
    /// To be used standalone or derived from. Conditions really starts to sing when you subclass them and use an IoC container to build trees.
    /// </summary>
    public class Condition : ICondition
    {
        private readonly Func<bool?> _criteria;
        private readonly IDisposable _subscription;
        private readonly IEnumerable<ICondition> _prerequisites;
        private readonly FixedSizedQueue<ConditionHistoryPoint> _history = new FixedSizedQueue<ConditionHistoryPoint>(100);
        private bool? _isSatisfied;
        private string _name;
        private bool _disposed;

        public Condition(Func<bool?> criteria, IObservable<object> observable, params IObservable<object>[] observables)
            : this(Observable.Merge(observables.Concat(new[] { observable })), criteria)
        {
        }

        protected Condition(ConditionCollection conditionCollection)
            : this(conditionCollection.ObservePropertyChanged(x => x.IsSatisfied, false), () => conditionCollection.IsSatisfied)
        {
            _prerequisites = conditionCollection;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Condition"/> class.
        /// </summary>
        /// <param name="observable">
        /// The observable that triggers notifications
        /// </param>
        /// <param name="criteria">
        /// The criteria that is evaluated to give IsSatisfied.
        /// </param>
        public Condition(IObservable<object> observable, Func<bool?> criteria)
        {
            if (observable == null)
            {
                throw new ArgumentNullException("observable");
            }
            if (criteria == null)
            {
                throw new ArgumentNullException("criteria");
            }
            _criteria = criteria;
            _prerequisites = Enumerable.Empty<ICondition>();
            _name = GetType().PrettyName();
            _subscription = observable.Subscribe(x => UpdateIsSatisfied());
            UpdateIsSatisfied(); 
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
                VerifyDisposed();
                return _criteria(); // No caching
            }

            private set
            {
                // This is only to raise inpc, value is always calculated
                if (_isSatisfied == value)
                {
                    return;
                }

                _isSatisfied = value;
                _history.Enqueue(new ConditionHistoryPoint(DateTime.UtcNow, _isSatisfied));
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the name. The default name is .GetType().PrettyName()
        /// </summary>
        public string Name
        {
            get
            {
                VerifyDisposed();
                return _name;
            }

            set
            {
                VerifyDisposed();
                if (value == _name)
                {
                    return;
                }

                _name = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// A log of the last 100 times the condition has signaled. Use for debugging.
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
                VerifyDisposed();
                return _prerequisites;
            }
        }

        /// <summary>
        /// Negates the condition. Calling Negate does not mutate the condition it is called on.
        /// Calling Negate on a negated condition returns the original condition.
        /// </summary>
        /// <returns>
        /// A new condition.
        /// </returns>
        public virtual ICondition Negate()
        {
            VerifyDisposed();
            return new NegatedCondition(this);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public override string ToString()
        {
            return string.Format("Name: {0}, IsSatisfied: {1}",
                string.IsNullOrEmpty(Name) ? GetType().PrettyName() : Name,
                IsSatisfied == null ? "null" : IsSatisfied.ToString());
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;
            if (disposing)
            {
                _subscription.Dispose();
            }
        }

        protected void VerifyDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }

        protected void UpdateIsSatisfied()
        {
            IsSatisfied = _criteria();
        }

        [NotifyPropertyChangedInvocator]
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Calls NameOf.Property(propety)
        /// </summary>
        /// <param name="propety"></param>
        protected void OnPropertyChanged<T>(Expression<Func<T>> propety)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(NameOf.Property(propety)));
            }
        }
    }
}
