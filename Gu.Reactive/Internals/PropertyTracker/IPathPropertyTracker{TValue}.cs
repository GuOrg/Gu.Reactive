namespace Gu.Reactive.Internals
{
    internal interface IPathPropertyTracker<TValue> : IPathPropertyTracker
    {
        event TrackedPropertyChangedEventHandler<TValue> TrackedPropertyChanged;

        Maybe<TValue> GetValue();
    }
}