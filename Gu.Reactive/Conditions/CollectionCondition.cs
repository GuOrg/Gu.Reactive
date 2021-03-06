﻿namespace Gu.Reactive
{
    using System.Collections.Generic;
    using System.Collections.Specialized;

    /// <summary>
    /// A base class for conditions that depend on collections of other conditions.
    /// </summary>
    [System.Diagnostics.DebuggerTypeProxy(typeof(CollectionConditionDebugView))]
    public abstract class CollectionCondition : Condition
    {
        private readonly ConditionCollection prerequisites;
        private IReadOnlyList<ICondition>? snapshot;

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionCondition"/> class.
        /// </summary>
        /// <param name="prerequisites">The children.</param>
        protected CollectionCondition(ConditionCollection prerequisites)
#pragma warning disable GUREA02 // Observable and criteria must match.
            : base(
                prerequisites.ObserveIsSatisfiedChanged(),
                () => prerequisites.IsSatisfied)
#pragma warning restore GUREA02 // Observable and criteria must match.
        {
            this.prerequisites = prerequisites ?? throw new System.ArgumentNullException(nameof(prerequisites));
            prerequisites.CollectionChanged += this.OnPreRequisitesChanged;
        }

        /// <inheritdoc/>
        public override IReadOnlyList<ICondition> Prerequisites
        {
            get
            {
                this.ThrowIfDisposed();
                return this.snapshot ??= this.prerequisites.Snapshot();
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
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/>.</param>
#pragma warning disable CA2109 // Review visible event handlers
        protected virtual void OnPreRequisitesChanged(object sender, NotifyCollectionChangedEventArgs e)
#pragma warning restore CA2109 // Review visible event handlers
        {
            this.snapshot = null;
            this.OnPropertyChanged(nameof(this.Prerequisites));
        }
    }
}
