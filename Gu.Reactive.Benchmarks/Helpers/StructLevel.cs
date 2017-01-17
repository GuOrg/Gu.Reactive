namespace Gu.Reactive.Benchmarks
{
    using System.ComponentModel;

    public struct StructLevel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged
        {
            add { }
            remove { }
        }

        public string Name { get; set; }
    }
}