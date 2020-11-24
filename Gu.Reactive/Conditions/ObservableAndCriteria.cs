namespace Gu.Reactive
{
    using System;

    /// <summary>
    /// An observable and a criteria for creating a <see cref="Condition"/>.
    /// </summary>
    public readonly struct ObservableAndCriteria : IEquatable<ObservableAndCriteria>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableAndCriteria"/> struct.
        /// </summary>
        /// <param name="observable">
        /// The observable that triggers updates of <see cref="ICondition"/>.
        /// </param>
        /// <param name="criteria">
        /// The criteria that is evaluated to give IsSatisfied.
        /// </param>
        public ObservableAndCriteria(IObservable<object> observable, Func<bool?> criteria)
        {
            this.Observable = observable;
            this.Criteria = criteria;
        }

        /// <summary>
        /// Gets the observable that signals when criteria should be evaluated.
        /// </summary>
        public IObservable<object> Observable { get;  }

        /// <summary>
        /// Gets the criteria evaluated by the condition to determine if satisfied.
        /// </summary>
        public Func<bool?> Criteria { get; }

        /// <summary>Returns a value indicating whether two specified instances of <see cref="ObservableAndCriteria" /> represent the same value.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns><see langword="true" /> if <paramref name="left" /> and <paramref name="right" /> are equal; otherwise, <see langword="false" />.</returns>
        public static bool operator ==(ObservableAndCriteria left, ObservableAndCriteria right)
        {
            return left.Equals(right);
        }

        /// <summary>Returns a value that indicates whether two <see cref="ObservableAndCriteria" /> objects have different values.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns><see langword="true" /> if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, <see langword="false" />.</returns>
        public static bool operator !=(ObservableAndCriteria left, ObservableAndCriteria right)
        {
            return !left.Equals(right);
        }

        /// <inheritdoc />
        public bool Equals(ObservableAndCriteria other)
        {
            return Equals(this.Observable, other.Observable) && Equals(this.Criteria, other.Criteria);
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj is ObservableAndCriteria other && this.Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                return ((this.Observable?.GetHashCode() ?? 0) * 397) ^ (this.Criteria?.GetHashCode() ?? 0);
            }
        }
    }
}
