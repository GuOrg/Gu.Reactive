namespace Gu.Reactive.Internals
{
    using System;
    using System.ComponentModel;

    internal interface IPropertyTracker : IDisposable
    {
        IGetter Property { get; }

        IPropertyPathTracker PathTracker { get; }

        INotifyPropertyChanged Source { get; set; }

        void OnTrackedPropertyChanged(object sender, INotifyPropertyChanged newSource, PropertyChangedEventArgs e);
    }
}