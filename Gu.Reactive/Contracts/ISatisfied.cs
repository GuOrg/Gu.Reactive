﻿namespace Gu.Reactive
{
    using System.ComponentModel;

    /// <summary>
    /// A reactive predicate.
    /// </summary>
    public interface ISatisfied : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets get if the criteria is satisfied.
        /// Notifies if value changes.
        /// </summary>
        bool? IsSatisfied { get; }
    }
}
