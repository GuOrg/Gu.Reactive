namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;

    public interface IChanges<TValue> : IDisposable
    {
        event Action<TValue> Add;

        event Action<TValue> Remove;

        event Action<IEnumerable<TValue>> Reset;

        IEnumerable<TValue> Values { get; }
    }
}