namespace Gu.Reactive.Internals
{
    using System.ComponentModel;

    /// <summary>
    /// Raised when a tracked property changes.
    /// </summary>
    /// <param name="sender">The instance that raised the event.</param>
    /// <param name="e">The property changed event args.</param>
    /// <param name="valueSource">The source of the value. Can be null.</param>
    /// <param name="value">The property value.</param>
    public delegate void TrackedPropertyChangedEventHandler(object sender, PropertyChangedEventArgs e, object valueSource, Maybe<object> value);
}