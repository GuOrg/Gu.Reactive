#pragma warning disable SA1600 // Elements must be documented, internal
namespace Gu.Reactive.Internals
{
    using System;
    using System.Collections.Generic;

    internal interface IPropertyPath : IReadOnlyList<IPathProperty>
    {
        [Obsolete("Remove")]
        IPathProperty Last { get; }
    }
}