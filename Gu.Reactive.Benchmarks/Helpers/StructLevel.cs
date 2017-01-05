namespace Gu.Reactive.Benchmarks
{
    using System.ComponentModel;

    public struct StructLevel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Name { get; private set; }
    }
}