// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AndCondition.cs" company="">
//   
// </copyright>
// <summary>
//   Creates an AndConditionCollection
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gu.Reactive
{
    using System;
    using System.Linq;

    /// <summary>
    /// Creates an AndConditionCollection
    /// </summary>
    public class AndCondition : Condition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AndCondition"/> class.
        /// </summary>
        /// <param name="prerequisites">
        /// The prerequisites.
        /// </param>
        /// <exception cref="ArgumentException">
        /// </exception>
        public AndCondition(params ICondition[] prerequisites)
            : base(new AndConditionCollection(prerequisites))
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