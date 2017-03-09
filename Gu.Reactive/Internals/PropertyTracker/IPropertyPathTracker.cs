namespace Gu.Reactive.Internals
{
    using System;

    internal interface IPropertyPathTracker : IDisposable
    {
        IPropertyTracker GetNext(IPropertyTracker propertyTracker);

        IPropertyTracker GetPrevious(IPropertyTracker propertyTracker);
    }
}