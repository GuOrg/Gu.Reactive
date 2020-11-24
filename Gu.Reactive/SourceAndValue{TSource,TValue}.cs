namespace Gu.Reactive
{
    using System;

    /// <summary>
    /// A value and the instance having the value.
    /// </summary>
    /// <typeparam name="TSource">The source type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    public struct SourceAndValue<TSource, TValue> : IEquatable<SourceAndValue<TSource, TValue>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SourceAndValue{TSource,TValue}"/> struct.
        /// </summary>
        /// <param name="source">The source value.</param>
        public SourceAndValue(TSource source, Maybe<TValue> value)
        {
            this.Source = source;
            this.Value = value;
        }

        /// <summary>
        /// Gets the source of the value or the first non-null source in the property path.
        /// </summary>
        public TSource Source { get; }

        /// <summary>
        /// Gets the value. If the property path is not complete HasValue will be false.
        /// </summary>
        public Maybe<TValue> Value { get; }

        /// <summary>Check if <paramref name="left"/> is equal to <paramref name="right"/>.</summary>
        /// <param name="left">The left <see cref="SourceAndValue{TSource, TValue}"/>.</param>
        /// <param name="right">The right <see cref="SourceAndValue{TSource, TValue}"/>.</param>
        /// <returns>True if <paramref name="left"/> is equal to <paramref name="right"/>.</returns>
        public static bool operator ==(SourceAndValue<TSource, TValue> left, SourceAndValue<TSource, TValue> right)
        {
            return left.Equals(right);
        }

        /// <summary>Check if <paramref name="left"/> is not equal to <paramref name="right"/>.</summary>
        /// <param name="left">The left <see cref="SourceAndValue{TSource, TValue}"/>.</param>
        /// <param name="right">The right <see cref="SourceAndValue{TSource, TValue}"/>.</param>
        /// <returns>True if <paramref name="left"/> is not equal to <paramref name="right"/>.</returns>
        public static bool operator !=(SourceAndValue<TSource, TValue> left, SourceAndValue<TSource, TValue> right)
        {
            return !left.Equals(right);
        }

        /// <inheritdoc/>
        public bool Equals(SourceAndValue<TSource, TValue> other)
        {
            return Equals(this.Source, other.Source) && this.Value.Equals(other.Value);
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return obj is SourceAndValue<TSource, TValue> value &&
                   this.Equals(value);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                return ((this.Source?.GetHashCode() ?? 0) * 397) ^ this.Value.GetHashCode();
            }
        }
    }
}
