namespace Gu.Reactive
{
    using System;

    /// <summary>
    /// Similar to <see cref="Nullable{T}"/> but T can be a reference type.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    public interface IMaybe<out T>
    {
        /// <summary>
        /// Gets a value indicating whether this instance has a value.
        /// </summary>
        bool HasValue { get; }

        /// <summary>
        /// Gets the value.
        /// Note that null is a valid value for reference types.
        /// </summary>
        /// <remarks>
        /// Check <see cref="HasValue"/> before getting the value.
        /// </remarks>
        T Value { get; }
    }
}
