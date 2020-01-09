namespace Gu.Reactive.Internals
{
    using System.ComponentModel;

    /// <summary>
    /// Raised when a tracked property changes.
    /// </summary>
    /// <param name="item">The tracker that notified the event.</param>
    /// <param name="sender">The instance that raised the event, can be the collection.</param>
    /// <param name="e">The property changed event args.</param>
    /// <param name="sourceAndValue">The source and of the value. Can be null.</param>
    internal delegate void TrackedItemPropertyChangedEventHandler<in TItem, TProperty>(TItem item, object sender, PropertyChangedEventArgs e, SourceAndValue<INotifyPropertyChanged?, TProperty> sourceAndValue);
}
