namespace Gu.Reactive
{
    using System;
    using System.Linq;

    public class AndCondition : Condition
    {
        public AndCondition(params ICondition[] prerequisites)
            : base(new AndConditionCollection(prerequisites))
        {
            if (prerequisites == null || !prerequisites.Any())
                throw new ArgumentException();
        }
    }
}