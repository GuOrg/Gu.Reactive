#pragma warning disable SA1600 // Elements must be documented, internal
namespace Gu.Reactive
{
    internal interface IUpdater
    {
        object IsUpdatingSourceItem { get; }
    }
}