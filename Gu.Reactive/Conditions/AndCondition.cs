namespace Gu.Reactive
{
    using System;
    using System.Linq;

    /// <summary>
    /// Creates an AndConditionCollection
    /// </summary>
    public class AndCondition : Condition
    {
        public AndCondition(params ICondition[] prerequisites)
            : base(new AndConditionCollection(prerequisites))
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