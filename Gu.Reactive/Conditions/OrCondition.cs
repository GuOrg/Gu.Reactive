// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrCondition.cs" company="">
//   
// </copyright>
// <summary>
//   The or condition.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gu.Reactive
{
    using System;
    /// <summary>
    /// The or condition.
    /// </summary>
    public class OrCondition : Condition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrCondition"/> class.
        /// </summary>
        /// <param name="prerequisites">
        /// The prerequisites.
        /// </param>
        /// <exception cref="ArgumentException">
        /// </exception>
        public OrCondition(params ICondition[] prerequisites)
            : base(new OrConditionCollection(prerequisites))
        {
        }

        /// <summary>
        /// Neagtes the condition, does not mutate.
        /// </summary>
        /// <returns>
        /// The <see cref="ICondition"/>.
        /// </returns>
        public override ICondition Negate()
        {
            return new NegatedCondition(this);
        }
    }
}