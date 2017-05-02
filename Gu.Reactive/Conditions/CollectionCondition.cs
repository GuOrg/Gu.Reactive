namespace Gu.Reactive
{
    using System.Collections.Generic;
    using System.Collections.Specialized;

    /// <summary>
    /// A base class for conditions that depend on collections of other conditions.
    /// </summary>
    public abstract class CollectionCondition : Condition
    {
        private readonly ConditionCollection prerequisites;
        private IReadOnlyList<ICondition> snapshot;

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionCondition"/> class.
        /// </summary>
        protected CollectionCondition(ConditionCollection prerequisites)
            : base(
                prerequisites.ObserveIsSatisfiedChanged(),
                () => prerequisites.IsSatisfied)
        {
            this.prerequisites = prerequisites;
            this.prerequisites.CollectionChanged += this.OnPreRequisitesChanged;
        }

        /// <inheritdoc/>
        public override IReadOnlyList<ICondition> Prerequisites
        {
            get
            {
                this.ThrowIfDisposed();
                return this.snapshot ?? (this.snapshot = this.prerequisites.Snapshot());
            }
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.prerequisites.CollectionChanged -= this.OnPreRequisitesChanged;
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Called when the condition collection changes.
        /// </summary>
        protected virtual void OnPreRequisitesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.snapshot = null;
            this.OnPropertyChanged(nameof(this.Prerequisites));
        }
    }
}