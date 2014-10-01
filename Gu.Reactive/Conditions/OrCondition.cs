namespace Gu.Reactive
{
    using System;
    using System.Linq;

    public class OrCondition : Condition
    {
        public OrCondition(params ICondition[] prerequisites)
            : base(new OrConditionCollection(prerequisites))
        {
            if (prerequisites == null || !prerequisites.Any())
                throw new ArgumentException();
        }

        public override ICondition Negate()
        {
            return new NegatedCondition(this);
        }
    }
}