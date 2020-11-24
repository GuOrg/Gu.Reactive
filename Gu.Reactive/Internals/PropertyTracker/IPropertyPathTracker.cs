namespace Gu.Reactive.Internals
{
    using System;

    /// <summary>
    /// A tracker for nested property changes similar to a WPF binding.
    /// </summary>
    internal interface IPropertyPathTracker : IDisposable
    {
        /// <summary>
        /// Get the next tracker in the path.
        /// </summary>
        /// <param name="propertyTracker">The <see cref="IPropertyTracker"/>.</param>
        /// <returns>The next tracker in the path.</returns>
        IPropertyTracker? GetNext(IPropertyTracker propertyTracker);
    }
}
