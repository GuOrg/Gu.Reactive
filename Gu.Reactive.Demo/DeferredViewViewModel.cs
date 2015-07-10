namespace Gu.Reactive.Demo
{
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Threading.Tasks;
    using System.Windows.Input;

    using Gu.Wpf.Reactive;

    public class DeferredViewViewModel
    {
        private readonly ObservableCollection<DummyItem> _observableCollection = new ObservableCollection<DummyItem>();
        private readonly ObservableCollection<NotifyCollectionChangedEventArgs> _observableCollectionEvents = new ObservableCollection<NotifyCollectionChangedEventArgs>();
        private readonly ObservableCollection<NotifyCollectionChangedEventArgs> _deferredEvents = new ObservableCollection<NotifyCollectionChangedEventArgs>();

        public DeferredViewViewModel()
        {
            DeferTime = TimeSpan.FromMilliseconds(10);
            Add(3);
            ReadOnlyObservableCollection = new ReadOnlyObservableCollection<DummyItem>(_observableCollection);
            ObservableCollectionDeferredView = _observableCollection.AsDeferredView(DeferTime,Schedulers.DispatcherOrCurrentThread);
            AddOneCommand = new RelayCommand(AddOne, () => true);
            AddOneToViewCommand = new RelayCommand(AddOneToView, () => true);
            AddTenCommand = new RelayCommand(AddTen, () => true);
            AddOneOnOtherThreadCommand = new RelayCommand(() => Task.Run(() => AddOne()), () => true);
            ClearCommand = new RelayCommand(() => Clear(), () => true);
            ObservableCollection.ObserveCollectionChanged()
                                .ObserveOnDispatcherOrCurrentThread()
                                .Subscribe(x => _observableCollectionEvents.Add(x.EventArgs));

            ObservableCollectionDeferredView.ObserveCollectionChanged()
                    .ObserveOnDispatcherOrCurrentThread()
                    .Subscribe(x => _deferredEvents.Add(x.EventArgs));
        }

        public ObservableCollection<NotifyCollectionChangedEventArgs> ObservableCollectionEvents
        {
            get { return _observableCollectionEvents; }
        }

        public ObservableCollection<NotifyCollectionChangedEventArgs> DeferredEvents
        {
            get { return _deferredEvents; }
        }

        public ObservableCollection<DummyItem> ObservableCollection
        {
            get { return _observableCollection; }
        }

        public ReadOnlyObservableCollection<DummyItem> ReadOnlyObservableCollection { get; private set; }

        public IDeferredView<DummyItem> ObservableCollectionDeferredView { get; private set; }

        public TimeSpan DeferTime { get; private set; }

        public ICommand AddOneCommand { get; private set; }

        public ICommand AddOneToViewCommand { get; private set; }

        public ICommand AddTenCommand { get; private set; }

        public ICommand AddOneOnOtherThreadCommand { get; private set; }

        public RelayCommand ClearCommand { get; private set; }

        private void AddOne()
        {
            _observableCollection.Add(new DummyItem(_observableCollection.Count + 1));
        }

        private void AddOneToView()
        {
            ObservableCollectionDeferredView.Add(new DummyItem(_observableCollection.Count + 1));
        }

        private void AddTen()
        {
            Add(10);
        }

        private void Add(int n)
        {
            for (int i = 0; i < n; i++)
            {
                AddOne();
            }
        }

        private void Clear()
        {
            _observableCollection.Clear();
            _observableCollectionEvents.Clear();
            _deferredEvents.Clear();
        }
    }
}
