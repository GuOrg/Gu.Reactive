namespace Gu.Reactive
{
    using System;

    /// <summary>
    /// A typed filtered view
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IReadOnlyFilteredView<T> : IReadOnlyDeferredView<T>
    {
        Func<T, bool> Filter { get; }
    }
}