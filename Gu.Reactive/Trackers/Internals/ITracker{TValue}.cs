namespace Gu.Reactive
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// For tracking aggregates of a collection.
    /// </summary>
    public interface ITracker<out TValue> : IDisposable, INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the value.
        /// </summary>
        TValue Value { get; }
    }
}
