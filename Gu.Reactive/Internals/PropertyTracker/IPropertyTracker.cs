namespace Gu.Reactive.Internals
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// A tracker for a property in a <see cref="IPropertyPathTracker"/>.
    /// </summary>
    internal interface IPropertyTracker : IDisposable
    {
        /// <summary>
        /// Notifies that the tracked property changed.
        /// </summary>
        event PropertyChangedEventHandler TrackedPropertyChanged;

        /// <summary>
        /// Gets the <see cref="IGetter"/> for the property.
        /// </summary>
        IGetter Getter { get; }

        /// <summary>
        /// Gets the parent tracker.
        /// </summary>
        IPropertyPathTracker PathTracker { get; }

        /// <summary>
        /// Gets or sets the source that the <see cref="Getter"/> gets value for.
        /// </summary>
        INotifyPropertyChanged? Source { get; set; }

        /// <summary>
        /// Previous property in the path notifies via this method.
        /// </summary>
        /// <param name="sender">The instance that notified.</param>
        /// <param name="newSource">The new source instance.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/>.</param>
        void OnTrackedPropertyChanged(object sender, INotifyPropertyChanged? newSource, PropertyChangedEventArgs e);
    }
}
