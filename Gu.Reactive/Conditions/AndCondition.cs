namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Creates an <see cref="ICondition"/> from a collection of conditions.
    /// It is Satisfied when all prerequisites are satisfied.
    /// If any prerequisite IsSatisfied returns false.
    /// If no prerequisite is IsSatisfied == false and any prerequisite is null the result is null
    /// </summary>
    public class AndCondition : Condition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AndCondition"/> class.
        /// </summary>
        public AndCondition(ICondition prerequisite1, ICondition prerequisite2, params ICondition[] prerequisites)
            : base(new AndConditionCollection(ConditionCollection.Prepend(prerequisite1, prerequisite2, prerequisites), true))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AndCondition"/> class.
        /// </summary>
        /// <param name="prerequisites">The children.</param>
        /// <param name="leaveOpen">True to not dispose <paramref name="prerequisites"/> when this instance is disposed.</param>
        public AndCondition(IReadOnlyList<ICondition> prerequisites, bool leaveOpen)
            : base(new AndConditionCollection(prerequisites, leaveOpen))
        {
        }

        /// <summary>
        /// Disposes of a <see cref="AndCondition"/>.
        /// </summary>
        /// <remarks>
        /// Called from Dispose() with disposing=true.
        /// Guidelines:
        /// 1. We may be called more than once: do nothing after the first call.
        /// 2. Avoid throwing exceptions if disposing is false, i.e. if we're being finalized.
        /// </remarks>
        /// <param name="disposing">True if called from Dispose(), false if called from the finalizer.</param>
        protected override void Dispose(bool disposing)
        {
            if (this.IsDisposed)
            {
                return;
            }

            if (disposing)
            {
                (this.Prerequisites as IDisposable)?.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}