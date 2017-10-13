namespace Gu.Wpf.Reactive.Tests.FakesAndHelpers
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class DummyClassWithCollection : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public List<int> Items { get; } = new List<int>();

        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}