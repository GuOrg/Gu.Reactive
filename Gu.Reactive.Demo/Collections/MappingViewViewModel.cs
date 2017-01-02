namespace Gu.Reactive.Demo
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Gu.Wpf.Reactive;
    using JetBrains.Annotations;

    public class MappingViewViewModel : INotifyPropertyChanged
    {
        private readonly ObservableCollection<int> ints = new ObservableCollection<int>(new[] { 1, 2, 3 });

        private int removeAt;

        public MappingViewViewModel()
        {
            this.Ints = this.ints.AsDispatchingView();

            this.FilteredMappedInts = this.ints.AsReadOnlyFilteredView(x => x % 2 == 0).AsMappingView(x => new MappedVm { Value = x },  WpfSchedulers.Dispatcher);
            this.MappedInts = this.ints.AsMappingView(x => new MappedVm { Value = x },  WpfSchedulers.Dispatcher);
            this.MappedIndexedInts = this.ints.AsMappingView((x, i) => new MappedVm { Value = x, Index = i },  WpfSchedulers.Dispatcher);

            this.FilteredMappedMapped = this.MappedInts.AsReadOnlyFilteredView(x => x.Value % 2 == 0)
                                             .AsMappingView(x => new MappedVm { Value = x.Value * 2 },  WpfSchedulers.Dispatcher);

            this.MappedMapped = this.MappedInts.AsMappingView(x => new MappedVm { Value = x.Value * 2 },  WpfSchedulers.Dispatcher);
            this.MappedMappedIndexed = this.MappedInts.AsMappingView((x, i) => new MappedVm { Value = x.Value * 2, Index = i },  WpfSchedulers.Dispatcher);
            this.MappedMappedUpdateIndexed = this.MappedInts.AsMappingView((x, i) => new MappedVm { Value = x.Value * 2, Index = i }, (x, i) => x.UpdateIndex(i),  WpfSchedulers.Dispatcher);
            this.MappedMappedUpdateNewIndexed = this.MappedInts.AsMappingView((x, i) => new MappedVm { Value = x.Value * 2, Index = i }, (x, i) => new MappedVm { Value = x.Value * 2, Index = i },  WpfSchedulers.Dispatcher);

            this.AddOneToSourceCommand = new RelayCommand(() => this.ints.Add(this.ints.Count + 1));

            this.AddOneToSourceOnOtherThreadCommand = new RelayCommand(() => Task.Run(() => this.ints.Add(this.ints.Count + 1)));

            this.ClearCommand = new ConditionRelayCommand(
                () => this.ints.Clear(),
                new Condition(() => this.ints.Any(), this.ints.ObserveCollectionChanged()));

            this.RemoveAtCommand = new ConditionRelayCommand(
                () => this.ints.RemoveAt(this.RemoveAt >= this.ints.Count ? this.ints.Count - 1 : this.RemoveAt),
                new Condition(() => this.ints.Any(), this.ints.ObserveCollectionChanged()));
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
            get
            {
                return this.removeAt;
            }

            set
            {
                if (value == this.removeAt)
                {
                    return;
                }

                if (value < 0 || value > this.Ints.Count)
                {
                    this.removeAt = 0;
                }
                else
                {
                    this.removeAt = value;
                }

                this.OnPropertyChanged();
            }
        }

        public ICommand AddOneToSourceCommand { get; private set; }

        public ICommand AddOneToSourceOnOtherThreadCommand { get; private set; }

        public ICommand ClearCommand { get; private set; }

        public ICommand RemoveAtCommand { get; private set; }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
