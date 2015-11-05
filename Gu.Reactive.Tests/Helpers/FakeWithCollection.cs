namespace Gu.Reactive.Tests.Helpers
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using JetBrains.Annotations;

    public class FakeWithCollection : INotifyPropertyChanged
    {
        private ObservableCollection<Fake> _collection;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Fake> Collection
        {
            get { return _collection; }
            set
            {
                if (Equals(value, _collection))
                {
                    return;
                }
                _collection = value;
                OnPropertyChanged();
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
