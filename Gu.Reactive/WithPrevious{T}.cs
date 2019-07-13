namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// An item with its previous value.
    /// </summary>
    /// <typeparam name="T">The type of the item.</typeparam>
    [System.Diagnostics.DebuggerDisplay("{Current} ({Previous})")]
    public struct WithPrevious<T> : IEquatable<WithPrevious<T>>
    {
#pragma warning disable CA1051 // Do not declare visible instance fields
        /// <summary>
        /// The current value.
        /// </summary>
        public readonly T Current;

        /// <summary>
        /// The previous value.
        /// </summary>
        public readonly T Previous;
#pragma warning restore CA1051 // Do not declare visible instance fields

        /// <summary>
        /// Initializes a new instance of the <see cref="WithPrevious{T}"/> struct.
        /// </summary>
        /// <param name="current">The current value.</param>
        /// <param name="previous">The previous value.</param>
        public WithPrevious(T current, T previous)
        {
            this.Current = current;
            this.Previous = previous;
        }

        public static bool operator ==(WithPrevious<T> left, WithPrevious<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(WithPrevious<T> left, WithPrevious<T> right)
        {
            return !left.Equals(right);
        }

        /// <inheritdoc/>
        public bool Equals(WithPrevious<T> other) =>
            EqualityComparer<T>.Default.Equals(this.Current, other.Current) &&
            EqualityComparer<T>.Default.Equals(this.Previous, other.Previous);

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is WithPrevious<T> other && this.Equals(other);

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                return (EqualityComparer<T>.Default.GetHashCode(this.Current) * 397) ^ EqualityComparer<T>.Default.GetHashCode(this.Previous);
            }
        }
    }
}
