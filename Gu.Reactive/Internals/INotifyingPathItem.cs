namespace Gu.Reactive.Internals
{
    using System;
    using System.ComponentModel;

    internal interface INotifyingPathItem : IPathItem, IDisposable, INotifyPropertyChanged
    {
        PropertyChangedEventArgs PropertyChangedEventArgs { get; }
        PathItem PathItem { get; }
        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        INotifyPropertyChanged Source { get; }
    }
}