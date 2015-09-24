namespace Gu.Reactive
{
    internal interface IUpdater
    {
        object IsUpdatingSourceItem { get; }

        //void NotifyImmediately(NotifyCollectionChangedEventArgs args);
    }
}