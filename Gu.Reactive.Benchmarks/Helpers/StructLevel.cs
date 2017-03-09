namespace Gu.Reactive.Benchmarks
{
    using System.ComponentModel;

#pragma warning disable WPF1001 // Struct must not implement INotifyPropertyChanged
    public struct StructLevel : INotifyPropertyChanged
#pragma warning restore WPF1001 // Struct must not implement INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged
        {
            add { }
            remove { }
        }

        public string Name { get; set; }
    }
}