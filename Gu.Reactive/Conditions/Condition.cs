// ReSharper disable VirtualMemberNeverOverridden.Global
// ReSharper disable MemberCanBePrivate.Global
namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Runtime.CompilerServices;
    using Gu.Reactive.Internals;

    /// <summary>
    /// To be used standalone or derived from. Conditions really starts to sing when you subclass them and use an IoC container to build trees.
    /// </summary>
    public class Condition : ICondition
    {
        private static readonly IReadOnlyList<ICondition> EmptyPrerequisites = new ICondition[0];
        private static readonly PropertyChangedEventArgs IsSatisfiedChangedEventArgs = new PropertyChangedEventArgs(nameof(IsSatisfied));
        private readonly Func<bool?> criteria;
        private readonly IDisposable subscription;
        private readonly IReadOnlyList<ICondition> prerequisites;
        private readonly FixedSizedQueue<ConditionHistoryPoint> history = new FixedSizedQueue<ConditionHistoryPoint>(100);
        private bool? isSatisfied;
        private string name;
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="Condition"/> class.
        /// </summary>
        /// <param name="observable">
        /// The observable that triggers updates of <see cref="IsSatisfied"/>
        /// </param>
        /// <param name="criteria">
        /// The criteria that is evaluated to give IsSatisfied.
        /// </param>
        /// <param name="observables">
        /// More observables that triggers updates of <see cref="IsSatisfied"/>
        /// </param>
        public Condition(Func<bool?> criteria, IObservable<object> observable, params IObservable<object>[] observables)
            : this(Observable.Merge(observables.Concat(new[] { observable })), criteria)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Condition"/> class.
        /// </summary>
        /// <param name="observable">
        /// The observable that triggers updates of <see cref="IsSatisfied"/>
        /// </param>
        /// <param name="criteria">
        /// The criteria that is evaluated to give IsSatisfied.
        /// </param>
        public Condition(IObservable<object> observable, Func<bool?> criteria)
        {
            Ensure.NotNull(observable, nameof(observable));
            Ensure.NotNull(criteria, nameof(criteria));

            this.criteria = criteria;
            this.prerequisites = EmptyPrerequisites;
            this.name = this.GetType().PrettyName();
            this.subscription = observable.StartWith((object)null)
                                          .Subscribe(x => this.UpdateIsSatisfied());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Condition"/> class.
        /// </summary>
        protected Condition(ConditionCollection prerequisites)
            : this(prerequisites.ObserveIsSatisfiedChanged(), () => prerequisites.IsSatisfied)
        {
            Ensure.NotNullOrEmpty(prerequisites, nameof(prerequisites));
            this.prerequisites = prerequisites;
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

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
                this.ThrowIfDisposed();
                return this.prerequisites;
            }
        }

        /// <summary>
        /// Evaluates the criteria and returns if it is satisfied.
        /// Notifies via PropertyChanged when it changes.
        /// </summary>
        public bool? IsSatisfied
        {
            get
            {
                this.ThrowIfDisposed();
                return this.criteria(); // No caching
            }

            private set
            {
                // The cached value is only for not raising PropertyChanged if the value has not changed.
                // The getter calculates the value.
                if (this.isSatisfied == value)
                {
                    if (this.history.Count == 0)
                    {
                        this.history.Enqueue(new ConditionHistoryPoint(DateTime.UtcNow, this.isSatisfied));
                    }

                    return;
                }

                this.isSatisfied = value;
                this.history.Enqueue(new ConditionHistoryPoint(DateTime.UtcNow, this.isSatisfied));
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
                this.ThrowIfDisposed();
                return this.name;
            }

            set
            {
                this.ThrowIfDisposed();
                if (value == this.name)
                {
                    return;
                }

                this.name = value;
                this.OnPropertyChanged();
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
            this.ThrowIfDisposed();
            return new NegatedCondition(this);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        public override string ToString() => $"Name: {(string.IsNullOrEmpty(this.Name) ? this.GetType().PrettyName() : this.Name)}, IsSatisfied: {this.IsSatisfied?.ToString() ?? "null"}";

        /// <summary>
        /// Disposes of a <see cref="Condition"/>.
        /// </summary>
        /// <remarks>
        /// Called from Dispose() with disposing=true.
        /// Guidelines:
        /// 1. We may be called more than once: do nothing after the first call.
        /// 2. Avoid throwing exceptions if disposing is false, i.e. if we're being finalized.
        /// </remarks>
        /// <param name="disposing">True if called from Dispose(), false if called from the finalizer.</param>
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

        /// <summary>
        /// Throws an <see cref="ObjectDisposedException"/> if the instance is disposed.
        /// </summary>
        protected void ThrowIfDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }

        /// <summary>
        /// Update <see cref="IsSatisfied"/> with <see cref="criteria"/>
        /// </summary>
        protected void UpdateIsSatisfied()
        {
            this.IsSatisfied = this.criteria();
        }

        /// <summary>
        /// Raise PropertyChanged event to any listeners.
        /// </summary>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Raise PropertyChanged event to any listeners.
        /// Properties/methods modifying this <see cref="Condition"/> will raise
        /// a property changed event through this virtual method.
        /// </summary>
        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            this.PropertyChanged?.Invoke(this, e);
        }
    }
}
