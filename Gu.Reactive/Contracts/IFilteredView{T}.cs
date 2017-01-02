namespace Gu.Reactive
{
    using System;

    /// <summary>
    /// A typed filtered view
    /// </summary>
    /// <typeparam name="T">The type of the items in the collection.</typeparam>
    public interface IFilteredView<T> : IThrottledView<T>
    {
        /// <summary>
        /// The predicate used when filtering.
        /// </summary>
        Func<T, bool> Filter { get; set; }
    }
}