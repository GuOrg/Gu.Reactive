namespace Gu.Reactive.Demo
{
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Runtime.CompilerServices;
    using System.Windows.Input;

    using Gu.Wpf.Reactive;

    public sealed class ReadOnlySerialViewViewModel : INotifyPropertyChanged, IDisposable
    {
        private string items;
        private bool disposed;

        public ReadOnlySerialViewViewModel()
        {
            this.UpdateCommand = new RelayCommand(() => this.View.SetSource(this.items.Split(',').Select(x => new DummyItem(int.Parse(x)))));
            this.ClearSourceCommand = new RelayCommand(() => this.View.ClearSource());
            this.ClearCommand = new RelayCommand(
                () =>
                    {
                        this.View.ClearSource();
                        this.ViewChanges.Clear();
                    });
            this.View
                .ObserveCollectionChangedSlim(false)
                .ObserveOnDispatcher()
                .Subscribe(x => this.ViewChanges.Add(x));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ReadOnlySerialView<DummyItem> View { get; } = new ReadOnlySerialView<DummyItem>();

        public ObservableCollection<NotifyCollectionChangedEventArgs> ViewChanges { get; } = new ObservableCollection<NotifyCollectionChangedEventArgs>();

        public ICommand UpdateCommand { get; }

        public ICommand ClearSourceCommand { get; }

        public ICommand ClearCommand { get; }

        public string Items
        {
            get
            {
                return this.items;
            }

            set
            {
                if (value == this.items)
                {
                    return;
                }

                this.items = value;
                this.OnPropertyChanged();
            }
        }

        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            this.View.Dispose();
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}