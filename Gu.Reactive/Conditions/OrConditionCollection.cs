namespace Gu.Reactive
{
    using System.Linq;

    internal class OrConditionCollection : ConditionCollection
    {
        public OrConditionCollection(params ICondition[] conditions)
            : base(conditions)
        {
        }

        protected override bool? InternalIsSatisfied()
        {
            if (!this.Any()) // Empty collection, throw here instead?
            {
                return null;
            }
            if (this.Any(x => x.IsSatisfied == true))
            {
                return true;
            }
            if (this.All(x => x.IsSatisfied == false))
            {
                return false;
            }
            return null; // Mix of falses & nulls means not enough info
        }
    }
}