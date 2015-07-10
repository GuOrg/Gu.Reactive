namespace Gu.Reactive
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Used internally in AndCondition
    /// </summary>
    internal class AndConditionCollection : ConditionCollection
    {
        public AndConditionCollection(params ICondition[] conditions)
            : base(IsSatisfied, conditions)
        {
        }

        private static bool? IsSatisfied(IReadOnlyList<ICondition> conditions)
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