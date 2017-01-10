namespace Gu.Reactive.Demo
{
    using System;
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Reactive.Linq;
    using System.Threading.Tasks;
    using System.Windows.Input;

    using Gu.Wpf.Reactive;

    public class DispatchingViewViewModel
    {
        private readonly ObservableCollection<DummyItem> observableCollection = new ObservableCollection<DummyItem>();
        private readonly ObservableCollection<NotifyCollectionChangedEventArgs> observableCollectionChanges = new ObservableCollection<NotifyCollectionChangedEventArgs>();
        private readonly ObservableCollection<NotifyCollectionChangedEventArgs> dispatchingChanges = new ObservableCollection<NotifyCollectionChangedEventArgs>();

        public DispatchingViewViewModel()
        {
            this.DeferTime = TimeSpan.FromSeconds(0.1);
            this.Add(3);
            this.ReadOnlyObservableCollection = new ReadOnlyObservableCollection<DummyItem>(this.observableCollection);
            this.DispatchingView = this.observableCollection.AsDispatchingView();
            this.AddOneCommand = new RelayCommand(this.AddOne, () => true);
            this.AddOneToViewCommand = new RelayCommand(this.AddOneToView, () => true);
            this.AddTenCommand = new RelayCommand(this.AddTen, () => true);
            this.AddOneOnOtherThreadCommand = new RelayCommand(() => Task.Run(() => this.AddOne()), () => true);
            this.ClearCommand = new RelayCommand(() => this.Clear(), () => true);
            this.ObservableCollection.ObserveCollectionChanged()
                                .ObserveOnDispatcher()
                                .Subscribe(x => this.observableCollectionChanges.Add(x.EventArgs));

            this.DispatchingView.ObserveCollectionChanged()
                    .ObserveOnDispatcher()
                    .Subscribe(x => this.dispatchingChanges.Add(x.EventArgs));
        }

        public ObservableCollection<NotifyCollectionChangedEventArgs> ObservableCollectionChanges => this.observableCollectionChanges;

        public ObservableCollection<NotifyCollectionChangedEventArgs> DispatchingChanges => this.dispatchingChanges;

        public ObservableCollection<DummyItem> ObservableCollection => this.observableCollection;

        public ReadOnlyObservableCollection<DummyItem> ReadOnlyObservableCollection { get; }

        public IObservableCollection<DummyItem> DispatchingView { get; }

        public TimeSpan DeferTime { get; }

        public ICommand AddOneCommand { get; }

        public ICommand AddOneToViewCommand { get; }

        public ICommand AddTenCommand { get; }

        public ICommand AddOneOnOtherThreadCommand { get; }

        public RelayCommand ClearCommand { get; }

        private void AddOne()
        {
            lock (((ICollection)this.observableCollection).SyncRoot)
            {
                this.observableCollection.Add(new DummyItem(this.observableCollection.Count + 1));
            }
        }

        private void AddOneToView()
        {
            this.DispatchingView.Add(new DummyItem(this.observableCollection.Count + 1));
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
