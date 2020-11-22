namespace Gu.Reactive
{
    /// <summary>
    /// A negated condition wraps a <see cref="ICondition"/> and negates the IsSatisfied value.
    /// Calling Negate on it returns the original condition.
    /// </summary>
    public class Negated<TCondition> : NegatedCondition
        where TCondition : ICondition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Negated{TCondition}"/> class.
        /// </summary>
        /// <param name="condition">The condition.</param>
        public Negated(TCondition condition)
            : base(condition)
        {
        }
    }
}
