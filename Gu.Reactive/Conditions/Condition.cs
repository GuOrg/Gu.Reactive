namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reactive.Linq;
    using System.Runtime.CompilerServices;
    using Gu.Reactive.Internals;
    using JetBrains.Annotations;

    /// <summary>
    /// To be used standalone or derived from. Conditions really starts to sing when you subclass them and use an IoC container to build trees.
    /// </summary>
    public class Condition : ICondition
    {
        private static readonly IReadOnlyList<ICondition> Empty = new ICondition[0];
        private static readonly PropertyChangedEventArgs IsSatisfiedChangedEventArgs = new PropertyChangedEventArgs(nameof(IsSatisfied));
        private readonly Func<bool?> criteria;
        private readonly IDisposable subscription;
        private readonly IReadOnlyList<ICondition> prerequisites;
        private readonly FixedSizedQueue<ConditionHistoryPoint> history = new FixedSizedQueue<ConditionHistoryPoint>(100);
        private bool? isSatisfied;
        private string name;
        private bool disposed;

        public Condition(Func<bool?> criteria, IObservable<object> observable, params IObservable<object>[] observables)
            : this(Observable.Merge(observables.Concat(new[] { observable })), criteria)
        {
        }

        protected Condition(ConditionCollection conditionCollection)
            : this(conditionCollection.ObserveIsSatisfiedChanged(), () => conditionCollection.IsSatisfied)
        {
            Ensure.NotNullOrEmpty(conditionCollection, nameof(conditionCollection));
            this.prerequisites = conditionCollection;
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
            Ensure.NotNull(observable, nameof(observable));
            Ensure.NotNull(criteria, nameof(criteria));

            this.criteria = criteria;
            this.prerequisites = Empty;
            this.name = this.GetType().PrettyName();
            this.subscription = observable.Subscribe(x => this.UpdateIsSatisfied());
            this.UpdateIsSatisfied();
            this.ObservePropertyChangedSlim(nameof(this.IsSatisfied), true)
                .Subscribe(_ => this.history.Enqueue(new ConditionHistoryPoint(DateTime.UtcNow, this.isSatisfied)));
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
                this.VerifyDisposed();
                return this.criteria(); // No caching
            }

            private set
            {
                // This is only to raise inpc, value is always calculated
                if (this.isSatisfied == value)
                {
                    return;
                }

                this.isSatisfied = value;
                this.OnPropertyChanged(IsSatisfiedChangedEventArgs);
            }
        }

        /// <summary>
        /// Gets or sets the name. The default name is .GetType().PrettyName()
        /// </summary>
        public string Name
        {
            get
            {
                this.VerifyDisposed();
                return this.name;
            }

            set
            {
                this.VerifyDisposed();
                if (value == this.name)
                {
                    return;
                }

                this.name = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// A log of the last 100 times the condition has signaled. Use for debugging.
        /// </summary>
        public IEnumerable<ConditionHistoryPoint> History => this.history;

        /// <summary>
        /// The subconditions for this condition
        /// </summary>
        public IReadOnlyList<ICondition> Prerequisites
        {
            get
            {
                this.VerifyDisposed();
                return this.prerequisites;
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
            this.VerifyDisposed();
            return new NegatedCondition(this);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public override string ToString() => $"Name: {(string.IsNullOrEmpty(this.Name) ? this.GetType() .PrettyName() : this.Name)}, IsSatisfied: {this.IsSatisfied?.ToString() ?? "null"}";

        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            if (disposing)
            {
                this.subscription.Dispose();
            }
        }

        protected void VerifyDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }

        protected void UpdateIsSatisfied()
        {
            this.IsSatisfied = this.criteria();
        }

        [NotifyPropertyChangedInvocator]
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            this.PropertyChanged?.Invoke(this, e);
        }

        /// <summary>
        /// Calls NameOf.Property(propety)
        /// </summary>
        /// <param name="propety"></param>
        [Obsolete("Use nameof")]
        protected void OnPropertyChanged<T>(Expression<Func<T>> propety)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(NameOf.Property(propety)));
        }
    }
}
