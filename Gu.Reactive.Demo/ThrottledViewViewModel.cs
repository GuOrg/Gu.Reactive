namespace Gu.Reactive.Demo
{
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Threading.Tasks;
    using System.Windows.Input;

    using Gu.Wpf.Reactive;

    public class ThrottledViewViewModel
    {
        private readonly ObservableCollection<DummyItem> _observableCollection = new ObservableCollection<DummyItem>();
        private readonly ObservableCollection<NotifyCollectionChangedEventArgs> _observableCollectionEvents = new ObservableCollection<NotifyCollectionChangedEventArgs>();
        private readonly ObservableCollection<NotifyCollectionChangedEventArgs> _throttledEvents = new ObservableCollection<NotifyCollectionChangedEventArgs>();

        public ThrottledViewViewModel()
        {
            DeferTime = TimeSpan.FromMilliseconds(10);
            Add(3);
            ReadOnlyObservableCollection = new ReadOnlyObservableCollection<DummyItem>(_observableCollection);
            ThrottledView = _observableCollection.AsThrottledView(DeferTime, Schedulers.DispatcherOrCurrentThread);
            AddOneCommand = new RelayCommand(AddOne, () => true);
            AddOneToViewCommand = new RelayCommand(AddOneToView, () => true);
            AddTenCommand = new RelayCommand(AddTen, () => true);
            AddOneOnOtherThreadCommand = new RelayCommand(() => Task.Run(() => AddOne()), () => true);
            ClearCommand = new RelayCommand(() => Clear(), () => true);
            ObservableCollection.ObserveCollectionChanged()
                                .ObserveOnDispatcherOrCurrentThread()
                                .Subscribe(x => _observableCollectionEvents.Add(x.EventArgs));

            ThrottledView.ObserveCollectionChanged()
                    .ObserveOnDispatcherOrCurrentThread()
                    .Subscribe(x => _throttledEvents.Add(x.EventArgs));
        }

        public ObservableCollection<NotifyCollectionChangedEventArgs> ObservableCollectionEvents
        {
            get { return _observableCollectionEvents; }
        }

        public ObservableCollection<NotifyCollectionChangedEventArgs> ThrottledEvents
        {
            get { return _throttledEvents; }
        }

        public ObservableCollection<DummyItem> ObservableCollection
        {
            get { return _observableCollection; }
        }

        public ReadOnlyObservableCollection<DummyItem> ReadOnlyObservableCollection { get; private set; }

        public IThrottledView<DummyItem> ThrottledView { get; private set; }

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
            ThrottledView.Add(new DummyItem(_observableCollection.Count + 1));
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
            _throttledEvents.Clear();
        }
    }
}
