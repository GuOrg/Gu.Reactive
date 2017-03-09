namespace Gu.Reactive.Internals
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// A tracker for a property in a <see cref="IPropertyPathTracker"/>
    /// </summary>
    internal interface IPropertyTracker : IDisposable
    {
        /// <summary>
        /// The <see cref="IGetter"/> for the property.
        /// </summary>
        IGetter Getter { get; }

        /// <summary>
        /// The parent tracker.
        /// </summary>
        IPropertyPathTracker PathTracker { get; }

        /// <summary>
        /// The source that the <see cref="Getter"/> gets value for.
        /// </summary>
        INotifyPropertyChanged Source { get; set; }

        /// <summary>
        /// Previous property in the path notifies via this method.
        /// </summary>
        void OnTrackedPropertyChanged(object sender, INotifyPropertyChanged newSource, PropertyChangedEventArgs e);
    }
}