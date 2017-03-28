namespace Gu.Reactive
{
    using System;

    /// <summary>
    /// A readonly view of a collection that can be bound by WPF-controls.
    /// </summary>
    public interface IReadOnlyView<out T> : IReadOnlyObservableCollection<T>, IDisposable
    {
    }
}