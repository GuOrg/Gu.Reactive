namespace Gu.Reactive.Internals
{
    /// <summary>
    /// A tracker for nested property changes similar to a WPF binding.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    internal interface IPropertyTracker<TValue> : IPropertyTracker
    {
        /// <summary>
        /// Notifies that the tracked property changed.
        /// </summary>
        new event TrackedPropertyChangedEventHandler<TValue> TrackedPropertyChanged;

        /// <summary>
        /// Get the value if source is not null.
        /// </summary>
        Maybe<TValue> GetMaybe();
    }
}
