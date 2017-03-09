namespace Gu.Reactive.Internals
{
    internal interface IPropertyTracker<TValue> : IPropertyTracker
    {
        event TrackedPropertyChangedEventHandler<TValue> TrackedPropertyChanged;

        Maybe<TValue> GetMaybe();
    }
}