namespace Gu.Reactive.Demo
{
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using System.Windows.Threading;
    using Gu.Wpf.Reactive;

    public sealed class ReadOnlyFilteredViewViewModel : IDisposable, INotifyPropertyChanged
    {
        private readonly Subject<object> trigger = new Subject<object>();
        private readonly System.Reactive.Disposables.CompositeDisposable disposable;
        private bool disposed;

        private int max = 5;

        public ReadOnlyFilteredViewViewModel()
        {
            this.Source = new ObservableCollection<DummyItem>();
            this.Add(3);
            this.View = this.Source.AsReadOnlyFilteredView(this.Filter, TimeSpan.FromMilliseconds(20), Schedulers.DispatcherOrCurrentThread, this.trigger);

            this.AddOneCommand = new RelayCommand(this.AddOne, () => true);
            this.AddTenCommand = new RelayCommand(this.AddTen, () => true);
            this.AddOneOnOtherThreadCommand = new RelayCommand(() => Task.Run(() => this.AddOne()), () => true);
            this.ClearCommand = new RelayCommand(this.Source.Clear, () => true);
            this.ResetCommand = new AsyncCommand(this.ResetAsync);
            this.TriggerCommand = new RelayCommand(() => this.trigger.OnNext(null), () => true);
            this.TriggerOnOtherThreadCommand = new RelayCommand(
                () => Task.Run(() => this.trigger.OnNext(null)),
                () => true);
            this.disposable = new System.Reactive.Disposables.CompositeDisposable
                              {
                                  this.Source
                                      .ObserveCollectionChanged(signalInitial: false)
                                      .ObserveOnDispatcher()
                                      .Subscribe(x => this.SourceChanges.Add(x.EventArgs)),
                                  this.View
                                      .ObserveCollectionChanged(signalInitial: false)
                                      .ObserveOnDispatcher()
                                      .Subscribe(x => this.ViewChanges.Add(x.EventArgs)),
                              };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<DummyItem> Source { get; }

        public IReadOnlyObservableCollection<DummyItem> View { get; }

        public ICommand AddOneCommand { get; }

        public ICommand AddTenCommand { get; }

        public ICommand AddOneOnOtherThreadCommand { get; }

        public ICommand ClearCommand { get; }

        public ICommand ResetCommand { get; }

        public ICommand TriggerCommand { get; }

        public ICommand TriggerOnOtherThreadCommand { get; }

        public ObservableCollection<NotifyCollectionChangedEventArgs> SourceChanges { get; } = new ObservableCollection<NotifyCollectionChangedEventArgs>();

        public ObservableCollection<NotifyCollectionChangedEventArgs> ViewChanges { get; } = new ObservableCollection<NotifyCollectionChangedEventArgs>();

        public int Max
        {
            get => this.max;

            set
            {
                if (value == this.max)
                {
                    return;
                }

                this.max = value;
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
            (this.View as IDisposable)?.Dispose();
            this.trigger.Dispose();
            this.disposable.Dispose();
            (this.ResetCommand as IDisposable)?.Dispose();
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool Filter(DummyItem arg)
        {
            if (arg == null)
            {
                return true;
            }

            return arg.Value < this.max;
        }

        private void AddOne()
        {
            this.Source.Add(new DummyItem(this.Source.Count + 1));
        }

        private void AddTen()
        {
            this.Add(10);
        }

        private void Add(int n)
        {
            for (int i = 0; i < n; i++)
            {
                this.AddOne();
            }
        }

        private async Task ResetAsync()
        {
            this.Source.Clear();
            this.Max = 5;
            ((IRefreshAble)this.View).Refresh();
            await Dispatcher.Yield(DispatcherPriority.Background);
            this.SourceChanges.Clear();
            this.ViewChanges.Clear();
        }
    }
}
