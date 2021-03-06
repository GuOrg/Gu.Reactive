﻿namespace Gu.Reactive
{
    using System;

    /// <summary>
    /// A class that can be use to wrap an IoC container.
    /// </summary>
    [Obsolete("This will be removed in future version. Not keeping anything mutable.")]
    public interface IConditionsService
    {
#pragma warning disable CA1716 // Identifiers should not match keywords
        /// <summary>
        /// Useful for returning mocks.
        /// </summary>
        /// <typeparam name="T">The type of condition to get.</typeparam>
        /// <returns>A condition of type <typeparamref name="T"/>.</returns>
        ICondition Get<T>()
#pragma warning restore CA1716 // Identifiers should not match keywords
            where T : ICondition;
    }
}
