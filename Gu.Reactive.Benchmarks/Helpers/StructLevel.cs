namespace Gu.Reactive.Benchmarks
{
    using System.ComponentModel;

#pragma warning disable INPC008 // Struct must not implement INotifyPropertyChanged
#pragma warning disable CA1815 // Override equals and operator equals on value types
    public struct StructLevel : INotifyPropertyChanged
#pragma warning restore CA1815 // Override equals and operator equals on value types
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
