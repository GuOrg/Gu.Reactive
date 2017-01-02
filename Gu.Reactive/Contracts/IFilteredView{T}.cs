namespace Gu.Reactive
{
    using System;

    /// <summary>
    /// A typed filtered view
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IFilteredView<T> : IThrottledView<T>
    {
        Func<T, bool> Filter { get; set; }
    }
}