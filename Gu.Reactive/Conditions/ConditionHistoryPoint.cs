namespace Gu.Reactive
{
    using System;

    /// <summary>
    /// Conditions keeps a log of the last changed states.
    /// </summary>
    public struct ConditionHistoryPoint : IEquatable<ConditionHistoryPoint>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionHistoryPoint"/> struct.
        /// </summary>
        public ConditionHistoryPoint(bool? state)
            : this(DateTime.UtcNow, state)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionHistoryPoint"/> struct.
        /// </summary>
        public ConditionHistoryPoint(DateTime timeStamp, bool? state)
            : this()
        {
            this.TimeStamp = timeStamp;
            this.State = state;
        }

        /// <summary>
        /// Gets the time when the change occurred.
        /// </summary>
        public DateTime TimeStamp { get; }

        /// <summary>
        /// Gets the state at <see cref="TimeStamp"/>.
        /// </summary>
        public bool? State { get; }

        /// <summary>Returns a value indicating whether two specified instances of <see cref="ConditionHistoryPoint" /> represent the same value.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns><see langword="true" /> if <paramref name="left" /> and <paramref name="right" /> are equal; otherwise, <see langword="false" />.</returns>
        public static bool operator ==(ConditionHistoryPoint left, ConditionHistoryPoint right)
        {
            return left.Equals(right);
        }

        /// <summary>Returns a value that indicates whether two <see cref="ConditionHistoryPoint" /> objects have different values.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns><see langword="true" /> if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, <see langword="false" />.</returns>
        public static bool operator !=(ConditionHistoryPoint left, ConditionHistoryPoint right)
        {
            return !left.Equals(right);
        }

        /// <inheritdoc/>
        public bool Equals(ConditionHistoryPoint other)
        {
            return this.TimeStamp.Equals(other.TimeStamp) && this.State == other.State;
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return obj is ConditionHistoryPoint other &&
                this.Equals(other);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                return (this.TimeStamp.GetHashCode() * 397) ^ this.State.GetHashCode();
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"TimeStamp: {this.TimeStamp}, State: {this.State}";
        }
    }
}
