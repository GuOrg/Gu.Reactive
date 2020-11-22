namespace Gu.Reactive
{
    using System;

    /// <summary>
    /// A readonly view of a collection that can be bound by WPF-controls.
    /// </summary>
    /// <typeparam name="T">The type of the items in the collection.</typeparam>
    public interface IReadOnlyView<out T> : IReadOnlyObservableCollection<T>, IDisposable
    {
    }
}
