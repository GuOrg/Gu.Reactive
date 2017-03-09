namespace Gu.Reactive
{
    using System;

    /// <summary>
    /// A value and the instance having the value.
    /// </summary>
    public struct SourceAndValue<TSource, TValue> : IEquatable<SourceAndValue<TSource, TValue>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SourceAndValue{TSource,TValue}"/> struct.
        /// </summary>
        public SourceAndValue(TSource source, Maybe<TValue> value)
        {
            this.Source = source;
            this.Value = value;
        }

        /// <summary>
        /// The source of the value or the first non-null source in the property path.
        /// </summary>
        public TSource Source { get; }

        /// <summary>
        /// The value. If the property path is not complete HasValue will be false.
        /// </summary>
        public Maybe<TValue> Value { get; }

        /// <summary>
        /// Determines if <paramref name="left"/> is equal to <paramref name="right"/>
        /// </summary>
        public static bool operator ==(SourceAndValue<TSource, TValue> left, SourceAndValue<TSource, TValue> right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Determines if <paramref name="left"/> is not equal to <paramref name="right"/>
        /// </summary>
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
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is SourceAndValue<TSource, TValue> && this.Equals((SourceAndValue<TSource, TValue>)obj);
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