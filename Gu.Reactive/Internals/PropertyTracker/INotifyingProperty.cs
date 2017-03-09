namespace Gu.Reactive.Internals
{
    /// <summary>
    /// A <see cref="IPathProperty"/> for a notifying property.
    /// </summary>
    interface INotifyingProperty : IPathProperty
    {
        /// <summary>
        /// Create a tracker for this item.
        /// </summary>
        IPathPropertyTracker CreateTracker(IPropertyPathTracker tracker);
    }
}