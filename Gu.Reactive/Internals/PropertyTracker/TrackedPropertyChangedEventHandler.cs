namespace Gu.Reactive.Internals
{
    using System.ComponentModel;

    /// <summary>
    /// Raised when a tracked property changes.
    /// </summary>
    /// <param name="tracker">The tracker that notified the event.</param>
    /// <param name="sender">The instance that raised the event.</param>
    /// <param name="e">The property changed event args.</param>
    /// <param name="sourceAndValue">The source and of the value. Can be null.</param>
    internal delegate void TrackedPropertyChangedEventHandler<TValue>(IPathPropertyTracker tracker, object sender, PropertyChangedEventArgs e, SourceAndValue<INotifyPropertyChanged, TValue> sourceAndValue);
}