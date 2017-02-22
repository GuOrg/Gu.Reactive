#pragma warning disable SA1600 // Elements must be documented, internal
namespace Gu.Reactive.Internals
{
    using System;
    using System.ComponentModel;

    internal interface IPathPropertyTracker : IDisposable
    {
        event PropertyChangedEventHandler TrackedPropertyChanged;

        object Value();
    }
}