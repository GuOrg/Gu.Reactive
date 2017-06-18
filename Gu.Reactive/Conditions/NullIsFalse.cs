namespace Gu.Reactive
{
    using System.Reactive.Linq;

    /// <summary>
    /// Wraps a <see cref="ICondition"/> and returns false if the wrapped condition returns null.
    /// </summary>
    public class NullIsFalse<TCondition> : Condition
        where TCondition : class, ICondition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NullIsFalse{TCondition}"/> class.
        /// </summary>
        public NullIsFalse(TCondition condition)
            : base(
                condition.ObserveValue(x => x.IsSatisfied)
                         .Select(x => (object)x.GetValueOrDefault(false))
                         .DistinctUntilChanged(),
                () => condition.IsSatisfied ?? false)
        {
        }
    }
}