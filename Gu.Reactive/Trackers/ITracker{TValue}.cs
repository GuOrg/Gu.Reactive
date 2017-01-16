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
        /// The Value.
        /// </summary>
        TValue Value { get; }
    }
}