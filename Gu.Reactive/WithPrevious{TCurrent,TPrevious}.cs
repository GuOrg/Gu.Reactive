namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// An item with its previous value.
    /// </summary>
    /// <typeparam name="TCurrent">The type of the item.</typeparam>
    /// <typeparam name="TPrevious">The type of the previous item.</typeparam>
    [System.Diagnostics.DebuggerDisplay("{Current} ({Previous})")]
    public struct WithPrevious<TCurrent, TPrevious> : IEquatable<WithPrevious<TCurrent, TPrevious>>
    {
#pragma warning disable CA1051 // Do not declare visible instance fields
        /// <summary>
        /// The current value.
        /// </summary>
        public readonly TCurrent Current;

        /// <summary>
        /// The previous value.
        /// </summary>
        public readonly TPrevious Previous;
#pragma warning restore CA1051 // Do not declare visible instance fields

        /// <summary>
        /// Initializes a new instance of the <see cref="WithPrevious{TCurrent, TPrevious}"/> struct.
        /// </summary>
        /// <param name="current">The current value.</param>
        /// <param name="previous">The previous value.</param>
        public WithPrevious(TCurrent current, TPrevious previous)
        {
            this.Current = current;
            this.Previous = previous;
        }

        /// <summary>Check if <paramref name="left"/> is equal to <paramref name="right"/>.</summary>
        /// <param name="left">The left <see cref="WithPrevious{TCurrent, TPrevious}"/>.</param>
        /// <param name="right">The right <see cref="WithPrevious{TCurrent, TPrevious}"/>.</param>
        /// <returns>True if <paramref name="left"/> is equal to <paramref name="right"/>.</returns>
        public static bool operator ==(WithPrevious<TCurrent, TPrevious> left, WithPrevious<TCurrent, TPrevious> right)
        {
            return left.Equals(right);
        }

        /// <summary>Check if <paramref name="left"/> is not equal to <paramref name="right"/>.</summary>
        /// <param name="left">The left <see cref="WithPrevious{TCurrent, TPrevious}"/>.</param>
        /// <param name="right">The right <see cref="WithPrevious{TCurrent, TPrevious}"/>.</param>
        /// <returns>True if <paramref name="left"/> is not equal to <paramref name="right"/>.</returns>
        public static bool operator !=(WithPrevious<TCurrent, TPrevious> left, WithPrevious<TCurrent, TPrevious> right)
        {
            return !left.Equals(right);
        }

        /// <inheritdoc/>
        public bool Equals(WithPrevious<TCurrent, TPrevious> other) =>
            EqualityComparer<TCurrent>.Default.Equals(this.Current, other.Current) &&
            EqualityComparer<TPrevious>.Default.Equals(this.Previous, other.Previous);

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is WithPrevious<TCurrent, TPrevious> other && this.Equals(other);

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                return (EqualityComparer<TCurrent>.Default.GetHashCode(this.Current) * 397) ^ EqualityComparer<TPrevious>.Default.GetHashCode(this.Previous);
            }
        }
    }
}
