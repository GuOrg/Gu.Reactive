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
        public static Maybe<T> Some<T>([AllowNull]T value) => Maybe<T>.Some(value);

        /// <summary>
        /// The default instance when value is missing.
        /// </summary>
        public static Maybe<T> None<T>() => Maybe<T>.None;

        /// <summary>
        /// Get the value if HasValue is true and default(T) if not.
        /// </summary>
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
        /// <returns>true if the specified objects are equal; otherwise, false.</returns>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        public static bool Equals<T>(Maybe<T> x, Maybe<T> y)
        {
            if (x.HasValue != y.HasValue)
            {
                return false;
            }

            return EqualityComparer<T>.Default.Equals(x.GetValueOrDefault(), y.GetValueOrDefault());
        }

        /// <summary>
        /// Determines whether two objects of type <see cref="Maybe{T}" /> are equal.
        /// </summary>
        /// <returns>true if the specified objects are equal; otherwise, false.
        /// null if <paramref name="x"/> doe snot have value.</returns>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <param name="compare">How to compare x and y.</param>
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
        public static Maybe<T> Cast<T>(this Maybe<object> maybe)
        {
            return maybe.HasValue
                       ? Some((T)maybe.Value)
                       : Maybe<T>.None;
        }

        /// <summary>
        /// Cast to Maybe{T}.
        /// </summary>
        public static Maybe<T> Cast<T>(this IMaybe<object> maybe)
        {
            if (maybe is null)
            {
                throw new ArgumentNullException(nameof(maybe));
            }

            return maybe.HasValue
                       ? Some((T)maybe.Value)
                       : Maybe<T>.None;
        }
    }
}
