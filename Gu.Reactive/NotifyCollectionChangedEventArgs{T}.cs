namespace Gu.Reactive
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;

    /// <summary>
    /// A generic decorator for <see cref="NotifyCollectionChangedEventArgs"/>.
    /// </summary>
    /// <typeparam name="T">The type of the items in the collection.</typeparam>
    public struct NotifyCollectionChangedEventArgs<T> : IEquatable<NotifyCollectionChangedEventArgs<T>>
    {
        private readonly NotifyCollectionChangedEventArgs args;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyCollectionChangedEventArgs{T}"/> struct.
        /// </summary>
        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedEventArgs args)
        {
            this.args = args ?? throw new ArgumentNullException(nameof(args));
            this.NewItems = GetItems(args.NewItems);
            this.OldItems = GetItems(args.OldItems);
        }

        /// <summary>Gets the list of new items involved in the change.</summary>
        public IReadOnlyList<T> NewItems { get; }

        /// <summary>Gets the list of items affected by a <see cref="System.Collections.Specialized.NotifyCollectionChangedAction.Replace" />, Remove, or Move action.</summary>
        public IReadOnlyList<T> OldItems { get; }

        /// <summary>Gets the action that caused the event. </summary>
        public NotifyCollectionChangedAction Action => this.args.Action;

        /// <summary>Gets the index at which the change occurred.</summary>
        public int NewStartingIndex => this.args.NewStartingIndex;

        /// <summary>Gets the index at which a <see cref="System.Collections.Specialized.NotifyCollectionChangedAction.Move" />, Remove, or Replace action occurred.</summary>
        public int OldStartingIndex => this.args.OldStartingIndex;

        /// <summary>Returns a value indicating whether two specified instances of <see cref="NotifyCollectionChangedEventArgs{T}" /> represent the same value.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns><see langword="true" /> if <paramref name="left" /> and <paramref name="right" /> are equal; otherwise, <see langword="false" />.</returns>
        public static bool operator ==(NotifyCollectionChangedEventArgs<T> left, NotifyCollectionChangedEventArgs<T> right)
        {
            return left.Equals(right);
        }

        /// <summary>Returns a value that indicates whether two <see cref="NotifyCollectionChangedEventArgs{T}" /> objects have different values.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns><see langword="true" /> if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, <see langword="false" />.</returns>
        public static bool operator !=(NotifyCollectionChangedEventArgs<T> left, NotifyCollectionChangedEventArgs<T> right)
        {
            return !left.Equals(right);
        }

        /// <inheritdoc />
        public bool Equals(NotifyCollectionChangedEventArgs<T> other)
        {
            return Equals(this.args, other.args);
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj is NotifyCollectionChangedEventArgs<T> other && this.Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return this.args?.GetHashCode() ?? 0;
        }

        private static IReadOnlyList<T> GetItems(IList items)
        {
            if (items is null || items.Count == 0)
            {
                return Array.Empty<T>();
            }

            if (items.Count == 1)
            {
#pragma warning disable CS8601 // Possible null reference assignment.
                return new[] { (T)items[0] };
#pragma warning restore CS8601 // Possible null reference assignment.
            }

            return items.Cast<T>()
                        .ToArray();
        }
    }
}
