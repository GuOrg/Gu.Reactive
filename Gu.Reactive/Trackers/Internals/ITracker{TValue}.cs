namespace Gu.Reactive
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// For tracking aggregates of a collection.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    public interface ITracker<out TValue> : IDisposable, INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the value.
        /// </summary>
        TValue Value { get; }
    }
}
