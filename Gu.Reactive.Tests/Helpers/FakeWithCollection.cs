namespace Gu.Reactive.Tests.Helpers
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using JetBrains.Annotations;

    public class FakeWithCollection : INotifyPropertyChanged
    {
        private ObservableCollection<Fake> collection;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Fake> Collection
        {
            get
            {
                return this.collection;
            }

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

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
