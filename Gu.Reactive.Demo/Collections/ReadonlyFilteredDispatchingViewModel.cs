namespace Gu.Reactive.Demo
{
    using System;
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Gu.Wpf.Reactive;

    public class ReadonlyFilteredDispatchingViewModel : IDisposable, INotifyPropertyChanged
    {
        private readonly ObservableCollection<DummyItem> observableCollection = new ObservableCollection<DummyItem>();

        private readonly ObservableCollection<NotifyCollectionChangedEventArgs> observableCollectionChanges =
            new ObservableCollection<NotifyCollectionChangedEventArgs>();

        private readonly ObservableCollection<NotifyCollectionChangedEventArgs> dispatchingChanges =
            new ObservableCollection<NotifyCollectionChangedEventArgs>();

        private readonly Subject<object> trigger = new Subject<object>();

        private bool disposed;

        private int max = 5;

        public ReadonlyFilteredDispatchingViewModel()
        {
            this.DeferTime = TimeSpan.FromSeconds(0.1);
            this.Add(3);
            this.ReadOnlyObservableCollection = new ReadOnlyObservableCollection<DummyItem>(this.observableCollection);
            this.DispatchingView = this.observableCollection
                                       .AsReadOnlyFilteredView(this.Filter, this.trigger)
                                       .AsDispatchingView();
            this.AddOneCommand = new RelayCommand(this.AddOne, () => true);
            this.AddTenCommand = new RelayCommand(this.AddTen, () => true);
            this.AddOneOnOtherThreadCommand = new RelayCommand(() => Task.Run(() => this.AddOne()), () => true);
            this.ClearCommand = new RelayCommand(this.Clear, () => true);
            this.TriggerCommand = new RelayCommand(() => this.trigger.OnNext(null), () => true);
            this.TriggerOnOtherThreadCommand = new RelayCommand(
                () => Task.Run(() => this.trigger.OnNext(null)),
                () => true);
            this.ObservableCollection
                .ObserveCollectionChanged()
                .ObserveOnDispatcher()
                .Subscribe(x => this.observableCollectionChanges.Add(x.EventArgs));

            this.DispatchingView
                .ObserveCollectionChanged()
                .ObserveOnDispatcher()
                .Subscribe(x => this.dispatchingChanges.Add(x.EventArgs));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ReadOnlyObservableCollection<DummyItem> ReadOnlyObservableCollection { get; }

        public IReadOnlyObservableCollection<DummyItem> DispatchingView { get; }

        public TimeSpan DeferTime { get; }

        public ICommand AddOneCommand { get; }

        public ICommand AddTenCommand { get; }

        public ICommand AddOneOnOtherThreadCommand { get; }

        public RelayCommand ClearCommand { get; }

        public RelayCommand TriggerCommand { get; }

        public RelayCommand TriggerOnOtherThreadCommand { get; }

        public ObservableCollection<NotifyCollectionChangedEventArgs> ObservableCollectionChanges => this.observableCollectionChanges;

        public ObservableCollection<NotifyCollectionChangedEventArgs> DispatchingChanges => this.dispatchingChanges;

        public ObservableCollection<DummyItem> ObservableCollection => this.observableCollection;

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
            (this.DispatchingView as IDisposable)?.Dispose();
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
            lock (((ICollection)this.observableCollection).SyncRoot)
            {
                this.observableCollection.Add(new DummyItem(this.observableCollection.Count + 1));
            }
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
            this.observableCollection.Clear();
            this.observableCollectionChanges.Clear();
            this.dispatchingChanges.Clear();
        }
    }
}
