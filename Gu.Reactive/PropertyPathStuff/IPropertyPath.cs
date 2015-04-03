namespace Gu.Reactive.PropertyPathStuff
{
    using System.Collections.Generic;

    internal interface IPropertyPath : IReadOnlyList<PathProperty>
    {
        PathProperty Last { get; }
    }
}