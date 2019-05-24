namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// An item with its previous value.
    /// </summary>
    /// <typeparam name="T">The type of the item.</typeparam>
    public struct Paired<T> : IEquatable<Paired<T>>
    {
        /// <summary>
        /// The current value.
        /// </summary>
        public readonly T Current;

        /// <summary>
        /// The previous value.
        /// </summary>
        public readonly T Previous;

        /// <summary>
        /// Initializes a new instance of the <see cref="Paired{T}"/> struct.
        /// </summary>
        /// <param name="current">The current value.</param>
        /// <param name="previous">The previous value.</param>
        public Paired(T current, T previous)
        {
            this.Current = current;
            this.Previous = previous;
        }

        public static bool operator ==(Paired<T> left, Paired<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Paired<T> left, Paired<T> right)
        {
            return !left.Equals(right);
        }

        /// <inheritdoc/>
        public bool Equals(Paired<T> other) =>
            EqualityComparer<T>.Default.Equals(this.Current, other.Current) &&
            EqualityComparer<T>.Default.Equals(this.Previous, other.Previous);

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is Paired<T> other && this.Equals(other);

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
