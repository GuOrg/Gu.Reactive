namespace Gu.Reactive.Internals
{
    using System.Collections.Generic;

    internal interface IPropertyPath : IReadOnlyList<PathProperty>
    {
        PathProperty Last { get; }
    }
}