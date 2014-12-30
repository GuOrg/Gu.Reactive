// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICondition.cs" company="">
//   
// </copyright>
// <summary>
//   The Condition interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    /// <summary>
    /// The Condition interface.
    /// </summary>
    public interface ICondition : IDisposable, INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the is satisfied.
        /// </summary>
        bool? IsSatisfied { get; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the prerequisites.
        /// </summary>
        IEnumerable<ICondition> Prerequisites { get; }

        /// <summary>
        /// Gets the history.
        /// </summary>
        IEnumerable<ConditionHistoryPoint> History { get; }

        /// <summary>
        /// The negate.
        /// </summary>
        /// <returns>
        /// The <see cref="ICondition"/>.
        /// </returns>
        ICondition Negate();
    }
}