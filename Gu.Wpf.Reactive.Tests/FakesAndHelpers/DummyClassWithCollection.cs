namespace Gu.Wpf.Reactive.Tests.FakesAndHelpers
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using JetBrains.Annotations;

    public class DummyClassWithCollection : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public List<int> Items { get; } = new List<int>();

        [NotifyPropertyChangedInvocator]
        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}