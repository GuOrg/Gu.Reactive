namespace Gu.Reactive
{
    using System;

    /// <summary>
    /// A typed filtered view.
    /// </summary>
    /// <typeparam name="T">The type of the items in the collection.</typeparam>
    [Obsolete("This will be removed in future version. Not keeping anything mutable.")]
    public interface IFilteredView<T> : IThrottledView<T>
    {
        /// <summary>
        /// Gets or sets the predicate used when filtering.
        /// </summary>
        Func<T, bool> Filter { get; set; }
    }
}
