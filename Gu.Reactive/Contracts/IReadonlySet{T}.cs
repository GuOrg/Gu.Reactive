namespace Gu.Reactive
{
    using System.Collections.Generic;

    /// <summary>
    /// A readonly view of <see cref="ISet{T}"/>
    /// </summary>
    public interface IReadonlySet<T> : IReadOnlyCollection<T>
    {
        /// <summary>
        /// Check if this is a subset of <paramref name="other"/>
        /// </summary>
        bool IsSubsetOf(IEnumerable<T> other);

        /// <summary>
        /// Check if this is a superset of <paramref name="other"/>
        /// </summary>
        bool IsSupersetOf(IEnumerable<T> other);

        /// <summary>
        /// Check if this is a proper subset of <paramref name="other"/> (i.e. strictly contained in)
        /// </summary>
        bool IsProperSupersetOf(IEnumerable<T> other);

        /// <summary>
        /// Check if this is a proper superset of <paramref name="other"/> (i.e. other strictly contained in this)
        /// </summary>
        bool IsProperSubsetOf(IEnumerable<T> other);

        /// <summary>
        /// Checks if this set overlaps other (i.e. they share at least one item)
        /// </summary>
        bool Overlaps(IEnumerable<T> other);

        /// <summary>
        /// Checks if this and other contain the same elements. This is set equality:
        /// duplicates and order are ignored
        /// </summary>
        bool SetEquals(IEnumerable<T> other);
    }
}