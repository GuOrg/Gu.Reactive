namespace Gu.Reactive.Tests.Helpers
{
    using System.ComponentModel;

    public struct StructLevel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Name { get; private set; }
    }
}