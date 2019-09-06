namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// An item with its previous value.
    /// </summary>
    /// <typeparam name="T">The type of the item.</typeparam>
    [System.Diagnostics.DebuggerDisplay("{Current} ({Previous})")]
    public struct WithMaybePrevious<T> : IEquatable<WithMaybePrevious<T>>
    {
#pragma warning disable CA1051 // Do not declare visible instance fields
        /// <summary>
        /// The current value.
        /// </summary>
        public readonly T Current;

        /// <summary>
        /// The previous value.
        /// </summary>
        public readonly Maybe<T> Previous;
#pragma warning restore CA1051 // Do not declare visible instance fields

        /// <summary>
        /// Initializes a new instance of the <see cref="WithMaybePrevious{T}"/> struct.
        /// </summary>
        /// <param name="current">The current value.</param>
        /// <param name="previous">The previous value.</param>
        public WithMaybePrevious(T current, Maybe<T> previous)
        {
            this.Current = current;
            this.Previous = previous;
        }

        /// <summary>Check if <paramref name="left"/> is equal to <paramref name="right"/>.</summary>
        /// <param name="left">The left <see cref="WithPrevious{T}"/>.</param>
        /// <param name="right">The right <see cref="WithPrevious{T}"/>.</param>
        /// <returns>True if <paramref name="left"/> is equal to <paramref name="right"/>.</returns>
        public static bool operator ==(WithMaybePrevious<T> left, WithMaybePrevious<T> right)
        {
            return left.Equals(right);
        }

        /// <summary>Check if <paramref name="left"/> is not equal to <paramref name="right"/>.</summary>
        /// <param name="left">The left <see cref="WithPrevious{T}"/>.</param>
        /// <param name="right">The right <see cref="WithPrevious{T}"/>.</param>
        /// <returns>True if <paramref name="left"/> is not equal to <paramref name="right"/>.</returns>
        public static bool operator !=(WithMaybePrevious<T> left, WithMaybePrevious<T> right)
        {
            return !left.Equals(right);
        }

        /// <inheritdoc/>
        public bool Equals(WithMaybePrevious<T> other) =>
            EqualityComparer<T>.Default.Equals(this.Current, other.Current) &&
            EqualityComparer<Maybe<T>>.Default.Equals(this.Previous, other.Previous);

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is WithMaybePrevious<T> other && this.Equals(other);

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                return (EqualityComparer<T>.Default.GetHashCode(this.Current) * 397) ^ this.Previous.GetHashCode();
            }
        }
    }
}
