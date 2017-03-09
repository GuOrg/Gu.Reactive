namespace Gu.Reactive.Internals
{
    using System;

    internal interface IPropertyPathTracker : IDisposable
    {
        IPathPropertyTracker GetNext(IPathPropertyTracker pathPropertyTracker);

        IPathPropertyTracker GetPrevious(IPathPropertyTracker pathPropertyTracker);
    }
}