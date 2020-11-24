namespace Gu.Reactive.Internals
{
    /// <summary>
    /// An <see cref="IGetter"/> for a notifying property.
    /// </summary>
    internal interface INotifyingGetter : IGetter
    {
        /// <summary>
        /// Create tracker for the property.
        /// </summary>
        /// <param name="tracker">The <see cref="IPropertyPathTracker"/>.</param>
        /// <returns>An <see cref="IPropertyTracker"/>.</returns>
        IPropertyTracker CreateTracker(IPropertyPathTracker tracker);
    }
}
