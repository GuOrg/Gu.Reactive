namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    public interface ICondition : IDisposable, INotifyPropertyChanged
    {
        bool? IsSatisfied { get; }

        /// <summary>
        /// Gets the name. The default name is .GetType().PrettyName()
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the conditions that are prerequisites to this condition.
        /// </summary>
        IEnumerable<ICondition> Prerequisites { get; }

        /// <summary>
        /// Gets a log of the last 100 times the condition has signaled changes. Use for debugging.
        /// </summary>
        IEnumerable<ConditionHistoryPoint> History { get; }

        /// <summary>
        /// Negates the condition. Calling Negate does not mutate the condition it is called on.
        /// Calling Negate on a negated condition returns the original condition.
        /// </summary>
        /// <returns>
        /// A new condition.
        /// </returns>
        ICondition Negate();
    }
}