namespace Gu.Reactive.Tests.Fakes
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using Gu.Reactive.Tests.Annotations;

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
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
