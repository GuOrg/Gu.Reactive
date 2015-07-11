namespace Gu.Reactive.Demo
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using System.Windows.Input;

    using Gu.Reactive.Demo.Annotations;
    using Gu.Wpf.Reactive;

    public class MappingViewViewModel : INotifyPropertyChanged
    {
        private readonly ObservableCollection<int> _ints = new ObservableCollection<int>(new[] { 1, 2, 3 });

        private int _removeAt;

        public MappingViewViewModel()
        {
            Ints = _ints.AsDispatchingView();

            MappedInts = _ints.AsMappingView(x => new MappedDummy { Value = x }, Schedulers.DispatcherOrCurrentThread);

            MappedMapped = MappedInts.AsMappingView(x => new MappedDummy { Value = x.Value * 2 }, Schedulers.DispatcherOrCurrentThread);

            AddOneCommand = new RelayCommand(() => _ints.Add(_ints.Count + 1));

            AddOneOnOtherThreadCommand = new RelayCommand(() => Task.Run(() => _ints.Add(_ints.Count + 1)));

            ClearCommand = new ConditionRelayCommand(
                () => _ints.Clear(),
                new Condition(() => _ints.Any(), _ints.ObserveCollectionChanged()));

            RemoveAtCommand = new ConditionRelayCommand(
                () => _ints.RemoveAt(RemoveAt >= _ints.Count ? _ints.Count - 1 : RemoveAt),
                new Condition(() => _ints.Any(), _ints.ObserveCollectionChanged()));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public DispatchingView<int> Ints { get; private set; }

        public IReadOnlyObservableCollection<MappedDummy> MappedInts { get; private set; }

        public IReadOnlyObservableCollection<MappedDummy> MappedMapped { get; private set; }

        public int RemoveAt
        {
            get { return _removeAt; }
            set
            {
                if (value == _removeAt)
                {
                    return;
                }
                if (value < 0 || value > Ints.Count)
                {
                    _removeAt = 0;
                }
                else
                {
                    _removeAt = value;
                }
                OnPropertyChanged();
            }
        }

        public ICommand AddOneCommand { get; private set; }

        public ICommand AddOneOnOtherThreadCommand { get; private set; }

        public ICommand ClearCommand { get; private set; }

        public ICommand RemoveAtCommand { get; private set; }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
