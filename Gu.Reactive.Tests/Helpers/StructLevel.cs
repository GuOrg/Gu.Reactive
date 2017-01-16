#pragma warning disable WPF1001
namespace Gu.Reactive.Tests.Helpers
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public struct StructLevel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Name { get; set; }

        // ReSharper disable once UnusedMember.Local
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}