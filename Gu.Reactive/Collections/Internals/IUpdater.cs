namespace Gu.Reactive
{
    /// <summary>
    /// Helper interface for updating collections.
    /// </summary>
    internal interface IUpdater
    {
        /// <summary>
        /// The item that is currently being updated.
        /// </summary>
        object CurrentlyUpdatingSourceItem { get; }
    }
}