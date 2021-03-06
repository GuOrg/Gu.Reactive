﻿namespace Gu.Reactive
{
    using System.Reflection;

    /// <summary>
    /// A wrapper around a delegate created from a <see cref="System.Reflection.PropertyInfo.GetMethod"/>.
    /// </summary>
    public interface IGetter
    {
#pragma warning disable CA1716 // Identifiers should not match keywords
        /// <summary>
        /// Gets the <see cref="PropertyInfo"/> that this instance is a getter for.
        /// </summary>
        PropertyInfo Property { get; }
#pragma warning restore CA1716 // Identifiers should not match keywords

        /// <summary>
        /// Get the value of the property for <paramref name="source"/>.
        /// </summary>
        /// <param name="source">The source value.</param>
        /// <returns>The property value.</returns>
        object? GetValue(object source);

        /// <summary>
        /// Get the value of the property for <paramref name="source"/>.
        /// </summary>
        /// <param name="source">The source value.</param>
        /// <returns>A <see cref="Maybe{Object}"/>.</returns>
        Maybe<object?> GetMaybe(object? source);
    }
}
