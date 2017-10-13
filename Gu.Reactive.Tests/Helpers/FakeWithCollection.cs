namespace Gu.Reactive.Tests.Helpers
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class FakeWithCollection : INotifyPropertyChanged
    {
        private ObservableCollection<Fake> collection;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Fake> Collection
        {
            get => this.collection;

            set
            {
                if (Equals(value, this.collection))
                {
                    return;
                }

                this.collection = value;
                this.OnPropertyChanged();
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
