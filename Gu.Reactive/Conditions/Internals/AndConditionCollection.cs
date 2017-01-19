namespace Gu.Reactive
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Used internally in AndCondition
    /// </summary>
    internal class AndConditionCollection : ConditionCollection
    {
        internal AndConditionCollection(params ICondition[] prerequisites)
            : base(GetIsSatisfied, prerequisites)
        {
        }

        private static bool? GetIsSatisfied(IReadOnlyList<ICondition> prerequisites)
        {
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