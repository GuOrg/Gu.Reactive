namespace Gu.Reactive
{
    using System;

    /// <summary>
    /// Creates an <see cref="ICondition"/> from a collection of condtions.
    /// It is Satisfied when any prerequisites is staisfied.
    /// If no prerequisite IsSatisfied IsSatisfied returns false.
    /// If no prerequisite is IsSatisFied == true and any prerequisite is null the result is null
    /// </summary>
    public class OrCondition : Condition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrCondition"/> class.
        /// </summary>
        public OrCondition(params ICondition[] prerequisites)
            : base(new OrConditionCollection(prerequisites))
        {
        }

        /// <summary>
        /// Disposes of a <see cref="OrCondition"/>.
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
            if (disposing)
            {
                (this.Prerequisites as IDisposable)?.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}