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
        /// Tells you if this instance has a value from source.
        /// Note that null can be the value. If source is null HasValue returns false.
        /// </summary>
        bool HasValue { get; }

        /// <summary>
        /// Check HasValue before getting.
        /// </summary>
        T Value { get; }
    }
}