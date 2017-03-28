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
        /// <param name="prerequisites">The children.</param>
        /// <param name="leaveOpen">True to not dispose <paramref name="prerequisites"/> when this instance is disposed.</param>
        internal AndConditionCollection(IReadOnlyList<ICondition> prerequisites, bool leaveOpen)
            : base(GetIsSatisfied, prerequisites, leaveOpen)
        {
        }

        private static bool? GetIsSatisfied(IReadOnlyList<ICondition> prerequisites)
        {
            if (prerequisites.Count == 0)
            {
                return null;
            }

            if (prerequisites.All(x => x.IsSatisfied == true))
            {
                return true;
            }

            if (prerequisites.Any(x => x.IsSatisfied == false))
            {
                return false;
            }

            return null; // Mix of trues and nulls means not enough info.
        }
    }
}