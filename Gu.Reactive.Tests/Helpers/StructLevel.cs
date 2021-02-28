#pragma warning disable INPC008
namespace Gu.Reactive.Tests.Helpers
{
    using System.ComponentModel;

#pragma warning disable CA1815 // Override equals and operator equals on value types
    public struct StructLevel : INotifyPropertyChanged
#pragma warning restore CA1815 // Override equals and operator equals on value types
    {
#pragma warning disable CS0067
        public event PropertyChangedEventHandler? PropertyChanged;
#pragma warning restore CS0067

        public string Name { get; set; }
    }
}
