namespace Gu.Reactive.Internals
{
    internal interface IPathProperty<TValue> : IPathProperty
    {
        Maybe<TValue> GetMaybe(object source);
    }
}