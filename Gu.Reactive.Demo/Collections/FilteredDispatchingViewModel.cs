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

    using Gu.Wpf.Reactive;

    public class FilteredDispatchingViewModel : IDisposable, INotifyPropertyChanged
    {
        private readonly Subject<object> trigger = new Subject<object>();

        private bool disposed;

        private int max = 5;

        public FilteredDispatchingViewModel()
        {
            this.Source = new ObservableCollection<DummyItem>();
            this.Add(3);
            this.View = this.Source
                            .AsFilteredView(this.Filter, this.trigger)
                            .AsDispatchingView();

            this.AddOneCommand = new RelayCommand(this.AddOne, () => true);
            this.AddTenCommand = new RelayCommand(this.AddTen, () => true);
            this.ClearCommand = new RelayCommand(this.Clear, () => true);
            this.TriggerCommand = new RelayCommand(() => this.trigger.OnNext(null), () => true);
            this.TriggerOnOtherThreadCommand = new RelayCommand(
                () => Task.Run(() => this.trigger.OnNext(null)),
                () => true);
            this.Source
                .ObserveCollectionChanged()
                .ObserveOnDispatcher()
                .Subscribe(x => this.SourceChanges.Add(x.EventArgs));

            this.View
                .ObserveCollectionChanged()
                .ObserveOnDispatcher()
                .Subscribe(x => this.ViewChanges.Add(x.EventArgs));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<DummyItem> Source { get; }

        public IReadOnlyObservableCollection<DummyItem> View { get; }

        public ICommand AddOneCommand { get; }

        public ICommand AddTenCommand { get; }

        public RelayCommand ClearCommand { get; }

        public RelayCommand TriggerCommand { get; }

        public RelayCommand TriggerOnOtherThreadCommand { get; }

        public ObservableCollection<NotifyCollectionChangedEventArgs> SourceChanges { get; } = new ObservableCollection<NotifyCollectionChangedEventArgs>();

        public ObservableCollection<NotifyCollectionChangedEventArgs> ViewChanges { get; } = new ObservableCollection<NotifyCollectionChangedEventArgs>();

        public int Max
        {
            get
            {
                return this.max;
            }

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
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
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

        private void Clear()
        {
            this.Source.Clear();
            ((IRefreshAble)this.View).Refresh();
            this.SourceChanges.Clear();
            this.ViewChanges.Clear();
        }
    }
}