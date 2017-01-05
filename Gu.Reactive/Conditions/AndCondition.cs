namespace Gu.Reactive
{
    using Gu.Reactive.Internals;

    /// <summary>
    /// Creates an <see cref="ICondition"/> from a collection of condtions.
    /// It is Satisfied when all prerequisites are staisfied.
    /// If any prerequisite IsSatisfied returns false.
    /// If no prerequisite is IsSatisFied == false and any prerequisite is null the result is null
    /// </summary>
    public class AndCondition : Condition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AndCondition"/> class.
        /// </summary>
        public AndCondition(params ICondition[] prerequisites)
            : base(new AndConditionCollection(prerequisites))
        {
            Ensure.NotNullOrEmpty(prerequisites, nameof(prerequisites));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AndCondition"/> class.
        /// </summary>
        public AndCondition(ICondition condition, ICondition[] prerequisites)
            : this(ConditionCollection.Prepend(condition, prerequisites))
        {
            Ensure.NotNull(condition, nameof(condition));
            Ensure.NotNullOrEmpty(prerequisites, nameof(prerequisites));
        }
    }
}