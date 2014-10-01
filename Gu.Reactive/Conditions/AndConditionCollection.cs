namespace Gu.Reactive
{
    using System.Linq;
    /// <summary>
    /// Used internally in AndCondition
    /// </summary>
    internal class AndConditionCollection : ConditionCollection
    {
        public AndConditionCollection(params ICondition[] conditions)
            : base(conditions)
        {
        }

        protected override bool? InternalIsSatisfied()
        {
            if (!this.Any()) // Empty collection, throw here instead?
            {
                return null;
            }
            if (this.All(x => x.IsSatisfied == true))
            {
                return true;
            }
            if (this.Any(x => x.IsSatisfied == false))
            {
                return false;
            }
            return null; // Mix of ands and nulls means not enough info.
        }
    }
}