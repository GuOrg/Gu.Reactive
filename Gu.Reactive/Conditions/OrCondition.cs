namespace Gu.Reactive
{
    using Gu.Reactive.Internals;

    public class OrCondition : Condition
    {
        public OrCondition(params ICondition[] prerequisites)
            : base(new OrConditionCollection(prerequisites))
        {
            Ensure.NotNullOrEmpty(prerequisites, nameof(prerequisites));
        }

        /// <summary>
        /// Negates the condition. Calling Negate does not mutate the condition it is called on.
        /// Calling Negate on a negated condition returns the original condition.
        /// </summary>
        /// <returns>
        /// A new condition.
        /// </returns>
        public override ICondition Negate()
        {
            return new NegatedCondition(this);
        }
    }
}