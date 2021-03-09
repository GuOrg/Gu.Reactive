namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Extension methods for maybe types.
    /// </summary>
    public static class Maybe
    {
        /// <summary>
        /// Create an instance with a value.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Maybe{T}"/>.</returns>
        public static Maybe<T> Some<T>(T value) => Maybe<T>.Some(value);

        /// <summary>
        /// The default instance when value is missing.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <returns>An empty <see cref="Maybe{T}"/>.</returns>
        public static Maybe<T> None<T>() => Maybe<T>.None;

        /// <summary>
        /// Get the value if HasValue is true and default(T) if not.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="maybe">The <see cref="IMaybe{T}"/>.</param>
        /// <returns>The <see cref="Maybe{T}.Value"/> or <see langword="default"/>.</returns>
        [return: MaybeNull]
        public static T GetValueOrDefault<T>(this IMaybe<T> maybe)
        {
            if (maybe is null)
            {
                throw new ArgumentNullException(nameof(maybe));
            }

            return maybe.HasValue
                ? maybe.Value
                : default;
        }

        /// <summary>
        /// Determines whether two objects of type <see cref="Maybe{T}" /> are equal.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>true if the specified objects are equal; otherwise, false.</returns>
        public static bool Equals<T>(Maybe<T> x, Maybe<T> y)
        {
            if (x.HasValue != y.HasValue)
            {
                return false;
            }

            return EqualityComparer<T?>.Default.Equals(x.GetValueOrDefault(), y.GetValueOrDefault());
        }

        /// <summary>
        /// Determines whether two objects of type <see cref="Maybe{T}" /> are equal.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <param name="compare">How to compare x and y.</param>
        /// <returns>true if the specified objects are equal; otherwise, false.
        /// null if <paramref name="x"/> doe snot have value.</returns>
        public static bool? Equals<T>(Maybe<T> x, T y, Func<T, T, bool> compare)
        {
            if (compare is null)
            {
                throw new ArgumentNullException(nameof(compare));
            }

            return x.HasValue
                ? compare(x.Value, y)
                : (bool?)null;
        }

        /// <summary>
        /// Cast to Maybe{T}.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="maybe">The <see cref="Maybe{Object}"/>.</param>
        /// <returns>A <see cref="Maybe{T}"/>.</returns>
        public static Maybe<T> Cast<T>(this Maybe<object?> maybe)
        {
            return maybe.HasValue
#pragma warning disable CS8600, CS8605, CS8619 // Nullability of reference types in value doesn't match target type.
                       ? Some((T)maybe.Value)
#pragma warning restore CS8600, CS8605, CS8619 // Nullability of reference types in value doesn't match target type.
                       : Maybe<T>.None;
        }

        /// <summary>
        /// Cast to Maybe{T}.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="maybe">The <see cref="IMaybe{Object}"/>.</param>
        /// <returns>A <see cref="Maybe{T}"/>.</returns>
        public static Maybe<T> Cast<T>(this IMaybe<object?> maybe)
        {
            if (maybe is null)
            {
                throw new ArgumentNullException(nameof(maybe));
            }

            return maybe.HasValue
#pragma warning disable CS8600, CS8605, CS8619 // Nullability of reference types in value doesn't match target type.
                       ? Some((T)maybe.Value)
#pragma warning restore CS8600, CS8605, CS8619 // Nullability of reference types in value doesn't match target type.
                       : Maybe<T>.None;
        }
    }
}
