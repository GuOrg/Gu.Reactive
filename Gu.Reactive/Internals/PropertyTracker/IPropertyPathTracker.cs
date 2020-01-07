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
        IPropertyTracker? GetNext(IPropertyTracker propertyTracker);
    }
}
