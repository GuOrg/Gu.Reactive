namespace Gu.Reactive
{
    using Gu.Reactive.Internals;

    /// <summary>
    /// Creates an <see cref="ICondition"/> from a collection of condtions.
    /// It is Satisfied when any prerequisites is staisfied.
    /// If no prerequisite IsSatisfied IsSatisfied returns false.
    /// If no prerequisite is IsSatisFied == true and any prerequisite is null the result is null
    /// </summary>
    public class OrCondition : Condition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrCondition"/> class.
        /// </summary>
        public OrCondition(params ICondition[] prerequisites)
            : base(new OrConditionCollection(prerequisites))
        {
            Ensure.NotNullOrEmpty(prerequisites, nameof(prerequisites));
        }
    }
}