namespace Gu.Reactive.Demo
{
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Reactive.Linq;
    using System.Threading.Tasks;
    using System.Windows.Input;

    using Gu.Wpf.Reactive;

    public sealed class ReadOnlyThrottledViewViewModel : IDisposable
    {
        private bool disposed;

        public ReadOnlyThrottledViewViewModel()
        {
            this.DeferTime = TimeSpan.FromMilliseconds(10);
            this.Add(3);
            this.View = this.Source.AsReadOnlyThrottledView(this.DeferTime, WpfSchedulers.Dispatcher);
            this.ReadOnlyIListThrottledView = this.View.AsReadonlyIListView();

            this.Source
                .ObserveCollectionChanged()
                .ObserveOnDispatcher()
                .Subscribe(x => this.SourceChanges.Add(x.EventArgs));

            this.View
                .ObserveCollectionChanged()
                .ObserveOnDispatcher()
                .Subscribe(x => this.ViewChanges.Add(x.EventArgs));

            this.AddOneCommand = new RelayCommand(this.AddOne, () => true);
            this.AddFourCommand = new RelayCommand(() => this.Add(4), () => true);
            this.AddOneOnOtherThreadCommand = new RelayCommand(() => Task.Run(() => this.AddOne()), () => true);
            this.ClearCommand = new RelayCommand(this.Clear, () => true);
        }

        public ObservableCollection<DummyItem> Source { get; } = new ObservableCollection<DummyItem>();

        public IReadOnlyObservableCollection<DummyItem> View { get; }

        public IReadOnlyObservableCollection<DummyItem> ReadOnlyIListThrottledView { get; }

        public ObservableCollection<NotifyCollectionChangedEventArgs> SourceChanges { get; } = new ObservableCollection<NotifyCollectionChangedEventArgs>();

        public ObservableCollection<NotifyCollectionChangedEventArgs> ViewChanges { get; } = new ObservableCollection<NotifyCollectionChangedEventArgs>();

        public TimeSpan DeferTime { get; }

        public ICommand AddOneCommand { get; }

        public ICommand AddFourCommand { get; }

        public ICommand AddOneOnOtherThreadCommand { get; }

        public ICommand ClearCommand { get; }

        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            (this.View as IDisposable)?.Dispose();
            (this.ReadOnlyIListThrottledView as IDisposable)?.Dispose();
        }

        private void AddOne()
        {
            this.Source.Add(new DummyItem(this.Source.Count + 1));
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
            this.ViewChanges.Clear();
        }
    }
}