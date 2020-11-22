namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A view of the changes in an observable collection.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    public interface IChanges<out TValue> : IDisposable
    {
        /// <summary>
        /// When an item is added. On replace add is called before remove.
        /// </summary>
        event Action<TValue> Add;

        /// <summary>
        /// When an item is removed. On replace add is called before remove.
        /// </summary>
        event Action<TValue> Remove;

        /// <summary>
        /// When the collection signals reset.
        /// </summary>
        event Action<IEnumerable<TValue>> Reset;

        /// <summary>
        /// Gets the values of the collection.
        /// </summary>
        IEnumerable<TValue> Values { get; }
    }
}
