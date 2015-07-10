namespace Gu.Wpf.Reactive.Tests.FakesAndHelpers
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using Gu.Wpf.Reactive.Tests.Annotations;

    public class DummyClassWithCollection : INotifyPropertyChanged
    {
        private readonly List<int> _items = new List<int>();

        public event PropertyChangedEventHandler PropertyChanged;

        public List<int> Items
        {
            get
            {
                return _items;
            }
        }

        [NotifyPropertyChangedInvocator]
        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}