namespace Gu.Reactive
{
    using Gu.Reactive.Internals;

    /// <summary>
    /// Creates an AndConditionCollection
    /// </summary>
    public class AndCondition : Condition
    {
        public AndCondition(params ICondition[] prerequisites)
            : base(new AndConditionCollection(prerequisites))
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