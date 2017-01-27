namespace Gu.Reactive.Demo
{
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Reactive.Linq;
    using System.Threading.Tasks;
    using System.Windows.Input;

    using Gu.Wpf.Reactive;

    public class DispatchingCollectionViewModel
    {
        public DispatchingCollectionViewModel()
        {
            this.Add(3);
            this.AddOneCommand = new RelayCommand(this.AddOne, () => true);
            this.AddFourCommand = new RelayCommand(this.AddFour, () => true);
            this.AddOneOnOtherThreadCommand = new RelayCommand(() => Task.Run(() => this.AddOne()), () => true);
            this.ClearCommand = new RelayCommand(this.Clear, () => true);
            this.Source
                .ObserveCollectionChangedSlim(false)
                .ObserveOnDispatcher()
                .Subscribe(x => this.SourceChanges.Add(x));
        }

        public DispatchingCollection<DummyItem> Source { get; } = new DispatchingCollection<DummyItem>();

        public ObservableCollection<NotifyCollectionChangedEventArgs> SourceChanges { get; } = new ObservableCollection<NotifyCollectionChangedEventArgs>();

        public ICommand AddOneCommand { get; }

        public ICommand AddFourCommand { get; }

        public ICommand AddOneOnOtherThreadCommand { get; }

        public ICommand ClearCommand { get; }

        private void AddOne()
        {
            this.Source.Add(new DummyItem(this.Source.Count + 1));
        }

        private void AddFour()
        {
            this.Add(4);
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
            this.SourceChanges.Clear();
        }
    }
}
