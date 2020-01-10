namespace Gu.Reactive.Demo
{
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using System.Windows.Threading;
    using Gu.Wpf.Reactive;

    public sealed class ReadOnlySerialViewViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly IDisposable disposable;
        private string items;
        private bool disposed;

        public ReadOnlySerialViewViewModel()
        {
            this.UpdateCommand = new RelayCommand(() => this.View.SetSource(this.items.Split(',').Select(x => new DummyItem(int.Parse(x, CultureInfo.InvariantCulture)))));
            this.ClearSourceCommand = new RelayCommand(() => this.View.ClearSource());
            this.ResetCommand = new AsyncCommand(this.ResetAsync);
            this.disposable = this.View
                .ObserveCollectionChangedSlim(signalInitial: false)
                .ObserveOnDispatcher()
                .Subscribe(x => this.ViewChanges.Add(x));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public ReadOnlySerialView<DummyItem> View { get; } = new ReadOnlySerialView<DummyItem>();

        public ObservableCollection<NotifyCollectionChangedEventArgs> ViewChanges { get; } = new ObservableCollection<NotifyCollectionChangedEventArgs>();

        public ICommand UpdateCommand { get; }

        public ICommand ClearSourceCommand { get; }

        public ICommand ResetCommand { get; }

        public string Items
        {
            get => this.items;

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
            this.disposable?.Dispose();
            (this.ResetCommand as IDisposable)?.Dispose();
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async Task ResetAsync()
        {
            this.View.ClearSource();
            await Dispatcher.Yield(DispatcherPriority.Background);
            this.ViewChanges.Clear();
        }
    }
}
