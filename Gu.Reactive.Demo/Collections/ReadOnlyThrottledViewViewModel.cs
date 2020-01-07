namespace Gu.Reactive.Demo
{
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Gu.Wpf.Reactive;

    public sealed class ReadOnlyThrottledViewViewModel : IDisposable
    {
        private readonly SerialDisposable sourceChanges = new SerialDisposable();
        private readonly SerialDisposable viewChanges = new SerialDisposable();
        private bool disposed;

        public ReadOnlyThrottledViewViewModel()
        {
            this.DeferTime = TimeSpan.FromMilliseconds(10);
            this.Add(3);
            this.View = this.Source.AsReadOnlyThrottledView(this.DeferTime, WpfSchedulers.Dispatcher);
            this.ReadOnlyIListThrottledView = this.View.AsReadonlyIListView();

            this.sourceChanges.Disposable = this.Source
                                                .ObserveCollectionChanged(signalInitial: false)
                                                .ObserveOnDispatcher()
                                                .Subscribe(x => this.SourceChanges.Add(x.EventArgs));

            this.viewChanges.Disposable = this.View
                                              .ObserveCollectionChanged(signalInitial: false)
                                              .ObserveOnDispatcher()
                                              .Subscribe(x => this.ViewChanges.Add(x.EventArgs));

            this.AddOneCommand = new RelayCommand(this.AddOne, () => true);
            this.AddFourCommand = new RelayCommand(() => this.Add(4), () => true);
            this.AddOneOnOtherThreadCommand = new RelayCommand(() => Task.Run(() => this.AddOne()), () => true);
            this.ClearCommand = new RelayCommand(this.Source.Clear, () => true);
            this.ResetCommand = new RelayCommand(this.Reset);
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

        public ICommand ResetCommand { get; }

        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            (this.View as IDisposable)?.Dispose();
            (this.ReadOnlyIListThrottledView as IDisposable)?.Dispose();
            this.sourceChanges.Dispose();
            this.viewChanges.Dispose();
        }

        private void AddOne()
        {
            this.Source.Add(new DummyItem(this.Source.Count + 1));
        }

        private void Add(int n)
        {
            for (var i = 0; i < n; i++)
            {
                this.AddOne();
            }
        }

        private void Reset()
        {
            this.sourceChanges.Disposable = null;
            this.viewChanges.Disposable = null;
            this.Source.Clear();
            this.SourceChanges.Clear();
            this.ViewChanges.Clear();
            this.sourceChanges.Disposable = this.Source
                                                .ObserveCollectionChanged(signalInitial: false)
                                                .ObserveOnDispatcher()
                                                .Subscribe(x => this.SourceChanges.Add(x.EventArgs));

            this.viewChanges.Disposable = this.View
                                              .ObserveCollectionChanged(signalInitial: false)
                                              .ObserveOnDispatcher()
                                              .Subscribe(x => this.ViewChanges.Add(x.EventArgs));
        }
    }
}