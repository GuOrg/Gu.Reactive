namespace Gu.Reactive.Internals
{
    internal interface INotifyingGetter : IGetter
    {
        IPropertyTracker CreateTracker(IPropertyPathTracker tracker);
    }
}