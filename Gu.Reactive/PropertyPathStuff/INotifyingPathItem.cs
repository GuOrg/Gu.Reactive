#pragma warning disable SA1600 // Elements must be documented, internal
namespace Gu.Reactive.PropertyPathStuff
{
    using System;
    using System.ComponentModel;

    internal interface INotifyingPathItem : IDisposable, INotifyPropertyChanged
    {
        PropertyChangedEventArgs PropertyChangedEventArgs { get; }

        PathProperty PathProperty { get; }

        object Value { get; }

        INotifyPropertyChanged Source { get; }
    }
}