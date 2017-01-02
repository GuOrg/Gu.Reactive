namespace Gu.Reactive
{
    using System;
    using System.ComponentModel;

    public interface ITracker<out TValue> : IDisposable, INotifyPropertyChanged
    {
        /// <summary> The Value. </summary>
        TValue Value { get; }
    }
}