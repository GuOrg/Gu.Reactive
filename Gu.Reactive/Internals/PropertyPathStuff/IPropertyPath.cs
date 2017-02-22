#pragma warning disable SA1600 // Elements must be documented, internal
namespace Gu.Reactive.Internals
{
    using System.Collections.Generic;

    internal interface IPropertyPath : IReadOnlyList<PathProperty>
    {
        PathProperty Last { get; }
    }
}