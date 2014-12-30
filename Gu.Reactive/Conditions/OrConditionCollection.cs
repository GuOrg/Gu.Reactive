// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrConditionCollection.cs" company="">
//   
// </copyright>
// <summary>
//   The or condition collection.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gu.Reactive
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The or condition collection.
    /// </summary>
    internal class OrConditionCollection : ConditionCollection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrConditionCollection"/> class.
        /// </summary>
        /// <param name="conditions">
        /// The conditions.
        /// </param>
        public OrConditionCollection(params ICondition[] conditions)
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
            if (conditions.Any(x => x.IsSatisfied == true))
            {
                return true;
            }

            if (conditions.All(x => x.IsSatisfied == false))
            {
                return false;
            }

            return null; // Mix of falses & nulls means not enough info
        }
    }
}