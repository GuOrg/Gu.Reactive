namespace Gu.Reactive
{
    using System.Linq;

    public class OrConditionCollection : ConditionCollection
    {
        public OrConditionCollection(params ICondition[] conditions)
            : base(conditions)
        {
        }

        protected override bool? InternalIsSatisfied()
        {
            if (this.Any(x => x.IsSatisfied == true))
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