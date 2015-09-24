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

        public ThrottledViewViewModel()
        {
            DeferTime = TimeSpan.FromMilliseconds(10);
            Add(3);
            ReadOnlyObservableCollection = new ReadOnlyObservableCollection<DummyItem>(_observableCollection);
            ThrottledView = _observableCollection.AsThrottledView(DeferTime, Schedulers.DispatcherOrCurrentThread);
            ReadOnlyThrottledView = _observableCollection.AsReadOnlyThrottledView(DeferTime, Schedulers.DispatcherOrCurrentThread);
            ReadOnlyIlistThrottledView = ReadOnlyThrottledView.AsReadonlyIListView();
            AddOneCommand = new RelayCommand(AddOne, () => true);
            AddOneToViewCommand = new RelayCommand(AddOneToView, () => true);
            AddTenCommand = new RelayCommand(AddTen, () => true);
            AddOneOnOtherThreadCommand = new RelayCommand(() => Task.Run(() => AddOne()), () => true);
        }

        public ObservableCollection<DummyItem> ObservableCollection => _observableCollection;

        public ReadOnlyObservableCollection<DummyItem> ReadOnlyObservableCollection { get; }

        public IThrottledView<DummyItem> ThrottledView { get;  }

        public IReadOnlyObservableCollection<DummyItem> ReadOnlyThrottledView { get;  }

        public IReadOnlyObservableCollection<DummyItem> ReadOnlyIlistThrottledView { get; private set; }

        public TimeSpan DeferTime { get; private set; }

        public ICommand AddOneCommand { get; private set; }

        public ICommand AddOneToViewCommand { get; private set; }

        public ICommand AddTenCommand { get; private set; }

        public ICommand AddOneOnOtherThreadCommand { get; private set; }

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
        }
    }
}
