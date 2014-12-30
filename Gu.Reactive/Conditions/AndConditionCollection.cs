// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AndConditionCollection.cs" company="">
//   
// </copyright>
// <summary>
//   Used internally in AndCondition
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gu.Reactive
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Used internally in AndCondition
    /// </summary>
    internal class AndConditionCollection : ConditionCollection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AndConditionCollection"/> class.
        /// </summary>
        /// <param name="conditions">
        /// The conditions.
        /// </param>
        public AndConditionCollection(params ICondition[] conditions)
            : base(IsSatisfied, conditions)
        {
        }

        /// <summary>
        /// The internal is satisfied.
        /// </summary>
        /// <returns>
        /// The <see cref="bool?"/>.
        /// </returns>
        private static bool? IsSatisfied(IEnumerable<ICondition> conditions)
        {
            if (conditions.All(x => x.IsSatisfied == true))
            {
                return true;
            }

            if (conditions.Any(x => x.IsSatisfied == false))
            {
                return false;
            }

            return null; // Mix of trues and nulls means not enough info.
        }
    }
}