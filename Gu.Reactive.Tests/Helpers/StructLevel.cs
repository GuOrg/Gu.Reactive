#pragma warning disable INPC008
namespace Gu.Reactive.Tests.Helpers
{
    using System.ComponentModel;

#pragma warning disable CA1815 // Override equals and operator equals on value types
    public struct StructLevel : INotifyPropertyChanged
#pragma warning restore CA1815 // Override equals and operator equals on value types
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public string Name { get; set; }
    }
}
