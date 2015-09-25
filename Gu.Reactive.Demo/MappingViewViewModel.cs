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

            FilteredMappedInts = _ints.AsReadOnlyFilteredView(x=>x%2==0).AsMappingView(x => new MappedVm { Value = x },  WpfSchedulers.Dispatcher);
            MappedInts = _ints.AsMappingView(x => new MappedVm { Value = x },  WpfSchedulers.Dispatcher);
            MappedIndexedInts = _ints.AsMappingView((x, i) => new MappedVm { Value = x, Index = i },  WpfSchedulers.Dispatcher);

            FilteredMappedMapped = MappedInts.AsReadOnlyFilteredView(x => x.Value % 2 == 0)
                                             .AsMappingView(x => new MappedVm { Value = x.Value * 2 },  WpfSchedulers.Dispatcher);

            MappedMapped = MappedInts.AsMappingView(x => new MappedVm { Value = x.Value * 2 },  WpfSchedulers.Dispatcher);
            MappedMappedIndexed = MappedInts.AsMappingView((x, i) => new MappedVm { Value = x.Value * 2, Index = i },  WpfSchedulers.Dispatcher);
            MappedMappedUpdateIndexed = MappedInts.AsMappingView((x, i) => new MappedVm { Value = x.Value * 2, Index = i }, (x, i) => x.UpdateIndex(i),  WpfSchedulers.Dispatcher);
            MappedMappedUpdateNewIndexed = MappedInts.AsMappingView((x, i) => new MappedVm { Value = x.Value * 2, Index = i }, (x, i) => new MappedVm { Value = x.Value * 2, Index = i },  WpfSchedulers.Dispatcher);

            AddOneToSourceCommand = new RelayCommand(() => _ints.Add(_ints.Count + 1));

            AddOneToSourceOnOtherThreadCommand = new RelayCommand(() => Task.Run(() => _ints.Add(_ints.Count + 1)));

            ClearCommand = new ConditionRelayCommand(
                () => _ints.Clear(),
                new Condition(() => _ints.Any(), _ints.ObserveCollectionChanged()));

            RemoveAtCommand = new ConditionRelayCommand(
                () => _ints.RemoveAt(RemoveAt >= _ints.Count ? _ints.Count - 1 : RemoveAt),
                new Condition(() => _ints.Any(), _ints.ObserveCollectionChanged()));
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public DispatchingView<int> Ints { get; }

        public MappingView<int, MappedVm> FilteredMappedInts { get; set; }

        public MappingView<int, MappedVm> MappedInts { get; }

        public MappingView<int, MappedVm> MappedIndexedInts { get; private set; }

        public MappingView<MappedVm, MappedVm> FilteredMappedMapped { get; set; }

        public MappingView<MappedVm, MappedVm> MappedMapped { get; private set; }

        public IReadOnlyObservableCollection<MappedVm> MappedMappedIndexed { get; private set; }

        public MappingView<MappedVm, MappedVm> MappedMappedUpdateIndexed { get; private set; }
        
        public MappingView<MappedVm, MappedVm> MappedMappedUpdateNewIndexed { get; private set; }

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

        public ICommand AddOneToSourceCommand { get; private set; }

        public ICommand AddOneToSourceOnOtherThreadCommand { get; private set; }

        public ICommand ClearCommand { get; private set; }

        public ICommand RemoveAtCommand { get; private set; }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
