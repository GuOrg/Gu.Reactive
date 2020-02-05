namespace Gu.Reactive.Demo
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Gu.Wpf.Reactive;

    public sealed class MappingViewViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly ObservableCollection<int> source = new ObservableCollection<int>(new[] { 1, 2, 3 });

        private int removeAt;
        private bool disposed;

        public MappingViewViewModel()
        {
            this.Ints = this.source.AsReadOnlyDispatchingView();

            this.FilteredMappedInts = this.source.AsReadOnlyFilteredView(x => x % 2 == 0)
                                          .AsMappingView(
                                              x => new MappedVm { Value = x },
                                              WpfSchedulers.Dispatcher);

            this.MappedInts = this.source.AsMappingView(
                x => new MappedVm { Value = x },
                WpfSchedulers.Dispatcher);

            this.MappedIndexedInts = this.source.AsMappingView(
                (x, i) => new MappedVm { Value = x, Index = i },
                (x, i) => x.UpdateIndex(i),
                WpfSchedulers.Dispatcher);

            this.MappedFilteredMapped = this.MappedInts
                                            .AsReadOnlyFilteredView(x => x.Value % 2 == 0)
                                            .AsMappingView(
                                                x => new MappedVm { Value = x.Value * 2 },
                                                WpfSchedulers.Dispatcher);

            this.MappedMapped = this.MappedInts.AsMappingView(x => new MappedVm { Value = x.Value * 2 }, WpfSchedulers.Dispatcher);
            this.MappedMappedIndexed = this.MappedInts.AsMappingView(
                (x, i) => new MappedVm { Value = x.Value * 2, Index = i },
                (x, i) => x.UpdateIndex(i),
                WpfSchedulers.Dispatcher);

            this.MappedMappedUpdateIndexed = this.MappedInts.AsMappingView(
                (x, i) => new MappedVm { Value = x.Value * 2, Index = i },
                (x, i) => x.UpdateIndex(i),
                WpfSchedulers.Dispatcher);

            this.MappedMappedUpdateNewIndexed = this.MappedInts.AsMappingView(
                selector: (x, i) => new MappedVm { Value = x.Value * 2, Index = i },
                updater: (x, i) => new MappedVm { Value = x.Value * 2, Index = i },
                onRemove: x => x.UpdateIndex(0),
                scheduler: WpfSchedulers.Dispatcher);

            this.AddOneToSourceCommand = new RelayCommand(() => this.source.Add(this.source.Count + 1));
            this.AddTenToSourceCommand = new RelayCommand(this.AddTen, () => true);
            this.AddOneToSourceOnOtherThreadCommand = new RelayCommand(() => Task.Run(() => this.source.Add(this.source.Count + 1)));

            this.ClearCommand = new RelayCommand(this.source.Clear);

            this.RemoveAtCommand = new ConditionRelayCommand(
                () => this.source.RemoveAt(this.RemoveAt >= this.source.Count ? this.source.Count - 1 : this.RemoveAt),
#pragma warning disable IDISP004 // Don't ignore created IDisposable.
                new Condition(() => this.source.Any(), this.source.ObserveCollectionChanged()));
#pragma warning restore IDISP004 // Don't ignore created IDisposable.
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public IReadOnlyObservableCollection<int> Ints { get; }

        public MappingView<int, MappedVm> FilteredMappedInts { get; }

        public MappingView<int, MappedVm> MappedInts { get; }

        public MappingView<int, MappedVm> MappedIndexedInts { get; }

        public MappingView<MappedVm, MappedVm> MappedFilteredMapped { get; }

        public MappingView<MappedVm, MappedVm> MappedMapped { get; }

        public IReadOnlyObservableCollection<MappedVm> MappedMappedIndexed { get; }

        public MappingView<MappedVm, MappedVm> MappedMappedUpdateIndexed { get; }

        public MappingView<MappedVm, MappedVm> MappedMappedUpdateNewIndexed { get; }

        public ICommand AddOneToSourceCommand { get; }

        public ICommand AddTenToSourceCommand { get; }

        public ICommand AddOneToSourceOnOtherThreadCommand { get; }

        public ICommand ClearCommand { get; }

        public ICommand RemoveAtCommand { get; }

        public int RemoveAt
        {
            get => this.removeAt;

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

        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            (this.Ints as IDisposable)?.Dispose();
            this.FilteredMappedInts.Dispose();
            this.MappedInts.Dispose();
            this.MappedIndexedInts.Dispose();
            this.MappedFilteredMapped.Dispose();
            this.MappedMapped.Dispose();
            (this.MappedMappedIndexed as IDisposable)?.Dispose();
            this.MappedMappedUpdateIndexed.Dispose();
            this.MappedMappedUpdateNewIndexed.Dispose();
            (this.ClearCommand as IDisposable)?.Dispose();
            (this.RemoveAtCommand as IDisposable)?.Dispose();
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void AddTen()
        {
            for (var i = 0; i < 10; i++)
            {
                this.source.Add(this.source.Count + 1);
            }
        }
    }
}
