namespace Gu.Reactive.Internals
{
    using System;
    using System.ComponentModel;

    internal interface INotifyingPathItem : IDisposable, INotifyPropertyChanged
    {
        PropertyChangedEventArgs PropertyChangedEventArgs { get; }
        
        PathProperty PathProperty { get; }

        object Value { get; }

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        INotifyPropertyChanged Source { get; }
    }
}