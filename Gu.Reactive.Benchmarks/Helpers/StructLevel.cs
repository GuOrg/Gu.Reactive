namespace Gu.Reactive.Benchmarks
{
    using System.ComponentModel;

#pragma warning disable INPC008 // Struct must not implement INotifyPropertyChanged
    public struct StructLevel : INotifyPropertyChanged
#pragma warning restore INPC008 // Struct must not implement INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged
        {
            add { }
            remove { }
        }

        public string Name { get; set; }
    }
}
