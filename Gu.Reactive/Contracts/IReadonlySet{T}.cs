namespace Gu.Reactive
{
    using System.Collections.Generic;

    /// <summary>
    /// A readonly view of <see cref="ISet{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the items in the collection.</typeparam>
    public interface IReadonlySet<T> : IReadOnlyCollection<T>
    {
        /// <summary>
        /// Check if this is a subset of <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The <see cref="IEnumerable{T}"/>.</param>
        /// <returns>True if this is a subset of <paramref name="other"/>.</returns>
        bool IsSubsetOf(IEnumerable<T> other);

        /// <summary>
        /// Check if this is a superset of <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The <see cref="IEnumerable{T}"/>.</param>
        /// <returns>True if this is a superset of <paramref name="other"/>.</returns>
        bool IsSupersetOf(IEnumerable<T> other);

        /// <summary>
        /// Check if this is a proper subset of <paramref name="other"/> (i.e. strictly contained in).
        /// </summary>
        /// <param name="other">The <see cref="IEnumerable{T}"/>.</param>
        /// <returns>True if this is a proper subset of <paramref name="other"/> (i.e. strictly contained in).</returns>
        bool IsProperSupersetOf(IEnumerable<T> other);

        /// <summary>
        /// Check if this is a proper superset of <paramref name="other"/> (i.e. other strictly contained in this).
        /// </summary>
        /// <param name="other">The <see cref="IEnumerable{T}"/>.</param>
        /// <returns>True if this is a proper superset of <paramref name="other"/> (i.e. other strictly contained in this).</returns>
        bool IsProperSubsetOf(IEnumerable<T> other);

        /// <summary>
        /// Checks if this set overlaps other (i.e. they share at least one item).
        /// </summary>
        /// <param name="other">The <see cref="IEnumerable{T}"/>.</param>
        /// <returns>True if this set overlaps other (i.e. they share at least one item).</returns>
        bool Overlaps(IEnumerable<T> other);

        /// <summary>
        /// Checks if this and other contain the same elements. This is set equality:
        /// duplicates and order are ignored.
        /// </summary>
        /// <param name="other">The <see cref="IEnumerable{T}"/>.</param>
        /// <returns>True if this and other contain the same elements.</returns>
        bool SetEquals(IEnumerable<T> other);
    }
}
