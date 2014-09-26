namespace Gu.Reactive
{
    using System.Linq;

    public class AndConditionCollection : ConditionCollection
    {
        public AndConditionCollection(params ICondition[] conditions)
            : base(conditions)
        {
        }

        protected override bool? InternalIsSatisfied()
        {
            if (this.All(x => x.IsSatisfied == true))
            {
                return true;
            }
            if (this.Any(x => x.IsSatisfied == false))
            {
                return false;
            }
            return null;
        }
    }
}